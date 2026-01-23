using MedicSoft.Domain.Enums;
using System.Collections.Generic;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Represents a question in an anamnesis template
    /// </summary>
    public class Question
    {
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public bool IsRequired { get; set; }
        public List<string>? Options { get; set; }  // For multiple choice questions
        public string? Unit { get; set; }  // For numeric fields (kg, cm, etc)
        public int Order { get; set; }
        public string? HelpText { get; set; }
        public string? SnomedCode { get; set; }  // SNOMED CT code (optional)
        
        public Question() { }
        
        public Question(string questionText, QuestionType type, bool isRequired = false, int order = 0)
        {
            QuestionText = questionText;
            Type = type;
            IsRequired = isRequired;
            Order = order;
        }
    }
}
