using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for glosas (rejections) summary
    /// </summary>
    public class GlosasSummaryDto
    {
        public decimal TotalBilled { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalBatches { get; set; }
        public int TotalGuides { get; set; }
        public int GlosedGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosas by operator
    /// </summary>
    public class GlosasByOperatorDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalGuides { get; set; }
        public int GlosedGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosas trend over time
    /// </summary>
    public class GlosasTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalGuides { get; set; }
    }

    /// <summary>
    /// DTO for procedure glosas (top procedures with rejections)
    /// </summary>
    public class ProcedureGlosasDto
    {
        public Guid ProcedureId { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalOccurrences { get; set; }
        public int GlosedOccurrences { get; set; }
    }

    /// <summary>
    /// DTO for authorization rate
    /// </summary>
    public class AuthorizationRateDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int PendingRequests { get; set; }
        public decimal ApprovalRate { get; set; }
    }

    /// <summary>
    /// DTO for approval time by operator
    /// </summary>
    public class ApprovalTimeDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public double AverageApprovalDays { get; set; }
        public int TotalProcessed { get; set; }
        public double MinApprovalDays { get; set; }
        public double MaxApprovalDays { get; set; }
    }

    /// <summary>
    /// DTO for monthly performance comparison
    /// </summary>
    public class MonthlyPerformanceDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal GlosaPercentage { get; set; }
        public double AverageApprovalDays { get; set; }
        public int TotalGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosa alerts
    /// </summary>
    public class GlosaAlertDto
    {
        public string AlertType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // "high", "medium", "low"
        public decimal Value { get; set; }
        public decimal Threshold { get; set; }
    }
}
