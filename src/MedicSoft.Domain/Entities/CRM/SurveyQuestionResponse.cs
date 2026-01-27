using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Resposta de um paciente a uma questão de pesquisa
    /// </summary>
    public class SurveyQuestionResponse : BaseEntity
    {
        public Guid SurveyResponseId { get; private set; }
        public SurveyResponse SurveyResponse { get; private set; } = null!;
        
        public Guid SurveyQuestionId { get; private set; }
        public SurveyQuestion SurveyQuestion { get; private set; } = null!;
        
        public string? TextAnswer { get; private set; }
        public int? NumericAnswer { get; private set; }
        public DateTime AnsweredAt { get; private set; }
        
        private SurveyQuestionResponse() { }
        
        public SurveyQuestionResponse(
            Guid surveyResponseId,
            Guid surveyQuestionId,
            string tenantId) : base(tenantId)
        {
            SurveyResponseId = surveyResponseId;
            SurveyQuestionId = surveyQuestionId;
            AnsweredAt = DateTime.UtcNow;
        }
        
        public void SetTextAnswer(string answer)
        {
            TextAnswer = answer;
            UpdateTimestamp();
        }
        
        public void SetNumericAnswer(int answer)
        {
            NumericAnswer = answer;
            UpdateTimestamp();
        }
    }
}
