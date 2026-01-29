using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface IGlobalSearchService
    {
        Task<GlobalSearchResultDto> SearchAsync(string query, int maxResults = 50);
    }

    public class GlobalSearchService : IGlobalSearchService
    {
        private readonly MedicSoftDbContext _context;

        public GlobalSearchService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<GlobalSearchResultDto> SearchAsync(string query, int maxResults = 50)
        {
            var stopwatch = Stopwatch.StartNew();
            
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                return new GlobalSearchResultDto { SearchDurationMs = stopwatch.Elapsed.TotalMilliseconds };
            }

            var searchTerm = query.Trim().ToLower();
            var result = new GlobalSearchResultDto();

            // Search in parallel
            var clinicsTask = SearchClinicsAsync(searchTerm, maxResults);
            var usersTask = SearchUsersAsync(searchTerm, maxResults);
            var ticketsTask = SearchTicketsAsync(searchTerm, maxResults);
            var plansTask = SearchPlansAsync(searchTerm, maxResults);
            var auditLogsTask = SearchAuditLogsAsync(searchTerm, maxResults);

            await Task.WhenAll(clinicsTask, usersTask, ticketsTask, plansTask, auditLogsTask);

            result.Clinics = await clinicsTask;
            result.Users = await usersTask;
            result.Tickets = await ticketsTask;
            result.Plans = await plansTask;
            result.AuditLogs = await auditLogsTask;

            result.TotalResults = result.Clinics.Count + result.Users.Count + 
                                 result.Tickets.Count + result.Plans.Count + result.AuditLogs.Count;

            stopwatch.Stop();
            result.SearchDurationMs = stopwatch.Elapsed.TotalMilliseconds;

            return result;
        }

        private async Task<List<ClinicSearchResult>> SearchClinicsAsync(string query, int max)
        {
            return await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c =>
                    EF.Functions.Like(c.Name.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(c.Document.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(c.Email.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(c.TenantId.ToLower(), $"%{query}%"))
                .OrderBy(c => c.Name)
                .Take(max)
                .Select(c => new ClinicSearchResult
                {
                    Id = c.Id,
                    Name = c.Name,
                    Document = c.Document,
                    Email = c.Email,
                    TenantId = c.TenantId,
                    IsActive = c.IsActive,
                    PlanName = _context.ClinicSubscriptions
                        .Where(s => s.ClinicId == c.Id)
                        .OrderByDescending(s => s.CreatedAt)
                        .Select(s => s.SubscriptionPlan!.Name)
                        .FirstOrDefault() ?? "N/A",
                    Status = c.IsActive ? "Active" : "Inactive"
                })
                .ToListAsync();
        }

        private async Task<List<UserSearchResult>> SearchUsersAsync(string query, int max)
        {
            return await _context.Users
                .IgnoreQueryFilters()
                .Where(u =>
                    EF.Functions.Like(u.Username.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(u.FullName.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(u.Email.ToLower(), $"%{query}%"))
                .OrderBy(u => u.Username)
                .Take(max)
                .Select(u => new UserSearchResult
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    ClinicName = u.ClinicLinks
                        .Select(l => l.Clinic!.Name)
                        .FirstOrDefault() ?? "N/A"
                })
                .ToListAsync();
        }

        private async Task<List<TicketSearchResult>> SearchTicketsAsync(string query, int max)
        {
            return await _context.Tickets
                .IgnoreQueryFilters()
                .Where(t =>
                    EF.Functions.Like(t.Title.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(t.Description.ToLower(), $"%{query}%"))
                .OrderByDescending(t => t.CreatedAt)
                .Take(max)
                .Select(t => new TicketSearchResult
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description.Length > 100 
                        ? t.Description.Substring(0, 100) + "..." 
                        : t.Description,
                    Status = t.Status.ToString(),
                    Priority = t.Priority.ToString(),
                    CreatedAt = t.CreatedAt,
                    ClinicName = t.ClinicId != null 
                        ? _context.Clinics.Where(c => c.Id == t.ClinicId).Select(c => c.Name).FirstOrDefault() ?? "System"
                        : "System"
                })
                .ToListAsync();
        }

        private async Task<List<PlanSearchResult>> SearchPlansAsync(string query, int max)
        {
            return await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .Where(p =>
                    EF.Functions.Like(p.Name.ToLower(), $"%{query}%") ||
                    EF.Functions.Like((p.Description ?? "").ToLower(), $"%{query}%"))
                .OrderBy(p => p.Name)
                .Take(max)
                .Select(p => new PlanSearchResult
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description ?? "",
                    MonthlyPrice = p.MonthlyPrice,
                    IsActive = p.IsActive,
                    ActiveSubscriptions = p.ClinicSubscriptions.Count(s => s.Status == "Active")
                })
                .ToListAsync();
        }

        private async Task<List<AuditLogSearchResult>> SearchAuditLogsAsync(string query, int max)
        {
            return await _context.AuditLogs
                .IgnoreQueryFilters()
                .Where(a =>
                    EF.Functions.Like(a.Action.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(a.EntityType.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(a.EntityId.ToLower(), $"%{query}%") ||
                    EF.Functions.Like((a.UserName ?? "").ToLower(), $"%{query}%"))
                .OrderByDescending(a => a.Timestamp)
                .Take(max)
                .Select(a => new AuditLogSearchResult
                {
                    Id = a.Id,
                    Action = a.Action,
                    EntityType = a.EntityType,
                    EntityId = a.EntityId,
                    UserName = a.UserName ?? "System",
                    Timestamp = a.Timestamp
                })
                .ToListAsync();
        }
    }
}
