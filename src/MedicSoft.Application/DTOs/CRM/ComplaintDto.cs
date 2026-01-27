using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class ComplaintDto
    {
        public Guid Id { get; set; }
        public string ProtocolNumber { get; set; } = string.Empty;
        
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public ComplaintCategory Category { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public ComplaintPriority Priority { get; set; }
        public string PriorityName { get; set; } = string.Empty;
        public ComplaintStatus Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        
        public Guid? AssignedToUserId { get; set; }
        public string? AssignedToUserName { get; set; }
        
        public DateTime ReceivedAt { get; set; }
        public DateTime? FirstResponseAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        
        public int? ResponseTimeMinutes { get; set; }
        public int? ResolutionTimeMinutes { get; set; }
        
        public int? SatisfactionRating { get; set; }
        public string? SatisfactionFeedback { get; set; }
        
        public List<ComplaintInteractionDto> Interactions { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ComplaintInteractionDto
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public DateTime InteractionDate { get; set; }
    }

    public class CreateComplaintDto
    {
        public Guid PatientId { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ComplaintCategory Category { get; set; }
        public ComplaintPriority? Priority { get; set; }
    }

    public class UpdateComplaintDto
    {
        public string? Subject { get; set; }
        public string? Description { get; set; }
        public ComplaintCategory? Category { get; set; }
        public ComplaintPriority? Priority { get; set; }
    }

    public class AddComplaintInteractionDto
    {
        public string Message { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
    }

    public class UpdateComplaintStatusDto
    {
        public ComplaintStatus Status { get; set; }
    }

    public class AssignComplaintDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class ComplaintDashboardDto
    {
        public int TotalComplaints { get; set; }
        public int OpenComplaints { get; set; }
        public int InProgressComplaints { get; set; }
        public int ResolvedComplaints { get; set; }
        public int ClosedComplaints { get; set; }
        
        public Dictionary<ComplaintCategory, int> ComplaintsByCategory { get; set; } = new();
        public Dictionary<ComplaintPriority, int> ComplaintsByPriority { get; set; } = new();
        public Dictionary<ComplaintStatus, int> ComplaintsByStatus { get; set; } = new();
        
        public double AverageResponseTimeMinutes { get; set; }
        public double AverageResolutionTimeMinutes { get; set; }
        
        public int ComplaintsWithinSLA { get; set; }
        public int ComplaintsOutsideSLA { get; set; }
        public double SLAComplianceRate { get; set; }
        
        public double AverageSatisfactionRating { get; set; }
        public int TotalRatings { get; set; }
        
        public List<ComplaintDto> RecentComplaints { get; set; } = new();
        public List<ComplaintDto> UrgentComplaints { get; set; } = new();
    }
}
