namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Type of time slot block
    /// </summary>
    public enum BlockedTimeSlotType
    {
        Break = 1,           // Intervalo (almoço, descanso)
        Unavailable = 2,     // Indisponível (férias, compromisso pessoal)
        Maintenance = 3,     // Manutenção (limpeza, equipamento)
        Training = 4,        // Treinamento
        Meeting = 5,         // Reunião
        Other = 6           // Outro motivo
    }

    /// <summary>
    /// Frequency of recurrence for appointments or blocks
    /// </summary>
    public enum RecurrenceFrequency
    {
        Daily = 1,           // Diário
        Weekly = 2,          // Semanal
        Biweekly = 3,        // Quinzenal
        Monthly = 4,         // Mensal
        Custom = 5           // Personalizado
    }

    /// <summary>
    /// Days of the week for recurrence patterns
    /// </summary>
    [System.Flags]
    public enum RecurrenceDays
    {
        None = 0,
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
        Weekdays = Monday | Tuesday | Wednesday | Thursday | Friday,
        Weekend = Saturday | Sunday,
        All = Sunday | Monday | Tuesday | Wednesday | Thursday | Friday | Saturday
    }
}
