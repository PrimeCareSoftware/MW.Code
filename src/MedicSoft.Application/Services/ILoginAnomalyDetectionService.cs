using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    public class LoginAttemptDto
    {
        public string IpAddress { get; set; } = null!;
        public string UserAgent { get; set; } = null!;
        public string? Country { get; set; }
    }

    public interface ILoginAnomalyDetectionService
    {
        Task<bool> IsLoginSuspicious(string userId, LoginAttemptDto attempt, string tenantId);
        Task RecordLoginAttempt(string userId, LoginAttemptDto attempt, bool success, string tenantId);
    }
}
