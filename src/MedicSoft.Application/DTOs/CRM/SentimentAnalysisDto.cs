using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class SentimentAnalysisDto
    {
        public Guid Id { get; set; }
        public string SourceText { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public Guid? SourceId { get; set; }
        
        public SentimentType Sentiment { get; set; }
        public string SentimentName { get; set; } = string.Empty;
        public double PositiveScore { get; set; }
        public double NeutralScore { get; set; }
        public double NegativeScore { get; set; }
        public double ConfidenceScore { get; set; }
        
        public List<string> Topics { get; set; } = new();
        public DateTime AnalyzedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSentimentAnalysisDto
    {
        public string SourceText { get; set; } = string.Empty;
        public string SourceType { get; set; } = string.Empty;
        public Guid? SourceId { get; set; }
    }

    public class SentimentAnalysisResultDto
    {
        public SentimentType Sentiment { get; set; }
        public string SentimentName { get; set; } = string.Empty;
        public double PositiveScore { get; set; }
        public double NeutralScore { get; set; }
        public double NegativeScore { get; set; }
        public double ConfidenceScore { get; set; }
        public List<string> Topics { get; set; } = new();
        public bool IsNegativeAlert { get; set; }
    }
}
