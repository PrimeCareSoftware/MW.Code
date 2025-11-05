using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id, string tenantId);
        Task<bool> ExistsAsync(Guid id, string tenantId);
        Task<int> CountAsync(string tenantId);
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation);
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}