using Microsoft.EntityFrameworkCore;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Domain.Entities;
using Rosheta.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rosheta.Infrastructure.Data.Repositories;

public class MedicationRepository : RepositoryBase<Medication>, IMedicationRepository
{
    public MedicationRepository(ApplicationDbContext context) : base(context)
    {
    }

    // Basic CRUD methods (GetById, Add, Update, Delete) are now handled by RepositoryBase!

    public async Task<IEnumerable<Medication>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var lowerTerm = searchTerm.Trim().ToLower();
        return await _dbSet
            .Where(m => m.Name.ToLower().Contains(lowerTerm))
            .OrderBy(m => m.Name)
            .ToListAsync();
    }

    public async Task<List<Medication>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerTerm = searchTerm.Trim().ToLower();
            query = query.Where(m => m.Name.ToLower().Contains(lowerTerm));
        }

        // Apply sorting
        query = sortOrder switch
        {
            "name_desc" => query.OrderByDescending(m => m.Name),
            _ => query.OrderBy(m => m.Name) // Default sort
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
            query = query.Where(m => m.Name.ToLower().Contains(lowerTerm));
        }

        return await query.CountAsync();
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return true;
        }

        var lowerName = name.Trim().ToLower();

        return !await _dbSet.AnyAsync(m =>
            m.Name.ToLower() == lowerName &&
            (!currentId.HasValue || m.Id != currentId.Value));
    }
}