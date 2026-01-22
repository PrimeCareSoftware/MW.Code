using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicSoft.Domain.ValueObjects
{
    public class SoapPrescription
    {
        public string MedicationName { get; private set; }
        public string? Dosage { get; private set; }
        public string? Frequency { get; private set; }
        public string? Duration { get; private set; }
        public string? Instructions { get; private set; }

        private SoapPrescription() 
        {
            MedicationName = null!;
        }

        public SoapPrescription(string medicationName, string? dosage = null, 
            string? frequency = null, string? duration = null, string? instructions = null)
        {
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name is required", nameof(medicationName));

            MedicationName = medicationName.Trim();
            Dosage = dosage?.Trim();
            Frequency = frequency?.Trim();
            Duration = duration?.Trim();
            Instructions = instructions?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SoapPrescription other)
                return false;

            return MedicationName == other.MedicationName &&
                   Dosage == other.Dosage &&
                   Frequency == other.Frequency &&
                   Duration == other.Duration &&
                   Instructions == other.Instructions;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(MedicationName, Dosage, Frequency, Duration, Instructions);
        }
    }

    public class SoapExamRequest
    {
        public string ExamName { get; private set; }
        public string? ExamType { get; private set; }  // Lab, Imaging, etc.
        public string? ClinicalIndication { get; private set; }
        public bool IsUrgent { get; private set; }

        private SoapExamRequest() 
        {
            ExamName = null!;
        }

        public SoapExamRequest(string examName, string? examType = null, 
            string? clinicalIndication = null, bool isUrgent = false)
        {
            if (string.IsNullOrWhiteSpace(examName))
                throw new ArgumentException("Exam name is required", nameof(examName));

            ExamName = examName.Trim();
            ExamType = examType?.Trim();
            ClinicalIndication = clinicalIndication?.Trim();
            IsUrgent = isUrgent;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SoapExamRequest other)
                return false;

            return ExamName == other.ExamName &&
                   ExamType == other.ExamType &&
                   ClinicalIndication == other.ClinicalIndication &&
                   IsUrgent == other.IsUrgent;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExamName, ExamType, ClinicalIndication, IsUrgent);
        }
    }

    public class SoapProcedure
    {
        public string ProcedureName { get; private set; }
        public string? Description { get; private set; }
        public DateTime? ScheduledDate { get; private set; }

        private SoapProcedure() 
        {
            ProcedureName = null!;
        }

        public SoapProcedure(string procedureName, string? description = null, DateTime? scheduledDate = null)
        {
            if (string.IsNullOrWhiteSpace(procedureName))
                throw new ArgumentException("Procedure name is required", nameof(procedureName));

            ProcedureName = procedureName.Trim();
            Description = description?.Trim();
            ScheduledDate = scheduledDate;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SoapProcedure other)
                return false;

            return ProcedureName == other.ProcedureName &&
                   Description == other.Description &&
                   ScheduledDate == other.ScheduledDate;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ProcedureName, Description, ScheduledDate);
        }
    }

    public class SoapReferral
    {
        public string SpecialtyName { get; private set; }
        public string? Reason { get; private set; }
        public string? Priority { get; private set; }  // Routine, Urgent, Emergency

        private SoapReferral() 
        {
            SpecialtyName = null!;
        }

        public SoapReferral(string specialtyName, string? reason = null, string? priority = null)
        {
            if (string.IsNullOrWhiteSpace(specialtyName))
                throw new ArgumentException("Specialty name is required", nameof(specialtyName));

            SpecialtyName = specialtyName.Trim();
            Reason = reason?.Trim();
            Priority = priority?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SoapReferral other)
                return false;

            return SpecialtyName == other.SpecialtyName &&
                   Reason == other.Reason &&
                   Priority == other.Priority;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecialtyName, Reason, Priority);
        }
    }

    public class PlanData
    {
        public List<SoapPrescription> Prescriptions { get; private set; }
        public List<SoapExamRequest> ExamRequests { get; private set; }
        public List<SoapProcedure> Procedures { get; private set; }
        public List<SoapReferral> Referrals { get; private set; }
        
        public string? ReturnInstructions { get; private set; }
        public DateTime? NextAppointmentDate { get; private set; }
        public string? PatientInstructions { get; private set; }  // Orientações
        public string? DietaryRecommendations { get; private set; }
        public string? ActivityRestrictions { get; private set; }
        public string? WarningSymptoms { get; private set; }  // Sinais de alerta

        private PlanData() 
        {
            Prescriptions = new List<SoapPrescription>();
            ExamRequests = new List<SoapExamRequest>();
            Procedures = new List<SoapProcedure>();
            Referrals = new List<SoapReferral>();
        }

        public PlanData(
            List<SoapPrescription>? prescriptions = null,
            List<SoapExamRequest>? examRequests = null,
            List<SoapProcedure>? procedures = null,
            List<SoapReferral>? referrals = null,
            string? returnInstructions = null,
            DateTime? nextAppointmentDate = null,
            string? patientInstructions = null,
            string? dietaryRecommendations = null,
            string? activityRestrictions = null,
            string? warningSymptoms = null)
        {
            Prescriptions = prescriptions ?? new List<SoapPrescription>();
            ExamRequests = examRequests ?? new List<SoapExamRequest>();
            Procedures = procedures ?? new List<SoapProcedure>();
            Referrals = referrals ?? new List<SoapReferral>();
            ReturnInstructions = returnInstructions?.Trim();
            NextAppointmentDate = nextAppointmentDate;
            PatientInstructions = patientInstructions?.Trim();
            DietaryRecommendations = dietaryRecommendations?.Trim();
            ActivityRestrictions = activityRestrictions?.Trim();
            WarningSymptoms = warningSymptoms?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not PlanData other)
                return false;

            return Prescriptions.SequenceEqual(other.Prescriptions) &&
                   ExamRequests.SequenceEqual(other.ExamRequests) &&
                   Procedures.SequenceEqual(other.Procedures) &&
                   Referrals.SequenceEqual(other.Referrals) &&
                   ReturnInstructions == other.ReturnInstructions &&
                   NextAppointmentDate == other.NextAppointmentDate &&
                   PatientInstructions == other.PatientInstructions &&
                   DietaryRecommendations == other.DietaryRecommendations &&
                   ActivityRestrictions == other.ActivityRestrictions &&
                   WarningSymptoms == other.WarningSymptoms;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var prescription in Prescriptions)
                hash.Add(prescription);
            foreach (var exam in ExamRequests)
                hash.Add(exam);
            foreach (var procedure in Procedures)
                hash.Add(procedure);
            foreach (var referral in Referrals)
                hash.Add(referral);
            hash.Add(ReturnInstructions);
            hash.Add(NextAppointmentDate);
            hash.Add(PatientInstructions);
            hash.Add(DietaryRecommendations);
            hash.Add(ActivityRestrictions);
            hash.Add(WarningSymptoms);
            return hash.ToHashCode();
        }
    }
}
