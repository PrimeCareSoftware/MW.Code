using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for DocumentView (read-only)
/// </summary>
public interface IDocumentViewRepository
{
    Task<DocumentView?> GetByIdAsync(Guid id, Guid patientId);
    Task<List<DocumentView>> GetByPatientIdAsync(Guid patientId, int skip = 0, int take = 50);
    Task<List<DocumentView>> GetByTypeAsync(Guid patientId, DocumentType documentType, int skip = 0, int take = 50);
    Task<List<DocumentView>> GetRecentByPatientIdAsync(Guid patientId, int take = 10);
    Task<int> GetCountByPatientIdAsync(Guid patientId);
}
