using Rosheta.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IPrescriptionRepository
{
    // We'll likely add Get methods later for viewing prescriptions
    Task<IEnumerable<Prescription>> GetAllAsync(); // Keep for potential other uses
    Task<IEnumerable<Prescription>> SearchAsync(string searchTerm); // Keep for potential other uses

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of prescriptions, optionally filtered by search term (Patient Name) and sorted.
    /// Includes Patient and Doctor details.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter prescriptions (by Patient Name).</param>
    /// <param name="sortOrder">Optional sort order string.</param>
    /// <returns>A list of prescriptions for the specified page.</returns>
    Task<List<Prescription>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of prescriptions, optionally filtered by search term (Patient Name).
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter prescriptions (by Patient Name).</param>
    /// <returns>The total number of matching prescriptions.</returns>
    Task<int> GetCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Prescription?> GetByIdAsync(int id); // Renamed from GetPrescriptionByIdAsync
    Task<Prescription> AddAsync(Prescription prescription); // Renamed from CreatePrescriptionAsync
    Task<bool> CancelAsync(int prescriptionId); // Add this
    // Consider adding Update/Delete if needed
}
