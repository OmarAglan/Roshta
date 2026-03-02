using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Application.DTOs.Doctor;
using Rosheta.Core.Application.Models;
using Rosheta.Core.Application.Common.Results;
using Rosheta.Core.Domain.Entities;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Services;

public interface IDoctorService
{
    Task<Result<Doctor>> SaveDoctorProfileResultAsync(Doctor doctor);
    Task<Result> UpdateDoctorProfileResultAsync(int doctorId, UpdateDoctorProfileDto profileDto);

    Task<Doctor?> GetDoctorProfileAsync();
    Task<Doctor?> GetDoctorProfileAsync(int doctorId);
    Task<Doctor> SaveDoctorProfileAsync(Doctor doctor);
    Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto);
}
