using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IUserFeedbackRepository : IRepository<UserFeedback>
    {
        Task<IEnumerable<UserFeedback>> GetByUserIdAsync(string userId, string tenantId);
        Task<IEnumerable<UserFeedback>> GetByStatusAsync(FeedbackStatus status, string tenantId);
        Task<IEnumerable<UserFeedback>> GetByTypeAsync(FeedbackType type, string tenantId);
        Task<IEnumerable<UserFeedback>> GetCriticalBugsAsync(string tenantId);
        Task<int> GetCountByStatusAsync(FeedbackStatus status, string tenantId);
    }

    public interface INpsSurveyRepository : IRepository<NpsSurvey>
    {
        Task<NpsSurvey?> GetByUserIdAsync(string userId, string tenantId);
        Task<bool> HasUserRespondedAsync(string userId, string tenantId);
        Task<IEnumerable<NpsSurvey>> GetByScoreRangeAsync(int minScore, int maxScore, string tenantId);
        Task<double> GetAverageScoreAsync(string tenantId);
        Task<int> GetCountByCategoryAsync(NpsCategory category, string tenantId);
    }
}
