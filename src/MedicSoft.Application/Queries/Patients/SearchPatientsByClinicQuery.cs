using System;
using System.Collections.Generic;
using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class SearchPatientsByClinicQuery : IRequest<IEnumerable<PatientDto>>
    {
        public string SearchTerm { get; }
        public string TenantId { get; }
        public Guid ClinicId { get; }

        public SearchPatientsByClinicQuery(string searchTerm, string tenantId, Guid clinicId)
        {
            SearchTerm = searchTerm;
            TenantId = tenantId;
            ClinicId = clinicId;
        }
    }
}
