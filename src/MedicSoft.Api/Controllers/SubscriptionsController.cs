using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for subscription management - upgrades, downgrades, freeze
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class SubscriptionsController : BaseController
    {
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionsController(
            ITenantContext tenantContext,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            ISubscriptionService subscriptionService) : base(tenantContext)
        {
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// Get current subscription for clinic
        /// </summary>
        [HttpGet("current")]
        public async Task<ActionResult<SubscriptionDto>> GetCurrentSubscription()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            if (clinicId == Guid.Empty)
                return BadRequest(new { message = "Clinic ID not found" });

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            return Ok(new SubscriptionDto
            {
                Id = subscription.Id,
                ClinicId = subscription.ClinicId,
                PlanName = subscription.SubscriptionPlan?.Name ?? "Unknown",
                Status = subscription.Status.ToString(),
                CurrentPrice = subscription.CurrentPrice,
                StartDate = subscription.StartDate,
                NextPaymentDate = subscription.NextPaymentDate,
                TrialEndDate = subscription.TrialEndDate,
                IsFrozen = subscription.IsFrozen,
                FrozenEndDate = subscription.FrozenEndDate,
                HasPendingChange = subscription.PendingPlanId.HasValue,
                PendingPlanName = subscription.PendingPlan?.Name,
                IsUpgrade = subscription.IsUpgrade,
                PlanChangeDate = subscription.PlanChangeDate,
                CanAccess = _subscriptionService.CanAccessSystem(subscription)
            });
        }

        /// <summary>
        /// Upgrade subscription plan - charges difference immediately
        /// </summary>
        [HttpPost("upgrade")]
        public async Task<ActionResult> UpgradePlan([FromBody] ChangePlanRequest request)
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            var newPlan = await _planRepository.GetByIdAsync(request.NewPlanId, tenantId);
            if (newPlan == null || !newPlan.IsActive)
                return BadRequest(new { message = "Invalid plan" });

            if (newPlan.MonthlyPrice <= subscription.CurrentPrice)
                return BadRequest(new { message = "New plan must have higher price for upgrade" });

            subscription.ScheduleUpgrade(newPlan.Id, newPlan.MonthlyPrice);
            await _subscriptionRepository.UpdateAsync(subscription);

            var upgradeAmount = subscription.GetUpgradeAmount();

            return Ok(new
            {
                message = "Upgrade scheduled successfully",
                upgradeAmount = upgradeAmount,
                newPlanName = newPlan.Name,
                newPrice = newPlan.MonthlyPrice,
                note = "Please complete payment to activate the upgrade"
            });
        }

        /// <summary>
        /// Apply upgrade after payment confirmation
        /// </summary>
        [HttpPost("{subscriptionId}/apply-upgrade")]
        public async Task<ActionResult> ApplyUpgrade(Guid subscriptionId)
        {
            var tenantId = GetTenantId();

            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            try
            {
                subscription.ApplyUpgrade();
                await _subscriptionRepository.UpdateAsync(subscription);

                return Ok(new { message = "Upgrade applied successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Downgrade subscription plan - applies in next billing cycle
        /// </summary>
        [HttpPost("downgrade")]
        public async Task<ActionResult> DowngradePlan([FromBody] ChangePlanRequest request)
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            var newPlan = await _planRepository.GetByIdAsync(request.NewPlanId, tenantId);
            if (newPlan == null || !newPlan.IsActive)
                return BadRequest(new { message = "Invalid plan" });

            if (newPlan.MonthlyPrice >= subscription.CurrentPrice)
                return BadRequest(new { message = "New plan must have lower price for downgrade" });

            subscription.ScheduleDowngrade(newPlan.Id, newPlan.MonthlyPrice);
            await _subscriptionRepository.UpdateAsync(subscription);

            return Ok(new
            {
                message = "Downgrade scheduled successfully",
                newPlanName = newPlan.Name,
                newPrice = newPlan.MonthlyPrice,
                effectiveDate = subscription.PlanChangeDate,
                note = "Downgrade will be applied in the next billing cycle"
            });
        }

        /// <summary>
        /// Freeze subscription for 1 month
        /// </summary>
        [HttpPost("freeze")]
        public async Task<ActionResult> FreezeSubscription()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            try
            {
                subscription.Freeze();
                await _subscriptionRepository.UpdateAsync(subscription);

                return Ok(new
                {
                    message = "Subscription frozen successfully",
                    frozenUntil = subscription.FrozenEndDate,
                    note = "Billing and access suspended for 1 month"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Unfreeze subscription
        /// </summary>
        [HttpPost("unfreeze")]
        public async Task<ActionResult> UnfreezeSubscription()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            try
            {
                subscription.Unfreeze();
                await _subscriptionRepository.UpdateAsync(subscription);

                return Ok(new { message = "Subscription unfrozen successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel pending plan change
        /// </summary>
        [HttpPost("cancel-pending-change")]
        public async Task<ActionResult> CancelPendingChange()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return NotFound(new { message = "Subscription not found" });

            subscription.CancelPendingPlanChange();
            await _subscriptionRepository.UpdateAsync(subscription);

            return Ok(new { message = "Pending plan change cancelled" });
        }

        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
        }
    }

    public class ChangePlanRequest
    {
        public Guid NewPlanId { get; set; }
    }

    public class SubscriptionDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal CurrentPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? NextPaymentDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public bool IsFrozen { get; set; }
        public DateTime? FrozenEndDate { get; set; }
        public bool HasPendingChange { get; set; }
        public string? PendingPlanName { get; set; }
        public bool IsUpgrade { get; set; }
        public DateTime? PlanChangeDate { get; set; }
        public bool CanAccess { get; set; }
    }
}
