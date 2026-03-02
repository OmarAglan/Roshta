using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Persistence;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Application.Common.Validation;

namespace Rosheta.Core.Application.Services;

public class MedicationService : IMedicationService
{
    private readonly IMedicationRepository _medicationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;

    public MedicationService(IMedicationRepository medicationRepository)
        : this(medicationRepository, new NoOpUnitOfWork(), new NoOpValidationService())
    {
    }

    public MedicationService(
        IMedicationRepository medicationRepository,
        IUnitOfWork unitOfWork,
        IValidationService validationService)
    {
        _medicationRepository = medicationRepository;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<IEnumerable<Medication>> GetAllMedicationsAsync()
    {
        try
        {
            return await _medicationRepository.GetAllAsync();
        }
        catch (Exception ex)
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
        catch (Exception ex)
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
        var result = await AddMedicationResultAsync(medication);
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<Result<Medication>> AddMedicationResultAsync(Medication medication)
    {
        var validation = await _validationService.ValidateAsync(medication);
        if (validation.IsFailure)
        {
            return Result<Medication>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        if (!await _medicationRepository.IsNameUniqueAsync(medication.Name))
        {
            return Result<Medication>.Failure("BusinessRule", $"A medication with the name '{medication.Name}' already exists.");
        }

        try
        {
            var created = await _medicationRepository.AddAsync(medication);
            await _unitOfWork.SaveChangesAsync();
            return Result<Medication>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<Medication>.Failure("Infrastructure", $"Failed to add medication. {ex.Message}");
        }
    }

    public async Task<Medication?> UpdateMedicationAsync(Medication medication)
    {
        var result = await UpdateMedicationResultAsync(medication);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<Result<Medication>> UpdateMedicationResultAsync(Medication medication)
    {
        var validation = await _validationService.ValidateAsync(medication);
        if (validation.IsFailure)
        {
            return Result<Medication>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        if (!await _medicationRepository.ExistsAsync(medication.Id))
        {
            return Result<Medication>.Failure("NotFound", $"Entity \"Medication\" with key ({medication.Id}) was not found.");
        }

        if (!await _medicationRepository.IsNameUniqueAsync(medication.Name, medication.Id))
        {
            return Result<Medication>.Failure("BusinessRule", $"A medication with the name '{medication.Name}' already exists.");
        }

        try
        {
            await _medicationRepository.UpdateAsync(medication);
            await _unitOfWork.SaveChangesAsync();
            return Result<Medication>.Success(medication);
        }
        catch (Exception ex)
        {
            return Result<Medication>.Failure("Infrastructure", $"Failed to update medication. {ex.Message}");
        }
    }

    public async Task<bool> DeleteMedicationAsync(int id)
    {
        var result = await DeleteMedicationResultAsync(id);
        if (result.IsSuccess)
        {
            return true;
        }

        throw ToException(result);
    }

    public async Task<Result> DeleteMedicationResultAsync(int id)
    {
        try
        {
            var medication = await _medicationRepository.GetByIdAsync(id);

            if (medication == null)
            {
                return Result.Failure("NotFound", $"Entity \"Medication\" with key ({id}) was not found.");
            }

            await _medicationRepository.DeleteAsync(medication);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Infrastructure", $"Failed to delete medication. {ex.Message}");
        }
    }

    public async Task<bool> MedicationExistsAsync(int id)
    {
        try
        {
            return await _medicationRepository.ExistsAsync(id);
        }
        catch (Exception ex)
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
        catch (Exception ex)
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
        catch (Exception ex)
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
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to count medications.", ex);
        }
    }

    private static Exception ToException(Result result)
    {
        if (result.ErrorCode == "Validation")
        {
            return new ValidationException(result.ErrorMessage);
        }
        if (result.ErrorCode == "BusinessRule")
        {
            return new BusinessRuleException(result.ErrorMessage);
        }
        if (result.ErrorCode == "NotFound")
        {
            return new NotFoundException(result.ErrorMessage);
        }

        return new InfrastructureException(result.ErrorMessage);
    }
}
