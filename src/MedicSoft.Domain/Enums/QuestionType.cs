namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of questions for anamnesis templates
    /// </summary>
    public enum QuestionType
    {
        /// <summary>
        /// Free text input
        /// </summary>
        Text = 1,
        
        /// <summary>
        /// Numeric input
        /// </summary>
        Number = 2,
        
        /// <summary>
        /// Yes/No question
        /// </summary>
        YesNo = 3,
        
        /// <summary>
        /// Single choice (one option)
        /// </summary>
        SingleChoice = 4,
        
        /// <summary>
        /// Multiple choice (multiple options)
        /// </summary>
        MultipleChoice = 5,
        
        /// <summary>
        /// Date input
        /// </summary>
        Date = 6,
        
        /// <summary>
        /// Scale (e.g., 0-10)
        /// </summary>
        Scale = 7
    }
}
