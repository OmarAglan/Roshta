using Rosheta.Models.Entities;
using Rosheta.ViewModels;
using System.Threading.Tasks;

namespace Rosheta.Repositories.Interfaces;

public interface IDoctorRepository
{
    /// <summary>
    /// Gets the single Doctor profile stored in the database.
    /// </summary>
    /// <returns>The Doctor profile, or null if none exists.</returns>
    Task<Doctor?> GetDoctorProfileAsync();

    /// <summary>
    /// Gets the Doctor profile stored in the database by ID.
    /// </summary>
    /// <param name="doctorId">The ID of the Doctor to retrieve.</param>
    /// <returns>The Doctor profile, or null if none exists.</returns>
    Task<Doctor?> GetDoctorProfileAsync(int doctorId);

    /// <summary>
    /// Saves the Doctor profile (creates if none exists, updates if one does).
    /// </summary>
    /// <param name="doctor">The Doctor profile to save.</param>
    /// <returns>The saved Doctor profile (with ID assigned if new).</returns>
    Task<Doctor> SaveDoctorProfileAsync(Doctor doctor);

    /// <summary>
    /// Updates the Doctor profile stored in the database.
    /// </summary>
    /// <param name="doctorId">The ID of the Doctor to update.</param>
    /// <param name="profileDto">The updated Doctor profile data transfer object.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    Task<bool> UpdateDoctorProfileAsync(int doctorId, UpdateDoctorProfileDto profileDto);
}
