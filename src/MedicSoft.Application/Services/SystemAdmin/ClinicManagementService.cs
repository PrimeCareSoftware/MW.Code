using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface IClinicManagementService
    {
        Task<ClinicDetailDto?> GetClinicDetail(Guid clinicId);
        Task<ClinicHealthScoreDto> CalculateHealthScore(Guid clinicId);
        Task<List<ClinicTimelineEventDto>> GetTimeline(Guid clinicId, int limit = 50);
        Task<ClinicUsageMetricsDto> GetUsageMetrics(Guid clinicId, DateTime? periodStart = null, DateTime? periodEnd = null);
        Task<(List<ClinicDetailDto> Clinics, int TotalCount)> GetClinicsWithFilters(ClinicFilterDto filters);
    }

    public class ClinicManagementService : IClinicManagementService
    {
        private readonly MedicSoftDbContext _context;

        public ClinicManagementService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicDetailDto?> GetClinicDetail(Guid clinicId)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.Id == clinicId)
                .Select(c => new ClinicDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    TradeName = c.TradeName,
                    Document = c.Document,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    IsActive = c.IsActive,
                    Subdomain = c.Subdomain,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (clinic == null)
                return null;

            // Get subscription info
            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == clinicId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SubscriptionInfoDto
                {
                    PlanName = s.SubscriptionPlan!.Name,
                    Status = s.Status.ToString(),
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    TrialEndDate = s.TrialEndDate,
                    CurrentPrice = s.CurrentPrice
                })
                .FirstOrDefaultAsync();

            clinic.CurrentSubscription = subscription;

            // Get user counts
            var userCounts = await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == clinicId.ToString())
                .GroupBy(u => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Active = g.Count(u => u.IsActive)
                })
                .FirstOrDefaultAsync();

            clinic.TotalUsers = userCounts?.Total ?? 0;
            clinic.ActiveUsers = userCounts?.Active ?? 0;

            // Get ticket counts (if ticket system exists)
            var ticketCounts = await _context.Set<Domain.Entities.SupportTicket>()
                .IgnoreQueryFilters()
                .Where(t => t.ClinicId == clinicId)
                .GroupBy(t => 1)
                .Select(g => new
                {
                    Total = g.Count(),
                    Open = g.Count(t => t.Status == Domain.Entities.TicketStatus.Open || t.Status == Domain.Entities.TicketStatus.InProgress)
                })
                .FirstOrDefaultAsync();

            clinic.TotalTickets = ticketCounts?.Total ?? 0;
            clinic.OpenTickets = ticketCounts?.Open ?? 0;

            // Get tags
            var tags = await _context.Set<Domain.Entities.ClinicTag>()
                .IgnoreQueryFilters()
                .Where(ct => ct.ClinicId == clinicId)
                .Select(ct => new TagDto
                {
                    Id = ct.Tag!.Id,
                    Name = ct.Tag.Name,
                    Description = ct.Tag.Description,
                    Category = ct.Tag.Category,
                    Color = ct.Tag.Color,
                    IsAutomatic = ct.Tag.IsAutomatic,
                    Order = ct.Tag.Order,
                    CreatedAt = ct.Tag.CreatedAt
                })
                .ToListAsync();

            clinic.Tags = tags;

            return clinic;
        }

        public async Task<ClinicHealthScoreDto> CalculateHealthScore(Guid clinicId)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == clinicId);

            if (clinic == null)
                throw new ArgumentException($"Clinic with ID {clinicId} not found");

            var score = new ClinicHealthScoreDto
            {
                ClinicId = clinicId,
                CalculatedAt = DateTime.UtcNow
            };

            // 1. Frequência de Uso (0-30 pontos)
            var lastActivity = await GetLastActivity(clinicId);
            score.LastActivity = lastActivity;
            var daysSinceActivity = lastActivity.HasValue 
                ? (DateTime.UtcNow - lastActivity.Value).Days 
                : 999;
            score.DaysSinceActivity = daysSinceActivity;

            score.UsageScore = daysSinceActivity switch
            {
                <= 1 => 30,
                <= 7 => 25,
                <= 14 => 20,
                <= 30 => 10,
                _ => 0
            };

            // 2. Usuários Ativos (0-25 pontos)
            var activeUsersCount = await GetActiveUsersCount(clinicId, days: 30);
            var totalUsersCount = await _context.Users
                .IgnoreQueryFilters()
                .CountAsync(u => u.TenantId == clinicId.ToString());

            score.ActiveUsersCount = activeUsersCount;
            score.TotalUsersCount = totalUsersCount;

            score.UserEngagementScore = totalUsersCount > 0
                ? (int)(25 * (activeUsersCount / (double)totalUsersCount))
                : 0;

            // 3. Tickets Abertos (0-20 pontos)
            var openTicketsCount = await _context.Set<Domain.Entities.SupportTicket>()
                .IgnoreQueryFilters()
                .CountAsync(t => t.ClinicId == clinicId && 
                    (t.Status == Domain.Entities.TicketStatus.Open || t.Status == Domain.Entities.TicketStatus.InProgress));

            score.OpenTicketsCount = openTicketsCount;
            score.SupportScore = openTicketsCount switch
            {
                0 => 20,
                1 => 15,
                2 => 10,
                3 => 5,
                _ => 0
            };

            // 4. Pagamentos em Dia (0-25 pontos)
            var hasPaymentIssues = await HasPaymentIssues(clinicId);
            score.HasPaymentIssues = hasPaymentIssues;
            score.PaymentScore = hasPaymentIssues ? 0 : 25;

            // Total Score
            score.TotalScore = score.UsageScore + score.UserEngagementScore
                + score.SupportScore + score.PaymentScore;

            score.HealthStatus = score.TotalScore switch
            {
                >= 80 => HealthStatus.Healthy,
                >= 50 => HealthStatus.NeedsAttention,
                _ => HealthStatus.AtRisk
            };

            return score;
        }

        public async Task<List<ClinicTimelineEventDto>> GetTimeline(Guid clinicId, int limit = 50)
        {
            var events = new List<ClinicTimelineEventDto>();

            // Eventos de assinatura
            var subscriptionEvents = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == clinicId)
                .OrderByDescending(s => s.CreatedAt)
                .Take(20)
                .Select(s => new ClinicTimelineEventDto
                {
                    Type = "subscription",
                    Title = $"Plano {s.SubscriptionPlan!.Name}",
                    Description = s.Status.ToString(),
                    Date = s.CreatedAt,
                    Icon = "card_membership"
                })
                .ToListAsync();

            events.AddRange(subscriptionEvents);

            // Eventos de tickets
            var ticketEvents = await _context.Set<Domain.Entities.SupportTicket>()
                .IgnoreQueryFilters()
                .Where(t => t.ClinicId == clinicId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(20)
                .Select(t => new ClinicTimelineEventDto
                {
                    Type = "ticket",
                    Title = $"Ticket #{t.Id}",
                    Description = t.Subject,
                    Date = t.CreatedAt,
                    Icon = "support"
                })
                .ToListAsync();

            events.AddRange(ticketEvents);

            // Eventos de usuários (criação)
            var userEvents = await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.TenantId == clinicId.ToString())
                .OrderByDescending(u => u.CreatedAt)
                .Take(20)
                .Select(u => new ClinicTimelineEventDto
                {
                    Type = "user",
                    Title = $"Usuário criado: {u.Name}",
                    Description = $"Role: {u.Role}",
                    Date = u.CreatedAt,
                    Icon = "person_add"
                })
                .ToListAsync();

            events.AddRange(userEvents);

            return events
                .OrderByDescending(e => e.Date)
                .Take(limit)
                .ToList();
        }

        public async Task<ClinicUsageMetricsDto> GetUsageMetrics(Guid clinicId, DateTime? periodStart = null, DateTime? periodEnd = null)
        {
            periodEnd ??= DateTime.UtcNow;
            periodStart ??= periodEnd.Value.AddDays(-30);

            var tenantId = clinicId.ToString();

            var metrics = new ClinicUsageMetricsDto
            {
                ClinicId = clinicId,
                MetricsPeriodStart = periodStart.Value,
                MetricsPeriodEnd = periodEnd.Value
            };

            // Get login counts
            var last7Days = DateTime.UtcNow.AddDays(-7);
            var last30Days = DateTime.UtcNow.AddDays(-30);

            metrics.Last7DaysLogins = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .CountAsync(s => s.TenantId == tenantId && s.CreatedAt >= last7Days);

            metrics.Last30DaysLogins = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .CountAsync(s => s.TenantId == tenantId && s.CreatedAt >= last30Days);

            metrics.LastLoginDate = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => (DateTime?)s.CreatedAt)
                .FirstOrDefaultAsync();

            // Get appointments created
            metrics.AppointmentsCreated = await _context.Appointments
                .IgnoreQueryFilters()
                .CountAsync(a => a.TenantId == tenantId && 
                    a.CreatedAt >= periodStart.Value && a.CreatedAt <= periodEnd.Value);

            // Get patients registered
            metrics.PatientsRegistered = await _context.Patients
                .IgnoreQueryFilters()
                .CountAsync(p => p.TenantId == tenantId && 
                    p.CreatedAt >= periodStart.Value && p.CreatedAt <= periodEnd.Value);

            // Documents generated (if document system exists)
            try
            {
                metrics.DocumentsGenerated = await _context.Set<Domain.Entities.Document>()
                    .IgnoreQueryFilters()
                    .CountAsync(d => d.TenantId == tenantId && 
                        d.CreatedAt >= periodStart.Value && d.CreatedAt <= periodEnd.Value);
            }
            catch
            {
                metrics.DocumentsGenerated = 0;
            }

            return metrics;
        }

        public async Task<(List<ClinicDetailDto> Clinics, int TotalCount)> GetClinicsWithFilters(ClinicFilterDto filters)
        {
            var query = _context.Clinics.IgnoreQueryFilters().AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchLower = filters.SearchTerm.ToLower();
                query = query.Where(c =>
                    c.Name.ToLower().Contains(searchLower) ||
                    c.TradeName.ToLower().Contains(searchLower) ||
                    c.Document.Contains(filters.SearchTerm) ||
                    c.Email.ToLower().Contains(searchLower));
            }

            if (filters.IsActive.HasValue)
            {
                query = query.Where(c => c.IsActive == filters.IsActive.Value);
            }

            if (filters.CreatedAfter.HasValue)
            {
                query = query.Where(c => c.CreatedAt >= filters.CreatedAfter.Value);
            }

            if (filters.CreatedBefore.HasValue)
            {
                query = query.Where(c => c.CreatedAt <= filters.CreatedBefore.Value);
            }

            // Filter by tags if specified
            if (filters.Tags != null && filters.Tags.Any())
            {
                var clinicIdsWithTags = await _context.Set<Domain.Entities.ClinicTag>()
                    .IgnoreQueryFilters()
                    .Where(ct => filters.Tags.Contains(ct.Tag!.Name))
                    .Select(ct => ct.ClinicId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(c => clinicIdsWithTags.Contains(c.Id));
            }

            var totalCount = await query.CountAsync();

            // Apply sorting
            query = filters.SortBy?.ToLower() switch
            {
                "name" => filters.SortDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "createdat" => filters.SortDescending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt),
                _ => query.OrderBy(c => c.Name)
            };

            // Pagination
            var clinics = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(c => new ClinicDetailDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    TradeName = c.TradeName,
                    Document = c.Document,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    IsActive = c.IsActive,
                    Subdomain = c.Subdomain,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            // Load additional data for each clinic
            foreach (var clinic in clinics)
            {
                // Get user counts
                var userCounts = await _context.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.TenantId == clinic.Id.ToString())
                    .GroupBy(u => 1)
                    .Select(g => new { Total = g.Count(), Active = g.Count(u => u.IsActive) })
                    .FirstOrDefaultAsync();

                clinic.TotalUsers = userCounts?.Total ?? 0;
                clinic.ActiveUsers = userCounts?.Active ?? 0;

                // Get tags
                clinic.Tags = await _context.Set<Domain.Entities.ClinicTag>()
                    .IgnoreQueryFilters()
                    .Where(ct => ct.ClinicId == clinic.Id)
                    .Select(ct => new TagDto
                    {
                        Id = ct.Tag!.Id,
                        Name = ct.Tag.Name,
                        Category = ct.Tag.Category,
                        Color = ct.Tag.Color
                    })
                    .ToListAsync();
            }

            return (clinics, totalCount);
        }

        private async Task<DateTime?> GetLastActivity(Guid clinicId)
        {
            var tenantId = clinicId.ToString();

            var lastLogin = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .Where(s => s.TenantId == tenantId)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => (DateTime?)s.CreatedAt)
                .FirstOrDefaultAsync();

            return lastLogin;
        }

        private async Task<int> GetActiveUsersCount(Guid clinicId, int days)
        {
            var tenantId = clinicId.ToString();
            var sinceDate = DateTime.UtcNow.AddDays(-days);

            var activeUserIds = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .Where(s => s.TenantId == tenantId && s.CreatedAt >= sinceDate)
                .Select(s => s.UserId)
                .Distinct()
                .CountAsync();

            return activeUserIds;
        }

        private async Task<bool> HasPaymentIssues(Guid clinicId)
        {
            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == clinicId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return true;

            // Check if subscription is in a bad payment state
            if (subscription.Status == Domain.Entities.SubscriptionStatus.PaymentPending ||
                subscription.Status == Domain.Entities.SubscriptionStatus.PaymentFailed ||
                subscription.Status == Domain.Entities.SubscriptionStatus.Suspended)
                return true;

            // Check if next payment is overdue
            if (subscription.NextPaymentDate.HasValue && subscription.NextPaymentDate.Value < DateTime.UtcNow)
                return true;

            return false;
        }
    }
}
