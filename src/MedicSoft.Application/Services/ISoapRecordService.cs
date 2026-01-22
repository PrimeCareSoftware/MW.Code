using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.SoapRecords;

namespace MedicSoft.Application.Services
{
    public interface ISoapRecordService
    {
        Task<SoapRecordDto> CreateSoapRecord(Guid appointmentId, string tenantId);
        Task<SoapRecordDto> UpdateSubjective(Guid soapId, UpdateSubjectiveDto data, string tenantId);
        Task<SoapRecordDto> UpdateObjective(Guid soapId, UpdateObjectiveDto data, string tenantId);
        Task<SoapRecordDto> UpdateAssessment(Guid soapId, UpdateAssessmentDto data, string tenantId);
        Task<SoapRecordDto> UpdatePlan(Guid soapId, UpdatePlanDto data, string tenantId);
        Task<SoapRecordDto> CompleteSoapRecord(Guid soapId, string tenantId);
        Task<SoapRecordDto?> GetBySoapId(Guid soapId, string tenantId);
        Task<SoapRecordDto?> GetByAppointmentId(Guid appointmentId, string tenantId);
        Task<IEnumerable<SoapRecordDto>> GetByPatientId(Guid patientId, string tenantId);
        Task<IEnumerable<SoapRecordDto>> GetByDoctorId(Guid doctorId, string tenantId);
        Task<SoapRecordValidationDto> ValidateCompleteness(Guid soapId, string tenantId);
        Task UnlockSoapRecord(Guid soapId, string tenantId);
    }
}
