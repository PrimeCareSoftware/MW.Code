using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IBlogPostRepository : IRepository<BlogPost>
    {
        Task<IEnumerable<BlogPost>> GetPublishedPostsAsync(int page = 1, int pageSize = 10);
        Task<IEnumerable<BlogPost>> GetAllPostsAsync(int page = 1, int pageSize = 10, bool publishedOnly = false);
        Task<BlogPost?> GetByIdAsync(Guid id);
        Task<BlogPost?> GetBySlugAsync(string slug);
        Task<IEnumerable<BlogPost>> GetByCategoryAsync(string category, bool publishedOnly = true);
        Task<int> GetTotalCountAsync(bool publishedOnly = true);
    }
}
