using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<T>> GetAllAsync(string tenantId);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string tenantId);
        Task<T> AddAsync(T entity);
        /// <summary>
        /// Adds an entity to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction
        /// to avoid issues with retrying execution strategies.
        /// </summary>
        Task<T> AddWithoutSaveAsync(T entity);
        /// <summary>
        /// Saves all pending changes to the database.
        /// Call this after batching multiple AddWithoutSaveAsync or DeleteWithoutSaveAsync operations.
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id, string tenantId);
        /// <summary>
        /// Marks an entity for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction
        /// to avoid issues with retrying execution strategies.
        /// </summary>
        Task DeleteWithoutSaveAsync(Guid id, string tenantId);
        Task<bool> ExistsAsync(Guid id, string tenantId);
        Task<int> CountAsync(string tenantId);
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Gets an entity by ID without tenant filtering.
        /// Use only for public APIs where tenant isolation is not required.
        /// </summary>
        Task<T?> GetByIdWithoutTenantAsync(Guid id);
        
        /// <summary>
        /// Gets a queryable for custom queries.
        /// Use with caution - ensure tenant filtering is applied when needed.
        /// </summary>
        IQueryable<T> GetAllQueryable();
    }
}