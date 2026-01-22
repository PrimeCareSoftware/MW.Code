using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class UpdatePlanDto
    {
        public List<SoapPrescriptionDto>? Prescriptions { get; set; }
        public List<SoapExamRequestDto>? ExamRequests { get; set; }
        public List<SoapProcedureDto>? Procedures { get; set; }
        public List<SoapReferralDto>? Referrals { get; set; }
        public string? ReturnInstructions { get; set; }
        public DateTime? NextAppointmentDate { get; set; }
        public string? PatientInstructions { get; set; }
        public string? DietaryRecommendations { get; set; }
        public string? ActivityRestrictions { get; set; }
        public string? WarningSymptoms { get; set; }
    }
}
