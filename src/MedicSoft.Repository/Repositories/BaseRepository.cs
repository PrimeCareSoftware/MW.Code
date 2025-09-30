using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public virtual async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
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
    }
}