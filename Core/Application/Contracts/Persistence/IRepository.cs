using Rosheta.Core.Domain.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> ExistsAsync(int id);
}