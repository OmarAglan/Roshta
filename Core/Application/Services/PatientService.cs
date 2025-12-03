using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;

namespace Rosheta.Core.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    // ... GetAllAsync and SearchAsync remain unchanged ... 
    public async Task<IEnumerable<Patient>> GetAllPatientsAsync()
    {
        try
        {
            return await _patientRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to retrieve patients.", ex);
        }
    }

    public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
    {
        try
        {
            return await _patientRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to search patients.", ex);
        }
    }

    public async Task<Patient?> GetPatientByIdAsync(int id)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), id);
            }
            return patient;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve patient.", ex);
        }
    }

    public async Task<Patient> AddPatientAsync(Patient patient)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(patient.Name)) throw new ValidationException("Patient name is required.");
        if (string.IsNullOrWhiteSpace(patient.ContactInfo)) throw new ValidationException("Patient contact information is required.");

        // Uniqueness
        if (!await _patientRepository.IsNameUniqueAsync(patient.Name))
            throw new BusinessRuleException($"A patient with the name '{patient.Name}' already exists.");

        if (!await _patientRepository.IsContactInfoUniqueAsync(patient.ContactInfo))
            throw new BusinessRuleException($"A patient with the contact info '{patient.ContactInfo}' already exists.");

        try
        {
            return await _patientRepository.AddAsync(patient);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to add patient.", ex);
        }
    }

    public async Task<Patient?> UpdatePatientAsync(Patient patient)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(patient.Name)) throw new ValidationException("Patient name is required.");
        if (string.IsNullOrWhiteSpace(patient.ContactInfo)) throw new ValidationException("Patient contact information is required.");

        // Check exists
        if (!await _patientRepository.ExistsAsync(patient.Id))
        {
            throw new NotFoundException(nameof(Patient), patient.Id);
        }

        // Uniqueness
        if (!await _patientRepository.IsNameUniqueAsync(patient.Name, patient.Id))
            throw new BusinessRuleException($"A patient with the name '{patient.Name}' already exists.");

        if (!await _patientRepository.IsContactInfoUniqueAsync(patient.ContactInfo, patient.Id))
            throw new BusinessRuleException($"A patient with the contact info '{patient.ContactInfo}' already exists.");

        try
        {
            // NEW: Void return
            await _patientRepository.UpdateAsync(patient);
            return patient;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to update patient.", ex);
        }
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        try
        {
            // NEW: Fetch entity first
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), id);
            }

            await _patientRepository.DeleteAsync(patient);
            return true;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to delete patient.", ex);
        }
    }

    public async Task<bool> PatientExistsAsync(int id)
    {
        try
        {
            return await _patientRepository.ExistsAsync(id);
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to check patient existence.", ex);
        }
    }

    public async Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null) => await _patientRepository.IsContactInfoUniqueAsync(contactInfo, currentId);
    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null) => await _patientRepository.IsNameUniqueAsync(name, currentId);
    public async Task<List<Patient>> GetPatientsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null) => await _patientRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
    public async Task<int> GetPatientsCountAsync(string? searchTerm = null) => await _patientRepository.GetCountAsync(searchTerm);
}