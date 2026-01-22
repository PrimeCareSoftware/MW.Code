using System;

namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapRecordDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public DateTime RecordDate { get; set; }
        public SubjectiveDataDto? Subjective { get; set; }
        public ObjectiveDataDto? Objective { get; set; }
        public AssessmentDataDto? Assessment { get; set; }
        public PlanDataDto? Plan { get; set; }
        public bool IsComplete { get; set; }
        public DateTime? CompletionDate { get; set; }
        public bool IsLocked { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
