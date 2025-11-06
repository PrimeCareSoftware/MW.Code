using System;

namespace MedicSoft.Application.DTOs
{
    public class WaitingQueueEntryDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CalledTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string? TriageNotes { get; set; }
        public int EstimatedWaitTimeMinutes { get; set; }
        public int ActualWaitTimeMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateWaitingQueueEntryDto
    {
        public Guid AppointmentId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid PatientId { get; set; }
        public string Priority { get; set; } = "Normal";
        public string? TriageNotes { get; set; }
    }

    public class UpdateQueueTriageDto
    {
        public string Priority { get; set; } = "Normal";
        public string? TriageNotes { get; set; }
    }

    public class WaitingQueueSummaryDto
    {
        public Guid ClinicId { get; set; }
        public int TotalWaiting { get; set; }
        public int TotalCalled { get; set; }
        public int TotalInProgress { get; set; }
        public int AverageWaitTimeMinutes { get; set; }
        public List<WaitingQueueEntryDto> Entries { get; set; } = new();
    }

    public class PublicQueueDisplayDto
    {
        public int Position { get; set; }
        public string PatientIdentifier { get; set; } = string.Empty; // Anonymized or partial name
        public string Status { get; set; } = string.Empty;
        public int? EstimatedWaitTimeMinutes { get; set; }
    }

    public class WaitingQueueConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string DisplayMode { get; set; } = "InternalOnly";
        public bool ShowEstimatedWaitTime { get; set; }
        public bool ShowPatientNames { get; set; }
        public bool ShowPriority { get; set; }
        public bool ShowPosition { get; set; }
        public int AutoRefreshSeconds { get; set; }
        public bool EnableSoundNotifications { get; set; }
    }

    public class UpdateWaitingQueueConfigurationDto
    {
        public string DisplayMode { get; set; } = "InternalOnly";
        public bool ShowEstimatedWaitTime { get; set; } = true;
        public bool ShowPatientNames { get; set; } = true;
        public bool ShowPriority { get; set; } = false;
        public bool ShowPosition { get; set; } = true;
        public int AutoRefreshSeconds { get; set; } = 30;
        public bool EnableSoundNotifications { get; set; } = true;
    }
}
