namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of identification documents for Brazilian citizens and entities
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// CPF - Cadastro de Pessoas Físicas (Individual Tax ID)
        /// Format: 000.000.000-00 (11 digits)
        /// Used for individual/autonomous professionals
        /// </summary>
        CPF = 1,
        
        /// <summary>
        /// CNPJ - Cadastro Nacional da Pessoa Jurídica (Company Tax ID)
        /// Format: 00.000.000/0000-00 (14 digits)
        /// Used for registered businesses/clinics
        /// </summary>
        CNPJ = 2
    }
}
