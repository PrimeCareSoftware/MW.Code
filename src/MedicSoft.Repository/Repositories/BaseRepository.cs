using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly MedicSoftDbContext _context;
        protected readonly DbSet<T> _dbSet;

        protected BaseRepository(MedicSoftDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Id == id && e.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId)
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        /// <summary>
        /// Adds an entity to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction
        /// to avoid issues with retrying execution strategies like NpgsqlRetryingExecutionStrategy.
        /// </summary>
        public virtual async Task<T> AddWithoutSaveAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            return entity;
        }

        /// <summary>
        /// Saves all pending changes to the database.
        /// Call this after batching multiple AddWithoutSaveAsync operations.
        /// </summary>
        public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);
            
            // If entity is detached, attach and mark as modified
            if (entry.State == EntityState.Detached)
            {
                _dbSet.Update(entity);
            }
            
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(Guid id, string tenantId)
        {
            var entity = await GetByIdAsync(id, tenantId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Marks an entity for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction
        /// to avoid issues with retrying execution strategies like NpgsqlRetryingExecutionStrategy.
        /// </summary>
        public virtual async Task DeleteWithoutSaveAsync(Guid id, string tenantId)
        {
            var entity = await GetByIdAsync(id, tenantId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .AnyAsync(e => e.Id == id && e.TenantId == tenantId);
        }

        public virtual async Task<int> CountAsync(string tenantId)
        {
            return await _dbSet
                .CountAsync(e => e.TenantId == tenantId);
        }

        /// <summary>
        /// Executes an operation within a database transaction. If the operation succeeds, 
        /// the transaction is committed. If an error occurs, the transaction is rolled back
        /// and the exception is re-thrown. This method is compatible with retrying execution
        /// strategies like NpgsqlRetryingExecutionStrategy.
        /// </summary>
        /// <typeparam name="TResult">The return type of the operation</typeparam>
        /// <param name="operation">The operation to execute within the transaction</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <returns>The result of the operation</returns>
        /// <exception cref="Exception">Re-throws any exception that occurs during the operation after rollback</exception>
        public virtual async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            // Check if there's already an active transaction
            if (_context.Database.CurrentTransaction != null)
            {
                // If already in a transaction, just execute the operation
                return await operation();
            }

            // Use execution strategy to handle retrying execution strategies (e.g., NpgsqlRetryingExecutionStrategy)
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var result = await operation();
                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        /// <summary>
        /// Executes an operation within a database transaction. If the operation succeeds, 
        /// the transaction is committed. If an error occurs, the transaction is rolled back
        /// and the exception is re-thrown. This method is compatible with retrying execution
        /// strategies like NpgsqlRetryingExecutionStrategy.
        /// </summary>
        /// <param name="operation">The operation to execute within the transaction</param>
        /// <param name="cancellationToken">Cancellation token to cancel the operation</param>
        /// <exception cref="Exception">Re-throws any exception that occurs during the operation after rollback</exception>
        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            // Check if there's already an active transaction
            if (_context.Database.CurrentTransaction != null)
            {
                // If already in a transaction, just execute the operation
                await operation();
                return;
            }

            // Use execution strategy to handle retrying execution strategies (e.g., NpgsqlRetryingExecutionStrategy)
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    await operation();
                    await transaction.CommitAsync(cancellationToken);
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            });
        }

        /// <summary>
        /// Gets an entity by ID without tenant filtering.
        /// Use only for public APIs where tenant isolation is not required.
        /// </summary>
        public virtual async Task<T?> GetByIdWithoutTenantAsync(Guid id)
        {
            return await _dbSet
                .Where(e => e.Id == id)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets a queryable for custom queries.
        /// Use with caution - ensure tenant filtering is applied when needed.
        /// </summary>
        public virtual IQueryable<T> GetAllQueryable()
        {
            return _dbSet.AsQueryable();
        }
    }
}