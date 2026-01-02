using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        
        // CFM 1.821 - Campos obrigatórios de Anamnese
        public string ChiefComplaint { get; private set; } // Queixa principal
        public string HistoryOfPresentIllness { get; private set; } // História da doença atual (HDA)
        
        // CFM 1.821 - Campos recomendados de História Clínica
        public string? PastMedicalHistory { get; private set; } // História patológica pregressa (HPP)
        public string? FamilyHistory { get; private set; } // História familiar
        public string? LifestyleHabits { get; private set; } // Hábitos de vida
        public string? CurrentMedications { get; private set; } // Medicações em uso
        
        // Campos existentes (manter compatibilidade)
        public string Diagnosis { get; private set; } // DEPRECATED: usar DiagnosticHypothesis
        public string Prescription { get; private set; } // DEPRECATED: usar TherapeuticPlan
        public string Notes { get; private set; }
        
        public int ConsultationDurationMinutes { get; private set; }
        public DateTime ConsultationStartTime { get; private set; }
        public DateTime? ConsultationEndTime { get; private set; }
        
        // Controle de fechamento (CFM 1.821 - Auditoria)
        public bool IsClosed { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public Guid? ClosedByUserId { get; private set; }

        // Navigation properties
        public Appointment Appointment { get; private set; } = null!;
        public Patient Patient { get; private set; } = null!;
        
        // Navigation property for prescription items
        private readonly List<PrescriptionItem> _prescriptionItems = new();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();
        
        // CFM 1.821 - Relacionamentos com novas entidades
        private readonly List<ClinicalExamination> _examinations = new();
        public IReadOnlyCollection<ClinicalExamination> Examinations => _examinations.AsReadOnly();
        
        private readonly List<DiagnosticHypothesis> _diagnoses = new();
        public IReadOnlyCollection<DiagnosticHypothesis> Diagnoses => _diagnoses.AsReadOnly();
        
        private readonly List<TherapeuticPlan> _plans = new();
        public IReadOnlyCollection<TherapeuticPlan> Plans => _plans.AsReadOnly();
        
        private readonly List<InformedConsent> _consents = new();
        public IReadOnlyCollection<InformedConsent> Consents => _consents.AsReadOnly();

        private MedicalRecord() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            ChiefComplaint = null!;
            HistoryOfPresentIllness = null!;
            Diagnosis = null!;
            Prescription = null!;
            Notes = null!;
        }

        public MedicalRecord(Guid appointmentId, Guid patientId, string tenantId, 
            DateTime consultationStartTime,
            string chiefComplaint,
            string historyOfPresentIllness,
            string? diagnosis = null, 
            string? prescription = null, 
            string? notes = null,
            string? pastMedicalHistory = null,
            string? familyHistory = null,
            string? lifestyleHabits = null,
            string? currentMedications = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            // CFM 1.821 - Validações obrigatórias
            if (string.IsNullOrWhiteSpace(chiefComplaint))
                throw new ArgumentException("Chief complaint is required (CFM 1.821)", nameof(chiefComplaint));
            
            if (chiefComplaint.Length < 10)
                throw new ArgumentException("Chief complaint must have at least 10 characters", nameof(chiefComplaint));
            
            if (string.IsNullOrWhiteSpace(historyOfPresentIllness))
                throw new ArgumentException("History of present illness is required (CFM 1.821)", nameof(historyOfPresentIllness));
            
            if (historyOfPresentIllness.Length < 50)
                throw new ArgumentException("History of present illness must have at least 50 characters", nameof(historyOfPresentIllness));

            AppointmentId = appointmentId;
            PatientId = patientId;
            ConsultationStartTime = consultationStartTime;
            
            // CFM 1.821 - Campos obrigatórios
            ChiefComplaint = chiefComplaint.Trim();
            HistoryOfPresentIllness = historyOfPresentIllness.Trim();
            
            // CFM 1.821 - Campos recomendados
            PastMedicalHistory = pastMedicalHistory?.Trim();
            FamilyHistory = familyHistory?.Trim();
            LifestyleHabits = lifestyleHabits?.Trim();
            CurrentMedications = currentMedications?.Trim();
            
            // Campos legados (manter compatibilidade)
            Diagnosis = diagnosis?.Trim() ?? string.Empty;
            Prescription = prescription?.Trim() ?? string.Empty;
            Notes = notes?.Trim() ?? string.Empty;
            
            ConsultationDurationMinutes = 0;
            IsClosed = false;
        }

        public void UpdateDiagnosis(string? diagnosis)
        {
            Diagnosis = diagnosis?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }

        public void UpdatePrescription(string? prescription)
        {
            Prescription = prescription?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }
        
        // CFM 1.821 - Métodos para atualizar campos obrigatórios
        public void UpdateChiefComplaint(string chiefComplaint)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            if (string.IsNullOrWhiteSpace(chiefComplaint))
                throw new ArgumentException("Chief complaint is required", nameof(chiefComplaint));
            
            if (chiefComplaint.Length < 10)
                throw new ArgumentException("Chief complaint must have at least 10 characters", nameof(chiefComplaint));
            
            ChiefComplaint = chiefComplaint.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateHistoryOfPresentIllness(string historyOfPresentIllness)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            if (string.IsNullOrWhiteSpace(historyOfPresentIllness))
                throw new ArgumentException("History of present illness is required", nameof(historyOfPresentIllness));
            
            if (historyOfPresentIllness.Length < 50)
                throw new ArgumentException("History of present illness must have at least 50 characters", nameof(historyOfPresentIllness));
            
            HistoryOfPresentIllness = historyOfPresentIllness.Trim();
            UpdateTimestamp();
        }
        
        public void UpdatePastMedicalHistory(string? pastMedicalHistory)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            PastMedicalHistory = pastMedicalHistory?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateFamilyHistory(string? familyHistory)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            FamilyHistory = familyHistory?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateLifestyleHabits(string? lifestyleHabits)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            LifestyleHabits = lifestyleHabits?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateCurrentMedications(string? currentMedications)
        {
            if (IsClosed)
                throw new InvalidOperationException("Cannot update a closed medical record");
            
            CurrentMedications = currentMedications?.Trim();
            UpdateTimestamp();
        }

        public void CompleteConsultation(string? diagnosis = null, string? prescription = null, string? notes = null)
        {
            ConsultationEndTime = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(diagnosis))
                Diagnosis = diagnosis.Trim();
            
            if (!string.IsNullOrWhiteSpace(prescription))
                Prescription = prescription.Trim();
            
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes.Trim();

            if (ConsultationEndTime.HasValue)
            {
                var duration = ConsultationEndTime.Value - ConsultationStartTime;
                ConsultationDurationMinutes = (int)duration.TotalMinutes;
            }

            UpdateTimestamp();
        }
        
        // CFM 1.821 - Método para fechar prontuário (impedir alterações)
        public void CloseMedicalRecord(Guid closedByUserId)
        {
            if (IsClosed)
                throw new InvalidOperationException("Medical record is already closed");
            
            // Validar se tem os dados mínimos obrigatórios
            ValidateRequiredFields();
            
            IsClosed = true;
            ClosedAt = DateTime.UtcNow;
            ClosedByUserId = closedByUserId;
            
            // Fechar consulta se ainda não foi fechada
            if (!ConsultationEndTime.HasValue)
            {
                ConsultationEndTime = DateTime.UtcNow;
                var duration = ConsultationEndTime.Value - ConsultationStartTime;
                ConsultationDurationMinutes = (int)duration.TotalMinutes;
            }
            
            UpdateTimestamp();
        }
        
        public void ReopenMedicalRecord()
        {
            if (!IsClosed)
                throw new InvalidOperationException("Medical record is not closed");
            
            IsClosed = false;
            ClosedAt = null;
            ClosedByUserId = null;
            
            UpdateTimestamp();
        }
        
        private void ValidateRequiredFields()
        {
            var errors = new List<string>();
            
            if (string.IsNullOrWhiteSpace(ChiefComplaint) || ChiefComplaint.Length < 10)
                errors.Add("Chief complaint is required and must have at least 10 characters");
            
            if (string.IsNullOrWhiteSpace(HistoryOfPresentIllness) || HistoryOfPresentIllness.Length < 50)
                errors.Add("History of present illness is required and must have at least 50 characters");
            
            if (!_examinations.Any())
                errors.Add("At least one clinical examination is required");
            
            if (!_diagnoses.Any())
                errors.Add("At least one diagnostic hypothesis is required");
            
            if (!_plans.Any())
                errors.Add("At least one therapeutic plan is required");
            
            if (errors.Any())
                throw new InvalidOperationException($"Cannot close medical record with missing required fields: {string.Join(", ", errors)}");
        }

        public void UpdateConsultationTime(int durationMinutes)
        {
            if (durationMinutes < 0)
                throw new ArgumentException("Duration cannot be negative", nameof(durationMinutes));

            ConsultationDurationMinutes = durationMinutes;
            UpdateTimestamp();
        }
    }
}
