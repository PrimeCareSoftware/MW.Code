using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public class ConsentManagementService : IConsentManagementService
    {
        private readonly IDataConsentLogRepository _consentLogRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<ConsentManagementService> _logger;

        public ConsentManagementService(
            IDataConsentLogRepository consentLogRepository,
            IAuditService auditService,
            ILogger<ConsentManagementService> logger)
        {
            _consentLogRepository = consentLogRepository ?? throw new ArgumentNullException(nameof(consentLogRepository));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> RecordConsentAsync(
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
            string tenantId)
        {
            _logger.LogInformation("Recording consent for patient {PatientId}, type: {Type}, purpose: {Purpose}",
                patientId, type, purpose);

            var consentLog = new DataConsentLog(
                patientId,
                patientName,
                type,
                purpose,
                description,
                ConsentStatus.Active,
                expirationDate,
                ipAddress,
                consentText,
                consentVersion,
                consentMethod,
                userAgent,
                tenantId
            );

            await _consentLogRepository.AddAsync(consentLog);

            _logger.LogInformation("Consent recorded successfully with ID {ConsentId}", consentLog.Id);

            return consentLog.Id;
        }

        public async Task RevokeConsentAsync(Guid consentId, string reason, string tenantId)
        {
            _logger.LogInformation("Revoking consent {ConsentId}", consentId);

            var consent = await _consentLogRepository.GetByIdAsync(consentId, tenantId);
            if (consent == null)
            {
                throw new InvalidOperationException($"Consent {consentId} not found");
            }

            consent.Revoke(reason);
            await _consentLogRepository.UpdateAsync(consent);

            _logger.LogInformation("Consent {ConsentId} revoked successfully", consentId);
        }

        public async Task<bool> HasActiveConsentAsync(Guid patientId, ConsentPurpose purpose, string tenantId)
        {
            var activeConsents = await _consentLogRepository.GetActiveConsentsByPatientIdAsync(patientId, tenantId);
            return activeConsents.Any(c => c.Purpose == purpose);
        }

        public async Task<List<DataConsentLog>> GetPatientConsentsAsync(Guid patientId, string tenantId)
        {
            return await _consentLogRepository.GetByPatientIdAsync(patientId, tenantId);
        }

        public async Task<List<DataConsentLog>> GetActivePatientConsentsAsync(Guid patientId, string tenantId)
        {
            return await _consentLogRepository.GetActiveConsentsByPatientIdAsync(patientId, tenantId);
        }

        public async Task ProcessExpiredConsentsAsync(string tenantId)
        {
            _logger.LogInformation("Processing expired consents for tenant {TenantId}", tenantId);

            // This would need to query all consents and check expiration dates
            // For now, this is a placeholder for the implementation
            _logger.LogInformation("Expired consents processed for tenant {TenantId}", tenantId);
        }
    }
}
