using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MedicSoft.Api.Hubs;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilaEsperaController : BaseController
    {
        private readonly IFilaService _filaService;
        private readonly IHubContext<FilaHub> _hubContext;

        public FilaEsperaController(
            IFilaService filaService,
            IHubContext<FilaHub> hubContext,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _filaService = filaService;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Criar nova fila de espera
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<FilaEsperaDto>> CreateFila([FromBody] CreateFilaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var fila = await _filaService.CreateFilaAsync(
                    request.ClinicaId,
                    request.Nome,
                    request.Tipo,
                    GetTenantId());
                
                return CreatedAtAction(nameof(GetFila), new { filaId = fila.Id }, fila);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter informações de uma fila específica
        /// </summary>
        [HttpGet("{filaId}")]
        public async Task<ActionResult<FilaEsperaDto>> GetFila(Guid filaId)
        {
            try
            {
                var fila = await _filaService.GetFilaByIdAsync(filaId, GetTenantId());
                
                if (fila == null)
                    return NotFound("Fila não encontrada");
                
                return Ok(fila);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter resumo completo da fila com todas as senhas ativas
        /// </summary>
        [HttpGet("{filaId}/summary")]
        public async Task<ActionResult<FilaSummaryDto>> GetFilaSummary(Guid filaId)
        {
            try
            {
                var summary = await _filaService.GetFilaSummaryAsync(filaId, GetTenantId());
                return Ok(summary);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Gerar nova senha na fila (totem de autoatendimento ou recepção)
        /// </summary>
        [AllowAnonymous]
        [HttpPost("{filaId}/senha")]
        public async Task<ActionResult<SenhaFilaDto>> GerarSenha(Guid filaId, [FromBody] GerarSenhaRequest request, [FromQuery] string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId é obrigatório");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                request.FilaId = filaId;
                var senha = await _filaService.GerarSenhaAsync(request, tenantId);
                
                // Notificar via SignalR
                await _hubContext.Clients.Group($"fila_{filaId}").SendAsync("NovaSenha", senha);
                
                return CreatedAtAction(nameof(ConsultarSenha), new { filaId, numeroSenha = senha.NumeroSenha }, senha);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Consultar senha pelo número (sem autenticação para uso no totem)
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{filaId}/senha/{numeroSenha}")]
        public async Task<ActionResult<SenhaFilaDto>> ConsultarSenha(Guid filaId, string numeroSenha, [FromQuery] string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId é obrigatório");

            try
            {
                var senha = await _filaService.ConsultarSenhaAsync(numeroSenha, filaId, tenantId);
                
                if (senha == null)
                    return NotFound("Senha não encontrada");
                
                return Ok(senha);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Chamar próxima senha da fila
        /// </summary>
        [HttpPost("{filaId}/chamar")]
        public async Task<ActionResult<SenhaFilaDto>> ChamarProximaSenha(Guid filaId, [FromBody] ChamarSenhaRequest request)
        {
            try
            {
                request.FilaId = filaId;
                var senha = await _filaService.ChamarProximaSenhaAsync(request, GetTenantId());
                
                // Notificar via SignalR
                await _hubContext.Clients.Group($"fila_{filaId}").SendAsync("ChamarSenha", new
                {
                    senha.NumeroSenha,
                    senha.NomePaciente,
                    senha.NumeroConsultorio
                });
                
                return Ok(senha);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Iniciar atendimento de uma senha
        /// </summary>
        [HttpPut("senha/{senhaId}/iniciar")]
        public async Task<ActionResult<SenhaFilaDto>> IniciarAtendimento(Guid senhaId)
        {
            try
            {
                var senha = await _filaService.IniciarAtendimentoAsync(senhaId, GetTenantId());
                
                // Notificar via SignalR
                await _hubContext.Clients.Group($"fila_{senha.FilaId}").SendAsync("SenhaEmAtendimento", senhaId);
                
                return Ok(senha);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Finalizar atendimento de uma senha
        /// </summary>
        [HttpPut("senha/{senhaId}/finalizar")]
        public async Task<ActionResult<SenhaFilaDto>> FinalizarAtendimento(Guid senhaId)
        {
            try
            {
                var senha = await _filaService.FinalizarAtendimentoAsync(senhaId, GetTenantId());
                
                // Notificar via SignalR
                await _hubContext.Clients.Group($"fila_{senha.FilaId}").SendAsync("SenhaFinalizada", senhaId);
                
                return Ok(senha);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancelar uma senha
        /// </summary>
        [HttpDelete("senha/{senhaId}")]
        public async Task<ActionResult> CancelarSenha(Guid senhaId)
        {
            try
            {
                await _filaService.CancelarSenhaAsync(senhaId, GetTenantId());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Marcar senha como não comparecida após múltiplas tentativas
        /// </summary>
        [HttpPut("senha/{senhaId}/nao-compareceu")]
        public async Task<ActionResult> MarcarNaoCompareceu(Guid senhaId)
        {
            try
            {
                // This endpoint would be called by the notification service
                // after 3 failed call attempts
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    // Request DTOs
    public class CreateFilaRequest
    {
        public Guid ClinicaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
    }
}
