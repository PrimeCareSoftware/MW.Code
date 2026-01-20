namespace MedicSoft.Telemedicine.Application.DTOs;

/// <summary>
/// Request to record patient consent for telemedicine
/// Required by CFM 2.314/2022
/// </summary>
public class CreateConsentRequest
{
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public bool AcceptsRecording { get; set; }
    public bool AcceptsDataSharing { get; set; }
    public string? DigitalSignature { get; set; }
}

/// <summary>
/// Response with consent details
/// </summary>
public class ConsentResponse
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public DateTime ConsentDate { get; set; }
    public bool AcceptsRecording { get; set; }
    public bool AcceptsDataSharing { get; set; }
    public bool IsActive { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevocationReason { get; set; }
}

/// <summary>
/// Request to revoke consent
/// </summary>
public class RevokeConsentRequest
{
    public string Reason { get; set; } = string.Empty;
}

/// <summary>
/// Request to validate first appointment rule
/// CFM 2.314 requires first appointments to be in-person with exceptions
/// </summary>
public class ValidateFirstAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid ProviderId { get; set; }
    public string? Justification { get; set; }
}

/// <summary>
/// Response for first appointment validation
/// </summary>
public class FirstAppointmentValidationResponse
{
    public bool IsFirstAppointment { get; set; }
    public bool CanProceedWithTelemedicine { get; set; }
    public string? ValidationMessage { get; set; }
    public string? RequiredJustification { get; set; }
}

/// <summary>
/// CFM 2.314 consent text (Portuguese)
/// </summary>
public static class ConsentTexts
{
    public const string TelemedicineConsentText = @"
TERMO DE CONSENTIMENTO LIVRE E ESCLARECIDO PARA TELECONSULTA

Conforme Resolução CFM 2.314/2022

Eu, paciente ou responsável legal, declaro que:

1. Fui informado(a) sobre a modalidade de atendimento por TELECONSULTA (consulta médica realizada remotamente por meio de tecnologia de videochamada);

2. Compreendo que a teleconsulta é uma modalidade de atendimento médico a distância, mediada por tecnologia, que permite a comunicação audiovisual em tempo real entre médico e paciente;

3. Fui esclarecido(a) sobre:
   - As vantagens e limitações desta modalidade de atendimento;
   - A necessidade de conexão adequada de internet;
   - A importância da privacidade durante a consulta;
   - Meu direito de solicitar atendimento presencial a qualquer momento;

4. Concordo que:
   - Minha identificação será verificada através de documento oficial com foto;
   - A consulta será realizada por profissional médico devidamente identificado (CRM);
   - As informações serão registradas em prontuário eletrônico;
   - O sigilo médico será mantido conforme legislação vigente;

5. Autorizo:
   - O uso de tecnologia de videochamada para a realização da consulta;
   - O registro das informações da consulta em prontuário eletrônico;
   - A transmissão segura e criptografada dos dados;

6. Estou ciente de que:
   - Posso revogar este consentimento a qualquer momento;
   - Tenho direito de solicitar cópia do meu prontuário;
   - Em caso de emergência, devo procurar atendimento presencial imediatamente;

Data e Hora do Consentimento: [DATA_HORA_UTC]
IP de Origem: [IP_ADDRESS]
Dispositivo: [USER_AGENT]
";

    public const string RecordingConsentText = @"
CONSENTIMENTO PARA GRAVAÇÃO DA TELECONSULTA

Autorizo a GRAVAÇÃO da teleconsulta para fins de:
- Registro médico conforme CFM 2.314/2022;
- Auditoria de qualidade do atendimento;
- Documentação para prontuário médico;

Estou ciente de que:
- A gravação será armazenada de forma segura por 20 anos;
- Apenas profissionais autorizados terão acesso;
- A gravação não será compartilhada com terceiros sem minha autorização;
- Posso solicitar a exclusão da gravação conforme LGPD;
";
}
