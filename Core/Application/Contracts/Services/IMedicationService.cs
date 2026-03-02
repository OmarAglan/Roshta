using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Common.Results;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Services;

public interface IMedicationService
{
    Task<Result<Medication>> AddMedicationResultAsync(Medication medication);
    Task<Result<Medication>> UpdateMedicationResultAsync(Medication medication);
    Task<Result> DeleteMedicationResultAsync(int id);

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
