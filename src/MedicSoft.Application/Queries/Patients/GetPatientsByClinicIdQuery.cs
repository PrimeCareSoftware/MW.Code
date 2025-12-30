using System;
using System.Collections.Generic;
using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class GetPatientsByClinicIdQuery : IRequest<IEnumerable<PatientDto>>
    {
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetPatientsByClinicIdQuery(Guid clinicId, string tenantId)
        {
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }
}
