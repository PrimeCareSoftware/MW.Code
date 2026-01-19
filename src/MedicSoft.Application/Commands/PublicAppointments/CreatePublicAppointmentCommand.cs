using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.PublicAppointments
{
    /// <summary>
    /// Comando para criar um agendamento público (sem autenticação).
    /// Este comando pode criar um novo paciente se ele não existir,
    /// ou usar um paciente existente baseado no CPF fornecido.
    /// </summary>
    public class CreatePublicAppointmentCommand : IRequest<PublicAppointmentResponseDto>
    {
        public PublicAppointmentRequestDto Request { get; }

        public CreatePublicAppointmentCommand(PublicAppointmentRequestDto request)
        {
            Request = request;
        }
    }
}
