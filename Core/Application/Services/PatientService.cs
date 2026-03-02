using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Persistence;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Application.Common.Validation;

namespace Rosheta.Core.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;

    public PatientService(IPatientRepository patientRepository)
        : this(patientRepository, new NoOpUnitOfWork(), new NoOpValidationService())
    {
    }

    public PatientService(
        IPatientRepository patientRepository,
        IUnitOfWork unitOfWork,
        IValidationService validationService)
    {
        _patientRepository = patientRepository;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
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
        var result = await AddPatientResultAsync(patient);
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<Result<Patient>> AddPatientResultAsync(Patient patient)
    {
        var validation = await _validationService.ValidateAsync(patient);
        if (validation.IsFailure)
        {
            return Result<Patient>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        if (!await _patientRepository.IsNameUniqueAsync(patient.Name))
        {
            return Result<Patient>.Failure("BusinessRule", $"A patient with the name '{patient.Name}' already exists.");
        }

        if (!await _patientRepository.IsContactInfoUniqueAsync(patient.ContactInfo!))
        {
            return Result<Patient>.Failure("BusinessRule", $"A patient with the contact info '{patient.ContactInfo}' already exists.");
        }

        try
        {
            var created = await _patientRepository.AddAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            return Result<Patient>.Success(created);
        }
        catch (Exception ex)
        {
            return Result<Patient>.Failure("Infrastructure", $"Failed to add patient. {ex.Message}");
        }
    }

    public async Task<Patient?> UpdatePatientAsync(Patient patient)
    {
        var result = await UpdatePatientResultAsync(patient);
        if (result.IsSuccess)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<Result<Patient>> UpdatePatientResultAsync(Patient patient)
    {
        var validation = await _validationService.ValidateAsync(patient);
        if (validation.IsFailure)
        {
            return Result<Patient>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        if (!await _patientRepository.ExistsAsync(patient.Id))
        {
            return Result<Patient>.Failure("NotFound", $"Entity \"Patient\" with key ({patient.Id}) was not found.");
        }

        if (!await _patientRepository.IsNameUniqueAsync(patient.Name, patient.Id))
        {
            return Result<Patient>.Failure("BusinessRule", $"A patient with the name '{patient.Name}' already exists.");
        }

        if (!await _patientRepository.IsContactInfoUniqueAsync(patient.ContactInfo!, patient.Id))
        {
            return Result<Patient>.Failure("BusinessRule", $"A patient with the contact info '{patient.ContactInfo}' already exists.");
        }

        try
        {
            await _patientRepository.UpdateAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            return Result<Patient>.Success(patient);
        }
        catch (Exception ex)
        {
            return Result<Patient>.Failure("Infrastructure", $"Failed to update patient. {ex.Message}");
        }
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        var result = await DeletePatientResultAsync(id);
        if (result.IsSuccess)
        {
            return true;
        }

        throw ToException(result);
    }

    public async Task<Result> DeletePatientResultAsync(int id)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return Result.Failure("NotFound", $"Entity \"Patient\" with key ({id}) was not found.");
            }

            await _patientRepository.DeleteAsync(patient);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Infrastructure", $"Failed to delete patient. {ex.Message}");
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
