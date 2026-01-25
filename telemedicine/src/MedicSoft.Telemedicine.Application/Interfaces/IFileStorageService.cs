using Microsoft.AspNetCore.Http;

namespace MedicSoft.Telemedicine.Application.Interfaces;

/// <summary>
/// Interface for secure file storage operations (Azure Blob, AWS S3, etc)
/// CFM 2.314/2022 - Verificação de Identidade
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file securely with encryption
    /// </summary>
    /// <param name="file">File to upload</param>
    /// <param name="containerName">Container/bucket name (e.g., "identity-documents")</param>
    /// <param name="fileName">Desired file name (will be sanitized)</param>
    /// <param name="encrypt">Whether to encrypt the file (default: true)</param>
    /// <returns>Secure URL or path to the stored file</returns>
    Task<string> SaveFileAsync(IFormFile file, string containerName, string fileName, bool encrypt = true);
    
    /// <summary>
    /// Retrieves a file from storage
    /// </summary>
    /// <param name="filePath">Path or URL returned from SaveFileAsync</param>
    /// <returns>File stream</returns>
    Task<Stream> GetFileAsync(string filePath);
    
    /// <summary>
    /// Deletes a file (soft delete for LGPD compliance)
    /// </summary>
    /// <param name="filePath">Path or URL to delete</param>
    Task DeleteFileAsync(string filePath);
    
    /// <summary>
    /// Generates a time-limited access URL (SAS token) for a file
    /// </summary>
    /// <param name="filePath">Path or URL to the file</param>
    /// <param name="expirationMinutes">Minutes until URL expires (default: 60)</param>
    /// <returns>Temporary access URL</returns>
    Task<string> GetTemporaryAccessUrlAsync(string filePath, int expirationMinutes = 60);
    
    /// <summary>
    /// Validates file type and size before upload
    /// </summary>
    /// <param name="file">File to validate</param>
    /// <param name="allowedExtensions">Allowed file extensions (e.g., [".jpg", ".png", ".pdf"])</param>
    /// <param name="maxSizeBytes">Maximum file size in bytes</param>
    /// <returns>True if valid, throws exception if invalid</returns>
    Task<bool> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeBytes);
}
