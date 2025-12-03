using Microsoft.EntityFrameworkCore;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Domain.Entities;
using Rosheta.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rosheta.Infrastructure.Data.Repositories;

public class PatientRepository : RepositoryBase<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Basic CRUD (Add, GetById, etc.) handled by RepositoryBase

    public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var lowerTerm = searchTerm.Trim().ToLower();
        return await _dbSet
            .Where(p => (p.Name != null && p.Name.ToLower().Contains(lowerTerm)) ||
                        (p.ContactInfo != null && p.ContactInfo.ToLower().Contains(lowerTerm)))
            .OrderBy(p => p.Name)
            .ToListAsync();
    }

    public async Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(contactInfo)) return true;

        var normalizedContact = contactInfo.Trim().ToLower();
        return !await _dbSet.AnyAsync(p =>
            p.ContactInfo != null &&
            p.ContactInfo.ToLower() == normalizedContact &&
            (!currentId.HasValue || p.Id != currentId.Value));
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(name)) return true;

        var normalizedName = name.Trim().ToLower();
        return !await _dbSet.AnyAsync(p =>
            p.Name != null &&
            p.Name.ToLower() == normalizedName &&
            (!currentId.HasValue || p.Id != currentId.Value));
    }

    public async Task<List<Patient>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.Trim().ToLower();
            query = query.Where(p => (p.Name != null && p.Name.ToLower().Contains(lowerTerm)) ||
                                     (p.ContactInfo != null && p.ContactInfo.ToLower().Contains(lowerTerm)));
        }

        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(p => p.Name),
            "Date" => query.OrderBy(p => p.DateOfBirth),
            "date_desc" => query.OrderByDescending(p => p.DateOfBirth),
            "VisitDate" => query.OrderBy(p => p.LastVisitDate == null).ThenBy(p => p.LastVisitDate),
            "visitdate_desc" => query.OrderByDescending(p => p.LastVisitDate.HasValue).ThenByDescending(p => p.LastVisitDate),
            _ => query.OrderBy(p => p.Name)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetCountAsync(string? searchTerm = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.Trim().ToLower();
            query = query.Where(p => (p.Name != null && p.Name.ToLower().Contains(lowerTerm)) ||
                                     (p.ContactInfo != null && p.ContactInfo.ToLower().Contains(lowerTerm)));
        }

        return await query.CountAsync();
    }
}