using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class MedicalRecordAuditService : IMedicalRecordAuditService
    {
        private readonly IMedicalRecordAccessLogRepository _accessLogRepository;

        public MedicalRecordAuditService(IMedicalRecordAccessLogRepository accessLogRepository)
        {
            _accessLogRepository = accessLogRepository;
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
                // Log error but don't fail the request
                // In production, this should use proper logging
                Console.WriteLine($"Error logging access: {ex.Message}");
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
