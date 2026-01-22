using System;

namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapProcedureDto
    {
        public string ProcedureName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }
}
