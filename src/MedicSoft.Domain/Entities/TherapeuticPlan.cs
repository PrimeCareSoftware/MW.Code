using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa o plano terapêutico conforme CFM 1.821/2007
    /// </summary>
    public class TherapeuticPlan : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        
        // Navigation properties
        public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
        
        // Conduta/Tratamento (obrigatório)
        public string Treatment { get; private set; }
        
        // Prescrição medicamentosa (se aplicável)
        public string? MedicationPrescription { get; private set; }
        
        // Solicitação de exames (se aplicável)
        public string? ExamRequests { get; private set; }
        
        // Encaminhamentos (se aplicável)
        public string? Referrals { get; private set; }
        
        // Orientações ao paciente (recomendado)
        public string? PatientGuidance { get; private set; }
        
        // Data de retorno (recomendado)
        public DateTime? ReturnDate { get; private set; }
        
        private TherapeuticPlan()
        {
            // EF Constructor
            Treatment = null!;
        }
        
        public TherapeuticPlan(
            Guid medicalRecordId,
            string tenantId,
            string treatment,
            string? medicationPrescription = null,
            string? examRequests = null,
            string? referrals = null,
            string? patientGuidance = null,
            DateTime? returnDate = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (string.IsNullOrWhiteSpace(treatment))
                throw new ArgumentException("Treatment is required", nameof(treatment));
            
            if (treatment.Length < 20)
                throw new ArgumentException("Treatment must have at least 20 characters", nameof(treatment));
            
            if (returnDate.HasValue && returnDate.Value <= DateTime.UtcNow)
                throw new ArgumentException("Return date must be in the future", nameof(returnDate));
            
            MedicalRecordId = medicalRecordId;
            Treatment = treatment.Trim();
            MedicationPrescription = medicationPrescription?.Trim();
            ExamRequests = examRequests?.Trim();
            Referrals = referrals?.Trim();
            PatientGuidance = patientGuidance?.Trim();
            ReturnDate = returnDate;
        }
        
        public void UpdateTreatment(string treatment)
        {
            if (string.IsNullOrWhiteSpace(treatment))
                throw new ArgumentException("Treatment is required", nameof(treatment));
            
            if (treatment.Length < 20)
                throw new ArgumentException("Treatment must have at least 20 characters", nameof(treatment));
            
            Treatment = treatment.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateMedicationPrescription(string? medicationPrescription)
        {
            MedicationPrescription = medicationPrescription?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateExamRequests(string? examRequests)
        {
            ExamRequests = examRequests?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateReferrals(string? referrals)
        {
            Referrals = referrals?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdatePatientGuidance(string? patientGuidance)
        {
            PatientGuidance = patientGuidance?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateReturnDate(DateTime? returnDate)
        {
            if (returnDate.HasValue && returnDate.Value <= DateTime.UtcNow)
                throw new ArgumentException("Return date must be in the future", nameof(returnDate));
            
            ReturnDate = returnDate;
            UpdateTimestamp();
        }
    }
}
