using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an NPS (Net Promoter Score) survey response.
    /// Part of Phase 2 validation for measuring user satisfaction.
    /// </summary>
    public class NpsSurvey : BaseEntity
    {
        public string UserId { get; private set; }
        public int Score { get; private set; } // 0-10
        public string? Feedback { get; private set; }
        public DateTime RespondedAt { get; private set; }
        public string? UserRole { get; private set; }
        public int DaysAsUser { get; private set; } // Days since user registration

        // Navigation property
        public User? User { get; private set; }

        private NpsSurvey()
        {
            // EF Constructor
            UserId = null!;
        }

        public NpsSurvey(
            string userId,
            int score,
            string tenantId,
            string? feedback = null,
            string? userRole = null,
            int daysAsUser = 0) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (score < 0 || score > 10)
                throw new ArgumentException("Score must be between 0 and 10", nameof(score));

            UserId = userId;
            Score = score;
            Feedback = feedback;
            UserRole = userRole;
            DaysAsUser = daysAsUser;
            RespondedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Calculates NPS category: Detractor (0-6), Passive (7-8), or Promoter (9-10)
        /// </summary>
        public NpsCategory GetCategory()
        {
            if (Score >= 0 && Score <= 6)
                return NpsCategory.Detractor;
            if (Score >= 7 && Score <= 8)
                return NpsCategory.Passive;
            return NpsCategory.Promoter;
        }
    }

    public enum NpsCategory
    {
        Detractor = 1,
        Passive = 2,
        Promoter = 3
    }
}
