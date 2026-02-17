using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class BlogPostRepository : BaseRepository<BlogPost>, IBlogPostRepository
    {
        public BlogPostRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page = 1, int pageSize = 10)
        {
            var skip = (page - 1) * pageSize;
            return await _dbSet
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.PublishedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlogPost>> GetAllPostsAsync(int page = 1, int pageSize = 10, bool publishedOnly = false)
        {
            var skip = (page - 1) * pageSize;
            var query = _dbSet.AsQueryable();

            if (publishedOnly)
                query = query.Where(p => p.IsPublished);

            return await query
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<BlogPost?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<BlogPost?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .FirstOrDefaultAsync(p => p.Slug == slug);
        }

        public async Task<IEnumerable<BlogPost>> GetByCategoryAsync(string category, bool publishedOnly = true)
        {
            var query = _dbSet.AsQueryable();

            if (publishedOnly)
                query = query.Where(p => p.IsPublished);

            return await query
                .Where(p => p.Category == category)
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(bool publishedOnly = true)
        {
            var query = _dbSet.AsQueryable();

            if (publishedOnly)
                query = query.Where(p => p.IsPublished);

            return await query.CountAsync();
        }
    }
}
