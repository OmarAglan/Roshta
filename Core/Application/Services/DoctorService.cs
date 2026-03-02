using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;
using Rosheta.Core.Application.Common.Persistence;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Application.Common.Validation;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IValidationService _validationService;

    public DoctorService(IDoctorRepository doctorRepository)
        : this(doctorRepository, new NoOpUnitOfWork(), new NoOpValidationService())
    {
    }

    public DoctorService(
        IDoctorRepository doctorRepository,
        IUnitOfWork unitOfWork,
        IValidationService validationService)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
        _validationService = validationService;
    }

    public async Task<Doctor?> GetDoctorProfileAsync()
    {
        try
        {
            return await _doctorRepository.GetDoctorProfileAsync();
        }
        catch (Exception ex)
        {
            throw new InfrastructureException("Failed to retrieve doctor profile.", ex);
        }
    }

    public async Task<Doctor?> GetDoctorProfileAsync(int doctorId)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new NotFoundException(nameof(Doctor), doctorId);
            }
            return doctor;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve doctor profile.", ex);
        }
    }

    public async Task<Result<Doctor>> SaveDoctorProfileResultAsync(Doctor doctor)
    {
        var validation = await _validationService.ValidateAsync(doctor);
        if (validation.IsFailure)
        {
            return Result<Doctor>.Failure(validation.ErrorCode, validation.ErrorMessage);
        }

        try
        {
            var existingDoctor = await _doctorRepository.GetDoctorProfileAsync();

            if (existingDoctor == null)
            {
                var created = await _doctorRepository.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
                return Result<Doctor>.Success(created);
            }

            existingDoctor.Name = doctor.Name;
            existingDoctor.Specialization = doctor.Specialization;
            existingDoctor.LicenseNumber = doctor.LicenseNumber;
            existingDoctor.ContactPhone = doctor.ContactPhone;
            existingDoctor.ContactEmail = doctor.ContactEmail;

            await _doctorRepository.UpdateAsync(existingDoctor);
            await _unitOfWork.SaveChangesAsync();
            return Result<Doctor>.Success(existingDoctor);
        }
        catch (Exception ex)
        {
            return Result<Doctor>.Failure("Infrastructure", $"Failed to save doctor profile. {ex.Message}");
        }
    }

    public async Task<Doctor> SaveDoctorProfileAsync(Doctor doctor)
    {
        var result = await SaveDoctorProfileResultAsync(doctor);
        if (result.IsSuccess && result.Value != null)
        {
            return result.Value;
        }

        throw ToException(result);
    }

    public async Task<Result> UpdateDoctorProfileResultAsync(int doctorId, UpdateDoctorProfileDto profileDto)
    {
        var validation = await _validationService.ValidateAsync(profileDto);
        if (validation.IsFailure)
        {
            return validation;
        }

        try
        {
            var existingDoctor = await _doctorRepository.GetDoctorProfileAsync();
            if (existingDoctor == null || existingDoctor.Id != doctorId)
            {
                var byId = await _doctorRepository.GetByIdAsync(doctorId);
                if (byId == null)
                {
                    return Result.Failure("NotFound", $"Entity \"Doctor\" with key ({doctorId}) was not found.");
                }
                existingDoctor = byId;
            }

            existingDoctor.Name = profileDto.Name;
            existingDoctor.Specialization = profileDto.Specialization;
            existingDoctor.LicenseNumber = profileDto.LicenseNumber;
            existingDoctor.ContactPhone = profileDto.Phone;
            existingDoctor.ContactEmail = profileDto.Email;

            await _doctorRepository.UpdateAsync(existingDoctor);
            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure("Infrastructure", $"Failed to update doctor profile. {ex.Message}");
        }
    }

    public async Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto)
    {
        var result = await UpdateDoctorProfileResultAsync(doctorId, profileDto);
        if (result.IsSuccess)
        {
            return true;
        }

        throw ToException(result);
    }

    private static Exception ToException(Result result)
    {
        if (result.ErrorCode == "Validation")
        {
            return new ValidationException(result.ErrorMessage);
        }
        if (result.ErrorCode == "NotFound")
        {
            return new NotFoundException(result.ErrorMessage);
        }

        return new InfrastructureException(result.ErrorMessage);
    }
}
