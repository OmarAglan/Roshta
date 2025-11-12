using Roshta.Models.Entities;
using Roshta.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Services.Interfaces;

public interface IPrescriptionService
{
    // Takes the view model and the ID of the doctor creating the prescription
    Task<Prescription?> CreatePrescriptionAsync(PrescriptionCreateModel model, int doctorId);
    Task<IEnumerable<Prescription>> GetAllPrescriptionsAsync(); // Keep for potential other uses
    Task<IEnumerable<Prescription>> SearchPrescriptionsAsync(string searchTerm); // Keep for potential other uses

    // --- Methods for Pagination ---
    /// <summary>
    /// Gets a paged list of prescriptions, optionally filtered and sorted.
    /// </summary>
    Task<List<Prescription>> GetPrescriptionsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);

    /// <summary>
    /// Gets the total count of prescriptions, optionally filtered.
    /// </summary>
    Task<int> GetPrescriptionsCountAsync(string? searchTerm = null);
    // -----------------------------

    Task<Prescription?> GetPrescriptionByIdAsync(int id);
    Task<bool> CancelPrescriptionAsync(int prescriptionId);
}
