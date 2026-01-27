using System;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IBruteForceProtectionService
    {
        Task<bool> IsAccountLockedAsync(string username, string tenantId);
        Task<bool> CanAttemptLoginAsync(string username, string ipAddress, string tenantId);
        Task RecordFailedAttemptAsync(string username, string ipAddress, string tenantId, string? failureReason = null);
        Task RecordSuccessfulLoginAsync(string username, string tenantId);
        Task<TimeSpan?> GetRemainingLockoutTimeAsync(string username, string tenantId);
    }

    public class BruteForceProtectionService : IBruteForceProtectionService
    {
        private readonly ILoginAttemptRepository _loginAttemptRepository;
        private readonly IAccountLockoutRepository _accountLockoutRepository;
        
        private const int MaxFailedAttempts = 5;
        private static readonly TimeSpan[] LockoutDurations = new[]
        {
            TimeSpan.FromMinutes(5),   // 1st lockout
            TimeSpan.FromMinutes(15),  // 2nd lockout
            TimeSpan.FromHours(1),     // 3rd lockout
            TimeSpan.FromHours(24)     // 4th+ lockout
        };

        public BruteForceProtectionService(
            ILoginAttemptRepository loginAttemptRepository,
            IAccountLockoutRepository accountLockoutRepository)
        {
            _loginAttemptRepository = loginAttemptRepository ?? throw new ArgumentNullException(nameof(loginAttemptRepository));
            _accountLockoutRepository = accountLockoutRepository ?? throw new ArgumentNullException(nameof(accountLockoutRepository));
        }

        public async Task<bool> IsAccountLockedAsync(string username, string tenantId)
        {
            var lockout = await _accountLockoutRepository.GetActiveLockedAccountAsync(username, tenantId);
            return lockout?.IsCurrentlyLocked() ?? false;
        }

        public async Task<bool> CanAttemptLoginAsync(string username, string ipAddress, string tenantId)
        {
            // Check if account is locked
            if (await IsAccountLockedAsync(username, tenantId))
                return false;

            // Additional IP-based rate limiting could be added here
            return true;
        }

        public async Task RecordFailedAttemptAsync(string username, string ipAddress, string tenantId, string? failureReason = null)
        {
            // Record the failed attempt
            var loginAttempt = new LoginAttempt(
                username,
                ipAddress,
                wasSuccessful: false,
                tenantId,
                failureReason);

            await _loginAttemptRepository.AddAsync(loginAttempt);

            // Check if account should be locked
            var recentFailedCount = await _loginAttemptRepository.GetFailedAttemptsCountAsync(username, tenantId, minutes: 30);

            if (recentFailedCount >= MaxFailedAttempts)
            {
                await LockAccountAsync(username, tenantId, recentFailedCount);
            }
        }

        public async Task RecordSuccessfulLoginAsync(string username, string tenantId)
        {
            // Record successful login - IP address would be obtained from HttpContext
            var loginAttempt = new LoginAttempt(
                username,
                "unknown", // IP should be passed from controller
                wasSuccessful: true,
                tenantId);

            await _loginAttemptRepository.AddAsync(loginAttempt);
        }

        public async Task<TimeSpan?> GetRemainingLockoutTimeAsync(string username, string tenantId)
        {
            var lockout = await _accountLockoutRepository.GetActiveLockedAccountAsync(username, tenantId);
            
            if (lockout == null || !lockout.IsCurrentlyLocked())
                return null;

            var remaining = lockout.UnlocksAt - DateTime.UtcNow;
            return remaining > TimeSpan.Zero ? remaining : null;
        }

        private async Task LockAccountAsync(string username, string tenantId, int failedAttempts)
        {
            // Get the number of previous lockouts
            var lockoutCount = await _accountLockoutRepository.GetLockoutCountAsync(username, tenantId);
            
            // Determine lockout duration based on lockout count
            var durationIndex = Math.Min(lockoutCount, LockoutDurations.Length - 1);
            var lockoutDuration = LockoutDurations[durationIndex];

            // Create the lockout record
            var accountLockout = new AccountLockout(
                username,
                failedAttempts,
                lockoutDuration,
                tenantId);

            await _accountLockoutRepository.AddAsync(accountLockout);
        }
    }
}
