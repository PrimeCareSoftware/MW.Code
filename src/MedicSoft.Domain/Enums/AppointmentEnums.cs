namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Status of an appointment
    /// </summary>
    public enum AppointmentStatus
    {
        Scheduled = 1,
        Confirmed = 2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5,
        NoShow = 6
    }

    /// <summary>
    /// Type of appointment
    /// </summary>
    public enum AppointmentType
    {
        Regular = 1,
        Emergency = 2,
        FollowUp = 3,
        Consultation = 4
    }

    /// <summary>
    /// Mode of appointment delivery
    /// </summary>
    public enum AppointmentMode
    {
        InPerson = 1,      // Presencial
        Telemedicine = 2   // Telemedicina
    }

    /// <summary>
    /// Payment type for appointments
    /// </summary>
    public enum PaymentType
    {
        Private = 1,       // Particular
        HealthInsurance = 2 // Convênio
    }

    /// <summary>
    /// Who receives the payment for the appointment
    /// </summary>
    public enum PaymentReceiverType
    {
        Doctor = 1,        // Médico recebe no final do atendimento
        Secretary = 2,     // Secretária recebe antes/depois do atendimento
        Other = 3          // Outro funcionário
    }
}
