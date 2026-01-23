using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Commands.Anamnesis
{
    public class CreateAnamnesisResponseCommand : IRequest<AnamnesisResponseDto>
    {
        public Guid AppointmentId { get; }
        public Guid TemplateId { get; }
        public Guid PatientId { get; }
        public Guid DoctorId { get; }
        public string TenantId { get; }

        public CreateAnamnesisResponseCommand(
            Guid appointmentId, 
            Guid templateId, 
            Guid patientId, 
            Guid doctorId, 
            string tenantId)
        {
            AppointmentId = appointmentId;
            TemplateId = templateId;
            PatientId = patientId;
            DoctorId = doctorId;
            TenantId = tenantId;
        }
    }
}
