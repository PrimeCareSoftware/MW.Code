using MediatR;
using MedicSoft.Application.Commands.PublicAppointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Handlers.Commands.PublicAppointments
{
    /// <summary>
    /// Handler para criar um agendamento público (sem autenticação).
    /// Cria ou encontra o paciente pelo CPF e agenda a consulta na clínica solicitada.
    /// Note: IBusinessConfigurationRepository is required (not optional) for public appointments
    /// to ensure business rules are always validated for unauthenticated users.
    /// </summary>
    public class CreatePublicAppointmentCommandHandler : IRequestHandler<CreatePublicAppointmentCommand, PublicAppointmentResponseDto>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientClinicLinkRepository _patientClinicLinkRepository;
        private readonly IBusinessConfigurationRepository _businessConfigurationRepository;

        public CreatePublicAppointmentCommandHandler(
            IClinicRepository clinicRepository,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IPatientClinicLinkRepository patientClinicLinkRepository,
            IBusinessConfigurationRepository businessConfigurationRepository)
        {
            _clinicRepository = clinicRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _patientClinicLinkRepository = patientClinicLinkRepository;
            _businessConfigurationRepository = businessConfigurationRepository;
        }

        public async Task<PublicAppointmentResponseDto> Handle(CreatePublicAppointmentCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Request;

            // Validações básicas
            if (dto.ScheduledDate < DateTime.Today)
                throw new InvalidOperationException("A data do agendamento não pode estar no passado.");

            if (string.IsNullOrWhiteSpace(dto.PatientName))
                throw new InvalidOperationException("O nome do paciente é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.PatientCpf))
                throw new InvalidOperationException("O CPF do paciente é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.PatientEmail))
                throw new InvalidOperationException("O e-mail do paciente é obrigatório.");

            if (string.IsNullOrWhiteSpace(dto.PatientPhone))
                throw new InvalidOperationException("O telefone do paciente é obrigatório.");

            // Busca a clínica (sem filtro de tenant, é público)
            var clinic = await _clinicRepository.GetByIdWithoutTenantAsync(dto.ClinicId);
            if (clinic == null)
                throw new InvalidOperationException("Clínica não encontrada.");

            if (!clinic.IsActive)
                throw new InvalidOperationException("Esta clínica não está aceitando agendamentos no momento.");

            // Check if online booking is enabled for this clinic
            var businessConfig = await _businessConfigurationRepository.GetByClinicIdAsync(dto.ClinicId, clinic.TenantId);
            
            // If business configuration doesn't exist yet, assume online booking should be allowed
            // This handles the case of newly created clinics that haven't set up their configuration
            if (businessConfig != null && !businessConfig.OnlineBooking)
                throw new InvalidOperationException("Agendamento online não está disponível para esta clínica.");
            
            // Check if the clinic has online scheduling enabled (clinic-level setting)
            if (!clinic.EnableOnlineAppointmentScheduling)
                throw new InvalidOperationException("Agendamento online não está ativo para esta clínica.");

            // Validate appointment time is within clinic working hours
            if (!clinic.IsWithinWorkingHours(dto.ScheduledTime))
                throw new InvalidOperationException($"O horário {dto.ScheduledTime:hh\\:mm} está fora do horário de funcionamento da clínica ({clinic.OpeningTime:hh\\:mm} - {clinic.ClosingTime:hh\\:mm}).");
            
            var endTime = dto.ScheduledTime.Add(TimeSpan.FromMinutes(dto.DurationMinutes));
            if (!clinic.IsWithinWorkingHours(endTime))
                throw new InvalidOperationException($"O término do atendimento ({endTime:hh\\:mm}) ultrapassa o horário de fechamento da clínica ({clinic.ClosingTime:hh\\:mm}).");

            // Verifica se existe conflito de horário
            var hasConflict = await _appointmentRepository.HasConflictingAppointmentAsync(
                dto.ScheduledDate,
                dto.ScheduledTime,
                dto.DurationMinutes,
                dto.ClinicId,
                clinic.TenantId
            );

            if (hasConflict)
                throw new InvalidOperationException("Este horário não está mais disponível. Por favor, escolha outro horário.");

            // Busca ou cria o paciente
            Patient patient;
            // Check if patient exists within the clinic's own tenant to respect tenant isolation
            var existingPatient = await _patientRepository.GetByDocumentAsync(dto.PatientCpf, clinic.TenantId);

            if (existingPatient != null)
            {
                // Paciente já existe, usa o existente
                patient = existingPatient;
                
                // Verifica se o paciente já está vinculado à clínica
                var existingLink = await _patientClinicLinkRepository.GetLinkAsync(
                    patient.Id,
                    dto.ClinicId,
                    clinic.TenantId
                );

                // Se não existe vínculo, cria um
                if (existingLink == null)
                {
                    var newLink = new PatientClinicLink(patient.Id, dto.ClinicId, clinic.TenantId);
                    await _patientClinicLinkRepository.AddAsync(newLink);
                }
            }
            else
            {
                // Cria novo paciente com informações mínimas
                // Endereço padrão para novos pacientes via agendamento público
                var defaultAddress = new Address(
                    street: "A confirmar",
                    number: "S/N",
                    neighborhood: "A confirmar",
                    city: "A confirmar",
                    state: "A confirmar",
                    zipCode: "00000-000",
                    country: "Brasil"
                );

                // Telefone padrão
                var phone = ParsePhoneNumber(dto.PatientPhone);

                patient = new Patient(
                    name: dto.PatientName,
                    document: dto.PatientCpf,
                    dateOfBirth: dto.PatientBirthDate,
                    gender: "Não informado", // Será atualizado no cadastro completo
                    email: new Email(dto.PatientEmail),
                    phone: phone,
                    address: defaultAddress,
                    tenantId: clinic.TenantId
                );

                patient = await _patientRepository.AddAsync(patient);

                // Cria vínculo com a clínica
                var link = new PatientClinicLink(patient.Id, dto.ClinicId, clinic.TenantId);
                await _patientClinicLinkRepository.AddAsync(link);
            }

            // Cria o agendamento
            var appointment = new Appointment(
                patientId: patient.Id,
                clinicId: dto.ClinicId,
                scheduledDate: dto.ScheduledDate,
                scheduledTime: dto.ScheduledTime,
                durationMinutes: dto.DurationMinutes,
                type: AppointmentType.Regular,
                tenantId: clinic.TenantId,
                notes: $"Agendamento online. {dto.Notes}".Trim()
            );

            appointment = await _appointmentRepository.AddAsync(appointment);

            // Retorna resposta de sucesso
            return new PublicAppointmentResponseDto
            {
                AppointmentId = appointment.Id,
                ClinicId = clinic.Id,
                ClinicName = clinic.Name,
                ScheduledDate = appointment.ScheduledDate,
                ScheduledTime = appointment.ScheduledTime,
                Status = "Agendado",
                Message = $"Agendamento realizado com sucesso! Você receberá uma confirmação por e-mail em {dto.PatientEmail}. " +
                         $"Consulta agendada para {dto.ScheduledDate:dd/MM/yyyy} às {dto.ScheduledTime:hh\\:mm}."
            };
        }

        /// <summary>
        /// Parseia o número de telefone em formato brasileiro.
        /// Exemplo: "(11) 98765-4321" ou "11987654321"
        /// </summary>
        private Phone ParsePhoneNumber(string phoneNumber)
        {
            // Remove caracteres não numéricos
            var digitsOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // Formato esperado: DDD + número (10 ou 11 dígitos)
            if (digitsOnly.Length < 10)
                throw new InvalidOperationException("Número de telefone inválido. Deve conter DDD e número (mínimo 10 dígitos).");

            return new Phone("+55", digitsOnly);
        }
    }
}
