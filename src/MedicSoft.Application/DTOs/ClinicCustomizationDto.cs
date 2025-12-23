namespace MedicSoft.Application.DTOs
{
    public class ClinicCustomizationDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string? LogoUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? FontColor { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateClinicCustomizationRequest
    {
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? FontColor { get; set; }
    }

    public class ClinicCustomizationPublicDto
    {
        public string? LogoUrl { get; set; }
        public string? BackgroundImageUrl { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? FontColor { get; set; }
        public string? ClinicName { get; set; }
    }
}
