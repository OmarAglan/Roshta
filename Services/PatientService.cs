using Roshta.Models.Entities;
using Roshta.Repositories.Interfaces;
using Roshta.Services.Interfaces;

namespace Roshta.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        // Business logic like filtering only active patients could go here later
        return await _patientRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
    {
        return await _patientRepository.SearchAsync(searchTerm);
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        return await _patientRepository.GetByIdAsync(id);
    }

    public async Task<Patient> AddPatientAsync(Patient patient)
    {
        // Add any service-level validation or logic here
        return await _patientRepository.AddAsync(patient);
    }

    public async Task<Patient?> UpdatePatientAsync(Patient patient)
    {
        // Add any service-level validation or logic here
        var updated = await _patientRepository.UpdateAsync(patient);
        return updated ? patient : null;
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        // Add any service-level validation or logic here
        return await _patientRepository.DeleteAsync(id);
    }

    public async Task<bool> PatientExistsAsync(int id)
    {
        return await _patientRepository.ExistsAsync(id);
    }

    // Implementation for the new interface method
    public async Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null)
    {
        // Could add validation here to check format before hitting repo?
        return await _patientRepository.IsContactInfoUniqueAsync(contactInfo, currentId);
    }

    // Implementation for Name uniqueness
    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        return await _patientRepository.IsNameUniqueAsync(name, currentId);
    }

    // --- Implementation for Pagination Methods ---

    public async Task<List<Patient>> GetPatientsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        // Add any service-level logic here if needed (e.g., default page size)
        return await _patientRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
    }

    public async Task<int> GetPatientsCountAsync(string? searchTerm = null)
    {
        // Add any service-level logic here if needed
        return await _patientRepository.GetCountAsync(searchTerm);
    }

    // ---------------------------------------------
}
