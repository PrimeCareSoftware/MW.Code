namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Categoria/tipo de reclamação
    /// </summary>
    public enum ComplaintCategory
    {
        /// <summary>
        /// Atendimento
        /// </summary>
        Service,
        
        /// <summary>
        /// Agendamento
        /// </summary>
        Scheduling,
        
        /// <summary>
        /// Faturamento
        /// </summary>
        Billing,
        
        /// <summary>
        /// Instalações
        /// </summary>
        Facilities,
        
        /// <summary>
        /// Profissional de saúde
        /// </summary>
        HealthcareProfessional,
        
        /// <summary>
        /// Tempo de espera
        /// </summary>
        WaitTime,
        
        /// <summary>
        /// Tratamento médico
        /// </summary>
        MedicalTreatment,
        
        /// <summary>
        /// Outro
        /// </summary>
        Other
    }
}
