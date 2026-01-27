namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Status de uma reclamação/ouvidoria
    /// </summary>
    public enum ComplaintStatus
    {
        /// <summary>
        /// Recebida, aguardando triagem
        /// </summary>
        Received,
        
        /// <summary>
        /// Em análise
        /// </summary>
        InAnalysis,
        
        /// <summary>
        /// Em tratamento/resolução
        /// </summary>
        InProgress,
        
        /// <summary>
        /// Aguardando resposta do paciente
        /// </summary>
        AwaitingResponse,
        
        /// <summary>
        /// Resolvida
        /// </summary>
        Resolved,
        
        /// <summary>
        /// Fechada
        /// </summary>
        Closed,
        
        /// <summary>
        /// Cancelada
        /// </summary>
        Cancelled
    }
}
