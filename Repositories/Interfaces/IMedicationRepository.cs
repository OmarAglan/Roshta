using Roshta.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roshta.Repositories.Interfaces;

public interface IMedicationRepository
{
    Task<IEnumerable<Medication>> GetAllAsync(); // Keep for potential other uses
    Task<IEnumerable<Medication>> SearchAsync(string searchTerm); // Keep for potential other uses

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of medications, optionally filtered by search term and sorted.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter medications (by Name).</param>
    /// <param name="sortOrder">Optional sort order string.</param>
    /// <returns>A list of medications for the specified page.</returns>
    Task<List<Medication>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of medications, optionally filtered by search term.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter medications (by Name).</param>
    /// <returns>The total number of matching medications.</returns>
    Task<int> GetCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Medication?> GetByIdAsync(int id);
    Task<Medication> AddAsync(Medication medication);
    Task<bool> UpdateAsync(Medication medication);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id); // Useful helper method
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}
