using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for TISS Batch
    /// </summary>
    public class TissBatchDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public string BatchNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? XmlFileName { get; set; }
        public string? XmlFilePath { get; set; }
        public string? ProtocolNumber { get; set; }
        public string? ResponseXmlFileName { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public int GuideCount { get; set; }
        public List<TissGuideDto> Guides { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating a TISS Batch
    /// </summary>
    public class CreateTissBatchDto
    {
        public Guid ClinicId { get; set; }
        public Guid OperatorId { get; set; }
    }

    /// <summary>
    /// DTO for processing batch response from operator
    /// </summary>
    public class ProcessBatchResponseDto
    {
        public string ProtocolNumber { get; set; } = string.Empty;
        public string? ResponseXmlFileName { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public List<GuideResponseDto> GuideResponses { get; set; } = new();
    }

    /// <summary>
    /// DTO for individual guide response in batch
    /// </summary>
    public class GuideResponseDto
    {
        public string GuideNumber { get; set; } = string.Empty;
        public decimal? ApprovedAmount { get; set; }
        public decimal? GlosedAmount { get; set; }
        public string? GlossReason { get; set; }
        public List<ProcedureResponseDto> ProcedureResponses { get; set; } = new();
    }

    /// <summary>
    /// DTO for XML generation result
    /// </summary>
    public class TissXmlGenerationResultDto
    {
        public bool Success { get; set; }
        public string? XmlFilePath { get; set; }
        public string? XmlFileName { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
