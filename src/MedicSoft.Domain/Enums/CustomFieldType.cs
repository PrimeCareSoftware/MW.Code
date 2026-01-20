namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Represents the type of custom field in consultation forms
    /// </summary>
    public enum CustomFieldType
    {
        /// <summary>
        /// Single line text input
        /// </summary>
        TextoSimples = 1,
        
        /// <summary>
        /// Multi-line text area
        /// </summary>
        TextoLongo = 2,
        
        /// <summary>
        /// Numeric input
        /// </summary>
        Numero = 3,
        
        /// <summary>
        /// Date input
        /// </summary>
        Data = 4,
        
        /// <summary>
        /// Date and time input
        /// </summary>
        DataHora = 5,
        
        /// <summary>
        /// Single selection dropdown
        /// </summary>
        SelecaoUnica = 6,
        
        /// <summary>
        /// Multiple selection checkboxes
        /// </summary>
        SelecaoMultipla = 7,
        
        /// <summary>
        /// Boolean yes/no checkbox
        /// </summary>
        SimNao = 8,
        
        /// <summary>
        /// Email input
        /// </summary>
        Email = 9,
        
        /// <summary>
        /// Phone number input
        /// </summary>
        Telefone = 10,
        
        /// <summary>
        /// URL input
        /// </summary>
        Url = 11
    }
}
