using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services.DigitalSignature
{
    /// <summary>
    /// Service for digital signature operations on medical documents (ICP-Brasil compliant).
    /// Implements CFM 1.821/2007 requirements for digital signatures.
    /// </summary>
    public interface IAssinaturaDigitalService
    {
        Task<ResultadoAssinatura> AssinarDocumentoAsync(
            Guid documentoId,
            TipoDocumento tipoDocumento,
            Guid medicoId,
            byte[] documentoBytes,
            string? senhaCertificado = null);

        Task<ResultadoValidacao> ValidarAssinaturaAsync(Guid assinaturaId);

        Task<List<AssinaturaDigitalDto>> ObterAssinaturasPorDocumentoAsync(
            Guid documentoId,
            TipoDocumento tipoDocumento);
    }

    /// <summary>
    /// Implementation of digital signature service using ICP-Brasil certificates and PKCS#7 standard.
    /// </summary>
    public class AssinaturaDigitalService : IAssinaturaDigitalService
    {
        private readonly ICertificateManager _certificateManager;
        private readonly ITimestampService _timestampService;
        private readonly IAssinaturaDigitalRepository _assinaturaRepository;
        private readonly ICertificadoDigitalRepository _certificadoRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AssinaturaDigitalService> _logger;
        
        private const string DefaultSystemName = "Sistema Omni Care";

        public AssinaturaDigitalService(
            ICertificateManager certificateManager,
            ITimestampService timestampService,
            IAssinaturaDigitalRepository assinaturaRepository,
            ICertificadoDigitalRepository certificadoRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AssinaturaDigitalService> logger)
        {
            _certificateManager = certificateManager;
            _timestampService = timestampService;
            _assinaturaRepository = assinaturaRepository;
            _certificadoRepository = certificadoRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ResultadoAssinatura> AssinarDocumentoAsync(
            Guid documentoId,
            TipoDocumento tipoDocumento,
            Guid medicoId,
            byte[] documentoBytes,
            string? senhaCertificado = null)
        {
            _logger.LogInformation(
                "Signing document. DocumentId={DocumentoId}, Type={TipoDocumento}, MedicoId={MedicoId}",
                documentoId, tipoDocumento, medicoId);

            try
            {
                // Validate input
                if (documentoId == Guid.Empty)
                    throw new ArgumentException("Document ID cannot be empty", nameof(documentoId));

                if (medicoId == Guid.Empty)
                    throw new ArgumentException("Medico ID cannot be empty", nameof(medicoId));

                if (documentoBytes == null || documentoBytes.Length == 0)
                    throw new ArgumentException("Document bytes cannot be empty", nameof(documentoBytes));

                // Get active certificate for the doctor
                var certificado = await _certificadoRepository.GetCertificadoAtivoAsync(medicoId);

                if (certificado == null)
                {
                    _logger.LogWarning("No active certificate found for medico {MedicoId}", medicoId);
                    return new ResultadoAssinatura
                    {
                        Sucesso = false,
                        Mensagem = "Médico não possui certificado digital cadastrado"
                    };
                }

                // Validate certificate is still valid
                if (!certificado.Valido || certificado.DataExpiracao < DateTime.UtcNow)
                {
                    _logger.LogWarning("Certificate expired or invalid. CertId={CertId}", certificado.Id);
                    return new ResultadoAssinatura
                    {
                        Sucesso = false,
                        Mensagem = "Certificado digital expirado ou inválido"
                    };
                }

                // Calculate SHA-256 hash
                string hash = CalcularHashSHA256(documentoBytes);
                _logger.LogDebug("Document hash calculated: {Hash}", hash);

                // Load X.509 certificate
                X509Certificate2 cert = await _certificateManager.CarregarCertificadoAsync(
                    certificado, 
                    senhaCertificado);

                // Sign the document using PKCS#7
                byte[] assinatura = AssinarPKCS7(documentoBytes, cert);
                _logger.LogDebug("Document signed with PKCS#7, signature size: {Size} bytes", assinatura.Length);

                // Obtain timestamp
                TimestampResponse? timestamp = null;
                try
                {
                    timestamp = await _timestampService.ObterTimestampAsync(hash);
                    if (timestamp != null)
                    {
                        _logger.LogInformation("Timestamp obtained successfully");
                    }
                    else
                    {
                        _logger.LogWarning("Could not obtain timestamp, continuing without it");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error obtaining timestamp, continuing without it");
                }

                // Get client IP
                string clientIp = GetClientIp();

                // Create signature entity
                var assinaturaDigital = new AssinaturaDigital(
                    documentoId: documentoId,
                    tipoDocumento: tipoDocumento,
                    medicoId: medicoId,
                    certificadoId: certificado.Id,
                    assinaturaDigitalBytes: assinatura,
                    hashDocumento: hash,
                    localAssinatura: DefaultSystemName, // TODO: Make configurable via app settings
                    ipAssinatura: clientIp,
                    tenantId: certificado.TenantId,
                    temTimestamp: timestamp != null,
                    dataTimestamp: timestamp?.Data,
                    timestampBytes: timestamp?.Bytes
                );

                await _assinaturaRepository.AddAsync(assinaturaDigital);

                // Update certificate signature count
                certificado.IncrementarAssinaturas();
                await _certificadoRepository.UpdateAsync(certificado);

                _logger.LogInformation(
                    "Document signed successfully. AssinaturaId={AssinaturaId}", 
                    assinaturaDigital.Id);

                // Create DTO from saved entity (relationships might not be loaded)
                var dto = new AssinaturaDigitalDto
                {
                    Id = assinaturaDigital.Id,
                    DocumentoId = assinaturaDigital.DocumentoId,
                    TipoDocumento = assinaturaDigital.TipoDocumento.ToString(),
                    MedicoId = assinaturaDigital.MedicoId,
                    MedicoNome = "N/A", // Will be loaded by consumer if needed
                    MedicoCRM = "N/A",
                    DataHoraAssinatura = assinaturaDigital.DataHoraAssinatura,
                    HashDocumento = assinaturaDigital.HashDocumento,
                    TemTimestamp = assinaturaDigital.TemTimestamp,
                    DataTimestamp = assinaturaDigital.DataTimestamp,
                    Valida = assinaturaDigital.Valida,
                    DataUltimaValidacao = assinaturaDigital.DataUltimaValidacao,
                    CertificadoSubject = certificado.SubjectName,
                    CertificadoExpiracao = certificado.DataExpiracao
                };

                return new ResultadoAssinatura
                {
                    Sucesso = true,
                    Mensagem = "Documento assinado com sucesso",
                    AssinaturaId = assinaturaDigital.Id,
                    Assinatura = dto
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing document");
                return new ResultadoAssinatura
                {
                    Sucesso = false,
                    Mensagem = $"Erro ao assinar documento: {ex.Message}"
                };
            }
        }

        public async Task<ResultadoValidacao> ValidarAssinaturaAsync(Guid assinaturaId)
        {
            _logger.LogInformation("Validating signature {AssinaturaId}", assinaturaId);

            try
            {
                // Load signature with related entities
                var assinatura = await _assinaturaRepository.GetAssinaturaComRelacoesAsync(assinaturaId);

                if (assinatura == null)
                {
                    _logger.LogWarning("Signature not found: {AssinaturaId}", assinaturaId);
                    return new ResultadoValidacao
                    {
                        Valida = false,
                        Motivo = "Assinatura não encontrada"
                    };
                }

                // TODO: Document Integrity Validation
                // In production, this method should:
                // 1. Retrieve the original document bytes from storage
                // 2. Recalculate SHA-256 hash
                // 3. Compare with stored assinatura.HashDocumento
                // 4. Return error if hashes don't match
                //
                // Current implementation validates:
                // - PKCS#7 signature structure
                // - Certificate validity
                // - Timestamp (if present)
                //
                // Document hash validation requires integration with document storage system.
                // See ASSINATURA_DIGITAL_DOCUMENTACAO_TECNICA.md for implementation details.

                // Validate PKCS#7 signature structure
                try
                {
                    var signedCms = new SignedCms();
                    signedCms.Decode(assinatura.AssinaturaDigitalBytes);
                    signedCms.CheckSignature(verifySignatureOnly: true);

                    _logger.LogDebug("PKCS#7 signature structure is valid");
                }
                catch (CryptographicException ex)
                {
                    _logger.LogWarning(ex, "Invalid PKCS#7 signature");
                    await UpdateSignatureValidation(assinatura, false);
                    
                    return new ResultadoValidacao
                    {
                        Valida = false,
                        Motivo = $"Assinatura digital inválida: {ex.Message}"
                    };
                }

                // Validate certificate was valid at signing time
                var signedCms2 = new SignedCms();
                signedCms2.Decode(assinatura.AssinaturaDigitalBytes);
                var cert = signedCms2.SignerInfos[0].Certificate;

                if (cert == null)
                {
                    _logger.LogWarning("Certificate not found in signature");
                    await UpdateSignatureValidation(assinatura, false);
                    
                    return new ResultadoValidacao
                    {
                        Valida = false,
                        Motivo = "Certificado não encontrado na assinatura"
                    };
                }

                if (cert.NotAfter < assinatura.DataHoraAssinatura)
                {
                    _logger.LogWarning("Certificate was expired at signing time");
                    await UpdateSignatureValidation(assinatura, false);
                    
                    return new ResultadoValidacao
                    {
                        Valida = false,
                        Motivo = "Certificado estava expirado no momento da assinatura"
                    };
                }

                // Validate timestamp if present
                if (assinatura.TemTimestamp && assinatura.TimestampBytes != null)
                {
                    var timestampValido = await _timestampService.ValidarTimestampAsync(
                        assinatura.TimestampBytes);

                    if (!timestampValido)
                    {
                        _logger.LogWarning("Invalid timestamp");
                        await UpdateSignatureValidation(assinatura, false);
                        
                        return new ResultadoValidacao
                        {
                            Valida = false,
                            Motivo = "Carimbo de tempo inválido"
                        };
                    }
                }

                // Update validation timestamp
                await UpdateSignatureValidation(assinatura, true);

                _logger.LogInformation("Signature validated successfully");

                return new ResultadoValidacao
                {
                    Valida = true,
                    DataAssinatura = assinatura.DataHoraAssinatura,
                    Assinante = assinatura.Medico?.FullName ?? "N/A",
                    CRM = assinatura.Medico?.ProfessionalId ?? "N/A",
                    Certificado = cert.Subject
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating signature");
                return new ResultadoValidacao
                {
                    Valida = false,
                    Motivo = $"Erro ao validar assinatura: {ex.Message}"
                };
            }
        }

        public async Task<List<AssinaturaDigitalDto>> ObterAssinaturasPorDocumentoAsync(
            Guid documentoId,
            TipoDocumento tipoDocumento)
        {
            _logger.LogInformation(
                "Getting signatures for document. DocumentId={DocumentoId}, Type={TipoDocumento}",
                documentoId, tipoDocumento);

            try
            {
                var assinaturas = await _assinaturaRepository.GetAssinaturasPorDocumentoAsync(
                    documentoId,
                    tipoDocumento);

                var dtos = assinaturas.Select(MapToDto).ToList();

                _logger.LogInformation("Found {Count} signatures", dtos.Count);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting signatures for document");
                throw;
            }
        }

        private byte[] AssinarPKCS7(byte[] documento, X509Certificate2 certificado)
        {
            _logger.LogDebug("Creating PKCS#7 signature");

            var contentInfo = new ContentInfo(documento);
            var signedCms = new SignedCms(contentInfo, detached: true);

            var signer = new CmsSigner(certificado)
            {
                IncludeOption = X509IncludeOption.WholeChain,
                DigestAlgorithm = new Oid("2.16.840.1.101.3.4.2.1") // SHA-256
            };

            // Add PKCS#9 attributes
            var signingTime = new Pkcs9SigningTime(DateTime.UtcNow);
            signer.SignedAttributes.Add(signingTime);

            // Compute signature
            signedCms.ComputeSignature(signer);

            return signedCms.Encode();
        }

        private string CalcularHashSHA256(byte[] dados)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(dados);
            
            // Convert to hex string
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("x2"));
            }
            
            return sb.ToString();
        }

        private string GetClientIp()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                    return "unknown";

                // Check for X-Forwarded-For header (proxy/load balancer)
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Check for X-Real-IP header
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                // Use remote IP address
                return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not determine client IP");
                return "unknown";
            }
        }

        private async Task UpdateSignatureValidation(AssinaturaDigital assinatura, bool valida)
        {
            assinatura.AtualizarValidacao(valida);
            await _assinaturaRepository.UpdateAsync(assinatura);
        }

        private AssinaturaDigitalDto MapToDto(AssinaturaDigital assinatura)
        {
            return new AssinaturaDigitalDto
            {
                Id = assinatura.Id,
                DocumentoId = assinatura.DocumentoId,
                TipoDocumento = assinatura.TipoDocumento.ToString(),
                MedicoId = assinatura.MedicoId,
                MedicoNome = assinatura.Medico?.FullName ?? "N/A",
                MedicoCRM = assinatura.Medico?.ProfessionalId ?? "N/A",
                DataHoraAssinatura = assinatura.DataHoraAssinatura,
                HashDocumento = assinatura.HashDocumento,
                TemTimestamp = assinatura.TemTimestamp,
                DataTimestamp = assinatura.DataTimestamp,
                Valida = assinatura.Valida,
                DataUltimaValidacao = assinatura.DataUltimaValidacao,
                CertificadoSubject = assinatura.Certificado?.SubjectName ?? "N/A",
                CertificadoExpiracao = assinatura.Certificado?.DataExpiracao ?? DateTime.MinValue
            };
        }
    }
}
