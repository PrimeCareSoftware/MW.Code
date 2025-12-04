using Microsoft.EntityFrameworkCore;
using MedicSoft.SystemAdmin.Api.Data;
using MedicSoft.SystemAdmin.Api.Models;
using BCrypt.Net;

namespace MedicSoft.SystemAdmin.Api.Services;

public interface ISystemAdminService
{
    Task<(IEnumerable<ClinicSummaryDto> clinics, int totalCount)> GetAllClinicsAsync(string? status, int page, int pageSize);
    Task<ClinicDetailDto?> GetClinicDetailsAsync(Guid clinicId);
    Task<(Guid clinicId, string tenantId)> CreateClinicAsync(CreateClinicRequest request);
    Task<bool> ToggleClinicStatusAsync(Guid clinicId);
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();
    Task<Guid> CreateSystemOwnerAsync(CreateSystemOwnerRequest request);
    Task<IEnumerable<object>> GetSystemOwnersAsync();
}

public class SystemAdminService : ISystemAdminService
{
    private readonly SystemAdminDbContext _context;
    private readonly ILogger<SystemAdminService> _logger;

    public SystemAdminService(SystemAdminDbContext context, ILogger<SystemAdminService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(IEnumerable<ClinicSummaryDto> clinics, int totalCount)> GetAllClinicsAsync(string? status, int page, int pageSize)
    {
        var query = _context.Clinics.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(c => c.IsActive == (status.ToLower() == "active"));
        }

        var totalCount = await query.CountAsync();

        var clinics = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new ClinicSummaryDto
            {
                Id = c.Id,
                Name = c.Name,
                Document = c.Document,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                IsActive = c.IsActive,
                TenantId = c.TenantId,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

        // Enrich with subscription info
        foreach (var clinic in clinics)
        {
            var subscription = await _context.ClinicSubscriptions
                .Where(s => s.ClinicId == clinic.Id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription != null)
            {
                var plan = await _context.SubscriptionPlans
                    .FirstOrDefaultAsync(p => p.Id == subscription.SubscriptionPlanId);

                clinic.SubscriptionStatus = GetStatusName(subscription.Status);
                clinic.PlanName = plan?.Name ?? "N/A";
                clinic.NextBillingDate = subscription.NextPaymentDate;
            }
        }

        return (clinics, totalCount);
    }

    public async Task<ClinicDetailDto?> GetClinicDetailsAsync(Guid clinicId)
    {
        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);

        if (clinic == null)
            return null;

        var subscription = await _context.ClinicSubscriptions
            .Where(s => s.ClinicId == clinicId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        var users = await _context.Users.Where(u => u.ClinicId == clinicId).ToListAsync();

        var plan = subscription != null
            ? await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == subscription.SubscriptionPlanId)
            : null;

        return new ClinicDetailDto
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Document = clinic.Document,
            Email = clinic.Email,
            Phone = clinic.Phone,
            Address = clinic.Address,
            IsActive = clinic.IsActive,
            TenantId = clinic.TenantId,
            CreatedAt = clinic.CreatedAt,
            SubscriptionStatus = subscription != null ? GetStatusName(subscription.Status) : "No Subscription",
            PlanName = plan?.Name ?? "N/A",
            PlanPrice = plan?.MonthlyPrice ?? 0,
            NextBillingDate = subscription?.NextPaymentDate,
            TrialEndsAt = subscription?.TrialEndDate,
            TotalUsers = users.Count,
            ActiveUsers = users.Count(u => u.IsActive)
        };
    }

    public async Task<(Guid clinicId, string tenantId)> CreateClinicAsync(CreateClinicRequest request)
    {
        var tenantId = Guid.NewGuid().ToString();

        var clinic = new ClinicEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            TradeName = request.Name,
            Document = request.Document,
            Phone = request.Phone,
            Email = request.Email,
            Address = request.Address,
            WorkStartTime = new TimeSpan(8, 0, 0),
            WorkEndTime = new TimeSpan(18, 0, 0),
            DefaultAppointmentDuration = 30,
            IsActive = true,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Clinics.Add(clinic);

        // Create owner
        var owner = new OwnerEntity
        {
            Id = Guid.NewGuid(),
            Username = request.OwnerUsername,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.OwnerPassword),
            FullName = request.OwnerFullName,
            Phone = request.Phone,
            IsActive = true,
            ClinicId = clinic.Id,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Owners.Add(owner);

        // Create subscription if plan is specified
        if (!string.IsNullOrEmpty(request.PlanId) && Guid.TryParse(request.PlanId, out var planId))
        {
            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId);
            if (plan != null)
            {
                var subscription = new ClinicSubscriptionEntity
                {
                    Id = Guid.NewGuid(),
                    ClinicId = clinic.Id,
                    SubscriptionPlanId = planId,
                    Status = 1, // Active
                    StartDate = DateTime.UtcNow,
                    NextPaymentDate = DateTime.UtcNow.AddMonths(1),
                    CurrentPrice = plan.MonthlyPrice,
                    TenantId = tenantId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ClinicSubscriptions.Add(subscription);
            }
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Created clinic: {ClinicId} with owner: {OwnerId}", clinic.Id, owner.Id);
        return (clinic.Id, tenantId);
    }

    public async Task<bool> ToggleClinicStatusAsync(Guid clinicId)
    {
        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);

        if (clinic == null)
            return false;

        clinic.IsActive = !clinic.IsActive;
        clinic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Toggled clinic status: {ClinicId} - Active: {IsActive}", clinicId, clinic.IsActive);
        return true;
    }

    public async Task<SystemAnalyticsDto> GetSystemAnalyticsAsync()
    {
        var totalClinics = await _context.Clinics.CountAsync();
        var activeClinics = await _context.Clinics.CountAsync(c => c.IsActive);
        var totalUsers = await _context.Users.CountAsync();
        var activeUsers = await _context.Users.CountAsync(u => u.IsActive);

        var subscriptionsByStatus = await _context.ClinicSubscriptions
            .GroupBy(s => s.Status)
            .Select(g => new { Status = GetStatusName(g.Key), Count = g.Count() })
            .ToListAsync();

        var subscriptionsByPlan = await _context.ClinicSubscriptions
            .Join(_context.SubscriptionPlans,
                s => s.SubscriptionPlanId,
                p => p.Id,
                (s, p) => new { Plan = p.Name })
            .GroupBy(x => x.Plan)
            .Select(g => new { Plan = g.Key, Count = g.Count() })
            .ToListAsync();

        var monthlyRevenue = await _context.ClinicSubscriptions
            .Where(s => s.Status == 1) // Active
            .Join(_context.SubscriptionPlans,
                s => s.SubscriptionPlanId,
                p => p.Id,
                (s, p) => p.MonthlyPrice)
            .SumAsync();

        return new SystemAnalyticsDto
        {
            TotalClinics = totalClinics,
            ActiveClinics = activeClinics,
            InactiveClinics = totalClinics - activeClinics,
            TotalUsers = totalUsers,
            ActiveUsers = activeUsers,
            TotalPatients = 0, // Would need patients table
            SubscriptionsByStatus = subscriptionsByStatus,
            SubscriptionsByPlan = subscriptionsByPlan,
            MonthlyRecurringRevenue = monthlyRevenue
        };
    }

    public async Task<Guid> CreateSystemOwnerAsync(CreateSystemOwnerRequest request)
    {
        // Check if username exists
        var existing = await _context.Owners
            .FirstOrDefaultAsync(o => o.Username == request.Username && o.TenantId == "system");

        if (existing != null)
            throw new InvalidOperationException("Username already exists");

        var owner = new OwnerEntity
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            FullName = request.FullName,
            Phone = request.Phone,
            IsActive = true,
            ClinicId = null, // System owner has no clinic
            TenantId = "system",
            CreatedAt = DateTime.UtcNow
        };

        _context.Owners.Add(owner);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created system owner: {OwnerId} - {Username}", owner.Id, owner.Username);
        return owner.Id;
    }

    public async Task<IEnumerable<object>> GetSystemOwnersAsync()
    {
        var owners = await _context.Owners
            .Where(o => o.ClinicId == null && o.TenantId == "system")
            .Select(o => new
            {
                o.Id,
                o.Username,
                o.Email,
                o.FullName,
                o.Phone,
                o.IsActive,
                o.LastLoginAt,
                IsSystemOwner = true
            })
            .ToListAsync();

        return owners;
    }

    private static string GetStatusName(int status)
    {
        return status switch
        {
            0 => "Trial",
            1 => "Active",
            2 => "PastDue",
            3 => "Cancelled",
            4 => "Suspended",
            _ => "Unknown"
        };
    }
}
