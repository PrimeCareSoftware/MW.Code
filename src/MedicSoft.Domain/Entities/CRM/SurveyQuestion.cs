using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Questão individual em uma pesquisa
    /// </summary>
    public class SurveyQuestion : BaseEntity
    {
        public Guid SurveyId { get; private set; }
        public Survey Survey { get; private set; } = null!;
        
        public int Order { get; private set; }
        public string QuestionText { get; private set; }
        public QuestionType Type { get; private set; }
        public bool IsRequired { get; private set; }
        
        // Para múltipla escolha
        public string? OptionsJson { get; private set; } // JSON array de opções
        
        private SurveyQuestion()
        {
            QuestionText = string.Empty;
        }
        
        public SurveyQuestion(
            Guid surveyId,
            int order,
            string questionText,
            QuestionType type,
            bool isRequired,
            string tenantId) : base(tenantId)
        {
            SurveyId = surveyId;
            Order = order;
            QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
            Type = type;
            IsRequired = isRequired;
        }
        
        public void SetOptions(string optionsJson)
        {
            if (Type != QuestionType.MultipleChoice)
                throw new InvalidOperationException("Options can only be set for multiple choice questions");
                
            OptionsJson = optionsJson;
            UpdateTimestamp();
        }
        
        public void UpdateQuestion(string questionText, bool isRequired)
        {
            QuestionText = questionText ?? throw new ArgumentNullException(nameof(questionText));
            IsRequired = isRequired;
            UpdateTimestamp();
        }
    }
}
