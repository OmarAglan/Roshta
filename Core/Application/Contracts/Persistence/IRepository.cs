using Rosheta.Core.Domain.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Rosheta.Core.Application.Contracts.Persistence;

/// <summary>
/// Generic repository interface defining standard CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type, must inherit from BaseEntity.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Gets an entity by its unique identifier.
    /// </summary>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Gets all entities of type T.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync();

    /// <summary>
    /// Adds a new entity to the database.
    /// </summary>
    Task<T> AddAsync(T entity);

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    Task UpdateAsync(T entity);

    /// <summary>
    /// Deletes an entity.
    /// </summary>
    Task DeleteAsync(T entity);

    /// <summary>
    /// Checks if an entity exists by ID.
    /// </summary>
    Task<bool> ExistsAsync(int id);
}