using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service interface for TISS notification system
    /// </summary>
    public interface ITissNotificationService
    {
        Task NotificarGlosaAsync(TissGlosa glosa);
        Task AlertarPrazoRecursoAsync(TissGlosa glosa, int diasRestantes);
        Task NotificarRecursoDeferidoAsync(TissGlosa glosa, decimal? valorRecuperado);
        Task NotificarRecursoIndeferidoAsync(TissGlosa glosa);
        Task NotificarTaxaGlosaAltaAsync(Guid operatorId, decimal taxaGlosa, string tenantId);
    }
}
