using System;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IRegistrationService
    {
        Task<(bool Success, string Message, Guid? ClinicId, Guid? OwnerId)> RegisterClinicWithOwnerAsync(RegistrationRequestDto request);
        Task<bool> CheckCNPJExistsAsync(string cnpj);
        Task<bool> CheckUsernameAvailableAsync(string username, string tenantId);
    }

    public class RegistrationService : IRegistrationService
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IOwnerService _ownerService;
        private readonly IOwnerRepository _ownerRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IClinicSubscriptionRepository _clinicSubscriptionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegistrationService(
            IClinicRepository clinicRepository,
            IOwnerService ownerService,
            IOwnerRepository ownerRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IClinicSubscriptionRepository clinicSubscriptionRepository,
            IPasswordHasher passwordHasher)
        {
            _clinicRepository = clinicRepository;
            _ownerService = ownerService;
            _ownerRepository = ownerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _clinicSubscriptionRepository = clinicSubscriptionRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Message, Guid? ClinicId, Guid? OwnerId)> RegisterClinicWithOwnerAsync(RegistrationRequestDto request)
        {
            // Validate required fields
            if (!request.AcceptTerms)
            {
                return (false, "You must accept the terms and conditions", null, null);
            }

            // Validate password strength
            var (isValidPassword, passwordError) = _passwordHasher.ValidatePasswordStrength(request.Password, 8);
            if (!isValidPassword)
            {
                return (false, $"Password validation failed: {passwordError}", null, null);
            }

            // Check if CNPJ already exists
            var existingClinic = await _clinicRepository.GetByCNPJAsync(request.ClinicCNPJ);
            if (existingClinic != null)
            {
                return (false, "CNPJ already registered", null, null);
            }

            // Generate tenant ID
            var tenantId = Guid.NewGuid().ToString();

            // Check if username already exists
            if (await _ownerService.ExistsByUsernameAsync(request.Username, tenantId))
            {
                return (false, "Username already taken", null, null);
            }

            // Get subscription plan - plans are system-wide, so we use "system" as tenantId
            var plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), "system");
            if (plan == null)
            {
                return (false, "Invalid subscription plan", null, null);
            }

            // Execute all creations within a transaction to ensure data consistency
            return await _clinicRepository.ExecuteInTransactionAsync(async () =>
            {
                // Build full address
                var fullAddress = $"{request.Street}, {request.Number} {request.Complement}, {request.Neighborhood} - {request.City}/{request.State} - CEP: {request.ZipCode}";

                // Create clinic with default schedule (8AM to 6PM)
                var clinic = new Clinic(
                    request.ClinicName,
                    request.ClinicName, // TradeName same as Name
                    request.ClinicCNPJ,
                    request.ClinicPhone,
                    request.ClinicEmail,
                    fullAddress,
                    new TimeSpan(8, 0, 0), // 8 AM
                    new TimeSpan(18, 0, 0), // 6 PM
                    tenantId,
                    30 // 30 minute appointments
                );

                await _clinicRepository.AddWithoutSaveAsync(clinic);

                // Create owner (use AddWithoutSaveAsync pattern for consistency within transaction)
                var passwordHash = _passwordHasher.HashPassword(request.Password);
                var owner = new Owner(
                    request.Username,
                    request.OwnerEmail,
                    passwordHash,
                    request.OwnerName,
                    request.OwnerPhone,
                    tenantId,
                    clinic.Id
                );
                await _ownerRepository.AddWithoutSaveAsync(owner);

                // Create subscription
                var trialDays = request.UseTrial ? plan.TrialDays : 0;
                var subscription = new ClinicSubscription(
                    clinic.Id,
                    plan.Id,
                    DateTime.UtcNow,
                    trialDays,
                    plan.MonthlyPrice,
                    tenantId
                );

                await _clinicSubscriptionRepository.AddWithoutSaveAsync(subscription);

                // Save all changes at once within the transaction
                await _clinicRepository.SaveChangesAsync();

                return (true, "Registration successful", clinic.Id, owner.Id);
            });
        }

        public async Task<bool> CheckCNPJExistsAsync(string cnpj)
        {
            var clinic = await _clinicRepository.GetByCNPJAsync(cnpj);
            return clinic != null;
        }

        public async Task<bool> CheckUsernameAvailableAsync(string username, string tenantId)
        {
            return !await _ownerService.ExistsByUsernameAsync(username, tenantId);
        }
    }
}
