using Microsoft.EntityFrameworkCore;
using Roshta.Data;
using Roshta.Models.Entities;
using Roshta.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roshta.Repositories;

public class PrescriptionRepository : IPrescriptionRepository
{
    private readonly ApplicationDbContext _context;

    public PrescriptionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Prescription>> GetAllAsync()
    {
        // Include Patient info for display in the list
        return await _context.Prescriptions
                             .Include(p => p.Patient)
                             .OrderByDescending(p => p.DateIssued) // Show newest first
                             .ToListAsync();
    }

    public async Task<IEnumerable<Prescription>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var lowerCaseSearchTerm = searchTerm.Trim().ToLower();

        // Search by Patient Name
        return await _context.Prescriptions
                             .Include(p => p.Patient) // Need patient info for searching and display
                             .Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm))
                             .OrderByDescending(p => p.DateIssued) // Keep consistent ordering
                             .ToListAsync();
    }

    public async Task<Prescription?> GetByIdAsync(int id)
    {
        // Include related data needed for the Details view
        return await _context.Prescriptions
                             .Include(p => p.Patient)
                             .Include(p => p.Doctor)
                             .Include(p => p.PrescriptionItems)
                                .ThenInclude(pi => pi.Medication)
                             .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Prescription> AddAsync(Prescription prescription)
    {
        await _context.Prescriptions.AddAsync(prescription);
        // Note: EF Core should handle adding related PrescriptionItems automatically
        // if they are part of the 'prescription' object graph being added.
        await _context.SaveChangesAsync();
        return prescription;
    }

    public async Task<bool> CancelAsync(int prescriptionId)
    {
        var prescription = await _context.Prescriptions.FindAsync(prescriptionId);
        if (prescription == null)
        {
            return false; // Not found
        }

        // Check if already cancelled?
        if (prescription.Status == PrescriptionStatus.Cancelled)
        {
            return true; // Already cancelled, consider success
        }

        prescription.Status = PrescriptionStatus.Cancelled;
        prescription.UpdatedAt = DateTime.UtcNow; // Explicitly update audit field
        _context.Entry(prescription).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch (DbUpdateException) // Catch potential DB errors
        {
            // TODO: Log exception
            return false;
        }
    }

    // Implement Update/Delete later if needed

    // --- Implementation for Pagination Methods ---

    public async Task<int> GetCountAsync(string? searchTerm = null)
    {
        IQueryable<Prescription> query = _context.Prescriptions;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            // Include Patient for filtering
            query = query.Include(p => p.Patient)
                         .Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        return await query.CountAsync();
    }

    public async Task<List<Prescription>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        IQueryable<Prescription> query = _context.Prescriptions
                                                 .Include(p => p.Patient) // Include for display and sorting
                                                 .Include(p => p.Doctor); // Include for display

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var lowerCaseSearchTerm = searchTerm.Trim().ToLower();
            query = query.Where(p => p.Patient != null && p.Patient.Name != null && p.Patient.Name.ToLower().Contains(lowerCaseSearchTerm));
        }

        // Apply sorting
        switch (sortOrder)
        {
            case "name_desc":
                query = query.OrderByDescending(p => p.Patient.Name);
                break;
            case "Date":
                query = query.OrderBy(p => p.DateIssued);
                break;
            case "date_desc":
                query = query.OrderByDescending(p => p.DateIssued);
                break;
            // Add cases for other sortable columns (Doctor Name, Status, ExpiryDate) if needed later
            default: // Default sort by DateIssued descending (newest first)
                query = query.OrderByDescending(p => p.DateIssued);
                break;
        }

        // Apply pagination
        return await query.Skip((pageNumber - 1) * pageSize)
                          .Take(pageSize)
                          .ToListAsync();
    }

    // ---------------------------------------------
}
