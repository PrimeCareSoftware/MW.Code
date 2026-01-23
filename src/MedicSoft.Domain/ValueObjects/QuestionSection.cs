using MedicSoft.Domain.ValueObjects;
using System.Collections.Generic;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Represents a section of related questions in an anamnesis template
    /// </summary>
    public class QuestionSection
    {
        public string SectionName { get; set; } = string.Empty;
        public int Order { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        
        public QuestionSection() { }
        
        public QuestionSection(string sectionName, int order)
        {
            SectionName = sectionName;
            Order = order;
        }
    }
}
