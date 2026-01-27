namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Estágios da jornada do paciente no sistema CRM
    /// </summary>
    public enum JourneyStageEnum
    {
        /// <summary>
        /// Lead capture, marketing inicial
        /// </summary>
        Descoberta = 1,
        
        /// <summary>
        /// Avaliando opções, comparando serviços
        /// </summary>
        Consideracao = 2,
        
        /// <summary>
        /// Primeiro atendimento/consulta
        /// </summary>
        PrimeiraConsulta = 3,
        
        /// <summary>
        /// Durante o tratamento
        /// </summary>
        Tratamento = 4,
        
        /// <summary>
        /// Consultas de retorno
        /// </summary>
        Retorno = 5,
        
        /// <summary>
        /// Cliente recorrente
        /// </summary>
        Fidelizacao = 6,
        
        /// <summary>
        /// Promotor da marca, recomenda para outros
        /// </summary>
        Advocacia = 7
    }
}
