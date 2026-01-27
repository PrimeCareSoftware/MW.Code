namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Tipos de gatilhos para automação de marketing
    /// </summary>
    public enum AutomationTriggerType
    {
        /// <summary>
        /// Gatilho baseado em mudança de estágio na jornada
        /// </summary>
        StageChange,
        
        /// <summary>
        /// Gatilho baseado em evento específico
        /// </summary>
        Event,
        
        /// <summary>
        /// Gatilho agendado (data/hora específica)
        /// </summary>
        Scheduled,
        
        /// <summary>
        /// Gatilho baseado em comportamento do paciente
        /// </summary>
        BehaviorBased,
        
        /// <summary>
        /// Gatilho baseado em data (aniversário, etc)
        /// </summary>
        DateBased
    }
}
