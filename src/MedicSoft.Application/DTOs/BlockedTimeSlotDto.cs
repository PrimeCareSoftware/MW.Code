using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public class BlockedTimeSlotDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public BlockedTimeSlotType Type { get; set; }
        public string? Reason { get; set; }
        public bool IsRecurring { get; set; }
        public Guid? RecurringPatternId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public string? ClinicName { get; set; }
        public string? ProfessionalName { get; set; }
    }

    public class CreateBlockedTimeSlotDto
    {
        public Guid ClinicId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public BlockedTimeSlotType Type { get; set; }
        public string? Reason { get; set; }
    }

    public class UpdateBlockedTimeSlotDto
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public BlockedTimeSlotType Type { get; set; }
        public string? Reason { get; set; }
    }
}
