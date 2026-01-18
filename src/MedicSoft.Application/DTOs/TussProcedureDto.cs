using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for TUSS Procedure
    /// </summary>
    public class TussProcedureDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal ReferencePrice { get; set; }
        public bool RequiresAuthorization { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a TUSS Procedure
    /// </summary>
    public class CreateTussProcedureDto
    {
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal ReferencePrice { get; set; }
        public bool RequiresAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for updating a TUSS Procedure
    /// </summary>
    public class UpdateTussProcedureDto
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal ReferencePrice { get; set; }
        public bool RequiresAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for TUSS import result
    /// </summary>
    public class TussImportResultDto
    {
        public bool Success { get; set; }
        public int TotalRecords { get; set; }
        public int ImportedRecords { get; set; }
        public int UpdatedRecords { get; set; }
        public int FailedRecords { get; set; }
        public List<string> Errors { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
