using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Entidade de auditoria para rastreabilidade de todas as ações no sistema
    /// Compliance com LGPD (Lei 13.709/2018) - Artigo 37
    /// </summary>
    public class AuditLog : BaseEntity
    {
        public DateTime Timestamp { get; private set; }
        
        // Usuário que executou a ação
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserEmail { get; private set; }
        
        // Ação executada
        public AuditAction Action { get; private set; }
        public string ActionDescription { get; private set; }
        
        // Entidade afetada
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public string? EntityDisplayName { get; private set; }
        
        // Dados da requisição
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        public string RequestPath { get; private set; }
        public string HttpMethod { get; private set; }
        
        // Dados antes e depois (para UPDATE)
        public string? OldValues { get; private set; }  // JSON
        public string? NewValues { get; private set; }  // JSON
        public List<string>? ChangedFields { get; private set; }  // Lista de campos alterados
        
        // Resultado da operação
        public OperationResult Result { get; private set; }
        public string? FailureReason { get; private set; }
        public int? StatusCode { get; private set; }
        
        // Categoria LGPD
        public DataCategory DataCategory { get; private set; }
        public LgpdPurpose Purpose { get; private set; }
        
        // Severidade
        public AuditSeverity Severity { get; private set; }

        // Construtor privado para EF Core
        private AuditLog() 
        {
            UserId = null!;
            UserName = null!;
            UserEmail = null!;
            ActionDescription = null!;
            EntityType = null!;
            EntityId = null!;
            IpAddress = null!;
            UserAgent = null!;
            RequestPath = null!;
            HttpMethod = null!;
        }

        public AuditLog(
            string userId,
            string userName,
            string userEmail,
            AuditAction action,
            string actionDescription,
            string entityType,
            string entityId,
            string ipAddress,
            string userAgent,
            string requestPath,
            string httpMethod,
            OperationResult result,
            DataCategory dataCategory,
            LgpdPurpose purpose,
            AuditSeverity severity,
            string tenantId) : base(tenantId)
        {
            Timestamp = DateTime.UtcNow;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            Action = action;
            ActionDescription = actionDescription ?? throw new ArgumentNullException(nameof(actionDescription));
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
            RequestPath = requestPath ?? throw new ArgumentNullException(nameof(requestPath));
            HttpMethod = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            Result = result;
            DataCategory = dataCategory;
            Purpose = purpose;
            Severity = severity;
        }

        public void SetEntityDisplayName(string displayName)
        {
            EntityDisplayName = displayName;
        }

        public void SetOldValues(string oldValues)
        {
            OldValues = oldValues;
        }

        public void SetNewValues(string newValues)
        {
            NewValues = newValues;
        }

        public void SetChangedFields(List<string> changedFields)
        {
            ChangedFields = changedFields;
        }

        public void SetFailureReason(string failureReason)
        {
            FailureReason = failureReason;
        }

        public void SetStatusCode(int statusCode)
        {
            StatusCode = statusCode;
        }
    }
}
