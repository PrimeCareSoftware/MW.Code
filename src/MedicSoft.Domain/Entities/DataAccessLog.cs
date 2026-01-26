using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Log específico para acesso a dados sensíveis (LGPD Art. 37)
    /// Rastreia todos os acessos a dados pessoais e sensíveis de pacientes
    /// </summary>
    public class DataAccessLog : BaseEntity
    {
        // Quem acessou
        public string UserId { get; private set; }
        public string UserName { get; private set; }
        public string UserRole { get; private set; }
        
        // O que foi acessado
        public string EntityType { get; private set; }
        public string EntityId { get; private set; }
        public List<string> FieldsAccessed { get; private set; }
        
        // Paciente titular dos dados
        public Guid? PatientId { get; private set; }
        public string PatientName { get; private set; }
        
        // Contexto
        public DateTime Timestamp { get; private set; }
        public string AccessReason { get; private set; }
        public string IpAddress { get; private set; }
        public string Location { get; private set; }
        
        // Resultado
        public bool WasAuthorized { get; private set; }
        public string? DenialReason { get; private set; }

        // Construtor privado para EF Core
        private DataAccessLog()
        {
            UserId = null!;
            UserName = null!;
            UserRole = null!;
            EntityType = null!;
            EntityId = null!;
            FieldsAccessed = null!;
            PatientName = null!;
            AccessReason = null!;
            IpAddress = null!;
            Location = null!;
        }

        public DataAccessLog(
            string userId,
            string userName,
            string userRole,
            string entityType,
            string entityId,
            List<string> fieldsAccessed,
            Guid? patientId,
            string patientName,
            string accessReason,
            string ipAddress,
            string location,
            bool wasAuthorized,
            string tenantId) : base(tenantId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            UserRole = userRole ?? throw new ArgumentNullException(nameof(userRole));
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            EntityId = entityId ?? throw new ArgumentNullException(nameof(entityId));
            FieldsAccessed = fieldsAccessed ?? new List<string>();
            PatientId = patientId;
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
            AccessReason = accessReason ?? throw new ArgumentNullException(nameof(accessReason));
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            WasAuthorized = wasAuthorized;
            Timestamp = DateTime.UtcNow;
        }

        public void SetDenialReason(string denialReason)
        {
            DenialReason = denialReason;
        }
    }
}
