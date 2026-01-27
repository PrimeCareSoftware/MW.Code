using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service interface for managing TISS glosa appeals (recursos)
    /// </summary>
    public interface ITissRecursoGlosaService
    {
        Task<TissRecursoGlosaDto> CreateAsync(CreateTissRecursoGlosaDto dto, string tenantId);
        Task<TissRecursoGlosaDto> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<TissRecursoGlosaDto>> GetByGlosaIdAsync(Guid glosaId, string tenantId);
        Task<IEnumerable<TissRecursoGlosaDto>> GetPendingResponseAsync(string tenantId);
        Task<IEnumerable<TissRecursoGlosaDto>> GetByResultadoAsync(ResultadoRecurso resultado, string tenantId);
        Task<TissRecursoGlosaDto> RegistrarRespostaAsync(
            Guid id, 
            ResultadoRecurso resultado, 
            string? justificativaOperadora, 
            decimal? valorDeferido, 
            string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
