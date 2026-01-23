using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Represents a clinic that a user has access to
    /// </summary>
    public class UserClinicDto
    {
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string ClinicAddress { get; set; } = string.Empty;
        public bool IsPreferred { get; set; }
        public bool IsActive { get; set; }
        public DateTime LinkedDate { get; set; }
    }

    /// <summary>
    /// Request to switch user's current clinic
    /// </summary>
    public class SwitchClinicRequest
    {
        public Guid ClinicId { get; set; }
    }

    /// <summary>
    /// Response after switching clinic
    /// </summary>
    public class SwitchClinicResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? CurrentClinicId { get; set; }
        public string? CurrentClinicName { get; set; }
    }
}
