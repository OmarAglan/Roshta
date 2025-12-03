using Rosheta.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IMedicationRepository : IRepository<Medication>
{
    // Specific methods that are NOT in the generic repository
    Task<IEnumerable<Medication>> SearchAsync(string searchTerm);

    // Pagination methods
    Task<List<Medication>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
    Task<int> GetCountAsync(string? searchTerm = null);

    // Validation methods
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}