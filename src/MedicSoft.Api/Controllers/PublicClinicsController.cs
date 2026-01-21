using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.PublicClinics;
using MedicSoft.Application.Commands.PublicAppointments;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// API pública para busca de clínicas e agendamento de consultas.
    /// Não requer autenticação - destinado ao site público.
    /// Retorna apenas informações essenciais e públicas, respeitando a LGPD.
    /// </summary>
    [ApiController]
    [Route("api/public/clinics")]
    public class PublicClinicsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublicClinicsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Busca clínicas disponíveis (pública, sem autenticação).
        /// Retorna apenas dados essenciais e públicos.
        /// </summary>
        /// <param name="request">Filtros de busca (nome, cidade, estado)</param>
        /// <returns>Lista paginada de clínicas</returns>
        [HttpGet("search")]
        public async Task<ActionResult<SearchClinicsResultDto>> SearchClinics([FromQuery] SearchClinicsRequestDto request)
        {
            try
            {
                var query = new SearchPublicClinicsQuery(
                    request.Name,
                    request.City,
                    request.State,
                    request.ClinicType,
                    request.PageNumber,
                    request.PageSize
                );

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Erro ao buscar clínicas.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém detalhes de uma clínica específica (pública, sem autenticação).
        /// Retorna apenas dados essenciais e públicos.
        /// </summary>
        /// <param name="clinicId">ID da clínica</param>
        /// <returns>Detalhes públicos da clínica</returns>
        [HttpGet("{clinicId}")]
        public async Task<ActionResult<PublicClinicDto>> GetClinicById(Guid clinicId)
        {
            try
            {
                var query = new GetPublicClinicByIdQuery(clinicId);
                var result = await _mediator.Send(query);

                if (result == null)
                    return NotFound(new { error = "Clínica não encontrada." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Erro ao buscar clínica.", details = ex.Message });
            }
        }

        /// <summary>
        /// Obtém horários disponíveis de uma clínica (pública, sem autenticação).
        /// </summary>
        /// <param name="clinicId">ID da clínica</param>
        /// <param name="date">Data desejada</param>
        /// <param name="durationMinutes">Duração da consulta em minutos (padrão: 30)</param>
        /// <returns>Lista de horários disponíveis</returns>
        [HttpGet("{clinicId}/available-slots")]
        public async Task<ActionResult<List<AvailableSlotDto>>> GetAvailableSlots(
            Guid clinicId,
            [FromQuery] DateTime date,
            [FromQuery] int durationMinutes = 30)
        {
            try
            {
                var query = new GetPublicAvailableSlotsQuery(clinicId, date, durationMinutes);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = "Erro ao buscar horários disponíveis.", details = ex.Message });
            }
        }

        /// <summary>
        /// Cria um agendamento público (sem autenticação).
        /// Destinado para pacientes que desejam agendar consulta pelo site.
        /// </summary>
        /// <param name="request">Dados do agendamento e do paciente</param>
        /// <returns>Confirmação do agendamento</returns>
        [HttpPost("appointments")]
        public async Task<ActionResult<PublicAppointmentResponseDto>> CreatePublicAppointment(
            [FromBody] PublicAppointmentRequestDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreatePublicAppointmentCommand(request);
                var result = await _mediator.Send(command);
                return CreatedAtAction(
                    nameof(GetClinicById),
                    new { clinicId = result.ClinicId },
                    result
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    error = "Erro ao criar agendamento. Por favor, tente novamente.", 
                    details = ex.Message 
                });
            }
        }
    }
}
