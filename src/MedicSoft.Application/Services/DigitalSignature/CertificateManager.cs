using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services.DigitalSignature
{
    /// <summary>
    /// Service for managing digital certificates (ICP-Brasil A1/A3).
    /// </summary>
    public interface ICertificateManager
    {
        Task<CertificadoDigital> ImportarCertificadoA1Async(Guid medicoId, string tenantId, byte[] pfxBytes, string senha);
        Task<List<CertificateInfo>> ListarCertificadosA3Disponiveis();
        Task<CertificadoDigital> RegistrarCertificadoA3Async(Guid medicoId, string tenantId, string thumbprint);
        Task<X509Certificate2> CarregarCertificadoAsync(CertificadoDigital certificado, string? senha = null);
        bool IsICPBrasil(X509Certificate2 cert);
        Task<List<CertificadoDigitalDto>> ListarCertificadosMedicoAsync(Guid medicoId);
        Task<CertificadoDigitalDto?> ObterCertificadoPorIdAsync(Guid certificadoId);
        Task InvalidarCertificadoAsync(Guid certificadoId, Guid medicoId);
    }

    /// <summary>
    /// Implementation of certificate manager for ICP-Brasil certificates.
    /// </summary>
    public class CertificateManager : ICertificateManager
    {
        private readonly ICertificadoDigitalRepository _certificadoRepository;
        private readonly IDataEncryptionService _encryptionService;
        private readonly ILogger<CertificateManager> _logger;

        public CertificateManager(
            ICertificadoDigitalRepository certificadoRepository,
            IDataEncryptionService encryptionService,
            ILogger<CertificateManager> logger)
        {
            _certificadoRepository = certificadoRepository;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        public async Task<CertificadoDigital> ImportarCertificadoA1Async(
            Guid medicoId,
            string tenantId,
            byte[] pfxBytes,
            string senha)
        {
            _logger.LogInformation("Importing A1 certificate for medico {MedicoId}", medicoId);

            // Validate PFX
            X509Certificate2 cert;
            try
            {
                cert = new X509Certificate2(pfxBytes, senha, X509KeyStorageFlags.Exportable);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid certificate or password");
                throw new InvalidOperationException("Certificado ou senha inválidos");
            }

            // Validate if is ICP-Brasil
            if (!IsICPBrasil(cert))
            {
                _logger.LogWarning("Certificate is not ICP-Brasil");
                throw new InvalidOperationException("Certificado não é ICP-Brasil");
            }

            // Validate validity
            if (cert.NotAfter < DateTime.UtcNow)
            {
                _logger.LogWarning("Certificate is expired");
                throw new InvalidOperationException("Certificado expirado");
            }

            // Check if doctor already has a certificate - invalidate it
            var existente = await _certificadoRepository.GetCertificadoAtivoAsync(medicoId);
            if (existente != null)
            {
                _logger.LogInformation("Invalidating existing certificate {CertId}", existente.Id);
                existente.Invalidar();
                await _certificadoRepository.UpdateAsync(existente);
            }

            // Encrypt certificate and private key
            byte[] certCriptografado = _encryptionService.EncryptBytes(pfxBytes);
            byte[] chaveCriptografada = Array.Empty<byte>(); // Placeholder for private key
            
            try
            {
                using var rsa = cert.GetRSAPrivateKey();
                if (rsa != null)
                {
                    var privateKeyBytes = rsa.ExportRSAPrivateKey();
                    chaveCriptografada = _encryptionService.EncryptBytes(privateKeyBytes);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not export private key separately, using full PFX");
                chaveCriptografada = certCriptografado;
            }

            // Save
            var certificado = new CertificadoDigital(
                medicoId: medicoId,
                tipo: TipoCertificado.A1,
                numeroCertificado: cert.SerialNumber,
                subjectName: cert.Subject,
                issuerName: cert.Issuer,
                thumbprint: cert.Thumbprint,
                dataEmissao: cert.NotBefore,
                dataExpiracao: cert.NotAfter,
                tenantId: tenantId,
                certificadoA1Criptografado: certCriptografado,
                chavePrivadaA1Criptografada: chaveCriptografada
            );

            await _certificadoRepository.AddAsync(certificado);

            _logger.LogInformation("Certificate imported successfully. CertId={CertId}", certificado.Id);

            return certificado;
        }

        public async Task<List<CertificateInfo>> ListarCertificadosA3Disponiveis()
        {
            _logger.LogInformation("Listing available A3 certificates");

            var certificados = new List<CertificateInfo>();

            try
            {
                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                foreach (var cert in store.Certificates)
                {
                    // Filter only ICP-Brasil with private key
                    if (cert.HasPrivateKey && IsICPBrasil(cert) && cert.NotAfter > DateTime.UtcNow)
                    {
                        certificados.Add(new CertificateInfo
                        {
                            Subject = cert.Subject,
                            Issuer = cert.Issuer,
                            Thumbprint = cert.Thumbprint,
                            ValidFrom = cert.NotBefore,
                            ValidTo = cert.NotAfter,
                            IsValid = true
                        });
                    }
                }

                store.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing A3 certificates");
            }

            return certificados;
        }

        public async Task<CertificadoDigital> RegistrarCertificadoA3Async(
            Guid medicoId,
            string tenantId,
            string thumbprint)
        {
            _logger.LogInformation("Registering A3 certificate for medico {MedicoId}, thumbprint {Thumbprint}",
                medicoId, thumbprint);

            // Load certificate from store
            X509Certificate2? cert = null;

            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                var certificates = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    thumbprint,
                    validOnly: false);

                if (certificates.Count > 0)
                {
                    cert = certificates[0];
                }

                store.Close();
            }

            if (cert == null)
            {
                _logger.LogWarning("A3 certificate not found in store");
                throw new InvalidOperationException("Token A3 não está conectado ou certificado não encontrado");
            }

            // Validate if is ICP-Brasil
            if (!IsICPBrasil(cert))
            {
                throw new InvalidOperationException("Certificado não é ICP-Brasil");
            }

            // Check if doctor already has a certificate - invalidate it
            var existente = await _certificadoRepository.GetCertificadoAtivoAsync(medicoId);
            if (existente != null)
            {
                existente.Invalidar();
                await _certificadoRepository.UpdateAsync(existente);
            }

            // Save (A3 doesn't store encrypted bytes, just metadata)
            var certificado = new CertificadoDigital(
                medicoId: medicoId,
                tipo: TipoCertificado.A3,
                numeroCertificado: cert.SerialNumber,
                subjectName: cert.Subject,
                issuerName: cert.Issuer,
                thumbprint: cert.Thumbprint,
                dataEmissao: cert.NotBefore,
                dataExpiracao: cert.NotAfter,
                tenantId: tenantId
            );

            await _certificadoRepository.AddAsync(certificado);

            _logger.LogInformation("A3 certificate registered successfully. CertId={CertId}", certificado.Id);

            return certificado;
        }

        public async Task<X509Certificate2> CarregarCertificadoAsync(
            CertificadoDigital certificado,
            string? senha = null)
        {
            _logger.LogInformation("Loading certificate {CertId}, Type={Type}",
                certificado.Id, certificado.Tipo);

            if (certificado.Tipo == TipoCertificado.A1)
            {
                // Decrypt and load
                byte[] pfxBytes = _encryptionService.DecryptBytes(
                    certificado.CertificadoA1Criptografado!);

                return new X509Certificate2(pfxBytes, senha ?? string.Empty,
                    X509KeyStorageFlags.Exportable);
            }
            else // A3
            {
                // Load from Windows Certificate Store (token must be connected)
                using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                store.Open(OpenFlags.ReadOnly);

                var certificates = store.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    certificado.Thumbprint,
                    validOnly: false);

                store.Close();

                if (certificates.Count == 0)
                {
                    _logger.LogError("A3 token not connected");
                    throw new InvalidOperationException("Token A3 não está conectado");
                }

                return certificates[0];
            }
        }

        public bool IsICPBrasil(X509Certificate2 cert)
        {
            // Check if the issuer is from ICP-Brasil authorities
            var icpIssuers = new[]
            {
                "AC Certisign",
                "AC Serasa",
                "AC Soluti",
                "Autoridade Certificadora Raiz Brasileira",
                "AC VALID",
                "AC SERPROPR",
                "ICP-Brasil"
            };

            return icpIssuers.Any(issuer =>
                cert.Issuer.Contains(issuer, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<CertificadoDigitalDto>> ListarCertificadosMedicoAsync(Guid medicoId)
        {
            _logger.LogInformation("Listing certificates for medico {MedicoId}", medicoId);

            // We need to get the tenantId - for now, we'll use a workaround
            // In a real scenario, this should be passed as a parameter or retrieved from context
            var certificados = await _certificadoRepository.GetCertificadosPorMedicoAsync(medicoId);

            return certificados.Select(c => new CertificadoDigitalDto
            {
                Id = c.Id,
                MedicoId = c.MedicoId,
                MedicoNome = c.Medico?.FullName ?? "",
                Tipo = c.Tipo.ToString(),
                NumeroCertificado = c.NumeroCertificado,
                SubjectName = c.SubjectName,
                IssuerName = c.IssuerName,
                DataEmissao = c.DataEmissao,
                DataExpiracao = c.DataExpiracao,
                Valido = c.Valido,
                TotalAssinaturas = c.TotalAssinaturas,
                DiasParaExpiracao = c.DiasParaExpiracao()
            }).ToList();
        }

        public async Task<CertificadoDigitalDto?> ObterCertificadoPorIdAsync(Guid certificadoId)
        {
            _logger.LogInformation("Getting certificate {CertificadoId}", certificadoId);

            // Use the repository method that includes navigation properties
            var certificado = await _certificadoRepository.GetCertificadoComMedicoAsync(certificadoId);
            if (certificado == null)
                return null;

            return new CertificadoDigitalDto
            {
                Id = certificado.Id,
                MedicoId = certificado.MedicoId,
                MedicoNome = certificado.Medico?.FullName ?? "",
                Tipo = certificado.Tipo.ToString(),
                NumeroCertificado = certificado.NumeroCertificado,
                SubjectName = certificado.SubjectName,
                IssuerName = certificado.IssuerName,
                DataEmissao = certificado.DataEmissao,
                DataExpiracao = certificado.DataExpiracao,
                Valido = certificado.Valido,
                TotalAssinaturas = certificado.TotalAssinaturas,
                DiasParaExpiracao = certificado.DiasParaExpiracao()
            };
        }

        public async Task InvalidarCertificadoAsync(Guid certificadoId, Guid medicoId)
        {
            _logger.LogInformation("Invalidating certificate {CertificadoId} for medico {MedicoId}", 
                certificadoId, medicoId);

            var certificado = await _certificadoRepository.GetCertificadoComMedicoAsync(certificadoId);
            if (certificado == null)
                throw new InvalidOperationException("Certificado não encontrado");

            if (certificado.MedicoId != medicoId)
                throw new UnauthorizedAccessException("Certificado não pertence ao médico");

            certificado.Invalidar();
            await _certificadoRepository.UpdateAsync(certificado);

            _logger.LogInformation("Certificate {CertificadoId} invalidated successfully", certificadoId);
        }
    }
}
