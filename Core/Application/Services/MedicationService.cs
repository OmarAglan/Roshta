using Rosheta.Models.Entities;
using Rosheta.Repositories.Interfaces;
using Rosheta.Services.Interfaces;

namespace Rosheta.Services;

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _medicationRepository;

    public MedicationService(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository;
    }

    public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
    {
        return await _medicationRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
    {
        return await _medicationRepository.SearchAsync(searchTerm);
    }

    public async Task<Medication?> GetMedicationByIdAsync(int id)
    {
        return await _medicationRepository.GetByIdAsync(id);
    }

    public async Task<Medication> AddMedicationAsync(Medication medication)
    {
        // Add any medication-specific business logic/validation here in the future
        return await _medicationRepository.AddAsync(medication);
    }

    public async Task<Medication?> UpdateMedicationAsync(Medication medication)
    {
        // Add any medication-specific business logic/validation here in the future
        var updated = await _medicationRepository.UpdateAsync(medication);
        return updated ? medication : null;
    }

    public async Task<bool> DeleteMedicationAsync(int id)
    {
        // Add any medication-specific business logic/validation here (e.g., check if medication is in use)
        return await _medicationRepository.DeleteAsync(id);
    }

    public async Task<bool> MedicationExistsAsync(int id)
    {
        return await _medicationRepository.ExistsAsync(id);
    }

    // Implementation for the new interface method
    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        return await _medicationRepository.IsNameUniqueAsync(name, currentId);
    }

    // --- Implementation for Pagination Methods ---

    public async Task<List<Medication>> GetMedicationsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        return await _medicationRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
    }

    public async Task<int> GetMedicationsCountAsync(string? searchTerm = null)
    {
        return await _medicationRepository.GetCountAsync(searchTerm);
    }

    // ---------------------------------------------
}
