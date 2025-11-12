using Rosheta.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync(); // Keep for potential other uses, but Index will use paged version
    Task<IEnumerable<Patient>> SearchAsync(string searchTerm); // Keep for potential other uses, but Index will use paged version

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of patients, optionally filtered by search term and sorted.
    /// </summary>
    /// <param name="pageNumber">The 1-based page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="searchTerm">Optional search term to filter patients.</param>
    /// <param name="sortOrder">Optional sort order string.</param>
    /// <returns>A list of patients for the specified page.</returns>
    Task<List<Patient>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of patients, optionally filtered by search term.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter patients.</param>
    /// <returns>The total number of matching patients.</returns>
    Task<int> GetCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Patient?> GetByIdAsync(int id);
    Task<Patient> AddAsync(Patient patient);
    Task<bool> UpdateAsync(Patient patient);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null);
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
    // Add any patient-specific methods here later if needed (e.g., FindByNameAsync)
}
