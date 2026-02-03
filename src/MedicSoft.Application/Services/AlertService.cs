using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Serviço para gerenciamento de alertas do sistema
    /// </summary>
    public class AlertService : IAlertService
    {
        private readonly MedicSoftDbContext _context;
        
        public AlertService(MedicSoftDbContext context)
        {
            _context = context;
        }
        
        public async Task<AlertDto> CreateAlertAsync(CreateAlertDto dto, string tenantId)
        {
            var alert = new Alert(
                dto.Category,
                dto.Priority,
                dto.Title,
                dto.Message,
                dto.RecipientType,
                tenantId,
                dto.UserId,
                dto.Role,
                dto.ClinicId,
                dto.ActionUrl,
                dto.SuggestedAction,
                dto.ActionLabel,
                dto.RelatedEntityType,
                dto.RelatedEntityId,
                dto.ExpiresAt,
                dto.DeliveryChannels,
                dto.Metadata
            );
            
            _context.Alerts.Add(alert);
            await _context.SaveChangesAsync();
            
            return MapToDto(alert);
        }
        
        public async Task<AlertDto?> GetAlertByIdAsync(Guid id, string tenantId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);
            
            return alert != null ? MapToDto(alert) : null;
        }
        
        public async Task<List<AlertDto>> GetAlertsAsync(AlertFilterDto filter, string tenantId)
        {
            var query = _context.Alerts.Where(a => a.TenantId == tenantId);
            
            if (filter.Status.HasValue)
                query = query.Where(a => a.Status == filter.Status.Value);
            
            if (filter.Priority.HasValue)
                query = query.Where(a => a.Priority == filter.Priority.Value);
            
            if (filter.Category.HasValue)
                query = query.Where(a => a.Category == filter.Category.Value);
            
            if (filter.UserId.HasValue)
                query = query.Where(a => a.UserId == filter.UserId.Value || 
                                        a.RecipientType == AlertRecipientType.Clinic ||
                                        a.RecipientType == AlertRecipientType.Role);
            
            if (filter.ClinicId.HasValue)
                query = query.Where(a => a.ClinicId == filter.ClinicId.Value);
            
            if (!filter.IncludeExpired)
                query = query.Where(a => a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow);
            
            var alerts = await query
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
            
            return alerts.Select(MapToDto).ToList();
        }
        
        public async Task<List<AlertDto>> GetActiveAlertsForUserAsync(Guid userId, string tenantId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId &&
                           a.Status == AlertStatus.Active &&
                           (a.UserId == userId || 
                            a.RecipientType == AlertRecipientType.Clinic ||
                            a.RecipientType == AlertRecipientType.Role) &&
                           (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(a => a.Priority)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
            
            return alerts.Select(MapToDto).ToList();
        }
        
        public async Task<List<AlertDto>> GetCriticalAlertsAsync(string tenantId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId &&
                           a.Priority == AlertPriority.Critical &&
                           a.Status == AlertStatus.Active &&
                           (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
            
            return alerts.Select(MapToDto).ToList();
        }
        
        public async Task<AlertStatisticsDto> GetAlertStatisticsAsync(string tenantId)
        {
            var alerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId)
                .ToListAsync();
            
            var stats = new AlertStatisticsDto
            {
                TotalActive = alerts.Count(a => a.Status == AlertStatus.Active),
                TotalAcknowledged = alerts.Count(a => a.Status == AlertStatus.Acknowledged),
                TotalResolved = alerts.Count(a => a.Status == AlertStatus.Resolved),
                TotalDismissed = alerts.Count(a => a.Status == AlertStatus.Dismissed),
                TotalExpired = alerts.Count(a => a.Status == AlertStatus.Expired),
                
                CriticalCount = alerts.Count(a => a.Priority == AlertPriority.Critical && a.Status == AlertStatus.Active),
                HighCount = alerts.Count(a => a.Priority == AlertPriority.High && a.Status == AlertStatus.Active),
                NormalCount = alerts.Count(a => a.Priority == AlertPriority.Normal && a.Status == AlertStatus.Active),
                LowCount = alerts.Count(a => a.Priority == AlertPriority.Low && a.Status == AlertStatus.Active),
                
                CategoryCounts = alerts
                    .GroupBy(a => a.Category.ToString())
                    .ToDictionary(g => g.Key, g => g.Count())
            };
            
            return stats;
        }
        
        public async Task AcknowledgeAlertAsync(Guid alertId, Guid userId, string tenantId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.TenantId == tenantId);
            
            if (alert == null)
                throw new KeyNotFoundException("Alert not found");
            
            alert.Acknowledge(userId);
            await _context.SaveChangesAsync();
        }
        
        public async Task ResolveAlertAsync(Guid alertId, Guid userId, string? notes, string tenantId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.TenantId == tenantId);
            
            if (alert == null)
                throw new KeyNotFoundException("Alert not found");
            
            alert.Resolve(userId, notes);
            await _context.SaveChangesAsync();
        }
        
        public async Task DismissAlertAsync(Guid alertId, Guid userId, string tenantId)
        {
            var alert = await _context.Alerts
                .FirstOrDefaultAsync(a => a.Id == alertId && a.TenantId == tenantId);
            
            if (alert == null)
                throw new KeyNotFoundException("Alert not found");
            
            alert.Dismiss(userId);
            await _context.SaveChangesAsync();
        }
        
        public async Task MarkExpiredAlertsAsync(string tenantId)
        {
            var expiredAlerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId &&
                           (a.Status == AlertStatus.Active || a.Status == AlertStatus.Acknowledged) &&
                           a.ExpiresAt.HasValue &&
                           a.ExpiresAt.Value <= DateTime.UtcNow)
                .ToListAsync();
            
            foreach (var alert in expiredAlerts)
            {
                alert.MarkAsExpired();
            }
            
            if (expiredAlerts.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteOldAlertsAsync(int daysOld, string tenantId)
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
            
            var oldAlerts = await _context.Alerts
                .Where(a => a.TenantId == tenantId &&
                           (a.Status == AlertStatus.Resolved || 
                            a.Status == AlertStatus.Dismissed ||
                            a.Status == AlertStatus.Expired) &&
                           a.UpdatedAt < cutoffDate)
                .ToListAsync();
            
            _context.Alerts.RemoveRange(oldAlerts);
            
            if (oldAlerts.Any())
            {
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task<int> GetActiveAlertCountForUserAsync(Guid userId, string tenantId)
        {
            return await _context.Alerts
                .CountAsync(a => a.TenantId == tenantId &&
                                a.Status == AlertStatus.Active &&
                                (a.UserId == userId || 
                                 a.RecipientType == AlertRecipientType.Clinic ||
                                 a.RecipientType == AlertRecipientType.Role) &&
                                (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow));
        }
        
        public async Task<int> GetCriticalAlertCountAsync(string tenantId)
        {
            return await _context.Alerts
                .CountAsync(a => a.TenantId == tenantId &&
                                a.Priority == AlertPriority.Critical &&
                                a.Status == AlertStatus.Active &&
                                (a.ExpiresAt == null || a.ExpiresAt > DateTime.UtcNow));
        }
        
        private AlertDto MapToDto(Alert alert)
        {
            return new AlertDto
            {
                Id = alert.Id,
                Category = alert.Category.ToString(),
                Priority = alert.Priority.ToString(),
                Status = alert.Status.ToString(),
                Title = alert.Title,
                Message = alert.Message,
                ActionUrl = alert.ActionUrl,
                SuggestedAction = alert.SuggestedAction.ToString(),
                ActionLabel = alert.ActionLabel,
                RecipientType = alert.RecipientType.ToString(),
                UserId = alert.UserId,
                Role = alert.Role,
                ClinicId = alert.ClinicId,
                RelatedEntityType = alert.RelatedEntityType,
                RelatedEntityId = alert.RelatedEntityId,
                AcknowledgedAt = alert.AcknowledgedAt,
                AcknowledgedBy = alert.AcknowledgedBy,
                ResolvedAt = alert.ResolvedAt,
                ResolvedBy = alert.ResolvedBy,
                ResolutionNotes = alert.ResolutionNotes,
                ExpiresAt = alert.ExpiresAt,
                DeliveryChannels = alert.DeliveryChannels.Select(c => c.ToString()).ToList(),
                Metadata = alert.Metadata,
                CreatedAt = alert.CreatedAt,
                UpdatedAt = alert.UpdatedAt ?? alert.CreatedAt,
                IsExpired = alert.IsExpired(),
                RequiresUrgentAction = alert.RequiresUrgentAction()
            };
        }
    }
}
