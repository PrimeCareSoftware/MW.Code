using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class MedicalRecordAuditService : IMedicalRecordAuditService
    {
        private readonly IMedicalRecordAccessLogRepository _accessLogRepository;
        private readonly ILogger<MedicalRecordAuditService> _logger;

        public MedicalRecordAuditService(
            IMedicalRecordAccessLogRepository accessLogRepository,
            ILogger<MedicalRecordAuditService> logger)
        {
            _accessLogRepository = accessLogRepository;
            _logger = logger;
        }

        public async Task LogAccessAsync(
            Guid recordId, 
            Guid userId, 
            string accessType, 
            string tenantId,
            string? ipAddress = null, 
            string? userAgent = null, 
            string? details = null)
        {
            try
            {
                var log = new MedicalRecordAccessLog(
                    medicalRecordId: recordId,
                    userId: userId,
                    accessType: accessType,
                    tenantId: tenantId,
                    ipAddress: ipAddress,
                    userAgent: userAgent,
                    details: details
                );

                await _accessLogRepository.CreateAsync(log);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request (audit failure should not block medical operations)
                _logger.LogError(ex, "Error logging access for medical record {MedicalRecordId} by user {UserId}", recordId, userId);
            }
        }

        public async Task<List<MedicalRecordAccessLog>> GetAccessLogsAsync(
            Guid recordId, 
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            return await _accessLogRepository.GetAccessLogsAsync(recordId, tenantId, startDate, endDate);
        }

        public async Task<List<MedicalRecordAccessLog>> GetUserAccessLogsAsync(
            Guid userId, 
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            return await _accessLogRepository.GetUserAccessLogsAsync(userId, tenantId, startDate, endDate);
        }
    }
}
