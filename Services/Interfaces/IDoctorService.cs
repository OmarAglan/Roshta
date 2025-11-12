using Rosheta.ViewModels;
using Rosheta.Models.Entities;
using System.Threading.Tasks;

namespace Rosheta.Services.Interfaces;

public interface IDoctorService
{
    Task<Doctor?> GetDoctorProfileAsync();
    Task<Doctor?> GetDoctorProfileAsync(int doctorId);
    Task<Doctor> SaveDoctorProfileAsync(Doctor doctor);
    Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto);
}
