namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Tipos de questões em pesquisas
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Escala numérica (0-10)
        /// </summary>
        NumericScale,
        
        /// <summary>
        /// Múltipla escolha
        /// </summary>
        MultipleChoice,
        
        /// <summary>
        /// Texto livre
        /// </summary>
        FreeText,
        
        /// <summary>
        /// Avaliação por estrelas (1-5)
        /// </summary>
        StarRating,
        
        /// <summary>
        /// Sim/Não
        /// </summary>
        YesNo
    }
}
