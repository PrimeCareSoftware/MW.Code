namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Direção do ponto de contato (iniciado pelo paciente ou pela clínica)
    /// </summary>
    public enum TouchpointDirection
    {
        /// <summary>
        /// Iniciado pelo paciente
        /// </summary>
        Inbound,
        
        /// <summary>
        /// Iniciado pela clínica
        /// </summary>
        Outbound
    }
}
