using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Interação/atualização em uma reclamação
    /// </summary>
    public class ComplaintInteraction : BaseEntity
    {
        public Guid ComplaintId { get; private set; }
        public Complaint Complaint { get; private set; } = null!;
        
        public Guid UserId { get; private set; } // Usuário que registrou a interação
        public string UserName { get; private set; }
        
        public string Message { get; private set; }
        public bool IsInternal { get; private set; } // Se true, não visível para paciente
        public DateTime InteractionDate { get; private set; }
        
        private ComplaintInteraction()
        {
            UserName = string.Empty;
            Message = string.Empty;
        }
        
        public ComplaintInteraction(
            Guid complaintId,
            Guid userId,
            string userName,
            string message,
            bool isInternal,
            string tenantId) : base(tenantId)
        {
            ComplaintId = complaintId;
            UserId = userId;
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            IsInternal = isInternal;
            InteractionDate = DateTime.UtcNow;
        }
    }
}
