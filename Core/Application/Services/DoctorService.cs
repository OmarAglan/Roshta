using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Domain.Entities;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Application.Contracts.Services;
using Rosheta.Core.Application.Common.Exceptions;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public async Task<Doctor?> GetDoctorProfileAsync()
    {
        try
        {
            return await _doctorRepository.GetDoctorProfileAsync();
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to retrieve doctor profile.", ex);
        }
    }

    public async Task<Doctor?> GetDoctorProfileAsync(int doctorId)
    {
        try
        {
            var doctor = await _doctorRepository.GetDoctorProfileAsync(doctorId);

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

    public async Task<Doctor> SaveDoctorProfileAsync(Doctor doctor)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(doctor.Name))
        {
            throw new ValidationException("Doctor name is required.");
        }

        if (string.IsNullOrWhiteSpace(doctor.Specialization))
        {
            throw new ValidationException("Doctor specialization is required.");
        }

        try
        {
            return await _doctorRepository.SaveDoctorProfileAsync(doctor);
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to save doctor profile.", ex);
        }
    }

    public async Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(profileDto.Name))
        {
            throw new ValidationException("Doctor name is required.");
        }

        if (string.IsNullOrWhiteSpace(profileDto.Specialization))
        {
            throw new ValidationException("Doctor specialization is required.");
        }

        try
        {
            var result = await _doctorRepository.UpdateDoctorProfileAsync(doctorId, profileDto);

            if (!result)
            {
                throw new NotFoundException(nameof(Doctor), doctorId);
            }

            return result;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to update doctor profile.", ex);
        }
    }
}
