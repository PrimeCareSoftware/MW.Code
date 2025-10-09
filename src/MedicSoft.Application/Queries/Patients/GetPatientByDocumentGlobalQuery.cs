using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class GetPatientByDocumentGlobalQuery : IRequest<PatientDto?>
    {
        public string Document { get; }

        public GetPatientByDocumentGlobalQuery(string document)
        {
            Document = document;
        }
    }
}
