namespace MedicSoft.Application.DTOs.Registration
{
    public class RegistrationRequestDto
    {
        // Clinic Information
        public string ClinicName { get; set; } = string.Empty;
        public string ClinicCNPJ { get; set; } = string.Empty;
        public string ClinicPhone { get; set; } = string.Empty;
        public string ClinicEmail { get; set; } = string.Empty;
        
        // Address
        public string Street { get; set; } = string.Empty;
        public string Number { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        
        // Owner Information
        public string OwnerName { get; set; } = string.Empty;
        public string OwnerCPF { get; set; } = string.Empty;
        public string OwnerPhone { get; set; } = string.Empty;
        public string OwnerEmail { get; set; } = string.Empty;
        
        // Login Credentials
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        
        // Subscription
        public string PlanId { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }
        
        // Trial
        public bool UseTrial { get; set; }
    }

    /// <summary>
    /// Result object for registration operations
    /// </summary>
    public class RegistrationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public Guid? OwnerId { get; set; }
        public string? TenantId { get; set; }
        public string? Subdomain { get; set; }
        public string? ClinicName { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? Username { get; set; }

        /// <summary>
        /// Creates a successful registration result
        /// </summary>
        public static RegistrationResult CreateSuccess(
            Guid clinicId, 
            Guid ownerId, 
            string tenantId, 
            string subdomain, 
            string clinicName, 
            string ownerName, 
            string ownerEmail, 
            string username)
        {
            return new RegistrationResult
            {
                Success = true,
                Message = "Registration successful",
                ClinicId = clinicId,
                OwnerId = ownerId,
                TenantId = tenantId,
                Subdomain = subdomain,
                ClinicName = clinicName,
                OwnerName = ownerName,
                OwnerEmail = ownerEmail,
                Username = username
            };
        }

        /// <summary>
        /// Creates a failed registration result
        /// </summary>
        public static RegistrationResult CreateFailure(string message)
        {
            return new RegistrationResult
            {
                Success = false,
                Message = message
            };
        }
    }

    public class RegistrationResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public string? TenantId { get; set; }
        public string? Subdomain { get; set; }
        public string? ClinicName { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public string? Username { get; set; }
    }

    public class CheckCNPJResponseDto
    {
        public bool Exists { get; set; }
    }

    public class CheckUsernameResponseDto
    {
        public bool Available { get; set; }
    }

    public class ContactRequestDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    public class ContactResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
