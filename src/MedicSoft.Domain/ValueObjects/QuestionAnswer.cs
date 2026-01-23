using MedicSoft.Domain.Enums;
using System;
using System.Collections.Generic;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Represents an answer to a question in an anamnesis response
    /// </summary>
    public class QuestionAnswer
    {
        public string QuestionText { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string? Answer { get; set; }  // JSON for complex responses
        public List<string>? SelectedOptions { get; set; }
        public int? NumericValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateValue { get; set; }
        
        public QuestionAnswer() { }
        
        public QuestionAnswer(string questionText, QuestionType type, string answer)
        {
            QuestionText = questionText;
            Type = type;
            Answer = answer;
        }
    }
}
