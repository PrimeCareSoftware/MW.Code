using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Entities
{
    public class SoapRecord : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        public DateTime RecordDate { get; private set; }
        
        // S - Subjetivo
        public SubjectiveData? Subjective { get; private set; }
        
        // O - Objetivo
        public ObjectiveData? Objective { get; private set; }
        
        // A - Avaliação
        public AssessmentData? Assessment { get; private set; }
        
        // P - Plano
        public PlanData? Plan { get; private set; }
        
        // Metadados
        public bool IsComplete { get; private set; }
        public DateTime? CompletionDate { get; private set; }
        public bool IsLocked { get; private set; }  // Após conclusão
        
        // Navigation properties
        public virtual Appointment Appointment { get; private set; } = null!;
        public virtual Patient Patient { get; private set; } = null!;
        public virtual User Doctor { get; private set; } = null!;

        private SoapRecord() 
        {
            // EF Constructor
        }

        public SoapRecord(Guid appointmentId, Guid patientId, Guid doctorId, string tenantId) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (doctorId == Guid.Empty)
                throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));

            AppointmentId = appointmentId;
            PatientId = patientId;
            DoctorId = doctorId;
            RecordDate = DateTime.UtcNow;
            IsComplete = false;
            IsLocked = false;
        }

        public void UpdateSubjective(SubjectiveData subjectiveData)
        {
            if (IsLocked)
                throw new InvalidOperationException("Cannot update a locked SOAP record");
            
            if (subjectiveData == null)
                throw new ArgumentNullException(nameof(subjectiveData));

            Subjective = subjectiveData;
            UpdateTimestamp();
        }

        public void UpdateObjective(ObjectiveData objectiveData)
        {
            if (IsLocked)
                throw new InvalidOperationException("Cannot update a locked SOAP record");
            
            if (objectiveData == null)
                throw new ArgumentNullException(nameof(objectiveData));

            Objective = objectiveData;
            UpdateTimestamp();
        }

        public void UpdateAssessment(AssessmentData assessmentData)
        {
            if (IsLocked)
                throw new InvalidOperationException("Cannot update a locked SOAP record");
            
            if (assessmentData == null)
                throw new ArgumentNullException(nameof(assessmentData));

            Assessment = assessmentData;
            UpdateTimestamp();
        }

        public void UpdatePlan(PlanData planData)
        {
            if (IsLocked)
                throw new InvalidOperationException("Cannot update a locked SOAP record");
            
            if (planData == null)
                throw new ArgumentNullException(nameof(planData));

            Plan = planData;
            UpdateTimestamp();
        }

        public SoapRecordValidation ValidateCompleteness()
        {
            var validation = new SoapRecordValidation
            {
                HasSubjective = Subjective != null && 
                               !string.IsNullOrWhiteSpace(Subjective.ChiefComplaint) &&
                               !string.IsNullOrWhiteSpace(Subjective.HistoryOfPresentIllness),
                HasObjective = Objective != null && 
                              (Objective.VitalSigns != null || Objective.PhysicalExam != null),
                HasAssessment = Assessment != null && 
                               !string.IsNullOrWhiteSpace(Assessment.PrimaryDiagnosis),
                HasPlan = Plan != null && 
                         (Plan.Prescriptions.Count > 0 || 
                          Plan.ExamRequests.Count > 0 || 
                          !string.IsNullOrWhiteSpace(Plan.PatientInstructions))
            };

            var missingFields = new List<string>();
            if (!validation.HasSubjective)
                missingFields.Add("Subjective section (Chief Complaint and History of Present Illness are required)");
            if (!validation.HasObjective)
                missingFields.Add("Objective section (Vital Signs or Physical Examination required)");
            if (!validation.HasAssessment)
                missingFields.Add("Assessment section (Primary Diagnosis required)");
            if (!validation.HasPlan)
                missingFields.Add("Plan section (At least one prescription, exam request, or patient instruction required)");

            validation.MissingFields = missingFields;
            validation.IsValid = missingFields.Count == 0;

            return validation;
        }

        public void CompleteSoapRecord()
        {
            if (IsLocked)
                throw new InvalidOperationException("SOAP record is already locked");

            var validation = ValidateCompleteness();
            
            if (!validation.IsValid)
            {
                throw new InvalidOperationException(
                    $"Cannot complete SOAP record with missing required fields: {string.Join(", ", validation.MissingFields)}");
            }

            IsComplete = true;
            CompletionDate = DateTime.UtcNow;
            IsLocked = true;
            UpdateTimestamp();
        }

        public void Unlock()
        {
            if (!IsLocked)
                throw new InvalidOperationException("SOAP record is not locked");

            IsLocked = false;
            IsComplete = false;
            CompletionDate = null;
            UpdateTimestamp();
        }
    }

    public class SoapRecordValidation
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
