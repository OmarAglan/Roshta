using Microsoft.EntityFrameworkCore;
using Rosheta.Data;
using Rosheta.Models.Entities;
using Rosheta.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rosheta.Repositories;

public class MedicationRepository : IMedicationRepository
{
    private readonly ApplicationDbContext _context;

    public MedicationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Medication>> GetAllAsync()
    {
        return await _context.Medications.ToListAsync();
    }

    public async Task<IEnumerable<Medication>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync(); // Return all if search term is empty
        }

        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

        return await _context.Medications
                             .Where(m => m.Name != null && m.Name.ToLower().Contains(lowerCaseSearchTerm))
                             .OrderBy(m => m.Name) // Keep consistent ordering
                             .ToListAsync();
    }

    public async Task<Medication?> GetByIdAsync(int id)
    {
        return await _context.Medications.FindAsync(id);
    }

    public async Task<Medication> AddAsync(Medication medication)
    {
        await _context.Medications.AddAsync(medication);
        await _context.SaveChangesAsync();
        return medication;
    }

    public async Task<bool> UpdateAsync(Medication medication)
    {
        _context.Entry(medication).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateConcurrencyException)
        {
            return false;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var medication = await _context.Medications.FindAsync(id);
        if (medication == null)
        {
            return false;
        }

        _context.Medications.Remove(medication);
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Medications.AnyAsync(e => e.Id == id);
    }

    // Implementation for the new interface method
    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return true; // Or false, depending on whether an empty name is allowed/unique
        }

        var normalizedName = name.Trim().ToLower();

        bool exists = await _context.Medications
            .Where(m => m.Name != null && m.Name.ToLower() == normalizedName)
            .Where(m => currentId == null || m.Id != currentId.Value) // Exclude current item if ID is provided
            .AnyAsync();

        return !exists; // True if no conflicting record exists
    }

    // --- Implementation for Pagination Methods ---

    public async Task<int> GetCountAsync(string? searchTerm = null)
    {
        IQueryable<Medication> query = _context.Medications;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(m => m.Name != null && m.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<List<Medication>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        IQueryable<Medication> query = _context.Medications;

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(m => m.Name != null && m.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        // Apply sorting
        switch (sortOrder)
        {
            case "name_desc":
                query = query.OrderByDescending(m => m.Name);
                break;
            // Add cases for other sortable columns (Dosage, Form, Manufacturer) if needed later
            default: // Default sort by Name ascending
                query = query.OrderBy(m => m.Name);
                break;
        }

        // Apply pagination
        return await query.Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }

    // ---------------------------------------------
}
