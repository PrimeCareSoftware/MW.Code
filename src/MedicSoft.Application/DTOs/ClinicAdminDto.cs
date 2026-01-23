namespace MedicSoft.Application.DTOs
{
    public class ClinicAdminInfoDto
    {
        public Guid ClinicId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Subdomain { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; }
        public bool AllowEmergencySlots { get; set; }
        public bool IsActive { get; set; }
        public bool ShowOnPublicSite { get; set; }
        public string ClinicType { get; set; } = string.Empty;
        public string? WhatsAppNumber { get; set; }
        public string DefaultPaymentReceiverType { get; set; } = "Secretary";
        public int NumberOfRooms { get; set; } = 1;
    }

    public class UpdateClinicInfoRequest
    {
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public int? AppointmentDurationMinutes { get; set; }
        public bool? AllowEmergencySlots { get; set; }
        public int? NumberOfRooms { get; set; }
    }

    public class ClinicUserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateClinicUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Document { get; set; }
    }

    public class UpdateClinicUserRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public bool? IsActive { get; set; }
    }
}
