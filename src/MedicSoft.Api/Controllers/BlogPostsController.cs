using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/blog-posts")]
    public class BlogPostsController : BaseController
    {
        private readonly IBlogPostRepository _blogPostRepository;

        public BlogPostsController(IBlogPostRepository blogPostRepository, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _blogPostRepository = blogPostRepository;
        }

        /// <summary>
        /// Get published blog posts (public access)
        /// </summary>
        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogPostsListResponse>> GetPublishedPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var posts = await _blogPostRepository.GetPublishedPostsAsync(page, pageSize);
            var totalCount = await _blogPostRepository.GetTotalCountAsync(publishedOnly: true);

            var response = new BlogPostsListResponse
            {
                Posts = posts.Select(p => new BlogPostSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Excerpt = p.Excerpt,
                    Category = p.Category,
                    FeaturedImage = p.FeaturedImage,
                    ReadTimeMinutes = p.ReadTimeMinutes,
                    IsPublished = p.IsPublished,
                    PublishedAt = p.PublishedAt,
                    AuthorName = p.AuthorName,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Get blog post by ID (public if published)
        /// </summary>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogPostDto>> GetBlogPost(Guid id)
        {
            var post = await _blogPostRepository.GetByIdAsync(id);

            if (post == null)
                return NotFound(new { message = "Post não encontrado" });

            // Only allow public access to published posts
            if (!post.IsPublished && !IsSystemOwner())
                return NotFound(new { message = "Post não encontrado" });

            var dto = new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Category = post.Category,
                FeaturedImage = post.FeaturedImage,
                ReadTimeMinutes = post.ReadTimeMinutes,
                IsPublished = post.IsPublished,
                PublishedAt = post.PublishedAt,
                AuthorName = post.AuthorName,
                AuthorId = post.AuthorId,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Get blog post by slug (public if published)
        /// </summary>
        [HttpGet("slug/{slug}")]
        [AllowAnonymous]
        public async Task<ActionResult<BlogPostDto>> GetBlogPostBySlug(string slug)
        {
            var post = await _blogPostRepository.GetBySlugAsync(slug);

            if (post == null)
                return NotFound(new { message = "Post não encontrado" });

            // Only allow public access to published posts
            if (!post.IsPublished && !IsSystemOwner())
                return NotFound(new { message = "Post não encontrado" });

            var dto = new BlogPostDto
            {
                Id = post.Id,
                Title = post.Title,
                Slug = post.Slug,
                Content = post.Content,
                Excerpt = post.Excerpt,
                Category = post.Category,
                FeaturedImage = post.FeaturedImage,
                ReadTimeMinutes = post.ReadTimeMinutes,
                IsPublished = post.IsPublished,
                PublishedAt = post.PublishedAt,
                AuthorName = post.AuthorName,
                AuthorId = post.AuthorId,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Get all blog posts (admin only - includes unpublished)
        /// </summary>
        [HttpGet("admin/all")]
        [Authorize]
        public async Task<ActionResult<BlogPostsListResponse>> GetAllPosts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool publishedOnly = false)
        {
            if (!IsSystemOwner())
                return Forbid();

            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            var posts = await _blogPostRepository.GetAllPostsAsync(page, pageSize, publishedOnly);
            var totalCount = await _blogPostRepository.GetTotalCountAsync(publishedOnly);

            var response = new BlogPostsListResponse
            {
                Posts = posts.Select(p => new BlogPostSummaryDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    Excerpt = p.Excerpt,
                    Category = p.Category,
                    FeaturedImage = p.FeaturedImage,
                    ReadTimeMinutes = p.ReadTimeMinutes,
                    IsPublished = p.IsPublished,
                    PublishedAt = p.PublishedAt,
                    AuthorName = p.AuthorName,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Ok(response);
        }

        /// <summary>
        /// Create a new blog post (admin only)
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BlogPostDto>> CreateBlogPost([FromBody] CreateBlogPostRequest request)
        {
            if (!IsSystemOwner())
                return Forbid();

            var userId = GetUserIdFromClaims();
            if (!userId.HasValue)
                return Unauthorized();

            var userName = GetUsername() ?? "System Admin";

            var blogPost = new BlogPost(
                request.Title,
                request.Content,
                request.Excerpt,
                request.Category,
                request.ReadTimeMinutes,
                userName,
                userId.Value,
                request.FeaturedImage
            );

            await _blogPostRepository.AddAsync(blogPost);
            await _blogPostRepository.SaveChangesAsync();

            var dto = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Slug = blogPost.Slug,
                Content = blogPost.Content,
                Excerpt = blogPost.Excerpt,
                Category = blogPost.Category,
                FeaturedImage = blogPost.FeaturedImage,
                ReadTimeMinutes = blogPost.ReadTimeMinutes,
                IsPublished = blogPost.IsPublished,
                PublishedAt = blogPost.PublishedAt,
                AuthorName = blogPost.AuthorName,
                AuthorId = blogPost.AuthorId,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt
            };

            return CreatedAtAction(nameof(GetBlogPost), new { id = blogPost.Id }, dto);
        }

        /// <summary>
        /// Update a blog post (admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogPostDto>> UpdateBlogPost(Guid id, [FromBody] UpdateBlogPostRequest request)
        {
            if (!IsSystemOwner())
                return Forbid();

            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
                return NotFound(new { message = "Post não encontrado" });

            blogPost.Update(
                request.Title,
                request.Content,
                request.Excerpt,
                request.Category,
                request.ReadTimeMinutes,
                request.FeaturedImage
            );

            await _blogPostRepository.UpdateAsync(blogPost);
            await _blogPostRepository.SaveChangesAsync();

            var dto = new BlogPostDto
            {
                Id = blogPost.Id,
                Title = blogPost.Title,
                Slug = blogPost.Slug,
                Content = blogPost.Content,
                Excerpt = blogPost.Excerpt,
                Category = blogPost.Category,
                FeaturedImage = blogPost.FeaturedImage,
                ReadTimeMinutes = blogPost.ReadTimeMinutes,
                IsPublished = blogPost.IsPublished,
                PublishedAt = blogPost.PublishedAt,
                AuthorName = blogPost.AuthorName,
                AuthorId = blogPost.AuthorId,
                CreatedAt = blogPost.CreatedAt,
                UpdatedAt = blogPost.UpdatedAt
            };

            return Ok(dto);
        }

        /// <summary>
        /// Publish or unpublish a blog post (admin only)
        /// </summary>
        [HttpPatch("{id}/publish")]
        [Authorize]
        public async Task<ActionResult> PublishBlogPost(Guid id, [FromBody] PublishBlogPostRequest request)
        {
            if (!IsSystemOwner())
                return Forbid();

            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
                return NotFound(new { message = "Post não encontrado" });

            if (request.IsPublished)
                blogPost.Publish();
            else
                blogPost.Unpublish();

            await _blogPostRepository.UpdateAsync(blogPost);
            await _blogPostRepository.SaveChangesAsync();

            return Ok(new { message = $"Post {(request.IsPublished ? "publicado" : "despublicado")} com sucesso" });
        }

        /// <summary>
        /// Delete a blog post (admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteBlogPost(Guid id)
        {
            if (!IsSystemOwner())
                return Forbid();

            var blogPost = await _blogPostRepository.GetByIdAsync(id);
            if (blogPost == null)
                return NotFound(new { message = "Post não encontrado" });

            await _blogPostRepository.DeleteAsync(blogPost);
            await _blogPostRepository.SaveChangesAsync();

            return Ok(new { message = "Post excluído com sucesso" });
        }

        /// <summary>
        /// Get posts by category (public - published only)
        /// </summary>
        [HttpGet("category/{category}")]
        [AllowAnonymous]
        public async Task<ActionResult<List<BlogPostSummaryDto>>> GetPostsByCategory(string category)
        {
            var posts = await _blogPostRepository.GetByCategoryAsync(category, publishedOnly: true);

            var dtos = posts.Select(p => new BlogPostSummaryDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                Excerpt = p.Excerpt,
                Category = p.Category,
                FeaturedImage = p.FeaturedImage,
                ReadTimeMinutes = p.ReadTimeMinutes,
                IsPublished = p.IsPublished,
                PublishedAt = p.PublishedAt,
                AuthorName = p.AuthorName,
                CreatedAt = p.CreatedAt
            }).ToList();

            return Ok(dtos);
        }
    }
}
