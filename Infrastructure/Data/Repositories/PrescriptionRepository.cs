using Microsoft.EntityFrameworkCore;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rosheta.Core.Domain.Enums;

namespace Rosheta.Infrastructure.Data.Repositories;

public class PrescriptionRepository : RepositoryBase<Prescription>, IPrescriptionRepository
{
    public PrescriptionRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Override generic GetAll to keep Prescription-specific include behavior.
    public override async Task<IReadOnlyList<Prescription>> GetAllAsync()
    {
        return await _dbSet
                             .Include(p => p.Patient)
                             .OrderByDescending(p => p.DateIssued)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

        return await _dbSet
                             .Include(p => p.Patient)
                             .Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm))
                             .OrderByDescending(p => p.DateIssued)
                             .ToListAsync();
    }

    // Override generic GetById to include related entities needed by Details and business rules.
    public override async Task<Prescription?> GetByIdAsync(int id)
    {
        return await _dbSet
                             .Include(p => p.Patient)
                             .Include(p => p.Doctor)
                             .Include(p => p.PrescriptionItems)
                                .ThenInclude(pi => pi.Medication)
                             .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> CancelAsync(int prescriptionId)
    {
        var prescription = await _dbSet.FindAsync(prescriptionId);
        if (prescription == null)
        {
            return false;
        }

        if (prescription.Status == PrescriptionStatus.Cancelled)
        {
            return true;
        }

        prescription.Status = PrescriptionStatus.Cancelled;
        prescription.UpdatedAt = DateTime.UtcNow;
        _dbContext.Entry(prescription).State = EntityState.Modified;
        return true;
    }

    public async Task<int> GetCountAsync(string? searchTerm = null)
    {
        IQueryable<Prescription> query = _dbSet;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Include(p => p.Patient)
                         .Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<List<Prescription>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        IQueryable<Prescription> query = _dbSet
                                                 .Include(p => p.Patient)
                                                 .Include(p => p.Doctor);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        switch (sortOrder)
        {
            case "name_desc":
                query = query.OrderByDescending(p => p.Patient!.Name);
                break;
            case "Date":
                query = query.OrderBy(p => p.DateIssued);
                break;
            case "date_desc":
                query = query.OrderByDescending(p => p.DateIssued);
                break;
            default:
                query = query.OrderByDescending(p => p.DateIssued);
                break;
        }

        return await query.Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }
}
