using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services.DigitalSignature;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing digital signatures on medical documents.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AssinaturaDigitalController : BaseController
    {
        private readonly IAssinaturaDigitalService _assinaturaService;
        private readonly ILogger<AssinaturaDigitalController> _logger;

        public AssinaturaDigitalController(
            IAssinaturaDigitalService assinaturaService,
            ITenantContext tenantContext,
            ILogger<AssinaturaDigitalController> logger) : base(tenantContext)
        {
            _assinaturaService = assinaturaService;
            _logger = logger;
        }

        /// <summary>
        /// Sign a medical document digitally.
        /// </summary>
        /// <param name="request">Signature request</param>
        /// <returns>Signature result</returns>
        [HttpPost("assinar")]
        public async Task<ActionResult<ResultadoAssinatura>> AssinarDocumento(
            [FromBody] AssinarDocumentoRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest(new { message = "Dados de assinatura não fornecidos" });
                }

                if (request.DocumentoId == Guid.Empty)
                {
                    return BadRequest(new { message = "ID do documento é obrigatório" });
                }

                if (request.DocumentoBytes == null || request.DocumentoBytes.Length == 0)
                {
                    return BadRequest(new { message = "Bytes do documento são obrigatórios" });
                }

                var medicoId = GetUserId();
                if (medicoId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Usuário não autenticado" });
                }

                _logger.LogInformation(
                    "Signing document {DocumentoId} of type {TipoDocumento} for medico {MedicoId}",
                    request.DocumentoId, request.TipoDocumento, medicoId);

                var resultado = await _assinaturaService.AssinarDocumentoAsync(
                    request.DocumentoId,
                    request.TipoDocumento,
                    medicoId,
                    request.DocumentoBytes,
                    request.SenhaCertificado);

                if (!resultado.Sucesso)
                {
                    _logger.LogWarning(
                        "Failed to sign document {DocumentoId}: {Message}",
                        request.DocumentoId, resultado.Mensagem);
                    return BadRequest(new { message = resultado.Mensagem });
                }

                _logger.LogInformation(
                    "Document {DocumentoId} signed successfully. SignatureId: {AssinaturaId}",
                    request.DocumentoId, resultado.AssinaturaId);

                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid signature request");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error signing document");
                return StatusCode(500, new { message = "Erro ao assinar documento" });
            }
        }

        /// <summary>
        /// Validate a digital signature.
        /// </summary>
        /// <param name="id">Signature ID</param>
        /// <returns>Validation result</returns>
        [HttpGet("{id}/validar")]
        public async Task<ActionResult<ResultadoValidacao>> ValidarAssinatura(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID da assinatura é obrigatório" });
                }

                _logger.LogInformation("Validating signature {AssinaturaId}", id);

                var resultado = await _assinaturaService.ValidarAssinaturaAsync(id);

                if (resultado == null)
                {
                    return NotFound(new { message = "Assinatura não encontrada" });
                }

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating signature {Id}", id);
                return StatusCode(500, new { message = "Erro ao validar assinatura" });
            }
        }

        /// <summary>
        /// Get all signatures for a specific document.
        /// </summary>
        /// <param name="documentoId">Document ID</param>
        /// <param name="tipoDocumento">Document type</param>
        /// <returns>List of signatures</returns>
        [HttpGet("documento/{documentoId}")]
        public async Task<ActionResult<List<AssinaturaDigitalDto>>> GetAssinaturasPorDocumento(
            Guid documentoId,
            [FromQuery] TipoDocumento tipoDocumento)
        {
            try
            {
                if (documentoId == Guid.Empty)
                {
                    return BadRequest(new { message = "ID do documento é obrigatório" });
                }

                _logger.LogInformation(
                    "Getting signatures for document {DocumentoId} of type {TipoDocumento}",
                    documentoId, tipoDocumento);

                var assinaturas = await _assinaturaService.ObterAssinaturasPorDocumentoAsync(
                    documentoId,
                    tipoDocumento);

                return Ok(assinaturas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting signatures for document {DocumentoId}", documentoId);
                return StatusCode(500, new { message = "Erro ao buscar assinaturas" });
            }
        }

        /// <summary>
        /// Get signature details by ID.
        /// </summary>
        /// <param name="id">Signature ID</param>
        /// <returns>Signature details</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<AssinaturaDigitalDto>> GetAssinatura(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID da assinatura é obrigatório" });
                }

                _logger.LogInformation("Getting signature {AssinaturaId}", id);

                var assinaturas = await _assinaturaService.ObterAssinaturasPorDocumentoAsync(
                    Guid.Empty, // We'll need to get this from the repository
                    TipoDocumento.Prontuario); // Placeholder

                // This is a simplified version - in production you'd want to add a specific method
                // to get a single signature by ID
                return NotFound(new { message = "Método não implementado - use validar ou documento/{id}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting signature {Id}", id);
                return StatusCode(500, new { message = "Erro ao buscar assinatura" });
            }
        }
    }

    /// <summary>
    /// Request model for signing a document.
    /// </summary>
    public class AssinarDocumentoRequest
    {
        public Guid DocumentoId { get; set; }
        public TipoDocumento TipoDocumento { get; set; }
        public byte[] DocumentoBytes { get; set; } = null!;
        public string? SenhaCertificado { get; set; }
    }
}
