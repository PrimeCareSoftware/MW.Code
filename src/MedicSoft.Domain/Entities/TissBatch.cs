using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um lote de faturamento TISS para uma operadora
    /// </summary>
    public class TissBatch : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid OperatorId { get; private set; }
        public string BatchNumber { get; private set; } // Número sequencial do lote
        public DateTime CreatedDate { get; private set; }
        public DateTime? SubmittedDate { get; private set; }
        public DateTime? ProcessedDate { get; private set; }
        public BatchStatus Status { get; private set; }
        
        // Arquivo XML
        public string? XmlFileName { get; private set; }
        public string? XmlFilePath { get; private set; }
        
        // Retorno da operadora
        public string? ProtocolNumber { get; private set; } // Protocolo de recebimento
        public string? ResponseXmlFileName { get; private set; }
        public decimal? ApprovedAmount { get; private set; }
        public decimal? GlosedAmount { get; private set; } // Valor glosado (rejeitado)
        
        // Navigation properties
        public Clinic? Clinic { get; private set; }
        public HealthInsuranceOperator? Operator { get; private set; }
        private readonly List<TissGuide> _guides = new();
        public IReadOnlyCollection<TissGuide> Guides => _guides.AsReadOnly();
        
        private TissBatch() 
        { 
            // EF Constructor
            BatchNumber = null!;
        }

        public TissBatch(
            Guid clinicId,
            Guid operatorId,
            string batchNumber,
            string tenantId) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));
            
            if (operatorId == Guid.Empty)
                throw new ArgumentException("Operator ID cannot be empty", nameof(operatorId));
            
            if (string.IsNullOrWhiteSpace(batchNumber))
                throw new ArgumentException("Batch number cannot be empty", nameof(batchNumber));

            ClinicId = clinicId;
            OperatorId = operatorId;
            BatchNumber = batchNumber.Trim();
            CreatedDate = DateTime.UtcNow;
            Status = BatchStatus.Draft;
        }

        public void AddGuide(TissGuide guide)
        {
            if (guide == null)
                throw new ArgumentNullException(nameof(guide));

            if (Status != BatchStatus.Draft && Status != BatchStatus.ReadyToSend)
                throw new InvalidOperationException($"Cannot add guides to batch in status {Status}");

            if (_guides.Any(g => g.Id == guide.Id))
                throw new InvalidOperationException("Guide is already in this batch");

            _guides.Add(guide);
            UpdateTimestamp();
        }

        public void RemoveGuide(Guid guideId)
        {
            if (Status != BatchStatus.Draft && Status != BatchStatus.ReadyToSend)
                throw new InvalidOperationException($"Cannot remove guides from batch in status {Status}");

            var guide = _guides.FirstOrDefault(g => g.Id == guideId);
            if (guide != null)
            {
                _guides.Remove(guide);
                UpdateTimestamp();
            }
        }

        public void MarkAsReadyToSend()
        {
            if (Status != BatchStatus.Draft)
                throw new InvalidOperationException($"Cannot mark batch as ready in status {Status}");

            if (!_guides.Any())
                throw new InvalidOperationException("Cannot mark batch as ready without guides");

            Status = BatchStatus.ReadyToSend;
            UpdateTimestamp();
        }

        public void GenerateXml(string xmlFileName, string xmlFilePath)
        {
            if (string.IsNullOrWhiteSpace(xmlFileName))
                throw new ArgumentException("XML file name cannot be empty", nameof(xmlFileName));
            
            if (string.IsNullOrWhiteSpace(xmlFilePath))
                throw new ArgumentException("XML file path cannot be empty", nameof(xmlFilePath));

            if (Status != BatchStatus.ReadyToSend && Status != BatchStatus.Draft)
                throw new InvalidOperationException($"Cannot generate XML for batch in status {Status}");

            XmlFileName = xmlFileName.Trim();
            XmlFilePath = xmlFilePath.Trim();
            Status = BatchStatus.ReadyToSend;
            UpdateTimestamp();
        }

        public void Submit(string? protocolNumber = null)
        {
            if (Status != BatchStatus.ReadyToSend)
                throw new InvalidOperationException($"Cannot submit batch in status {Status}");

            if (string.IsNullOrWhiteSpace(XmlFileName))
                throw new InvalidOperationException("Cannot submit batch without generated XML");

            SubmittedDate = DateTime.UtcNow;
            ProtocolNumber = protocolNumber?.Trim();
            Status = BatchStatus.Sent;
            UpdateTimestamp();
        }

        public void MarkAsProcessing()
        {
            if (Status != BatchStatus.Sent)
                throw new InvalidOperationException($"Cannot mark batch as processing in status {Status}");

            Status = BatchStatus.Processing;
            UpdateTimestamp();
        }

        public void ProcessResponse(string? responseXmlFileName, decimal? approvedAmount, decimal? glosedAmount)
        {
            if (Status != BatchStatus.Processing && Status != BatchStatus.Sent)
                throw new InvalidOperationException($"Cannot process response for batch in status {Status}");

            ResponseXmlFileName = responseXmlFileName?.Trim();
            ApprovedAmount = approvedAmount;
            GlosedAmount = glosedAmount;
            ProcessedDate = DateTime.UtcNow;
            
            // Determina o status final baseado nos valores
            if (approvedAmount.HasValue && approvedAmount.Value > 0)
            {
                Status = glosedAmount.HasValue && glosedAmount.Value > 0 
                    ? BatchStatus.PartiallyPaid 
                    : BatchStatus.Processed;
            }
            else
            {
                Status = BatchStatus.Rejected;
            }
            
            UpdateTimestamp();
        }

        public void MarkAsPaid()
        {
            if (Status != BatchStatus.Processed && Status != BatchStatus.PartiallyPaid)
                throw new InvalidOperationException($"Cannot mark batch as paid in status {Status}");

            Status = BatchStatus.Paid;
            UpdateTimestamp();
        }

        public void Reject()
        {
            if (Status == BatchStatus.Draft || Status == BatchStatus.ReadyToSend)
                throw new InvalidOperationException($"Cannot reject batch in status {Status}");

            Status = BatchStatus.Rejected;
            UpdateTimestamp();
        }

        public decimal GetTotalAmount()
        {
            return _guides.Sum(g => g.TotalAmount);
        }

        public int GetGuideCount()
        {
            return _guides.Count;
        }
    }

    /// <summary>
    /// Status do lote de faturamento
    /// </summary>
    public enum BatchStatus
    {
        Draft = 1,           // Em elaboração
        ReadyToSend = 2,     // Pronto para enviar
        Sent = 3,            // Enviado à operadora
        Processing = 4,      // Em processamento pela operadora
        Processed = 5,       // Processado pela operadora
        PartiallyPaid = 6,   // Parcialmente pago (com glosas)
        Paid = 7,            // Pago integralmente
        Rejected = 8         // Rejeitado pela operadora
    }
}
