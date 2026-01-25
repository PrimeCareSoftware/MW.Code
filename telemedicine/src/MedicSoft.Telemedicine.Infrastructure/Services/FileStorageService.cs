using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicSoft.Telemedicine.Application.Interfaces;
using System.Security.Cryptography;

namespace MedicSoft.Telemedicine.Infrastructure.Services;

/// <summary>
/// Local/Azure Blob file storage implementation
/// CFM 2.314/2022 - Secure storage for identity verification documents
/// TODO: For production, integrate with Azure Blob Storage or AWS S3
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<FileStorageService> _logger;
    private readonly string _storageBasePath;
    private readonly string _storageType; // "Local", "AzureBlob", "S3"
    
    // Allowed extensions for security
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
    private readonly string[] _allowedDocumentExtensions = { ".pdf" };
    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10MB
    
    public FileStorageService(IConfiguration configuration, ILogger<FileStorageService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Get storage configuration
        _storageType = _configuration["FileStorage:Type"] ?? "Local";
        _storageBasePath = _configuration["FileStorage:BasePath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "secure-storage");
        
        // Ensure base path exists for local storage
        if (_storageType == "Local" && !Directory.Exists(_storageBasePath))
        {
            Directory.CreateDirectory(_storageBasePath);
            _logger.LogInformation($"Created local storage directory: {_storageBasePath}");
        }
        
        _logger.LogInformation($"FileStorageService initialized with type: {_storageType}");
    }

    public async Task<string> SaveFileAsync(IFormFile file, string containerName, string fileName, bool encrypt = true)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null", nameof(file));

        // CFM 2.314/2022 Compliance: Identity documents MUST be encrypted
        if (!encrypt)
        {
            _logger.LogWarning("Attempting to save file without encryption. Forcing encryption for CFM 2.314/2022 compliance.");
            encrypt = true;
        }

        // Validate file
        await ValidateFileAsync(file, _allowedImageExtensions.Concat(_allowedDocumentExtensions).ToArray(), MaxFileSizeBytes);
        
        // Sanitize file name
        var sanitizedFileName = SanitizeFileName(fileName);
        var uniqueFileName = $"{Guid.NewGuid()}_{sanitizedFileName}";
        
        // Create container directory
        var containerPath = Path.Combine(_storageBasePath, containerName);
        if (!Directory.Exists(containerPath))
        {
            Directory.CreateDirectory(containerPath);
        }
        
        var filePath = Path.Combine(containerPath, uniqueFileName);
        
        try
        {
            // Always encrypt for CFM compliance
            await SaveEncryptedFileAsync(file, filePath);
            _logger.LogInformation($"Encrypted file saved: {uniqueFileName} in {containerName}");
            
            // Return relative path (container/filename)
            return $"{containerName}/{uniqueFileName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving file: {uniqueFileName}");
            throw;
        }
    }

    public async Task<Stream> GetFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path is required", nameof(filePath));

        var fullPath = Path.Combine(_storageBasePath, filePath);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning($"File not found: {filePath}");
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        try
        {
            // Check if file is encrypted (by metadata or naming convention)
            // For now, assume all files are encrypted
            return await DecryptFileAsync(fullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving file: {filePath}");
            throw;
        }
    }

    public async Task DeleteFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path is required", nameof(filePath));

        var fullPath = Path.Combine(_storageBasePath, filePath);
        
        if (!File.Exists(fullPath))
        {
            _logger.LogWarning($"File not found for deletion: {filePath}");
            return; // Already deleted
        }

        try
        {
            // LGPD Compliance: Soft delete by renaming with .deleted extension
            var deletedPath = $"{fullPath}.deleted.{DateTime.UtcNow:yyyyMMddHHmmss}";
            File.Move(fullPath, deletedPath);
            
            _logger.LogInformation($"File soft-deleted: {filePath}");
            
            // TODO: Schedule permanent deletion after retention period
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting file: {filePath}");
            throw;
        }
    }

    public async Task<string> GetTemporaryAccessUrlAsync(string filePath, int expirationMinutes = 60)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path is required", nameof(filePath));

        // For local storage, return a token-based URL
        // In production with Azure/S3, this would generate a SAS token
        var token = GenerateAccessToken(filePath, expirationMinutes);
        
        // TODO: In production, integrate with Azure Blob SAS or S3 pre-signed URLs
        var baseUrl = _configuration["FileStorage:BaseUrl"] ?? "http://localhost:5000/api/files";
        var temporaryUrl = $"{baseUrl}/{filePath}?token={token}&expires={DateTime.UtcNow.AddMinutes(expirationMinutes):O}";
        
        _logger.LogInformation($"Generated temporary access URL for: {filePath} (expires in {expirationMinutes} min)");
        
        return await Task.FromResult(temporaryUrl);
    }

    public async Task<bool> ValidateFileAsync(IFormFile file, string[] allowedExtensions, long maxSizeBytes)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is empty or null");

        // Check file size
        if (file.Length > maxSizeBytes)
        {
            throw new InvalidOperationException($"File size exceeds maximum allowed size of {maxSizeBytes / (1024 * 1024)}MB");
        }

        // Check file extension
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
        }

        // Check for null bytes (potential exploit)
        if (file.FileName.Contains('\0'))
        {
            throw new InvalidOperationException("File name contains invalid characters");
        }

        // TODO: Add virus scanning integration (ClamAV, Azure Antimalware)
        _logger.LogInformation($"File validated: {file.FileName} ({file.Length} bytes)");
        
        return await Task.FromResult(true);
    }

    #region Private Helper Methods

    private string SanitizeFileName(string fileName)
    {
        // Remove path traversal attempts and special characters
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName.Where(ch => !invalidChars.Contains(ch) && ch != '/' && ch != '\\').ToArray());
        
        // Limit length - use modern string slicing
        if (sanitized.Length > 200)
            sanitized = sanitized[..200];
        
        return sanitized;
    }

    private async Task SaveEncryptedFileAsync(IFormFile file, string filePath)
    {
        // Simple encryption using AES
        // TODO: Integrate with Azure Key Vault or AWS KMS for production
        using var aes = Aes.Create();
        
        // Generate key and IV (in production, retrieve from Key Vault)
        // For demo purposes, using a derived key from configuration
        var encryptionKey = GetEncryptionKey();
        aes.Key = encryptionKey;
        aes.GenerateIV();
        
        // Save IV at the beginning of the file (first 16 bytes)
        using var outputStream = new FileStream(filePath, FileMode.Create);
        await outputStream.WriteAsync(aes.IV, 0, aes.IV.Length);
        
        // Encrypt and save file content
        using var cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await file.CopyToAsync(cryptoStream);
    }

    private async Task<Stream> DecryptFileAsync(string filePath)
    {
        // Read encrypted file
        var inputStream = new FileStream(filePath, FileMode.Open);
        
        try
        {
            using var aes = Aes.Create();
            
            // Read IV from file (first 16 bytes)
            var iv = new byte[16];
            await inputStream.ReadAsync(iv, 0, 16);
            aes.IV = iv;
            
            // Get encryption key
            var encryptionKey = GetEncryptionKey();
            aes.Key = encryptionKey;
            
            // Decrypt file to memory stream
            var memoryStream = new MemoryStream();
            using (var cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
            {
                await cryptoStream.CopyToAsync(memoryStream);
            }
            
            memoryStream.Position = 0;
            return memoryStream;
        }
        finally
        {
            // Dispose inputStream after decryption is complete
            inputStream.Dispose();
        }
    }

    private byte[] GetEncryptionKey()
    {
        // Retrieve from Azure Key Vault or AWS KMS in production
        var keyString = _configuration["FileStorage:EncryptionKey"];
        
        if (string.IsNullOrWhiteSpace(keyString))
        {
            throw new InvalidOperationException(
                "FileStorage:EncryptionKey configuration is required for security. " +
                "Please configure a secure encryption key in appsettings.json or environment variables. " +
                "For production, use Azure Key Vault or AWS KMS.");
        }
        
        // Derive a 256-bit key
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(keyString));
    }

    private string GenerateAccessToken(string filePath, int expirationMinutes)
    {
        // Generate a simple HMAC token
        // In production, use JWT with Azure AD or similar
        var message = $"{filePath}|{DateTime.UtcNow.AddMinutes(expirationMinutes):O}";
        
        using var hmac = new HMACSHA256(GetEncryptionKey());
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hash);
    }

    #endregion
}
