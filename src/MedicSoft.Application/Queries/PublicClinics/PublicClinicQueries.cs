using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.PublicClinics
{
    /// <summary>
    /// Query pública para buscar clínicas no site (sem autenticação).
    /// Retorna apenas informações públicas e essenciais, respeitando a LGPD.
    /// </summary>
    public record SearchPublicClinicsQuery(
        string? Name,
        string? City,
        string? State,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<SearchClinicsResultDto>;

    /// <summary>
    /// Query pública para obter detalhes de uma clínica específica.
    /// Retorna apenas informações públicas e essenciais.
    /// </summary>
    public record GetPublicClinicByIdQuery(Guid ClinicId) : IRequest<PublicClinicDto?>;

    /// <summary>
    /// Query pública para obter horários disponíveis de uma clínica.
    /// Não requer autenticação.
    /// </summary>
    public record GetPublicAvailableSlotsQuery(
        Guid ClinicId,
        DateTime Date,
        int DurationMinutes = 30
    ) : IRequest<List<AvailableSlotDto>>;
}
