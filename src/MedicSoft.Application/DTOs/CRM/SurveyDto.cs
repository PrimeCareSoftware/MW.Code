using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class SurveyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SurveyType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Triggers
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayHours { get; set; }
        
        // Métricas
        public double AverageScore { get; set; }
        public int TotalResponses { get; set; }
        public int ResponseRate { get; set; }
        
        // Questões
        public List<SurveyQuestionDto> Questions { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SurveyQuestionDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
        public bool IsRequired { get; set; }
        public List<string> Options { get; set; } = new();
    }

    public class SurveyResponseDto
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyName { get; set; } = string.Empty;
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public bool IsCompleted { get; set; }
        
        public int? NpsScore { get; set; }
        public int? CsatScore { get; set; }
        
        public List<SurveyQuestionResponseDto> QuestionResponses { get; set; } = new();
    }

    public class SurveyQuestionResponseDto
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string? TextAnswer { get; set; }
        public int? NumericAnswer { get; set; }
        public bool? BooleanAnswer { get; set; }
    }

    public class CreateSurveyDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public SurveyType Type { get; set; }
        
        // Triggers (optional)
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayHours { get; set; }
        
        public List<CreateSurveyQuestionDto> Questions { get; set; } = new();
    }

    public class CreateSurveyQuestionDto
    {
        public int Order { get; set; }
        public string Text { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public bool IsRequired { get; set; }
        public List<string> Options { get; set; } = new();
    }

    public class UpdateSurveyDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayHours { get; set; }
        public List<CreateSurveyQuestionDto>? Questions { get; set; }
    }

    public class SubmitSurveyResponseDto
    {
        public Guid SurveyId { get; set; }
        public Guid PatientId { get; set; }
        public List<SubmitQuestionResponseDto> Responses { get; set; } = new();
    }

    public class SubmitQuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string? TextAnswer { get; set; }
        public int? NumericAnswer { get; set; }
        public bool? BooleanAnswer { get; set; }
    }

    public class SurveyAnalyticsDto
    {
        public Guid SurveyId { get; set; }
        public string SurveyName { get; set; } = string.Empty;
        public SurveyType Type { get; set; }
        
        // Overall metrics
        public int TotalSent { get; set; }
        public int TotalResponses { get; set; }
        public double ResponseRate { get; set; }
        public double AverageScore { get; set; }
        
        // NPS specific
        public int? Promoters { get; set; } // 9-10
        public int? Passives { get; set; } // 7-8
        public int? Detractors { get; set; } // 0-6
        public double? NpsScore { get; set; } // (Promoters - Detractors) / TotalResponses * 100
        
        // CSAT specific
        public int? VerySatisfied { get; set; } // 5
        public int? Satisfied { get; set; } // 4
        public int? Neutral { get; set; } // 3
        public int? Unsatisfied { get; set; } // 2
        public int? VeryUnsatisfied { get; set; } // 1
        
        // Detailed responses
        public List<SurveyResponseDto> RecentResponses { get; set; } = new();
    }
}
