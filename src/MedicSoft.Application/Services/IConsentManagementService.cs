using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    public interface IConsentManagementService
    {
        /// <summary>
        /// Registra um novo consentimento do paciente
        /// </summary>
        Task<Guid> RecordConsentAsync(
            Guid patientId,
            string patientName,
            ConsentType type,
            ConsentPurpose purpose,
            string description,
            DateTime? expirationDate,
            string ipAddress,
            string consentText,
            string consentVersion,
            string consentMethod,
            string userAgent,
            string tenantId);

        /// <summary>
        /// Revoga um consentimento existente
        /// </summary>
        Task RevokeConsentAsync(Guid consentId, string reason, string tenantId);

        /// <summary>
        /// Verifica se o paciente tem consentimento ativo para determinada finalidade
        /// </summary>
        Task<bool> HasActiveConsentAsync(Guid patientId, ConsentPurpose purpose, string tenantId);

        /// <summary>
        /// Obtém todos os consentimentos de um paciente
        /// </summary>
        Task<List<DataConsentLog>> GetPatientConsentsAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Obtém consentimentos ativos de um paciente
        /// </summary>
        Task<List<DataConsentLog>> GetActivePatientConsentsAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Processa consentimentos expirados
        /// </summary>
        Task ProcessExpiredConsentsAsync(string tenantId);
    }
}
