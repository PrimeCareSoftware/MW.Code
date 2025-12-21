using System;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IRegistrationService
    {
        Task<RegistrationResult> RegisterClinicWithOwnerAsync(RegistrationRequestDto request);
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

        public async Task<RegistrationResult> RegisterClinicWithOwnerAsync(RegistrationRequestDto request)
        {
            // Validate required fields
            if (!request.AcceptTerms)
            {
                return RegistrationResult.CreateFailure("You must accept the terms and conditions");
            }

            // Validate password strength
            var (isValidPassword, passwordError) = _passwordHasher.ValidatePasswordStrength(request.Password, 8);
            if (!isValidPassword)
            {
                return RegistrationResult.CreateFailure($"Password validation failed: {passwordError}");
            }

            // Check if CNPJ already exists (CNPJ is unique across all tenants, so safe to check outside transaction)
            var existingClinic = await _clinicRepository.GetByCNPJAsync(request.ClinicCNPJ);
            if (existingClinic != null)
            {
                return RegistrationResult.CreateFailure("CNPJ already registered");
            }

            // Get subscription plan - plans are system-wide, so we use "system" as tenantId
            var plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), "system");
            if (plan == null)
            {
                return RegistrationResult.CreateFailure("Invalid subscription plan");
            }

            // Generate friendly subdomain from clinic name - this will be used as tenantId
            var subdomain = GenerateFriendlySubdomain(request.ClinicName);
            
            // Use the friendly subdomain as the tenant ID
            var tenantId = subdomain;

            // Execute all creations within a transaction to ensure data consistency
            // Username and email validation is done inside the transaction with the unique tenantId
            return await _clinicRepository.ExecuteInTransactionAsync(async () =>
            {
                // Validate username within transaction (although tenantId is unique, this ensures consistency)
                if (await _ownerRepository.ExistsByUsernameAsync(request.Username, tenantId))
                {
                    return RegistrationResult.CreateFailure("Username already taken");
                }

                // Validate email within transaction
                if (await _ownerRepository.ExistsByEmailAsync(request.OwnerEmail, tenantId))
                {
                    return RegistrationResult.CreateFailure("Email already registered");
                }

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
                
                // Set the friendly subdomain
                clinic.SetSubdomain(subdomain);

                // Create owner
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

                return RegistrationResult.CreateSuccess(
                    clinic.Id, 
                    owner.Id, 
                    tenantId, 
                    subdomain, 
                    clinic.Name, 
                    owner.FullName, 
                    owner.Email, 
                    owner.Username
                );
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

        /// <summary>
        /// Generate a friendly subdomain from clinic name
        /// </summary>
        private static string GenerateFriendlySubdomain(string clinicName)
        {
            if (string.IsNullOrWhiteSpace(clinicName))
            {
                return $"clinic-{Guid.NewGuid().ToString("N")[..8]}";
            }

            // Convert to lowercase and remove accents
            var subdomain = clinicName.ToLowerInvariant()
                .Normalize(System.Text.NormalizationForm.FormD);

            // Remove diacritics (accents)
            var chars = subdomain.Where(c => 
                System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != 
                System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
            subdomain = new string(chars).Normalize(System.Text.NormalizationForm.FormC);

            // Replace spaces and invalid characters with hyphens
            subdomain = System.Text.RegularExpressions.Regex.Replace(subdomain, @"[^a-z0-9]+", "-");

            // Remove leading/trailing hyphens
            subdomain = subdomain.Trim('-');

            // Ensure length constraints (3-63 characters)
            if (subdomain.Length < 3)
            {
                subdomain = $"{subdomain}-{Guid.NewGuid().ToString("N")[..4]}";
            }
            else if (subdomain.Length > 63)
            {
                subdomain = subdomain[..63].TrimEnd('-');
            }

            // Ensure it doesn't start or end with hyphen
            if (subdomain.StartsWith('-') || subdomain.EndsWith('-'))
            {
                subdomain = subdomain.Trim('-');
            }

            // Add a short random suffix to ensure uniqueness
            subdomain = $"{subdomain}-{Guid.NewGuid().ToString("N")[..4]}";

            return subdomain;
        }
    }
}
