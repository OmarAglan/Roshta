using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;

namespace Rosheta.Core.Application.Services;

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _medicationRepository;

    public MedicationService(IMedicationRepository medicationRepository)
    {
        _medicationRepository = medicationRepository;
    }

    public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
    {
        try
        {
            return await _medicationRepository.GetAllAsync();
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve medications.", ex);
        }
    }

    public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
    {
        try
        {
            return await _medicationRepository.SearchAsync(searchTerm);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to search medications.", ex);
        }
    }

    public async Task<Medication?> GetMedicationByIdAsync(int id)
    {
        try
        {
            var medication = await _medicationRepository.GetByIdAsync(id);

            if (medication == null)
            {
                throw new NotFoundException(nameof(Medication), id);
            }

            return medication;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve medication.", ex);
        }
    }

    public async Task<Medication> AddMedicationAsync(Medication medication)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(medication.Name))
        {
            throw new ValidationException("Medication name is required.");
        }

        // Check uniqueness
        if (!await _medicationRepository.IsNameUniqueAsync(medication.Name))
        {
            throw new BusinessRuleException($"A medication with the name '{medication.Name}' already exists.");
        }

        try
        {
            return await _medicationRepository.AddAsync(medication);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to add medication.", ex);
        }
    }

    public async Task<Medication?> UpdateMedicationAsync(Medication medication)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(medication.Name))
        {
            throw new ValidationException("Medication name is required.");
        }

        // Check if medication exists
        if (!await _medicationRepository.ExistsAsync(medication.Id))
        {
            throw new NotFoundException(nameof(Medication), medication.Id);
        }

        // Check uniqueness
        if (!await _medicationRepository.IsNameUniqueAsync(medication.Name, medication.Id))
        {
            throw new BusinessRuleException($"A medication with the name '{medication.Name}' already exists.");
        }

        try
        {
            var updated = await _medicationRepository.UpdateAsync(medication);
            return updated ? medication : null;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to update medication.", ex);
        }
    }

    public async Task<bool> DeleteMedicationAsync(int id)
    {
        // Check if medication exists
        if (!await _medicationRepository.ExistsAsync(id))
        {
            throw new NotFoundException(nameof(Medication), id);
        }

        try
        {
            return await _medicationRepository.DeleteAsync(id);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to delete medication.", ex);
        }
    }

    public async Task<bool> MedicationExistsAsync(int id)
    {
        try
        {
            return await _medicationRepository.ExistsAsync(id);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to check medication existence.", ex);
        }
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? currentId = null)
    {
        try
        {
            return await _medicationRepository.IsNameUniqueAsync(name, currentId);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to validate name uniqueness.", ex);
        }
    }

    public async Task<List<Medication>> GetMedicationsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null)
    {
        try
        {
            return await _medicationRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, sortOrder);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve paged medications.", ex);
        }
    }

    public async Task<int> GetMedicationsCountAsync(string? searchTerm = null)
    {
        try
        {
            return await _medicationRepository.GetCountAsync(searchTerm);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to count medications.", ex);
        }
    }
}
