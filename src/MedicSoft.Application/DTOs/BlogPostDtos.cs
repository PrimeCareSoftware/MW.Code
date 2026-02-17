using System;

namespace MedicSoft.Application.DTOs
{
    public class BlogPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public Guid AuthorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class BlogPostSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public int ReadTimeMinutes { get; set; }
        public bool IsPublished { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateBlogPostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public int ReadTimeMinutes { get; set; }
    }

    public class UpdateBlogPostRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Excerpt { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? FeaturedImage { get; set; }
        public int ReadTimeMinutes { get; set; }
    }

    public class PublishBlogPostRequest
    {
        public bool IsPublished { get; set; }
    }

    public class BlogPostsListResponse
    {
        public List<BlogPostSummaryDto> Posts { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
