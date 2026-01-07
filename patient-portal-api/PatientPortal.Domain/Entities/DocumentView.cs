using PatientPortal.Domain.Enums;

namespace PatientPortal.Domain.Entities;

/// <summary>
/// Read-only view of medical documents for patients
/// </summary>
public class DocumentView
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public DocumentType DocumentType { get; set; }
    public string? Description { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime IssuedDate { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public long? FileSizeBytes { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}
