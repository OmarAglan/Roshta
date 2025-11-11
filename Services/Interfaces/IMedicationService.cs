using Roshta.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roshta.Services.Interfaces;

public interface IMedicationService
{
    Task<IEnumerable<Medication>> GetAllMedicationsAsync(); // Keep for potential other uses
    Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm); // Keep for potential other uses

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of medications, optionally filtered and sorted.
    /// </summary>
    Task<List<Medication>> GetMedicationsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of medications, optionally filtered.
    /// </summary>
    Task<int> GetMedicationsCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Medication?> GetMedicationByIdAsync(int id);
    Task<Medication> AddMedicationAsync(Medication medication);
    Task<Medication?> UpdateMedicationAsync(Medication medication);
    Task<bool> DeleteMedicationAsync(int id);
    Task<bool> MedicationExistsAsync(int id);
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}
