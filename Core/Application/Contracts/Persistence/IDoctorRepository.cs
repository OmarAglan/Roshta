using Rosheta.Core.Domain.Entities;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IDoctorRepository : IRepository<Doctor>
{
    // Specific method to get the "primary" profile (usually the first one)
    Task<Doctor?> GetDoctorProfileAsync();

    // NOTE: SaveDoctorProfileAsync and UpdateDoctorProfileAsync(DTO) are removed.
    // The Service will use the generic AddAsync and UpdateAsync instead.
}