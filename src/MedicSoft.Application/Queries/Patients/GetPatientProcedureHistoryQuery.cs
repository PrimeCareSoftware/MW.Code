using System;
using MediatR;
using System.Collections.Generic;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    /// <summary>
    /// Query to get procedure history for a specific patient
    /// </summary>
    public record GetPatientProcedureHistoryQuery(
        Guid PatientId,
        string TenantId
    ) : IRequest<IEnumerable<PatientProcedureHistoryDto>>;
}
