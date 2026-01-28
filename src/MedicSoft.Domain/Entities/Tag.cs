using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a tag for categorizing and segmenting clinics.
    /// Tags can be assigned manually or automatically based on rules.
    /// </summary>
    public class Tag : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string Category { get; private set; } // type, region, value, status, custom
        public string Color { get; private set; } // Hex color for UI display
        public bool IsAutomatic { get; private set; } // If true, applied by background job
        public string? AutomationRules { get; private set; } // JSON rules for automatic assignment
        public int Order { get; private set; } // Display order within category

        private Tag()
        {
            // EF Constructor
            Name = null!;
            Category = null!;
            Color = null!;
        }

        public Tag(string name, string category, string color, string tenantId,
            string? description = null, bool isAutomatic = false, string? automationRules = null, int order = 0)
            : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Tag category cannot be empty", nameof(category));

            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Tag color cannot be empty", nameof(color));

            Name = name;
            Description = description;
            Category = category;
            Color = color;
            IsAutomatic = isAutomatic;
            AutomationRules = automationRules;
            Order = order;
        }

        public void Update(string name, string category, string color, string? description = null, int order = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tag name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Tag category cannot be empty", nameof(category));

            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Tag color cannot be empty", nameof(color));

            Name = name;
            Category = category;
            Color = color;
            Description = description;
            Order = order;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAutomation(bool isAutomatic, string? automationRules = null)
        {
            IsAutomatic = isAutomatic;
            AutomationRules = automationRules;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
