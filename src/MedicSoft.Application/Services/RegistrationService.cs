using System;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        private readonly IAccessProfileRepository _accessProfileRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICompanyRepository _companyRepository;
        private readonly IUserClinicLinkRepository _userClinicLinkRepository;
        
        private const int MaxSubdomainAttempts = 100;
        private const int MaxCampaignJoinRetries = 3;

        public RegistrationService(
            IClinicRepository clinicRepository,
            IOwnerService ownerService,
            IOwnerRepository ownerRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IClinicSubscriptionRepository clinicSubscriptionRepository,
            IAccessProfileRepository accessProfileRepository,
            IPasswordHasher passwordHasher,
            ICompanyRepository companyRepository,
            IUserClinicLinkRepository userClinicLinkRepository)
        {
            _clinicRepository = clinicRepository;
            _ownerService = ownerService;
            _ownerRepository = ownerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _clinicSubscriptionRepository = clinicSubscriptionRepository;
            _accessProfileRepository = accessProfileRepository;
            _passwordHasher = passwordHasher;
            _companyRepository = companyRepository;
            _userClinicLinkRepository = userClinicLinkRepository;
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

            // Determine company document (support both old ClinicCNPJ and new ClinicDocument)
            // In the new model, this represents the company/enterprise document
            var companyDocument = !string.IsNullOrWhiteSpace(request.ClinicDocument) 
                ? request.ClinicDocument 
                : request.ClinicCNPJ;

            // Determine document type
            DocumentType companyDocumentType;
            if (!string.IsNullOrWhiteSpace(request.ClinicDocumentType))
            {
                if (!Enum.TryParse<DocumentType>(request.ClinicDocumentType, true, out companyDocumentType))
                {
                    return RegistrationResult.CreateFailure("Invalid document type specified");
                }
            }
            else
            {
                // Auto-detect based on length for backward compatibility
                var cleanDocument = new string(companyDocument.Where(char.IsDigit).ToArray());
                companyDocumentType = cleanDocument.Length == 11 ? DocumentType.CPF : DocumentType.CNPJ;
            }

            // Check if company document already exists
            var existingCompany = await _companyRepository.GetByDocumentAsync(companyDocument);
            if (existingCompany != null)
            {
                var docTypeLabel = companyDocumentType == DocumentType.CPF ? "CPF" : "CNPJ";
                return RegistrationResult.CreateFailure($"{docTypeLabel} already registered");
            }

            // Get subscription plan - plans are system-wide, so we use "system" as tenantId
            var plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), "system");
            if (plan == null)
            {
                return RegistrationResult.CreateFailure("Invalid subscription plan");
            }

            // Determine company and clinic names based on document type and provided data
            // For CNPJ, company name is required (validation should enforce this)
            // For CPF without company name, use owner name
            var companyName = !string.IsNullOrWhiteSpace(request.CompanyName) 
                ? request.CompanyName 
                : (companyDocumentType == DocumentType.CPF ? request.OwnerName : request.ClinicName);
            
            // Validate that CNPJ registrations have a company name
            if (companyDocumentType == DocumentType.CNPJ && string.IsNullOrWhiteSpace(companyName))
            {
                return RegistrationResult.CreateFailure("Company name is required for CNPJ registration");
            }
            
            var clinicName = !string.IsNullOrWhiteSpace(request.ClinicName) 
                ? request.ClinicName 
                : companyName;

            // Generate friendly subdomain from company/clinic name - this will be used as tenantId
            // In the new architecture, Company owns the subdomain/tenantId
            var baseSubdomain = GenerateFriendlySubdomain(clinicName);
            var subdomain = baseSubdomain;
            
            // Ensure subdomain uniqueness at Company level
            var isUnique = await _companyRepository.IsSubdomainUniqueAsync(subdomain);
            if (!isUnique)
            {
                // If not unique, append sequential numbers (2, 3, 4, etc.) to make it unique
                var counter = 2;
                
                while (!isUnique && counter <= MaxSubdomainAttempts)
                {
                    subdomain = $"{baseSubdomain}-{counter}";
                    isUnique = await _companyRepository.IsSubdomainUniqueAsync(subdomain);
                    counter++;
                }
                
                if (!isUnique)
                {
                    return RegistrationResult.CreateFailure("Unable to generate unique identifier. Please try again with a different clinic name.");
                }
            }
            
            // Use the friendly subdomain as the tenant ID (now owned by Company)
            var tenantId = subdomain;

            // Determine owner document type
            DocumentType? ownerDocumentType = null;
            if (!string.IsNullOrWhiteSpace(request.OwnerCPF))
            {
                ownerDocumentType = DocumentType.CPF;
            }

            // Execute all creations within a transaction to ensure data consistency
            // Username and email validation is done inside the transaction with the unique tenantId
            // Retry logic for handling campaign concurrency conflicts
            for (int attempt = 1; attempt <= MaxCampaignJoinRetries; attempt++)
            {
                try
                {
                    return await _clinicRepository.ExecuteInTransactionAsync(async () =>
                    {
                        return await RegisterClinicWithCampaignAsync(request, plan, tenantId, subdomain, 
                            companyDocument, companyDocumentType, ownerDocumentType, companyName, clinicName);
                    });
                }
                catch (DbUpdateConcurrencyException) when (attempt < MaxCampaignJoinRetries)
                {
                    // Concurrency conflict - reload plan and retry
                    // This can happen if multiple users try to join the campaign simultaneously
                    plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), "system");
                    if (plan == null || !plan.CanJoinCampaign())
                    {
                        return RegistrationResult.CreateFailure("Campaign is no longer available");
                    }
                    // Continue to next retry attempt
                    await Task.Delay(100 * attempt); // Exponential backoff
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Cannot join campaign") && attempt < MaxCampaignJoinRetries)
                {
                    // Campaign slots filled during registration - reload and retry
                    plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), "system");
                    if (plan == null || !plan.CanJoinCampaign())
                    {
                        return RegistrationResult.CreateFailure("Campaign slots are now full");
                    }
                    await Task.Delay(100 * attempt); // Exponential backoff
                }
            }

            // If we get here, all retries failed
            return RegistrationResult.CreateFailure("Unable to complete registration. Please try again.");
        }

        private async Task<RegistrationResult> RegisterClinicWithCampaignAsync(
            RegistrationRequestDto request, 
            SubscriptionPlan plan, 
            string tenantId, 
            string subdomain,
            string companyDocument,
            DocumentType companyDocumentType,
            DocumentType? ownerDocumentType,
            string companyName,
            string clinicName)
        {
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

                // Step 1: Create Company (the new tenant entity)
                // For CPF (physical person): use owner name if company name not provided
                // For CNPJ (legal entity): use provided company/clinic name
                var company = new Company(
                    companyName,     // Company name
                    companyName,     // Trade name (same as name)
                    companyDocument,
                    companyDocumentType,
                    request.ClinicPhone,
                    request.ClinicEmail,
                    tenantId
                );
                company.SetSubdomain(subdomain);
                await _companyRepository.AddAsync(company);
                await _companyRepository.SaveChangesAsync(); // Save to get Company.Id

                // Build full address
                var fullAddress = $"{request.Street}, {request.Number} {request.Complement}, {request.Neighborhood} - {request.City}/{request.State} - CEP: {request.ZipCode}";

                // Step 2: Create the first clinic linked to this company
                var clinic = new Clinic(
                    clinicName,
                    clinicName,         // TradeName same as Name
                    companyDocument,    // For now, clinic uses same document as company
                    request.ClinicPhone,
                    request.ClinicEmail,
                    fullAddress,
                    new TimeSpan(8, 0, 0), // 8 AM
                    new TimeSpan(18, 0, 0), // 6 PM
                    tenantId,
                    30, // 30 minute appointments
                    companyDocumentType,
                    company.Id  // Link to company
                );
                
                clinic.SetSubdomain(subdomain); // Clinic can have subdomain for backward compatibility
                
                // Set clinic type/specialty if provided
                if (!string.IsNullOrWhiteSpace(request.ClinicType))
                {
                    if (Enum.TryParse<ClinicType>(request.ClinicType, true, out var clinicType))
                    {
                        // Update clinic type using the existing method
                        clinic.UpdatePublicSiteSettings(false, clinicType, null);
                    }
                }

                await _clinicRepository.AddWithoutSaveAsync(clinic);

                // Step 3: Create owner
                var passwordHash = _passwordHasher.HashPassword(request.Password);
                var owner = new Owner(
                    request.Username,
                    request.OwnerEmail,
                    passwordHash,
                    request.OwnerName,
                    request.OwnerPhone,
                    tenantId,
                    clinic.Id,
                    null, // professionalId
                    null, // specialty
                    request.OwnerCPF, // document
                    ownerDocumentType // documentType
                );
                await _ownerRepository.AddWithoutSaveAsync(owner);
                
                // Step 4: UserClinicLinks
                // NOTE: Owner entity is separate from User entity in this system.
                // - Owners are clinic proprietors who manage the business
                // - Users are staff (doctors, receptionists, etc.) who work at clinics
                // When an Owner also works as a User (doctor/staff), a separate User record
                // will be created and linked via UserClinicLink.
                // The Phase 2 migration handles creating UserClinicLinks for existing users.
                
                // Step 5: Create subscription with campaign price if available
                var trialDays = request.UseTrial ? plan.TrialDays : 0;
                var effectivePrice = plan.GetEffectivePrice(); // Use campaign price if active, otherwise regular price
                
                var subscription = new ClinicSubscription(
                    clinic.Id,
                    plan.Id,
                    DateTime.UtcNow,
                    trialDays,
                    effectivePrice,
                    tenantId
                );
                
                // If joining a campaign, increment early adopter count
                if (plan.CanJoinCampaign())
                {
                    plan.IncrementEarlyAdopters();
                    await _subscriptionPlanRepository.UpdateAsync(plan);
                }

                await _clinicSubscriptionRepository.AddWithoutSaveAsync(subscription);

                // Step 6: Create default access profiles for the clinic
                var defaultProfiles = new[]
                {
                    AccessProfile.CreateDefaultOwnerProfile(tenantId, clinic.Id),
                    AccessProfile.CreateDefaultMedicalProfile(tenantId, clinic.Id),
                    AccessProfile.CreateDefaultReceptionProfile(tenantId, clinic.Id),
                    AccessProfile.CreateDefaultFinancialProfile(tenantId, clinic.Id)
                };

                foreach (var profile in defaultProfiles)
                {
                    await _accessProfileRepository.AddAsync(profile);
                }

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
                return "clinic";
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
                subdomain = "clinic";
            }
            else if (subdomain.Length > 63)
            {
                subdomain = subdomain[..63].TrimEnd('-');
            }

            // Ensure it doesn't start or end with hyphen (should be covered by Trim('-') above, but double-check)
            if (subdomain.StartsWith('-') || subdomain.EndsWith('-'))
            {
                subdomain = subdomain.Trim('-');
            }

            return subdomain;
        }
    }
}
