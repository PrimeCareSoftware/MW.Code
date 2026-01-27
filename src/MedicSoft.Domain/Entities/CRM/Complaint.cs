using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Reclamação/Ouvidoria de paciente
    /// </summary>
    public class Complaint : BaseEntity
    {
        public string ProtocolNumber { get; private set; } // Número de protocolo único
        
        public Guid PatientId { get; private set; }
        public Patient Patient { get; private set; } = null!;
        
        public string Subject { get; private set; }
        public string Description { get; private set; }
        
        public ComplaintCategory Category { get; private set; }
        public ComplaintPriority Priority { get; private set; }
        public ComplaintStatus Status { get; private set; }
        
        // Interações/histórico
        private readonly List<ComplaintInteraction> _interactions = new();
        public IReadOnlyCollection<ComplaintInteraction> Interactions => _interactions.AsReadOnly();
        
        // Atribuição
        public Guid? AssignedToUserId { get; private set; }
        public string? AssignedToUserName { get; private set; }
        
        // SLA e métricas
        public DateTime ReceivedAt { get; private set; }
        public DateTime? FirstResponseAt { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        
        public int? ResponseTimeMinutes { get; private set; }
        public int? ResolutionTimeMinutes { get; private set; }
        
        // Satisfação com resolução
        public int? SatisfactionRating { get; private set; } // 1-5
        public string? SatisfactionFeedback { get; private set; }
        
        private Complaint()
        {
            ProtocolNumber = string.Empty;
            Subject = string.Empty;
            Description = string.Empty;
        }
        
        public Complaint(
            string protocolNumber,
            Guid patientId,
            string subject,
            string description,
            ComplaintCategory category,
            string tenantId) : base(tenantId)
        {
            ProtocolNumber = protocolNumber ?? throw new ArgumentNullException(nameof(protocolNumber));
            PatientId = patientId;
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Category = category;
            Status = ComplaintStatus.Received;
            Priority = ComplaintPriority.Medium;
            ReceivedAt = DateTime.UtcNow;
        }
        
        public void AssignTo(Guid userId, string userName)
        {
            AssignedToUserId = userId;
            AssignedToUserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UpdateTimestamp();
        }
        
        public void UpdateStatus(ComplaintStatus newStatus)
        {
            var previousStatus = Status;
            Status = newStatus;
            
            // Track metrics based on status changes
            if (newStatus == ComplaintStatus.InProgress && FirstResponseAt == null)
            {
                FirstResponseAt = DateTime.UtcNow;
                ResponseTimeMinutes = (int)(FirstResponseAt.Value - ReceivedAt).TotalMinutes;
            }
            else if (newStatus == ComplaintStatus.Resolved && ResolvedAt == null)
            {
                ResolvedAt = DateTime.UtcNow;
                ResolutionTimeMinutes = (int)(ResolvedAt.Value - ReceivedAt).TotalMinutes;
            }
            else if (newStatus == ComplaintStatus.Closed && ClosedAt == null)
            {
                ClosedAt = DateTime.UtcNow;
            }
            
            UpdateTimestamp();
        }
        
        public void SetPriority(ComplaintPriority priority)
        {
            Priority = priority;
            UpdateTimestamp();
        }
        
        public void AddInteraction(ComplaintInteraction interaction)
        {
            if (interaction == null)
                throw new ArgumentNullException(nameof(interaction));
                
            _interactions.Add(interaction);
            UpdateTimestamp();
        }
        
        public void RecordSatisfaction(int rating, string? feedback)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5", nameof(rating));
                
            SatisfactionRating = rating;
            SatisfactionFeedback = feedback;
            UpdateTimestamp();
        }
    }
}
