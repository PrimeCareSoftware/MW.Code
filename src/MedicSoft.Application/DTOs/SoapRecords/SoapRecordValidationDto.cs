using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapRecordValidationDto
    {
        public bool IsValid { get; set; }
        public List<string> MissingFields { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public bool HasSubjective { get; set; }
        public bool HasObjective { get; set; }
        public bool HasAssessment { get; set; }
        public bool HasPlan { get; set; }
    }
}
