using PatientPortal.Application.DTOs.Documents;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Document service interface
/// </summary>
public interface IDocumentService
{
    Task<DocumentDto?> GetByIdAsync(Guid documentId, Guid patientUserId);
    Task<List<DocumentDto>> GetMyDocumentsAsync(Guid patientUserId, int skip = 0, int take = 50);
    Task<List<DocumentDto>> GetByTypeAsync(Guid patientUserId, DocumentType documentType, int skip = 0, int take = 50);
    Task<List<DocumentDto>> GetRecentDocumentsAsync(Guid patientUserId, int take = 10);
    Task<int> GetCountAsync(Guid patientUserId);
    Task<byte[]?> DownloadDocumentAsync(Guid documentId, Guid patientUserId);
}
