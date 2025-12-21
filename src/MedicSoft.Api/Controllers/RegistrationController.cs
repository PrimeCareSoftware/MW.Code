using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for clinic registration and subscription management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly ISubscriptionPlanRepository _planRepository;

        public RegistrationController(
            IRegistrationService registrationService,
            ISubscriptionPlanRepository planRepository)
        {
            _registrationService = registrationService;
            _planRepository = planRepository;
        }

        /// <summary>
        /// Register a new clinic with owner user and subscription
        /// </summary>
        /// <param name="request">Registration data</param>
        /// <returns>Registration result with clinic and user IDs</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegistrationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegistrationResponseDto>> Register([FromBody] RegistrationRequestDto request)
        {
            try
            {
                var result = await _registrationService.RegisterClinicWithOwnerAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = result.Message
                    });
                }

                return Ok(new RegistrationResponseDto
                {
                    Success = true,
                    Message = "Registration successful! Welcome to MedicWarehouse. You can now login with your credentials.",
                    ClinicId = result.ClinicId,
                    UserId = result.OwnerId,
                    TenantId = result.TenantId,
                    Subdomain = result.Subdomain,
                    ClinicName = result.ClinicName,
                    OwnerName = result.OwnerName,
                    OwnerEmail = result.OwnerEmail,
                    Username = result.Username
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegistrationResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Check if CNPJ is already registered
        /// </summary>
        /// <param name="cnpj">CNPJ to check</param>
        /// <returns>True if CNPJ exists</returns>
        [HttpGet("check-cnpj/{cnpj}")]
        [ProducesResponseType(typeof(CheckCNPJResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckCNPJResponseDto>> CheckCNPJ(string cnpj)
        {
            var exists = await _registrationService.CheckCNPJExistsAsync(cnpj);
            return Ok(new CheckCNPJResponseDto { Exists = exists });
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if username is available</returns>
        [HttpGet("check-username/{username}")]
        [ProducesResponseType(typeof(CheckUsernameResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckUsernameResponseDto>> CheckUsername(string username)
        {
            // Use default tenant for registration check
            var available = await _registrationService.CheckUsernameAvailableAsync(username, "default-tenant");
            return Ok(new CheckUsernameResponseDto { Available = available });
        }

        /// <summary>
        /// Get all active subscription plans
        /// </summary>
        /// <returns>List of available subscription plans</returns>
        [HttpGet("plans")]
        [ProducesResponseType(typeof(List<SubscriptionPlanDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<SubscriptionPlanDto>>> GetPlans()
        {
            // Get all active plans from system tenant
            var plans = await _planRepository.GetActiveInPlansAsync();
            
            var planDtos = plans.Select(p => new SubscriptionPlanDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                MonthlyPrice = p.MonthlyPrice,
                TrialDays = p.TrialDays,
                MaxUsers = p.MaxUsers,
                MaxPatients = p.MaxPatients,
                HasReports = p.HasReports,
                HasWhatsAppIntegration = p.HasWhatsAppIntegration,
                HasSMSNotifications = p.HasSMSNotifications,
                HasTissExport = p.HasTissExport,
                IsActive = p.IsActive,
                Type = (int)p.Type,
                Features = GeneratePlanFeatures(p),
                IsRecommended = p.Type == PlanType.Standard // Mark Standard plan as recommended
            }).ToList();

            return Ok(planDtos);
        }

        // Constants for plan limits thresholds
        private const int UnlimitedUsersThreshold = 100;
        private const int UnlimitedPatientsThreshold = 10000;
        private const int LargeUsersLimit = 999;
        private const int LargePatientsLimit = 999999;

        private List<string> GeneratePlanFeatures(SubscriptionPlan plan)
        {
            var features = new List<string>();

            // Add user and patient limits
            if (plan.MaxUsers >= LargeUsersLimit || plan.MaxUsers >= UnlimitedUsersThreshold)
                features.Add("Usuários ilimitados");
            else
                features.Add($"Até {plan.MaxUsers} usuários");

            if (plan.MaxPatients >= LargePatientsLimit || plan.MaxPatients >= UnlimitedPatientsThreshold)
                features.Add("Pacientes ilimitados");
            else if (plan.MaxPatients >= 1000)
                features.Add($"Até {plan.MaxPatients / 1000}k pacientes");
            else
                features.Add($"Até {plan.MaxPatients} pacientes");

            // Add basic features
            features.Add("Agenda de consultas");
            features.Add("Cadastro de pacientes");
            features.Add("Prontuário médico digital");

            // Add conditional features based on plan type
            if (plan.Type == PlanType.Trial)
            {
                features.Add("Suporte por email");
            }
            else if (plan.Type == PlanType.Basic)
            {
                if (plan.HasReports)
                    features.Add("Relatórios básicos");
                if (plan.HasSMSNotifications)
                    features.Add("Lembretes de consulta");
                features.Add("Suporte por email");
            }
            else if (plan.Type == PlanType.Standard)
            {
                if (plan.HasReports)
                    features.Add("Relatórios gerenciais");
                if (plan.HasWhatsAppIntegration)
                    features.Add("Integração WhatsApp");
                if (plan.HasSMSNotifications)
                    features.Add("Lembretes de consulta");
                features.Add("Suporte prioritário");
            }
            else if (plan.Type == PlanType.Premium)
            {
                if (plan.HasReports)
                    features.Add("Relatórios gerenciais");
                if (plan.HasWhatsAppIntegration)
                    features.Add("Integração WhatsApp");
                if (plan.HasSMSNotifications)
                    features.Add("Notificações por SMS");
                if (plan.HasTissExport)
                    features.Add("Exportação TISS");
                features.Add("Dashboard avançado");
                features.Add("API de integração");
                features.Add("Suporte 24/7");
            }
            else if (plan.Type == PlanType.Enterprise)
            {
                if (plan.HasReports)
                    features.Add("Todos os recursos Premium");
                features.Add("Desenvolvimento de funcionalidades específicas");
                features.Add("Treinamento personalizado");
                features.Add("Gerente de conta dedicado");
                features.Add("SLA garantido");
            }

            return features;
        }
    }
}
