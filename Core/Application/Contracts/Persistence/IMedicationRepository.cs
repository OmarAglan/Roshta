using Rosheta.Core.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

// Inherit from IRepository<Medication>
public interface IMedicationRepository : IRepository<Medication>
{
    // Custom Search/Filter methods specific to Medication
    Task<IEnumerable<Medication>> SearchAsync(string searchTerm);
    Task<List<Medication>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm = null, string? sortOrder = null);
    Task<int> GetCountAsync(string? searchTerm = null);
    Task<bool> IsNameUniqueAsync(string name, int? currentId = null);
}