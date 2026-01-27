using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services.DigitalSignature;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing digital certificates (ICP-Brasil A1 and A3).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CertificadoDigitalController : BaseController
    {
        private readonly ICertificateManager _certificateManager;
        private readonly ILogger<CertificadoDigitalController> _logger;

        public CertificadoDigitalController(
            ICertificateManager certificateManager,
            ITenantContext tenantContext,
            ILogger<CertificadoDigitalController> logger) : base(tenantContext)
        {
            _certificateManager = certificateManager;
            _logger = logger;
        }

        /// <summary>
        /// Get all certificates for the authenticated doctor.
        /// </summary>
        /// <returns>List of certificates</returns>
        [HttpGet]
        public async Task<ActionResult<List<CertificadoDigitalDto>>> GetCertificados()
        {
            try
            {
                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    _logger.LogWarning("User ID not found in token");
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var tenantId = GetTenantId();
                _logger.LogInformation("Getting certificates for medico {MedicoId}, tenant {TenantId}", 
                    medicoId, tenantId);

                var certificados = await _certificateManager.ListarCertificadosMedicoAsync(medicoId);
                return Ok(certificados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certificates");
                return StatusCode(500, new { message = "Erro ao buscar certificados" });
            }
        }

        /// <summary>
        /// Get certificate details by ID.
        /// </summary>
        /// <param name="id">Certificate ID</param>
        /// <returns>Certificate details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CertificadoDigitalDto>> GetCertificado(Guid id)
        {
            try
            {
                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var certificado = await _certificateManager.ObterCertificadoPorIdAsync(id);
                if (certificado == null)
                {
                    return NotFound(new { message = "Certificado não encontrado" });
                }

                // Verify ownership
                if (certificado.MedicoId != medicoId)
                {
                    _logger.LogWarning("User {MedicoId} attempted to access certificate {CertificadoId} owned by {OwnerId}",
                        medicoId, id, certificado.MedicoId);
                    return Forbid();
                }

                return Ok(certificado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting certificate {Id}", id);
                return StatusCode(500, new { message = "Erro ao buscar certificado" });
            }
        }

        /// <summary>
        /// Import an A1 certificate (PFX file).
        /// </summary>
        /// <param name="request">Import request with PFX file and password</param>
        /// <returns>Created certificate</returns>
        [HttpPost("a1/importar")]
        public async Task<ActionResult<CertificadoDigitalDto>> ImportarCertificadoA1(
            [FromForm] ImportarCertificadoA1Request request)
        {
            try
            {
                if (request.Arquivo == null || request.Arquivo.Length == 0)
                {
                    return BadRequest(new { message = "Arquivo PFX é obrigatório" });
                }

                if (string.IsNullOrWhiteSpace(request.Senha))
                {
                    return BadRequest(new { message = "Senha do certificado é obrigatória" });
                }

                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var tenantId = GetTenantId();

                // Read PFX file
                byte[] pfxBytes;
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    await request.Arquivo.CopyToAsync(memoryStream);
                    pfxBytes = memoryStream.ToArray();
                }

                _logger.LogInformation("Importing A1 certificate for medico {MedicoId}", medicoId);

                var certificado = await _certificateManager.ImportarCertificadoA1Async(
                    medicoId,
                    tenantId,
                    pfxBytes,
                    request.Senha);

                return CreatedAtAction(
                    nameof(GetCertificado), 
                    new { id = certificado.Id }, 
                    certificado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid certificate data");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing A1 certificate");
                return StatusCode(500, new { message = "Erro ao importar certificado" });
            }
        }

        /// <summary>
        /// List available A3 certificates from Windows Certificate Store.
        /// </summary>
        /// <returns>List of available A3 certificates</returns>
        [HttpGet("a3/disponiveis")]
        public async Task<ActionResult<List<CertificateInfo>>> ListarCertificadosA3Disponiveis()
        {
            try
            {
                _logger.LogInformation("Listing available A3 certificates");

                var certificados = await _certificateManager.ListarCertificadosA3Disponiveis();
                return Ok(certificados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing A3 certificates");
                return StatusCode(500, new { message = "Erro ao listar certificados A3" });
            }
        }

        /// <summary>
        /// Register an A3 certificate (token/smartcard).
        /// </summary>
        /// <param name="request">Registration request with thumbprint</param>
        /// <returns>Created certificate</returns>
        [HttpPost("a3/registrar")]
        public async Task<ActionResult<CertificadoDigitalDto>> RegistrarCertificadoA3(
            [FromBody] RegistrarCertificadoA3Request request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.Thumbprint))
                {
                    return BadRequest(new { message = "Thumbprint do certificado é obrigatório" });
                }

                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                var tenantId = GetTenantId();

                _logger.LogInformation("Registering A3 certificate for medico {MedicoId}, thumbprint {Thumbprint}", 
                    medicoId, request.Thumbprint);

                var certificado = await _certificateManager.RegistrarCertificadoA3Async(
                    medicoId,
                    tenantId,
                    request.Thumbprint);

                return CreatedAtAction(
                    nameof(GetCertificado), 
                    new { id = certificado.Id }, 
                    certificado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid certificate data");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering A3 certificate");
                return StatusCode(500, new { message = "Erro ao registrar certificado" });
            }
        }

        /// <summary>
        /// Invalidate a certificate.
        /// </summary>
        /// <param name="id">Certificate ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> InvalidarCertificado(Guid id)
        {
            try
            {
                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                _logger.LogInformation("Invalidating certificate {Id} for medico {MedicoId}", id, medicoId);

                await _certificateManager.InvalidarCertificadoAsync(id, medicoId);

                return Ok(new { message = "Certificado invalidado com sucesso" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to certificate {Id}", id);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating certificate {Id}", id);
                return StatusCode(500, new { message = "Erro ao invalidar certificado" });
            }
        }
    }

    /// <summary>
    /// Request model for importing A1 certificate.
    /// </summary>
    public class ImportarCertificadoA1Request
    {
        public IFormFile Arquivo { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }

    /// <summary>
    /// Request model for registering A3 certificate.
    /// </summary>
    public class RegistrarCertificadoA3Request
    {
        public string Thumbprint { get; set; } = null!;
    }
}
