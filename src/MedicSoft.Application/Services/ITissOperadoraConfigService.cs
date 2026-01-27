using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service interface for managing TISS operator webservice configurations
    /// </summary>
    public interface ITissOperadoraConfigService
    {
        Task<TissOperadoraConfigDto> CreateAsync(CreateTissOperadoraConfigDto dto, string tenantId);
        Task<TissOperadoraConfigDto> GetByIdAsync(Guid id, string tenantId);
        Task<TissOperadoraConfigDto?> GetByOperatorIdAsync(Guid operatorId, string tenantId);
        Task<IEnumerable<TissOperadoraConfigDto>> GetAllAsync(string tenantId);
        Task<IEnumerable<TissOperadoraConfigDto>> GetActiveConfigsAsync(string tenantId);
        Task<TissOperadoraConfigDto> UpdateAsync(Guid id, CreateTissOperadoraConfigDto dto, string tenantId);
        Task<bool> ActivateAsync(Guid id, string tenantId);
        Task<bool> DeactivateAsync(Guid id, string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
