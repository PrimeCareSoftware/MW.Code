using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.Interfaces
{
    /// <summary>
    /// Interface para serviço de alertas do sistema
    /// </summary>
    public interface IAlertService
    {
        /// <summary>
        /// Criar um novo alerta
        /// </summary>
        Task<AlertDto> CreateAlertAsync(CreateAlertDto dto, string tenantId);
        
        /// <summary>
        /// Obter alerta por ID
        /// </summary>
        Task<AlertDto?> GetAlertByIdAsync(Guid id, string tenantId);
        
        /// <summary>
        /// Obter alertas com filtros
        /// </summary>
        Task<List<AlertDto>> GetAlertsAsync(AlertFilterDto filter, string tenantId);
        
        /// <summary>
        /// Obter alertas ativos para um usuário
        /// </summary>
        Task<List<AlertDto>> GetActiveAlertsForUserAsync(Guid userId, string tenantId);
        
        /// <summary>
        /// Obter alertas críticos ativos
        /// </summary>
        Task<List<AlertDto>> GetCriticalAlertsAsync(string tenantId);
        
        /// <summary>
        /// Obter estatísticas de alertas
        /// </summary>
        Task<AlertStatisticsDto> GetAlertStatisticsAsync(string tenantId);
        
        /// <summary>
        /// Reconhecer alerta
        /// </summary>
        Task AcknowledgeAlertAsync(Guid alertId, Guid userId, string tenantId);
        
        /// <summary>
        /// Resolver alerta
        /// </summary>
        Task ResolveAlertAsync(Guid alertId, Guid userId, string? notes, string tenantId);
        
        /// <summary>
        /// Dispensar alerta
        /// </summary>
        Task DismissAlertAsync(Guid alertId, Guid userId, string tenantId);
        
        /// <summary>
        /// Marcar alertas expirados
        /// </summary>
        Task MarkExpiredAlertsAsync(string tenantId);
        
        /// <summary>
        /// Deletar alertas antigos
        /// </summary>
        Task DeleteOldAlertsAsync(int daysOld, string tenantId);
        
        /// <summary>
        /// Obter contagem de alertas ativos por usuário
        /// </summary>
        Task<int> GetActiveAlertCountForUserAsync(Guid userId, string tenantId);
        
        /// <summary>
        /// Obter contagem de alertas críticos
        /// </summary>
        Task<int> GetCriticalAlertCountAsync(string tenantId);
    }
}
