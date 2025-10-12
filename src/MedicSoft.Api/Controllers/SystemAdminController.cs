using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for System Owner/Admin to manage all clinics and system-wide operations
    /// </summary>
    [ApiController]
    [Route("api/system-admin")]
    public class SystemAdminController : BaseController
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly MedicSoftDbContext _context;
        private readonly IOwnerService _ownerService;

        public SystemAdminController(
            ITenantContext tenantContext,
            IClinicRepository clinicRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            ISubscriptionPlanRepository planRepository,
            IPasswordHasher passwordHasher,
            MedicSoftDbContext context,
            IOwnerService ownerService) : base(tenantContext)
        {
            _clinicRepository = clinicRepository;
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _planRepository = planRepository;
            _passwordHasher = passwordHasher;
            _context = context;
            _ownerService = ownerService;
        }

        /// <summary>
        /// Get all clinics in the system (cross-tenant)
        /// </summary>
        [HttpGet("clinics")]
        public async Task<ActionResult<IEnumerable<ClinicSummaryDto>>> GetAllClinics(
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            // In production, verify user has SystemAdmin role
            // var userRole = User.FindFirst("role")?.Value;
            // if (userRole != "SystemAdmin") return Forbid();

            var query = _context.Clinics
                .IgnoreQueryFilters() // Bypass tenant filter for cross-tenant access
                .AsQueryable();

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

            // Get subscription info for each clinic
            foreach (var clinic in clinics)
            {
                var subscription = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.ClinicId == clinic.Id)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefaultAsync();

                if (subscription != null)
                {
                    var plan = await _context.SubscriptionPlans
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(p => p.Id == subscription.SubscriptionPlanId);

                    clinic.SubscriptionStatus = subscription.Status.ToString();
                    clinic.PlanName = plan?.Name ?? "N/A";
                    clinic.NextBillingDate = subscription.NextPaymentDate;
                }
            }

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                clinics
            });
        }

        /// <summary>
        /// Get detailed information about a specific clinic
        /// </summary>
        [HttpGet("clinics/{id}")]
        public async Task<ActionResult<ClinicDetailDto>> GetClinic(Guid id)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound(new { message = "Clínica não encontrada" });

            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            var users = await _context.Users
                .IgnoreQueryFilters()
                .Where(u => u.ClinicId == id)
                .ToListAsync();

            var plan = subscription != null 
                ? await _context.SubscriptionPlans
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(p => p.Id == subscription.SubscriptionPlanId)
                : null;

            return Ok(new ClinicDetailDto
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
                SubscriptionStatus = subscription?.Status.ToString() ?? "No Subscription",
                PlanName = plan?.Name ?? "N/A",
                PlanPrice = plan?.MonthlyPrice ?? 0,
                NextBillingDate = subscription?.NextPaymentDate,
                TrialEndsAt = subscription?.TrialEndDate,
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive)
            });
        }

        /// <summary>
        /// Update clinic subscription
        /// </summary>
        [HttpPut("clinics/{id}/subscription")]
        public async Task<ActionResult> UpdateClinicSubscription(
            Guid id,
            [FromBody] UpdateSubscriptionRequest request)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound(new { message = "Clínica não encontrada" });

            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound(new { message = "Assinatura não encontrada" });

            var newPlan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == request.NewPlanId);

            if (newPlan == null)
                return BadRequest(new { message = "Plano não encontrado" });

            // Update subscription
            if (request.NewPlanId != Guid.Empty)
            {
                subscription.ScheduleUpgrade(newPlan.Id, newPlan.MonthlyPrice);
                subscription.ApplyUpgrade();
            }
            
            if (request.Status.HasValue)
            {
                // Update status based on request
                // This is a simplified version - in production, handle all state transitions properly
                _context.ClinicSubscriptions.Update(subscription);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Assinatura atualizada com sucesso" });
        }

        /// <summary>
        /// Activate or deactivate a clinic
        /// </summary>
        [HttpPost("clinics/{id}/toggle-status")]
        public async Task<ActionResult> ToggleClinicStatus(Guid id)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound(new { message = "Clínica não encontrada" });

            if (clinic.IsActive)
                clinic.Deactivate();
            else
                clinic.Activate();

            await _context.SaveChangesAsync();

            return Ok(new { 
                message = clinic.IsActive ? "Clínica ativada com sucesso" : "Clínica desativada com sucesso",
                isActive = clinic.IsActive
            });
        }

        /// <summary>
        /// Get system-wide analytics
        /// </summary>
        [HttpGet("analytics")]
        public async Task<ActionResult<SystemAnalyticsDto>> GetSystemAnalytics()
        {
            var totalClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .CountAsync();

            var activeClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .CountAsync(c => c.IsActive);

            var totalUsers = await _context.Users
                .IgnoreQueryFilters()
                .CountAsync();

            var activeUsers = await _context.Users
                .IgnoreQueryFilters()
                .CountAsync(u => u.IsActive);

            var totalPatients = await _context.Patients
                .IgnoreQueryFilters()
                .CountAsync();

            var subscriptionsByStatus = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .GroupBy(s => s.Status)
                .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                .ToListAsync();

            var subscriptionsByPlan = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Join(_context.SubscriptionPlans.IgnoreQueryFilters(),
                    s => s.SubscriptionPlanId,
                    p => p.Id,
                    (s, p) => new { Plan = p.Name })
                .GroupBy(x => x.Plan)
                .Select(g => new { Plan = g.Key, Count = g.Count() })
                .ToListAsync();

            var monthlyRevenue = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active)
                .Join(_context.SubscriptionPlans.IgnoreQueryFilters(),
                    s => s.SubscriptionPlanId,
                    p => p.Id,
                    (s, p) => p.MonthlyPrice)
                .SumAsync();

            return Ok(new SystemAnalyticsDto
            {
                TotalClinics = totalClinics,
                ActiveClinics = activeClinics,
                InactiveClinics = totalClinics - activeClinics,
                TotalUsers = totalUsers,
                ActiveUsers = activeUsers,
                TotalPatients = totalPatients,
                SubscriptionsByStatus = subscriptionsByStatus,
                SubscriptionsByPlan = subscriptionsByPlan,
                MonthlyRecurringRevenue = monthlyRevenue
            });
        }

        /// <summary>
        /// Create a new System Owner (like Igor) - not tied to any clinic
        /// </summary>
        [HttpPost("system-owners")]
        public async Task<ActionResult> CreateSystemOwner([FromBody] CreateSystemOwnerRequest request)
        {
            // Verify caller is SystemAdmin/SystemOwner
            // In production, check JWT claims

            try
            {
                // Create system owner with no clinic association
                var owner = await _ownerService.CreateOwnerAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.FullName,
                    request.Phone,
                    "system", // System owners have special tenant
                    null, // No clinic for system owner
                    request.ProfessionalId,
                    request.Specialty
                );

                return Ok(new { 
                    message = "System owner criado com sucesso",
                    ownerId = owner.Id,
                    username = owner.Username,
                    isSystemOwner = true
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all system owners
        /// </summary>
        [HttpGet("system-owners")]
        public async Task<ActionResult> GetSystemOwners()
        {
            var systemOwners = await _ownerService.GetSystemOwnersAsync("system");

            return Ok(systemOwners.Select(o => new {
                o.Id,
                o.Username,
                o.Email,
                o.FullName,
                o.Phone,
                o.IsActive,
                o.LastLoginAt,
                IsSystemOwner = true
            }));
        }

        /// <summary>
        /// Create a new SystemAdmin user (deprecated - use system-owners instead)
        /// </summary>
        [HttpPost("users")]
        [Obsolete("Use POST /api/system-admin/system-owners instead")]
        public async Task<ActionResult> CreateSystemAdmin([FromBody] CreateSystemAdminRequest request)
        {
            // Verify caller is SystemAdmin
            // In production, check JWT claims

            var tenantId = "system"; // System admins have special tenant

            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username, tenantId);
            if (existingUser != null)
                return BadRequest(new { message = "Username já existe" });

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create system admin user with no clinic association
            var user = new User(
                request.Username,
                request.Email,
                passwordHash,
                request.FullName,
                request.Phone,
                UserRole.SystemAdmin,
                tenantId,
                null, // No clinic for system admin
                null,
                null
            );

            await _userRepository.AddAsync(user);

            return Ok(new { 
                message = "Administrador do sistema criado com sucesso (deprecated endpoint - use system-owners)",
                userId = user.Id,
                username = user.Username
            });
        }

        /// <summary>
        /// Get all subscription plans
        /// </summary>
        [HttpGet("plans")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlan>>> GetAllPlans()
        {
            var plans = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();

            return Ok(plans);
        }

        /// <summary>
        /// Enable manual override for a clinic subscription
        /// Allows keeping clinic active even if payment is overdue or not registered via website
        /// Used for giving free access to friends or special cases
        /// </summary>
        [HttpPost("clinics/{id}/subscription/manual-override/enable")]
        public async Task<ActionResult> EnableManualOverride(
            Guid id,
            [FromBody] ManualOverrideRequest request)
        {
            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound(new { message = "Assinatura não encontrada" });

            var username = User.Identity?.Name ?? "System";
            subscription.EnableManualOverride(request.Reason, username);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Override manual ativado com sucesso",
                reason = subscription.ManualOverrideReason,
                setBy = subscription.ManualOverrideSetBy,
                setAt = subscription.ManualOverrideSetAt
            });
        }

        /// <summary>
        /// Disable manual override for a clinic subscription
        /// Returns to normal subscription payment rules
        /// </summary>
        [HttpPost("clinics/{id}/subscription/manual-override/disable")]
        public async Task<ActionResult> DisableManualOverride(Guid id)
        {
            var subscription = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.ClinicId == id)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefaultAsync();

            if (subscription == null)
                return NotFound(new { message = "Assinatura não encontrada" });

            subscription.DisableManualOverride();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Override manual desativado com sucesso" });
        }
    }

    public class ClinicSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string SubscriptionStatus { get; set; } = string.Empty;
        public string PlanName { get; set; } = string.Empty;
        public DateTime? NextBillingDate { get; set; }
    }

    public class ClinicDetailDto : ClinicSummaryDto
    {
        public decimal PlanPrice { get; set; }
        public DateTime? TrialEndsAt { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
    }

    public class UpdateSubscriptionRequest
    {
        public Guid NewPlanId { get; set; }
        public SubscriptionStatus? Status { get; set; }
    }

    public class SystemAnalyticsDto
    {
        public int TotalClinics { get; set; }
        public int ActiveClinics { get; set; }
        public int InactiveClinics { get; set; }
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalPatients { get; set; }
        public decimal MonthlyRecurringRevenue { get; set; }
        public object SubscriptionsByStatus { get; set; } = null!;
        public object SubscriptionsByPlan { get; set; } = null!;
    }

    public class CreateSystemAdminRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class CreateSystemOwnerRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
    }

    public class ManualOverrideRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}