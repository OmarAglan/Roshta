using Roshta.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Roshta.Services.Interfaces;

public interface IPatientService
{
    Task<IEnumerable<Patient>> GetAllPatientsAsync(); // Keep for potential other uses
    Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm); // Keep for potential other uses

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of patients, optionally filtered and sorted.
    /// </summary>
    Task<List<Patient>> GetPatientsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of patients, optionally filtered.
    /// </summary>
    Task<int> GetPatientsCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Patient?> GetPatientByIdAsync(int id);
    Task<Patient> AddPatientAsync(Patient patient);
    Task<Patient?> UpdatePatientAsync(Patient patient);
    Task<bool> DeletePatientAsync(int id);
    Task<bool> PatientExistsAsync(int id);
    Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null);
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}
