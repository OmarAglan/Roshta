using Rosheta.Core.Application.DTOs.Doctor;
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

    public async Task<Doctor> SaveDoctorProfileAsync(Doctor doctor)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(doctor.Name)) throw new ValidationException("Doctor name is required.");
        if (string.IsNullOrWhiteSpace(doctor.Specialization)) throw new ValidationException("Doctor specialization is required.");

        try
        {
            // Logic: If ID is 0, Add. If ID exists, Update.
            // Or, simpler logic based on GetDoctorProfileAsync() existence check from old repo

            var existingDoctor = await _doctorRepository.GetDoctorProfileAsync();

            if (existingDoctor == null)
            {
                // Create New
                return await _doctorRepository.AddAsync(doctor);
            }
            else
            {
                // Update Existing (Upsert logic)
                // We map the incoming properties to the existing one to preserve ID
                existingDoctor.Name = doctor.Name;
                existingDoctor.Specialization = doctor.Specialization;
                existingDoctor.LicenseNumber = doctor.LicenseNumber;
                existingDoctor.ContactPhone = doctor.ContactPhone;
                existingDoctor.ContactEmail = doctor.ContactEmail;
                // Preserve other fields if necessary

                await _doctorRepository.UpdateAsync(existingDoctor);
                return existingDoctor;
            }
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to save doctor profile.", ex);
        }
    }

    public async Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(profileDto.Name)) throw new ValidationException("Doctor name is required.");
        if (string.IsNullOrWhiteSpace(profileDto.Specialization)) throw new ValidationException("Doctor specialization is required.");

        try
        {
            // 1. Fetch Entity
            var doctor = await _doctorRepository.GetByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new NotFoundException(nameof(Doctor), doctorId);
            }

            // 2. Map DTO to Entity (Logic moved from Repo to Service)
            doctor.Name = profileDto.Name;
            doctor.Specialization = profileDto.Specialization;
            doctor.LicenseNumber = profileDto.LicenseNumber;
            doctor.ContactPhone = profileDto.Phone;
            doctor.ContactEmail = profileDto.Email;

            // 3. Persist
            await _doctorRepository.UpdateAsync(doctor);
            return true;
        }
        catch (Exception ex) when (ex is not Rosheta.Core.Application.Common.Exceptions.ApplicationException)
        {
            throw new InfrastructureException("Failed to update doctor profile.", ex);
        }
    }
}