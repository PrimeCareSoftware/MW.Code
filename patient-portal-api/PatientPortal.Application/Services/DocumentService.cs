using PatientPortal.Application.DTOs.Documents;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Application.Services;

public class DocumentService : IDocumentService
{
    private readonly IDocumentViewRepository _documentViewRepository;
    private readonly IPatientUserRepository _patientUserRepository;

    public DocumentService(
        IDocumentViewRepository documentViewRepository,
        IPatientUserRepository patientUserRepository)
    {
        _documentViewRepository = documentViewRepository;
        _patientUserRepository = patientUserRepository;
    }

    public async Task<DocumentDto?> GetByIdAsync(Guid documentId, Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return null;

        var document = await _documentViewRepository.GetByIdAsync(documentId, patientUser.PatientId);
        
        return document == null ? null : MapToDto(document);
    }

    public async Task<List<DocumentDto>> GetMyDocumentsAsync(Guid patientUserId, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<DocumentDto>();

        var documents = await _documentViewRepository.GetByPatientIdAsync(patientUser.PatientId, skip, take);
        
        return documents.Select(MapToDto).ToList();
    }

    public async Task<List<DocumentDto>> GetByTypeAsync(Guid patientUserId, DocumentType documentType, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<DocumentDto>();

        var documents = await _documentViewRepository.GetByTypeAsync(patientUser.PatientId, documentType, skip, take);
        
        return documents.Select(MapToDto).ToList();
    }

    public async Task<List<DocumentDto>> GetRecentDocumentsAsync(Guid patientUserId, int take = 10)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<DocumentDto>();

        var documents = await _documentViewRepository.GetRecentByPatientIdAsync(patientUser.PatientId, take);
        
        return documents.Select(MapToDto).ToList();
    }

    public async Task<int> GetCountAsync(Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return 0;

        return await _documentViewRepository.GetCountByPatientIdAsync(patientUser.PatientId);
    }

    public async Task<byte[]?> DownloadDocumentAsync(Guid documentId, Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return null;

        var document = await _documentViewRepository.GetByIdAsync(documentId, patientUser.PatientId);
        
        if (document == null)
            return null;

        // TODO: Implement actual file download from storage system
        // This should integrate with the main application's file storage (e.g., Azure Blob Storage, S3, or local filesystem)
        // For now, return null - implementation depends on the storage strategy used by PrimeCare Software
        // Example implementation:
        // - Get file path from document.FileUrl
        // - Read file from storage
        // - Return byte array
        return null;
    }

    private static DocumentDto MapToDto(Domain.Entities.DocumentView document)
    {
        return new DocumentDto
        {
            Id = document.Id,
            Title = document.Title,
            DocumentType = document.DocumentType.ToString(),
            Description = document.Description,
            DoctorName = document.DoctorName,
            IssuedDate = document.IssuedDate,
            FileUrl = document.FileUrl,
            FileName = document.FileName,
            FileSizeFormatted = FormatFileSize(document.FileSizeBytes),
            IsAvailable = document.IsAvailable
        };
    }

    private static string? FormatFileSize(long? bytes)
    {
        if (!bytes.HasValue)
            return null;

        string[] sizes = { "B", "KB", "MB", "GB" };
        double len = bytes.Value;
        int order = 0;
        
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }
}
