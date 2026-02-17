using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents customization settings for a clinic's branding and appearance
    /// </summary>
    public class ClinicCustomization : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public string? LogoUrl { get; private set; }
        public string? BackgroundImageUrl { get; private set; }
        public string? PrimaryColor { get; private set; }
        public string? SecondaryColor { get; private set; }
        public string? FontColor { get; private set; }

        // Navigation property
        public virtual Clinic? Clinic { get; private set; }

        private ClinicCustomization()
        {
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
        }

        public ClinicCustomization(Guid clinicId, string tenantId) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            ClinicId = clinicId;
            
            // Set default colors
            PrimaryColor = "#2563eb"; // Blue
            SecondaryColor = "#7c3aed"; // Purple
            FontColor = "#1f2937"; // Dark gray

            // Ensure UpdatedAt is populated for DB constraints
            UpdateTimestamp();
        }

        public void UpdateColors(string? primaryColor, string? secondaryColor, string? fontColor)
        {
            // Validate color format if provided (basic hex color validation)
            if (!string.IsNullOrWhiteSpace(primaryColor) && !IsValidHexColor(primaryColor))
                throw new ArgumentException("Invalid primary color format. Use hex format like #RRGGBB", nameof(primaryColor));
            
            if (!string.IsNullOrWhiteSpace(secondaryColor) && !IsValidHexColor(secondaryColor))
                throw new ArgumentException("Invalid secondary color format. Use hex format like #RRGGBB", nameof(secondaryColor));
            
            if (!string.IsNullOrWhiteSpace(fontColor) && !IsValidHexColor(fontColor))
                throw new ArgumentException("Invalid font color format. Use hex format like #RRGGBB", nameof(fontColor));

            PrimaryColor = string.IsNullOrWhiteSpace(primaryColor) ? null : primaryColor.Trim();
            SecondaryColor = string.IsNullOrWhiteSpace(secondaryColor) ? null : secondaryColor.Trim();
            FontColor = string.IsNullOrWhiteSpace(fontColor) ? null : fontColor.Trim();
            UpdateTimestamp();
        }

        public void SetLogoUrl(string? logoUrl)
        {
            LogoUrl = string.IsNullOrWhiteSpace(logoUrl) ? null : logoUrl.Trim();
            UpdateTimestamp();
        }

        public void SetBackgroundImageUrl(string? backgroundImageUrl)
        {
            BackgroundImageUrl = string.IsNullOrWhiteSpace(backgroundImageUrl) ? null : backgroundImageUrl.Trim();
            UpdateTimestamp();
        }

        private static bool IsValidHexColor(string color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return false;

            var colorTrimmed = color.Trim();
            
            // Check if it matches #RGB or #RRGGBB format
            return System.Text.RegularExpressions.Regex.IsMatch(
                colorTrimmed, 
                "^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"
            );
        }
    }
}
