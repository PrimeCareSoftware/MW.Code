using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service interface for managing TISS glosas (rejections/discounts)
    /// </summary>
    public interface ITissGlosaService
    {
        Task<TissGlosaDto> CreateAsync(CreateTissGlosaDto dto, string tenantId);
        Task<TissGlosaDto> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<TissGlosaDto>> GetByGuideIdAsync(Guid guideId, string tenantId);
        Task<IEnumerable<TissGlosaDto>> GetByStatusAsync(StatusGlosa status, string tenantId);
        Task<IEnumerable<TissGlosaDto>> GetByTipoAsync(TipoGlosa tipo, string tenantId);
        Task<IEnumerable<TissGlosaDto>> GetByDateRangeAsync(DateTime start, DateTime end, string tenantId);
        Task<IEnumerable<TissGlosaDto>> GetPendingRecursosAsync(string tenantId);
        Task<TissGlosaDto> MarcarEmAnaliseAsync(Guid id, string tenantId);
        Task<TissGlosaDto> AcatarGlosaAsync(Guid id, string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
