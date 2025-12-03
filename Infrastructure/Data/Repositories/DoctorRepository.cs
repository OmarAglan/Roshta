using Microsoft.EntityFrameworkCore;
using Rosheta.Core.Application.Contracts.Persistence;
using Rosheta.Core.Domain.Entities;
using Rosheta.Infrastructure.Data;
using System.Threading.Tasks;

namespace Rosheta.Infrastructure.Data.Repositories;

public class DoctorRepository : RepositoryBase<Doctor>, IDoctorRepository
{
    public DoctorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Doctor?> GetDoctorProfileAsync()
    {
        // Return the first doctor found, or null
        return await _dbSet.FirstOrDefaultAsync();
    }
}