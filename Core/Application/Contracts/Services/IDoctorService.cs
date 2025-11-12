using Rosheta.Core.Application.DTOs;
using Rosheta.Core.Domain.Entities;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Services;

public interface IDoctorService
{
    Task<Doctor?> GetDoctorProfileAsync();
    Task<Doctor?> GetDoctorProfileAsync(int doctorId);
    Task<Doctor> SaveDoctorProfileAsync(Doctor doctor);
    Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto);
}
