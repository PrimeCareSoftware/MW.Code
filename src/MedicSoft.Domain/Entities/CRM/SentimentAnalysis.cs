using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Análise de sentimento de texto usando IA (Azure Cognitive Services)
    /// </summary>
    public class SentimentAnalysis : BaseEntity
    {
        public string SourceText { get; private set; }
        public string SourceType { get; private set; } // Complaint, Survey, Email, etc.
        public Guid? SourceId { get; private set; }
        
        // Resultado da análise
        public SentimentType Sentiment { get; private set; }
        public double PositiveScore { get; private set; } // 0-1
        public double NeutralScore { get; private set; } // 0-1
        public double NegativeScore { get; private set; } // 0-1
        public double ConfidenceScore { get; private set; } // 0-1
        
        // Tópicos/entidades extraídas
        private readonly List<string> _topics = new();
        public IReadOnlyCollection<string> Topics => _topics.AsReadOnly();
        
        public DateTime AnalyzedAt { get; private set; }
        
        private SentimentAnalysis()
        {
            SourceText = string.Empty;
            SourceType = string.Empty;
        }
        
        public SentimentAnalysis(
            string sourceText,
            string sourceType,
            Guid? sourceId,
            string tenantId) : base(tenantId)
        {
            SourceText = sourceText ?? throw new ArgumentNullException(nameof(sourceText));
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            SourceId = sourceId;
            AnalyzedAt = DateTime.UtcNow;
        }
        
        public void SetAnalysisResult(
            SentimentType sentiment,
            double positiveScore,
            double neutralScore,
            double negativeScore,
            double confidenceScore)
        {
            Sentiment = sentiment;
            PositiveScore = Math.Clamp(positiveScore, 0, 1);
            NeutralScore = Math.Clamp(neutralScore, 0, 1);
            NegativeScore = Math.Clamp(negativeScore, 0, 1);
            ConfidenceScore = Math.Clamp(confidenceScore, 0, 1);
            UpdateTimestamp();
        }
        
        public void AddTopic(string topic)
        {
            if (string.IsNullOrWhiteSpace(topic))
                throw new ArgumentException("Topic cannot be null or empty", nameof(topic));
                
            if (!_topics.Contains(topic))
            {
                _topics.Add(topic);
                UpdateTimestamp();
            }
        }
    }
}
