using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
    [Authorize(Roles = "SystemAdmin")]
    public class SystemAdminController : BaseController
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly MedicSoftDbContext _context;
        private readonly IOwnerService _ownerService;
        private readonly BusinessConfigurationService _businessConfigService;
        private readonly ILogger<SystemAdminController> _logger;

        public SystemAdminController(
            ITenantContext tenantContext,
            IClinicRepository clinicRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            ISubscriptionPlanRepository planRepository,
            IPasswordHasher passwordHasher,
            MedicSoftDbContext context,
            IOwnerService ownerService,
            BusinessConfigurationService businessConfigService,
            ILogger<SystemAdminController> logger) : base(tenantContext)
        {
            _clinicRepository = clinicRepository;
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _planRepository = planRepository;
            _passwordHasher = passwordHasher;
            _context = context;
            _ownerService = ownerService;
            _businessConfigService = businessConfigService;
            _logger = logger;
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
        /// Create a new clinic with owner
        /// </summary>
        [HttpPost("clinics")]
        public async Task<ActionResult> CreateClinic([FromBody] CreateClinicRequest request)
        {
            try
            {
                // Generate unique tenant ID for the clinic
                var tenantId = Guid.NewGuid().ToString();

                // Check if a plan was specified and validate it
                SubscriptionPlan? plan = null;
                if (!string.IsNullOrEmpty(request.PlanId))
                {
                    if (!Guid.TryParse(request.PlanId, out var planId))
                        return BadRequest(new { message = "Formato de ID do plano inválido" });

                    plan = await _context.SubscriptionPlans
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(p => p.Id == planId);
                    
                    if (plan == null)
                        return BadRequest(new { message = "Plano não encontrado" });
                }

                // Create clinic
                var clinic = new Clinic(
                    request.Name,
                    request.Name, // TradeName same as Name
                    request.Document,
                    request.Phone,
                    request.Email,
                    request.Address,
                    new TimeSpan(8, 0, 0), // Default 8 AM
                    new TimeSpan(18, 0, 0), // Default 6 PM
                    tenantId,
                    30 // Default 30 minute appointments
                );

                await _clinicRepository.AddAsync(clinic);

                // Create default business configuration for the clinic
                try
                {
                    // Use values from request, or default to SmallClinic and Medico
                    var businessType = request.BusinessType.HasValue 
                        ? (Domain.Enums.BusinessType)request.BusinessType.Value 
                        : Domain.Enums.BusinessType.SmallClinic;
                    
                    var primarySpecialty = request.PrimarySpecialty.HasValue 
                        ? (Domain.Enums.ProfessionalSpecialty)request.PrimarySpecialty.Value 
                        : Domain.Enums.ProfessionalSpecialty.Medico;
                    
                    await _businessConfigService.CreateAsync(
                        clinic.Id,
                        businessType,
                        primarySpecialty,
                        tenantId
                    );
                    _logger.LogInformation(
                        "Business configuration created successfully for clinic {ClinicId} with tenant {TenantId}, BusinessType: {BusinessType}, PrimarySpecialty: {PrimarySpecialty}",
                        clinic.Id, tenantId, businessType, primarySpecialty);
                }
                catch (Exception ex)
                {
                    // Log but don't fail clinic creation if business config fails
                    _logger.LogWarning(ex,
                        "Failed to create business configuration for clinic {ClinicId} with tenant {TenantId}",
                        clinic.Id, tenantId);
                }

                // Create owner for the clinic
                var owner = await _ownerService.CreateOwnerAsync(
                    request.OwnerUsername,
                    request.Email, // Use clinic email if owner email not provided
                    request.OwnerPassword,
                    request.OwnerFullName,
                    request.Phone, // Use clinic phone if owner phone not provided
                    tenantId,
                    clinic.Id,
                    null, // ProfessionalId
                    null  // Specialty
                );

                // Create subscription if plan was validated
                if (plan != null)
                {
                    var subscription = new ClinicSubscription(
                        clinic.Id,
                        plan.Id,
                        DateTime.UtcNow, // startDate
                        0, // trialDays - no trial for admin-created clinics
                        plan.MonthlyPrice,
                        tenantId
                    );
                    
                    await _subscriptionRepository.AddAsync(subscription);
                }

                return Ok(new
                {
                    message = "Clínica criada com sucesso",
                    clinicId = clinic.Id,
                    tenantId = tenantId
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Erro ao criar clínica: {ex.Message}" });
            }
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
        /// Update clinic information
        /// </summary>
        [HttpPut("clinics/{id}")]
        public async Task<ActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicRequest request)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (clinic == null)
                return NotFound(new { message = "Clínica não encontrada" });

            clinic.UpdateInfo(
                request.Name,
                request.Name, // TradeName same as Name
                request.Phone,
                request.Email,
                request.Address
            );

            await _context.SaveChangesAsync();

            return Ok(new { message = "Clínica atualizada com sucesso" });
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
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            var subscriptionsByPlan = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Join(_context.SubscriptionPlans.IgnoreQueryFilters(),
                    s => s.SubscriptionPlanId,
                    p => p.Id,
                    (s, p) => new { Plan = p.Name })
                .GroupBy(x => x.Plan)
                .Select(g => new { Plan = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Plan, x => x.Count);

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
        /// Toggle system owner active status
        /// </summary>
        [HttpPost("system-owners/{id}/toggle-status")]
        public async Task<ActionResult> ToggleSystemOwnerStatus(Guid id)
        {
            var owner = await _context.Owners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId == null);

            if (owner == null)
                return NotFound(new { message = "System owner não encontrado" });

            if (owner.IsActive)
                owner.Deactivate();
            else
                owner.Activate();

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = owner.IsActive ? "System owner ativado com sucesso" : "System owner desativado com sucesso",
                isActive = owner.IsActive
            });
        }

        /// <summary>
        /// Get all clinic owners
        /// </summary>
        [HttpGet("clinic-owners")]
        public async Task<ActionResult> GetClinicOwners([FromQuery] Guid? clinicId = null)
        {
            var query = _context.Owners
                .IgnoreQueryFilters()
                .Where(o => o.ClinicId != null); // Only owners associated with clinics

            if (clinicId.HasValue)
            {
                query = query.Where(o => o.ClinicId == clinicId.Value);
            }

            var owners = await query
                .Select(o => new
                {
                    o.Id,
                    o.Username,
                    o.Email,
                    o.FullName,
                    o.Phone,
                    o.IsActive,
                    o.ClinicId,
                    ClinicName = o.Clinic != null ? o.Clinic.Name : null,
                    o.LastLoginAt,
                    o.CreatedAt
                })
                .ToListAsync();

            return Ok(owners);
        }

        /// <summary>
        /// Reset clinic owner password
        /// </summary>
        [HttpPost("clinic-owners/{id}/reset-password")]
        public async Task<ActionResult> ResetOwnerPassword(Guid id, [FromBody] ResetPasswordRequest request)
        {
            var owner = await _context.Owners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId != null);

            if (owner == null)
                return NotFound(new { message = "Proprietário não encontrado" });

            // Validate password strength
            var (isValid, message) = _passwordHasher.ValidatePasswordStrength(request.NewPassword);
            if (!isValid)
                return BadRequest(new { message = message ?? "Senha inválida" });

            owner.UpdatePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _context.SaveChangesAsync();

            return Ok(new { message = "Senha redefinida com sucesso" });
        }

        /// <summary>
        /// Toggle clinic owner status
        /// </summary>
        [HttpPost("clinic-owners/{id}/toggle-status")]
        public async Task<ActionResult> ToggleClinicOwnerStatus(Guid id)
        {
            var owner = await _context.Owners
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(o => o.Id == id && o.ClinicId != null);

            if (owner == null)
                return NotFound(new { message = "Proprietário não encontrado" });

            if (owner.IsActive)
                owner.Deactivate();
            else
                owner.Activate();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Status do proprietário alterado com sucesso" });
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
        /// Get all subscription plans (alias for frontend consistency)
        /// </summary>
        [HttpGet("subscription-plans")]
        public async Task<ActionResult<IEnumerable<SubscriptionPlan>>> GetSubscriptionPlans([FromQuery] bool? activeOnly = null)
        {
            var query = _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .AsQueryable();

            if (activeOnly.HasValue && activeOnly.Value)
            {
                query = query.Where(p => p.IsActive);
            }

            var plans = await query
                .OrderBy(p => p.MonthlyPrice)
                .ToListAsync();

            return Ok(plans);
        }

        /// <summary>
        /// Get a specific subscription plan
        /// </summary>
        [HttpGet("subscription-plans/{id}")]
        public async Task<ActionResult<SubscriptionPlan>> GetSubscriptionPlan(Guid id)
        {
            var plan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound(new { message = "Plano não encontrado" });

            return Ok(plan);
        }

        /// <summary>
        /// Create a new subscription plan
        /// </summary>
        [HttpPost("subscription-plans")]
        public async Task<ActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequest request)
        {
            var plan = new SubscriptionPlan(
                request.Name,
                request.Description ?? string.Empty,
                request.MonthlyPrice,
                request.TrialDays,
                request.MaxUsers,
                request.MaxPatients,
                (SubscriptionPlanType)request.Type,
                "system", // System-wide plan
                request.HasReports,
                request.HasWhatsAppIntegration,
                request.HasSMSNotifications,
                request.HasTissExport,
                request.MaxClinics
            );

            // Set campaign pricing if provided
            if (!string.IsNullOrEmpty(request.CampaignName) && 
                request.OriginalPrice.HasValue && 
                request.CampaignPrice.HasValue)
            {
                plan.SetCampaignPricing(
                    request.CampaignName,
                    request.CampaignDescription ?? string.Empty,
                    request.OriginalPrice.Value,
                    request.CampaignPrice.Value,
                    request.CampaignStartDate,
                    request.CampaignEndDate,
                    request.MaxEarlyAdopters
                );
            }

            // Set early adopter benefits if provided
            if (request.EarlyAdopterBenefits != null && request.EarlyAdopterBenefits.Any())
            {
                plan.SetEarlyAdopterBenefits(request.EarlyAdopterBenefits.ToArray());
            }

            // Set features available if provided
            if (request.FeaturesAvailable != null && request.FeaturesAvailable.Any())
            {
                plan.SetFeaturesAvailable(request.FeaturesAvailable.ToArray());
            }

            // Set features in development if provided
            if (request.FeaturesInDevelopment != null && request.FeaturesInDevelopment.Any())
            {
                plan.SetFeaturesInDevelopment(request.FeaturesInDevelopment.ToArray());
            }

            _context.SubscriptionPlans.Add(plan);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Plano criado com sucesso",
                planId = plan.Id
            });
        }

        /// <summary>
        /// Update a subscription plan
        /// </summary>
        [HttpPut("subscription-plans/{id}")]
        public async Task<ActionResult> UpdateSubscriptionPlan(Guid id, [FromBody] UpdateSubscriptionPlanRequest request)
        {
            var plan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound(new { message = "Plano não encontrado" });

            plan.Update(
                request.Name,
                request.Description ?? string.Empty,
                request.MonthlyPrice,
                request.MaxUsers,
                request.MaxPatients,
                request.HasReports,
                request.HasWhatsAppIntegration,
                request.HasSMSNotifications,
                request.HasTissExport,
                request.MaxClinics
            );

            // Update campaign pricing if provided
            if (!string.IsNullOrEmpty(request.CampaignName) && 
                request.OriginalPrice.HasValue && 
                request.CampaignPrice.HasValue)
            {
                plan.SetCampaignPricing(
                    request.CampaignName,
                    request.CampaignDescription ?? string.Empty,
                    request.OriginalPrice.Value,
                    request.CampaignPrice.Value,
                    request.CampaignStartDate,
                    request.CampaignEndDate,
                    request.MaxEarlyAdopters
                );
            }
            else if (string.IsNullOrEmpty(request.CampaignName))
            {
                // Clear campaign if no campaign name provided
                plan.ClearCampaignPricing();
            }

            // Update early adopter benefits
            if (request.EarlyAdopterBenefits != null && request.EarlyAdopterBenefits.Any())
            {
                plan.SetEarlyAdopterBenefits(request.EarlyAdopterBenefits.ToArray());
            }

            // Update features available
            if (request.FeaturesAvailable != null && request.FeaturesAvailable.Any())
            {
                plan.SetFeaturesAvailable(request.FeaturesAvailable.ToArray());
            }

            // Update features in development
            if (request.FeaturesInDevelopment != null && request.FeaturesInDevelopment.Any())
            {
                plan.SetFeaturesInDevelopment(request.FeaturesInDevelopment.ToArray());
            }

            if (request.IsActive != plan.IsActive)
            {
                if (request.IsActive)
                    plan.Activate();
                else
                    plan.Deactivate();
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Plano atualizado com sucesso" });
        }

        /// <summary>
        /// Update enabled modules for a subscription plan
        /// </summary>
        [HttpPut("subscription-plans/{id}/modules")]
        public async Task<ActionResult> UpdatePlanModules(Guid id, [FromBody] UpdatePlanModulesRequest request)
        {
            var plan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound(new { message = "Plano não encontrado" });

            try
            {
                plan.SetEnabledModules(request.EnabledModules);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Módulos do plano atualizados com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a subscription plan
        /// </summary>
        [HttpDelete("subscription-plans/{id}")]
        public async Task<ActionResult> DeleteSubscriptionPlan(Guid id)
        {
            var plan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound(new { message = "Plano não encontrado" });

            // Check if any active subscriptions use this plan
            var hasActiveSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .AnyAsync(s => s.SubscriptionPlanId == id && s.Status == SubscriptionStatus.Active);

            if (hasActiveSubscriptions)
                return BadRequest(new { message = "Não é possível excluir um plano com assinaturas ativas" });

            _context.SubscriptionPlans.Remove(plan);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Plano excluído com sucesso" });
        }

        /// <summary>
        /// Toggle subscription plan status
        /// </summary>
        [HttpPost("subscription-plans/{id}/toggle-status")]
        public async Task<ActionResult> ToggleSubscriptionPlanStatus(Guid id)
        {
            var plan = await _context.SubscriptionPlans
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (plan == null)
                return NotFound(new { message = "Plano não encontrado" });

            if (plan.IsActive)
                plan.Deactivate();
            else
                plan.Activate();

            await _context.SaveChangesAsync();

            return Ok(new { message = "Status do plano alterado com sucesso" });
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

        /// <summary>
        /// Enable manual override for a clinic subscription (alternative endpoint for frontend)
        /// </summary>
        [HttpPost("clinics/{id}/subscription/manual-override")]
        public async Task<ActionResult> EnableManualOverrideAlt(
            Guid id,
            [FromBody] EnableManualOverrideRequest request)
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

            return Ok(new { message = "Override manual ativado com sucesso" });
        }

        /// <summary>
        /// Disable manual override for a clinic subscription (alternative endpoint for frontend)
        /// </summary>
        [HttpDelete("clinics/{id}/subscription/manual-override")]
        public async Task<ActionResult> DisableManualOverrideAlt(Guid id)
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

        /// <summary>
        /// Get all subdomains (returns clinic tenantIds as subdomains for compatibility)
        /// </summary>
        [HttpGet("subdomains")]
        public async Task<ActionResult> GetSubdomains()
        {
            var clinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Select(c => new
                {
                    Id = c.Id,
                    Subdomain = c.TenantId,
                    ClinicId = c.Id,
                    ClinicName = c.Name,
                    TenantId = c.TenantId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return Ok(clinics);
        }

        /// <summary>
        /// Get subdomain by name (public endpoint for tenant resolution)
        /// </summary>
        [HttpGet("subdomains/resolve/{subdomain}")]
        public async Task<ActionResult> ResolveSubdomain(string subdomain)
        {
            var clinic = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.TenantId == subdomain)
                .Select(c => new
                {
                    Id = c.Id,
                    Subdomain = c.TenantId,
                    ClinicId = c.Id,
                    ClinicName = c.Name,
                    TenantId = c.TenantId,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();

            if (clinic == null)
                return NotFound(new { message = "Subdomínio não encontrado" });

            return Ok(clinic);
        }

        /// <summary>
        /// Create a new subdomain (not implemented - tenantIds are auto-generated with clinics)
        /// </summary>
        [HttpPost("subdomains")]
        public Task<ActionResult> CreateSubdomain([FromBody] CreateSubdomainRequest request)
        {
            // In the monolithic API, subdomains are essentially tenant IDs which are auto-generated
            // This endpoint returns a message indicating the feature is not supported
            return Task.FromResult<ActionResult>(BadRequest(new { message = "Criação manual de subdomínios não é suportada. Os subdomínios são gerados automaticamente ao criar uma clínica." }));
        }

        /// <summary>
        /// Delete a subdomain (not implemented - would require deleting entire clinic)
        /// </summary>
        [HttpDelete("subdomains/{id}")]
        public Task<ActionResult> DeleteSubdomain(Guid id)
        {
            // In the monolithic API, deleting a subdomain would mean deleting a clinic
            // This is a dangerous operation and should be done through clinic deletion
            return Task.FromResult<ActionResult>(BadRequest(new { message = "Exclusão de subdomínios não é suportada. Para remover um subdomínio, desative a clínica correspondente." }));
        }

        /// <summary>
        /// Check if current user is a system owner
        /// </summary>
        private bool IsSystemOwner()
        {
            var isSystemOwnerClaim = User.FindFirst("is_system_owner");
            if (isSystemOwnerClaim != null && bool.TryParse(isSystemOwnerClaim.Value, out var isSystemOwner))
            {
                return isSystemOwner;
            }
            return false;
        }

        #region MFA Compliance

        /// <summary>
        /// Get MFA compliance statistics across all users
        /// </summary>
        [HttpGet("mfa-compliance")]
        public async Task<ActionResult<MfaComplianceResponse>> GetMfaCompliance()
        {
            var allUsers = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.ClinicLinks)
                .ToListAsync();

            var adminUsers = allUsers.Where(u => u.MfaRequiredByPolicy).ToList();
            var userIds = adminUsers.Select(u => u.Id.ToString()).ToList();

            // Get MFA status for all admin users
            var twoFactorAuths = await _context.Set<TwoFactorAuth>()
                .IgnoreQueryFilters()
                .Where(t => userIds.Contains(t.UserId) && t.IsEnabled)
                .ToListAsync();

            var enabledUserIds = twoFactorAuths.Select(t => t.UserId).ToHashSet();

            var totalAdmins = adminUsers.Count;
            var adminsWithMfa = adminUsers.Count(u => enabledUserIds.Contains(u.Id.ToString()));
            var adminsWithoutMfa = totalAdmins - adminsWithMfa;
            var adminsInGracePeriod = adminUsers.Count(u => !enabledUserIds.Contains(u.Id.ToString()) && u.IsInMfaGracePeriod);
            var adminsGraceExpired = adminUsers.Count(u => !enabledUserIds.Contains(u.Id.ToString()) && u.MfaGracePeriodExpired);

            return Ok(new MfaComplianceResponse
            {
                TotalAdministrators = totalAdmins,
                WithMfaEnabled = adminsWithMfa,
                WithoutMfaEnabled = adminsWithoutMfa,
                InGracePeriod = adminsInGracePeriod,
                GracePeriodExpired = adminsGraceExpired,
                CompliancePercentage = totalAdmins > 0 ? (decimal)adminsWithMfa / totalAdmins * 100 : 0
            });
        }

        /// <summary>
        /// Get list of administrators without MFA enabled
        /// </summary>
        [HttpGet("users-without-mfa")]
        public async Task<ActionResult<List<UserMfaStatusDto>>> GetUsersWithoutMfa(
            [FromQuery] bool? graceExpiredOnly = null)
        {
            var allUsers = await _context.Users
                .IgnoreQueryFilters()
                .Include(u => u.ClinicLinks)
                .ThenInclude(cl => cl.Clinic)
                .ToListAsync();

            var adminUsers = allUsers.Where(u => u.MfaRequiredByPolicy).ToList();
            var userIds = adminUsers.Select(u => u.Id.ToString()).ToList();

            // Get MFA status for all admin users
            var twoFactorAuths = await _context.Set<TwoFactorAuth>()
                .IgnoreQueryFilters()
                .Where(t => userIds.Contains(t.UserId) && t.IsEnabled)
                .ToListAsync();

            var enabledUserIds = twoFactorAuths.Select(t => t.UserId).ToHashSet();

            var usersWithoutMfa = adminUsers
                .Where(u => !enabledUserIds.Contains(u.Id.ToString()))
                .ToList();

            if (graceExpiredOnly == true)
            {
                usersWithoutMfa = usersWithoutMfa.Where(u => u.MfaGracePeriodExpired).ToList();
            }

            var result = usersWithoutMfa.Select(u => new UserMfaStatusDto
            {
                UserId = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Role = u.Role.ToString(),
                TenantId = u.TenantId,
                MfaEnabled = false,
                IsInGracePeriod = u.IsInMfaGracePeriod,
                GracePeriodEndsAt = u.MfaGracePeriodEndsAt,
                GracePeriodExpired = u.MfaGracePeriodExpired,
                FirstLoginAt = u.FirstLoginAt,
                LastLoginAt = u.LastLoginAt,
                ClinicName = u.ClinicLinks.FirstOrDefault()?.Clinic?.Name ?? "N/A"
            }).ToList();

            return Ok(result);
        }

        #endregion
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
        public Dictionary<string, int> SubscriptionsByStatus { get; set; } = new();
        public Dictionary<string, int> SubscriptionsByPlan { get; set; } = new();
    }

    public class CreateSystemAdminRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class CreateClinicRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string OwnerUsername { get; set; } = string.Empty;
        public string OwnerPassword { get; set; } = string.Empty;
        public string OwnerFullName { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public int? BusinessType { get; set; }
        public int? PrimarySpecialty { get; set; }
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

    public class EnableManualOverrideRequest
    {
        public string Reason { get; set; } = string.Empty;
        public DateTime? ExtendUntil { get; set; }
    }

    public class CreateSubscriptionPlanRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int MaxUsers { get; set; }
        public int MaxPatients { get; set; }
        public int MaxClinics { get; set; } = 1;
        public int TrialDays { get; set; } = 14;
        public bool HasReports { get; set; }
        public bool HasWhatsAppIntegration { get; set; }
        public bool HasSMSNotifications { get; set; }
        public bool HasTissExport { get; set; }
        public int Type { get; set; } = (int)SubscriptionPlanType.Standard; // Default to Standard
        
        // Campaign fields
        public string? CampaignName { get; set; }
        public string? CampaignDescription { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? CampaignPrice { get; set; }
        public DateTime? CampaignStartDate { get; set; }
        public DateTime? CampaignEndDate { get; set; }
        public int? MaxEarlyAdopters { get; set; }
        public List<string>? EarlyAdopterBenefits { get; set; }
        public List<string>? FeaturesAvailable { get; set; }
        public List<string>? FeaturesInDevelopment { get; set; }
    }

    public class UpdateSubscriptionPlanRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal MonthlyPrice { get; set; }
        public int MaxUsers { get; set; }
        public int MaxPatients { get; set; }
        public int MaxClinics { get; set; } = 1;
        public int TrialDays { get; set; }
        public bool IsActive { get; set; }
        public bool HasReports { get; set; }
        public bool HasWhatsAppIntegration { get; set; }
        public bool HasSMSNotifications { get; set; }
        public bool HasTissExport { get; set; }
        
        // Campaign fields
        public string? CampaignName { get; set; }
        public string? CampaignDescription { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? CampaignPrice { get; set; }
        public DateTime? CampaignStartDate { get; set; }
        public DateTime? CampaignEndDate { get; set; }
        public int? MaxEarlyAdopters { get; set; }
        public List<string>? EarlyAdopterBenefits { get; set; }
        public List<string>? FeaturesAvailable { get; set; }
        public List<string>? FeaturesInDevelopment { get; set; }
    }

    public class UpdatePlanModulesRequest
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "EnabledModules cannot be null")]
        public string[] EnabledModules { get; set; } = Array.Empty<string>();
    }

    public class CreateSubdomainRequest
    {
        public string Subdomain { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
    }

    public class UpdateClinicRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
    
    #region MFA Compliance DTOs

    public class MfaComplianceResponse
    {
        public int TotalAdministrators { get; set; }
        public int WithMfaEnabled { get; set; }
        public int WithoutMfaEnabled { get; set; }
        public int InGracePeriod { get; set; }
        public int GracePeriodExpired { get; set; }
        public decimal CompliancePercentage { get; set; }
    }

    public class UserMfaStatusDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool MfaEnabled { get; set; }
        public bool IsInGracePeriod { get; set; }
        public DateTime? GracePeriodEndsAt { get; set; }
        public bool GracePeriodExpired { get; set; }
        public DateTime? FirstLoginAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string ClinicName { get; set; } = string.Empty;
    }

    #endregion
}
