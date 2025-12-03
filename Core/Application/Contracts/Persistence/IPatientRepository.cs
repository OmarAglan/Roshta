using Rosheta.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IPatientRepository : IRepository<Patient>
{
    // Specific methods NOT in generic repository
    Task<IEnumerable<Patient>> SearchAsync(string searchTerm);

    // Pagination
    Task<List<Patient>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
    Task<int> GetCountAsync(string? searchTerm = null);

    // Validations
    Task<bool> IsContactInfoUniqueAsync(string contactInfo, int? currentId = null);
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}