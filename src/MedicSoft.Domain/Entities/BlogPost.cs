using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class BlogPost : BaseEntity
    {
        public string Title { get; private set; }
        public string Slug { get; private set; }
        public string Content { get; private set; }
        public string Excerpt { get; private set; }
        public string Category { get; private set; }
        public string? FeaturedImage { get; private set; }
        public int ReadTimeMinutes { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime? PublishedAt { get; private set; }
        public string AuthorName { get; private set; }
        public Guid AuthorId { get; private set; }

        protected BlogPost() { }

        public BlogPost(
            string title,
            string content,
            string excerpt,
            string category,
            int readTimeMinutes,
            string authorName,
            Guid authorId,
            string? featuredImage = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Slug = GenerateSlug(title);
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Excerpt = excerpt ?? throw new ArgumentNullException(nameof(excerpt));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            ReadTimeMinutes = readTimeMinutes > 0 ? readTimeMinutes : throw new ArgumentException("Read time must be positive", nameof(readTimeMinutes));
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
            AuthorId = authorId;
            FeaturedImage = featuredImage;
            IsPublished = false;
            TenantId = "system"; // Blog posts are system-level, not tenant-specific
        }

        public void Update(
            string title,
            string content,
            string excerpt,
            string category,
            int readTimeMinutes,
            string? featuredImage = null)
        {
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Slug = GenerateSlug(title);
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Excerpt = excerpt ?? throw new ArgumentNullException(nameof(excerpt));
            Category = category ?? throw new ArgumentNullException(nameof(category));
            ReadTimeMinutes = readTimeMinutes > 0 ? readTimeMinutes : throw new ArgumentException("Read time must be positive", nameof(readTimeMinutes));
            FeaturedImage = featuredImage;
            UpdateTimestamp();
        }

        public void Publish()
        {
            if (!IsPublished)
            {
                IsPublished = true;
                PublishedAt = DateTime.UtcNow;
                UpdateTimestamp();
            }
        }

        public void Unpublish()
        {
            if (IsPublished)
            {
                IsPublished = false;
                UpdateTimestamp();
            }
        }

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));

            // Convert to lowercase and normalize
            var slug = title.ToLowerInvariant().Trim();

            // Replace common Portuguese accented characters
            var accentMap = new Dictionary<string, string>
            {
                { "á", "a" }, { "à", "a" }, { "â", "a" }, { "ã", "a" }, { "ä", "a" },
                { "é", "e" }, { "è", "e" }, { "ê", "e" }, { "ë", "e" },
                { "í", "i" }, { "ì", "i" }, { "î", "i" }, { "ï", "i" },
                { "ó", "o" }, { "ò", "o" }, { "ô", "o" }, { "õ", "o" }, { "ö", "o" },
                { "ú", "u" }, { "ù", "u" }, { "û", "u" }, { "ü", "u" },
                { "ç", "c" }, { "ñ", "n" }
            };

            foreach (var pair in accentMap)
            {
                slug = slug.Replace(pair.Key, pair.Value);
            }

            // Replace spaces and underscores with hyphens
            slug = slug.Replace(" ", "-").Replace("_", "-");

            // Remove all non-alphanumeric characters except hyphens
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Replace multiple consecutive hyphens with a single hyphen
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");

            // Remove leading and trailing hyphens
            slug = slug.Trim('-');

            return slug;
        }
    }
}
