using System;

namespace MedicSoft.Application.DTOs
{
    public class ClinicDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; }
        public bool AllowEmergencySlots { get; set; }
        public bool IsActive { get; set; }
        public string DefaultPaymentReceiverType { get; set; } = "Secretary"; // Doctor, Secretary, Other
        public int NumberOfRooms { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateClinicDto
    {
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; } = 30;
    }

    public class UpdateClinicDto
    {
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; }
        public bool AllowEmergencySlots { get; set; }
        public string? DefaultPaymentReceiverType { get; set; } // Doctor, Secretary, Other
        public int? NumberOfRooms { get; set; }
    }
}