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
    Task<bool> UpdateClinicAsync(Guid clinicId, UpdateClinicRequest request);
    Task<bool> ToggleClinicStatusAsync(Guid clinicId);
    Task<SystemAnalyticsDto> GetSystemAnalyticsAsync();
    Task<Guid> CreateSystemOwnerAsync(CreateSystemOwnerRequest request);
    Task<IEnumerable<object>> GetSystemOwnersAsync();
    
    // Subscription Plans
    Task<IEnumerable<SubscriptionPlanDto>> GetSubscriptionPlansAsync(bool? activeOnly = null);
    Task<SubscriptionPlanDto?> GetSubscriptionPlanAsync(Guid planId);
    Task<Guid> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequest request);
    Task<bool> UpdateSubscriptionPlanAsync(Guid planId, UpdateSubscriptionPlanRequest request);
    Task<bool> DeleteSubscriptionPlanAsync(Guid planId);
    Task<bool> ToggleSubscriptionPlanStatusAsync(Guid planId);
    
    // Subscription Management
    Task<bool> UpdateClinicSubscriptionAsync(Guid clinicId, UpdateSubscriptionRequest request);
    Task<bool> EnableManualOverrideAsync(Guid clinicId, EnableManualOverrideRequest request);
    Task<bool> DisableManualOverrideAsync(Guid clinicId);
    
    // Clinic Owners Management
    Task<IEnumerable<ClinicOwnerDto>> GetClinicOwnersAsync(Guid? clinicId = null);
    Task<bool> ResetOwnerPasswordAsync(Guid ownerId, string newPassword);
    Task<bool> ToggleOwnerStatusAsync(Guid ownerId);
    
    // Subdomain Management
    Task<IEnumerable<SubdomainDto>> GetSubdomainsAsync();
    Task<SubdomainDto?> GetSubdomainByNameAsync(string subdomain);
    Task<Guid> CreateSubdomainAsync(CreateSubdomainRequest request);
    Task<bool> DeleteSubdomainAsync(Guid subdomainId);
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
            // TODO: Patient count requires cross-microservice communication with Patients microservice
            // Consider implementing an API call to Patients service or data aggregation strategy
            TotalPatients = 0,
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

    public async Task<bool> UpdateClinicAsync(Guid clinicId, UpdateClinicRequest request)
    {
        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == clinicId);
        if (clinic == null)
            return false;

        clinic.Name = request.Name;
        clinic.Document = request.Document;
        clinic.Email = request.Email;
        clinic.Phone = request.Phone;
        clinic.Address = request.Address;
        clinic.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated clinic: {ClinicId}", clinicId);
        return true;
    }

    // Subscription Plans Management
    public async Task<IEnumerable<SubscriptionPlanDto>> GetSubscriptionPlansAsync(bool? activeOnly = null)
    {
        var query = _context.SubscriptionPlans.AsQueryable();

        if (activeOnly.HasValue && activeOnly.Value)
        {
            query = query.Where(p => p.IsActive);
        }

        var plans = await query
            .OrderBy(p => p.Name)
            .Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                MonthlyPrice = p.MonthlyPrice,
                YearlyPrice = p.YearlyPrice,
                MaxUsers = p.MaxUsers,
                MaxPatients = p.MaxPatients,
                TrialDays = p.TrialDays,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync();

        return plans;
    }

    public async Task<SubscriptionPlanDto?> GetSubscriptionPlanAsync(Guid planId)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
            return null;

        return new SubscriptionPlanDto
        {
            Id = plan.Id,
            Name = plan.Name,
            Description = plan.Description,
            MonthlyPrice = plan.MonthlyPrice,
            YearlyPrice = plan.YearlyPrice,
            MaxUsers = plan.MaxUsers,
            MaxPatients = plan.MaxPatients,
            TrialDays = plan.TrialDays,
            IsActive = plan.IsActive,
            CreatedAt = plan.CreatedAt
        };
    }

    public async Task<Guid> CreateSubscriptionPlanAsync(CreateSubscriptionPlanRequest request)
    {
        var plan = new SubscriptionPlanEntity
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            MonthlyPrice = request.MonthlyPrice,
            YearlyPrice = request.YearlyPrice,
            MaxUsers = request.MaxUsers,
            MaxPatients = request.MaxPatients,
            TrialDays = request.TrialDays,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.SubscriptionPlans.Add(plan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created subscription plan: {PlanId} - {PlanName}", plan.Id, plan.Name);
        return plan.Id;
    }

    public async Task<bool> UpdateSubscriptionPlanAsync(Guid planId, UpdateSubscriptionPlanRequest request)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
            return false;

        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.MonthlyPrice = request.MonthlyPrice;
        plan.YearlyPrice = request.YearlyPrice;
        plan.MaxUsers = request.MaxUsers;
        plan.MaxPatients = request.MaxPatients;
        plan.TrialDays = request.TrialDays;
        plan.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        _logger.LogInformation("Updated subscription plan: {PlanId}", planId);
        return true;
    }

    public async Task<bool> DeleteSubscriptionPlanAsync(Guid planId)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
            return false;

        // Check if any clinics are using this plan
        var subscriptionsCount = await _context.ClinicSubscriptions
            .CountAsync(s => s.SubscriptionPlanId == planId);

        if (subscriptionsCount > 0)
        {
            throw new InvalidOperationException("Não é possível excluir um plano que está sendo usado por clínicas");
        }

        _context.SubscriptionPlans.Remove(plan);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted subscription plan: {PlanId}", planId);
        return true;
    }

    public async Task<bool> ToggleSubscriptionPlanStatusAsync(Guid planId)
    {
        var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == planId);
        if (plan == null)
            return false;

        plan.IsActive = !plan.IsActive;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Toggled subscription plan status: {PlanId} - Active: {IsActive}", planId, plan.IsActive);
        return true;
    }

    // Subscription Management
    public async Task<bool> UpdateClinicSubscriptionAsync(Guid clinicId, UpdateSubscriptionRequest request)
    {
        var subscription = await _context.ClinicSubscriptions
            .Where(s => s.ClinicId == clinicId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return false;

        if (request.NewPlanId != Guid.Empty)
        {
            var plan = await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == request.NewPlanId);
            if (plan != null)
            {
                subscription.SubscriptionPlanId = request.NewPlanId;
                subscription.CurrentPrice = plan.MonthlyPrice;
            }
        }

        if (request.Status.HasValue)
        {
            subscription.Status = request.Status.Value;
        }

        subscription.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Updated clinic subscription: {ClinicId}", clinicId);
        return true;
    }

    public async Task<bool> EnableManualOverrideAsync(Guid clinicId, EnableManualOverrideRequest request)
    {
        var subscription = await _context.ClinicSubscriptions
            .Where(s => s.ClinicId == clinicId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return false;

        subscription.HasManualOverride = true;
        subscription.ManualOverrideReason = request.Reason;
        subscription.ManualOverrideSetBy = "System Admin"; // TODO: Get from authenticated user
        subscription.ManualOverrideSetAt = DateTime.UtcNow;
        subscription.ManualOverrideExpiresAt = request.ExtendUntil;
        subscription.Status = 1; // Set to Active
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Enabled manual override for clinic: {ClinicId}", clinicId);
        return true;
    }

    public async Task<bool> DisableManualOverrideAsync(Guid clinicId)
    {
        var subscription = await _context.ClinicSubscriptions
            .Where(s => s.ClinicId == clinicId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return false;

        subscription.HasManualOverride = false;
        subscription.ManualOverrideReason = null;
        subscription.ManualOverrideSetBy = null;
        subscription.ManualOverrideSetAt = null;
        subscription.ManualOverrideExpiresAt = null;
        subscription.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Disabled manual override for clinic: {ClinicId}", clinicId);
        return true;
    }

    // Clinic Owners Management
    public async Task<IEnumerable<ClinicOwnerDto>> GetClinicOwnersAsync(Guid? clinicId = null)
    {
        var query = _context.Owners.AsQueryable();

        if (clinicId.HasValue)
        {
            query = query.Where(o => o.ClinicId == clinicId.Value);
        }

        var owners = await query
            .OrderBy(o => o.FullName)
            .ToListAsync();

        var result = new List<ClinicOwnerDto>();

        foreach (var owner in owners)
        {
            string? clinicName = null;
            if (owner.ClinicId.HasValue)
            {
                var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == owner.ClinicId.Value);
                clinicName = clinic?.Name;
            }

            result.Add(new ClinicOwnerDto
            {
                Id = owner.Id,
                Username = owner.Username,
                Email = owner.Email,
                FullName = owner.FullName,
                Phone = owner.Phone,
                IsActive = owner.IsActive,
                ClinicId = owner.ClinicId,
                ClinicName = clinicName,
                LastLoginAt = owner.LastLoginAt,
                CreatedAt = owner.CreatedAt
            });
        }

        return result;
    }

    public async Task<bool> ResetOwnerPasswordAsync(Guid ownerId, string newPassword)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
        if (owner == null)
            return false;

        owner.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        owner.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Reset password for owner: {OwnerId}", ownerId);
        return true;
    }

    public async Task<bool> ToggleOwnerStatusAsync(Guid ownerId)
    {
        var owner = await _context.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
        if (owner == null)
            return false;

        owner.IsActive = !owner.IsActive;
        owner.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Toggled owner status: {OwnerId} - Active: {IsActive}", ownerId, owner.IsActive);
        return true;
    }

    // Subdomain Management
    public async Task<IEnumerable<SubdomainDto>> GetSubdomainsAsync()
    {
        var subdomains = await _context.Subdomains
            .OrderBy(s => s.Subdomain)
            .ToListAsync();

        var result = new List<SubdomainDto>();

        foreach (var subdomain in subdomains)
        {
            var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == subdomain.ClinicId);

            result.Add(new SubdomainDto
            {
                Id = subdomain.Id,
                Subdomain = subdomain.Subdomain,
                ClinicId = subdomain.ClinicId,
                ClinicName = clinic?.Name ?? "N/A",
                TenantId = subdomain.TenantId,
                IsActive = subdomain.IsActive,
                CreatedAt = subdomain.CreatedAt
            });
        }

        return result;
    }

    public async Task<SubdomainDto?> GetSubdomainByNameAsync(string subdomain)
    {
        var subdomainEntity = await _context.Subdomains
            .FirstOrDefaultAsync(s => s.Subdomain.ToLower() == subdomain.ToLower() && s.IsActive);

        if (subdomainEntity == null)
            return null;

        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == subdomainEntity.ClinicId);

        return new SubdomainDto
        {
            Id = subdomainEntity.Id,
            Subdomain = subdomainEntity.Subdomain,
            ClinicId = subdomainEntity.ClinicId,
            ClinicName = clinic?.Name ?? "N/A",
            TenantId = subdomainEntity.TenantId,
            IsActive = subdomainEntity.IsActive,
            CreatedAt = subdomainEntity.CreatedAt
        };
    }

    public async Task<Guid> CreateSubdomainAsync(CreateSubdomainRequest request)
    {
        // Validate subdomain format
        if (!System.Text.RegularExpressions.Regex.IsMatch(request.Subdomain, "^[a-z0-9-]+$"))
        {
            throw new InvalidOperationException("Subdomínio deve conter apenas letras minúsculas, números e hífens");
        }

        // Check if subdomain already exists
        var existing = await _context.Subdomains
            .FirstOrDefaultAsync(s => s.Subdomain.ToLower() == request.Subdomain.ToLower());

        if (existing != null)
        {
            throw new InvalidOperationException("Este subdomínio já está em uso");
        }

        // Get clinic and its tenant ID
        var clinic = await _context.Clinics.FirstOrDefaultAsync(c => c.Id == request.ClinicId);
        if (clinic == null)
        {
            throw new InvalidOperationException("Clínica não encontrada");
        }

        var subdomain = new SubdomainEntity
        {
            Id = Guid.NewGuid(),
            Subdomain = request.Subdomain.ToLower(),
            ClinicId = request.ClinicId,
            TenantId = clinic.TenantId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Subdomains.Add(subdomain);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created subdomain: {Subdomain} for clinic: {ClinicId}", subdomain.Subdomain, request.ClinicId);
        return subdomain.Id;
    }

    public async Task<bool> DeleteSubdomainAsync(Guid subdomainId)
    {
        var subdomain = await _context.Subdomains.FirstOrDefaultAsync(s => s.Id == subdomainId);
        if (subdomain == null)
            return false;

        _context.Subdomains.Remove(subdomain);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted subdomain: {SubdomainId}", subdomainId);
        return true;
    }
}
