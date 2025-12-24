using System;
using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    /// <summary>
    /// Query to get appointment history for a specific patient
    /// </summary>
    public record GetPatientAppointmentHistoryQuery(
        Guid PatientId,
        string TenantId,
        bool IncludeMedicalRecords = false
    ) : IRequest<PatientCompleteHistoryDto>;
}
