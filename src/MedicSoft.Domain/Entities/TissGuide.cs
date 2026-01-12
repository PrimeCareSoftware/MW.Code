using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa uma guia TISS de atendimento
    /// </summary>
    public class TissGuide : BaseEntity
    {
        public Guid TissBatchId { get; private set; }
        public Guid AppointmentId { get; private set; }
        public Guid PatientHealthInsuranceId { get; private set; }
        public string GuideNumber { get; private set; } // Número sequencial da guia
        public TissGuideType GuideType { get; private set; }
        public DateTime ServiceDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public GuideStatus Status { get; private set; }
        
        // Autorização (se aplicável)
        public string? AuthorizationNumber { get; private set; }
        
        // Retorno da operadora
        public decimal? ApprovedAmount { get; private set; }
        public decimal? GlosedAmount { get; private set; }
        public string? GlossReason { get; private set; }
        
        // Navigation properties
        public TissBatch? TissBatch { get; private set; }
        public Appointment? Appointment { get; private set; }
        public PatientHealthInsurance? PatientHealthInsurance { get; private set; }
        private readonly List<TissGuideProcedure> _procedures = new();
        public IReadOnlyCollection<TissGuideProcedure> Procedures => _procedures.AsReadOnly();
        
        private TissGuide() 
        { 
            // EF Constructor
            GuideNumber = null!;
        }

        public TissGuide(
            Guid tissBatchId,
            Guid appointmentId,
            Guid patientHealthInsuranceId,
            string guideNumber,
            TissGuideType guideType,
            DateTime serviceDate,
            string tenantId,
            string? authorizationNumber = null) : base(tenantId)
        {
            if (tissBatchId == Guid.Empty)
                throw new ArgumentException("TISS Batch ID cannot be empty", nameof(tissBatchId));
            
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
            
            if (patientHealthInsuranceId == Guid.Empty)
                throw new ArgumentException("Patient Health Insurance ID cannot be empty", nameof(patientHealthInsuranceId));
            
            if (string.IsNullOrWhiteSpace(guideNumber))
                throw new ArgumentException("Guide number cannot be empty", nameof(guideNumber));

            TissBatchId = tissBatchId;
            AppointmentId = appointmentId;
            PatientHealthInsuranceId = patientHealthInsuranceId;
            GuideNumber = guideNumber.Trim();
            GuideType = guideType;
            ServiceDate = serviceDate;
            Status = GuideStatus.Draft;
            AuthorizationNumber = authorizationNumber?.Trim();
            TotalAmount = 0; // Will be calculated when procedures are added
        }

        public void AddProcedure(TissGuideProcedure procedure)
        {
            if (procedure == null)
                throw new ArgumentNullException(nameof(procedure));

            if (Status != GuideStatus.Draft)
                throw new InvalidOperationException($"Cannot add procedures to guide in status {Status}");

            if (_procedures.Any(p => p.Id == procedure.Id))
                throw new InvalidOperationException("Procedure is already in this guide");

            _procedures.Add(procedure);
            RecalculateTotalAmount();
            UpdateTimestamp();
        }

        public void RemoveProcedure(Guid procedureId)
        {
            if (Status != GuideStatus.Draft)
                throw new InvalidOperationException($"Cannot remove procedures from guide in status {Status}");

            var procedure = _procedures.FirstOrDefault(p => p.Id == procedureId);
            if (procedure != null)
            {
                _procedures.Remove(procedure);
                RecalculateTotalAmount();
                UpdateTimestamp();
            }
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _procedures.Sum(p => p.TotalPrice);
        }

        public void MarkAsSent()
        {
            if (Status != GuideStatus.Draft)
                throw new InvalidOperationException($"Cannot mark guide as sent in status {Status}");

            if (!_procedures.Any())
                throw new InvalidOperationException("Cannot send guide without procedures");

            Status = GuideStatus.Sent;
            UpdateTimestamp();
        }

        public void Approve(decimal approvedAmount)
        {
            if (Status != GuideStatus.Sent)
                throw new InvalidOperationException($"Cannot approve guide in status {Status}");

            if (approvedAmount < 0)
                throw new ArgumentException("Approved amount cannot be negative", nameof(approvedAmount));

            ApprovedAmount = approvedAmount;
            GlosedAmount = TotalAmount - approvedAmount;
            
            Status = approvedAmount == TotalAmount 
                ? GuideStatus.Approved 
                : GuideStatus.PartiallyApproved;
            
            UpdateTimestamp();
        }

        public void Reject(string glossReason)
        {
            if (string.IsNullOrWhiteSpace(glossReason))
                throw new ArgumentException("Gloss reason cannot be empty", nameof(glossReason));

            if (Status != GuideStatus.Sent)
                throw new InvalidOperationException($"Cannot reject guide in status {Status}");

            GlossReason = glossReason.Trim();
            ApprovedAmount = 0;
            GlosedAmount = TotalAmount;
            Status = GuideStatus.Rejected;
            UpdateTimestamp();
        }

        public void MarkAsPaid()
        {
            if (Status != GuideStatus.Approved && Status != GuideStatus.PartiallyApproved)
                throw new InvalidOperationException($"Cannot mark guide as paid in status {Status}");

            Status = GuideStatus.Paid;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Tipo de guia TISS
    /// </summary>
    public enum TissGuideType
    {
        Consultation = 1,     // Consulta médica
        SPSADT = 2,          // Serviços Profissionais / SADT (exames)
        Hospitalization = 3, // Internação
        Fees = 4,            // Honorários médicos
        Dental = 5           // Odontológico
    }

    /// <summary>
    /// Status da guia TISS
    /// </summary>
    public enum GuideStatus
    {
        Draft = 1,              // Rascunho
        Sent = 2,               // Enviada à operadora
        Approved = 3,           // Aprovada integralmente
        PartiallyApproved = 4,  // Parcialmente aprovada (com glosas)
        Rejected = 5,           // Rejeitada
        Paid = 6                // Paga
    }
}
