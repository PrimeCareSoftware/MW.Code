using PatientPortal.Domain.Enums;

namespace PatientPortal.Application.DTOs.Documents;

/// <summary>
/// DTO for medical document details
/// </summary>
public class DocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileSizeFormatted { get; set; }
    public bool IsAvailable { get; set; }
}
