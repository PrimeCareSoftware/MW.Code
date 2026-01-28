using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller para exportação de arquivos SPED (Fiscal e Contábil)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SPEDController : BaseController
    {
        private readonly ISPEDFiscalService _spedFiscalService;
        private readonly ISPEDContabilService _spedContabilService;

        public SPEDController(
            ITenantContext tenantContext,
            ISPEDFiscalService spedFiscalService,
            ISPEDContabilService spedContabilService) : base(tenantContext)
        {
            _spedFiscalService = spedFiscalService;
            _spedContabilService = spedContabilService;
        }

        /// <summary>
        /// Gera arquivo SPED Fiscal para o período especificado
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial (formato: yyyy-MM-dd)</param>
        /// <param name="fim">Data final (formato: yyyy-MM-dd)</param>
        /// <returns>Conteúdo do arquivo SPED Fiscal</returns>
        [HttpGet("fiscal/gerar")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GerarSPEDFiscal(
            [FromQuery] Guid clinicaId,
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                var conteudo = await _spedFiscalService.GerarSPEDFiscalAsync(clinicaId, inicio, fim, tenantId);
                return Ok(new { conteudo, tipo = "SPED Fiscal" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar SPED Fiscal", error = ex.Message });
            }
        }

        /// <summary>
        /// Exporta arquivo SPED Fiscal e retorna como download
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial (formato: yyyy-MM-dd)</param>
        /// <param name="fim">Data final (formato: yyyy-MM-dd)</param>
        /// <returns>Arquivo SPED Fiscal para download</returns>
        [HttpGet("fiscal/download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DownloadSPEDFiscal(
            [FromQuery] Guid clinicaId,
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                var conteudo = await _spedFiscalService.GerarSPEDFiscalAsync(clinicaId, inicio, fim, tenantId);
                var nomeArquivo = $"SPED_Fiscal_{clinicaId}_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.txt";
                
                var bytes = System.Text.Encoding.UTF8.GetBytes(conteudo);
                return File(bytes, "text/plain", nomeArquivo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao exportar SPED Fiscal", error = ex.Message });
            }
        }

        /// <summary>
        /// Valida arquivo SPED Fiscal
        /// </summary>
        /// <param name="conteudoSPED">Conteúdo do arquivo SPED</param>
        /// <returns>Resultado da validação</returns>
        [HttpPost("fiscal/validar")]
        [ProducesResponseType(typeof(SPEDValidationResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ValidarSPEDFiscal([FromBody] string conteudoSPED)
        {
            try
            {
                var resultado = await _spedFiscalService.ValidarSPEDFiscalAsync(conteudoSPED);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao validar SPED Fiscal", error = ex.Message });
            }
        }

        /// <summary>
        /// Gera arquivo SPED Contábil (ECD) para o período especificado
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial (formato: yyyy-MM-dd)</param>
        /// <param name="fim">Data final (formato: yyyy-MM-dd)</param>
        /// <returns>Conteúdo do arquivo SPED Contábil</returns>
        [HttpGet("contabil/gerar")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GerarSPEDContabil(
            [FromQuery] Guid clinicaId,
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                var conteudo = await _spedContabilService.GerarSPEDContabilAsync(clinicaId, inicio, fim, tenantId);
                return Ok(new { conteudo, tipo = "SPED Contábil (ECD)" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao gerar SPED Contábil", error = ex.Message });
            }
        }

        /// <summary>
        /// Exporta arquivo SPED Contábil e retorna como download
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="inicio">Data inicial (formato: yyyy-MM-dd)</param>
        /// <param name="fim">Data final (formato: yyyy-MM-dd)</param>
        /// <returns>Arquivo SPED Contábil para download</returns>
        [HttpGet("contabil/download")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> DownloadSPEDContabil(
            [FromQuery] Guid clinicaId,
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                var conteudo = await _spedContabilService.GerarSPEDContabilAsync(clinicaId, inicio, fim, tenantId);
                var nomeArquivo = $"SPED_Contabil_{clinicaId}_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.txt";
                
                var bytes = System.Text.Encoding.UTF8.GetBytes(conteudo);
                return File(bytes, "text/plain", nomeArquivo);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao exportar SPED Contábil", error = ex.Message });
            }
        }

        /// <summary>
        /// Valida arquivo SPED Contábil
        /// </summary>
        /// <param name="conteudoSPED">Conteúdo do arquivo SPED</param>
        /// <returns>Resultado da validação</returns>
        [HttpPost("contabil/validar")]
        [ProducesResponseType(typeof(SPEDValidationResult), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ValidarSPEDContabil([FromBody] string conteudoSPED)
        {
            try
            {
                var resultado = await _spedContabilService.ValidarSPEDContabilAsync(conteudoSPED);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao validar SPED Contábil", error = ex.Message });
            }
        }
    }
}
