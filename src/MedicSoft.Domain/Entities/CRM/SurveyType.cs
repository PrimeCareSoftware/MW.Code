namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Tipos de pesquisas de satisfação
    /// </summary>
    public enum SurveyType
    {
        /// <summary>
        /// Net Promoter Score - "Em uma escala de 0 a 10, quanto você recomendaria nossos serviços?"
        /// </summary>
        NPS,
        
        /// <summary>
        /// Customer Satisfaction Score - Satisfação geral
        /// </summary>
        CSAT,
        
        /// <summary>
        /// Customer Effort Score - Facilidade de uso
        /// </summary>
        CES,
        
        /// <summary>
        /// Pesquisa personalizada
        /// </summary>
        Custom
    }
}
