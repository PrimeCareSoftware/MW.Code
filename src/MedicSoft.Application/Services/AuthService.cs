using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IAuthService
    {
        Task<User?> AuthenticateUserAsync(string username, string password, string tenantId);
        Task<Owner?> AuthenticateOwnerAsync(string username, string password, string tenantId);
        Task<string> RecordUserLoginAsync(Guid userId, string tenantId);
        Task<string> RecordOwnerLoginAsync(Guid ownerId, string tenantId);
        Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId);
        Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository, 
            IOwnerRepository ownerRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _ownerRepository = ownerRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> AuthenticateUserAsync(string username, string password, string tenantId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            
            if (user == null || !user.IsActive)
                return null;

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<Owner?> AuthenticateOwnerAsync(string username, string password, string tenantId)
        {
            var owner = await _ownerRepository.GetByUsernameAsync(username, tenantId);
            
            if (owner == null || !owner.IsActive)
                return null;

            if (!_passwordHasher.VerifyPassword(password, owner.PasswordHash))
                return null;

            return owner;
        }

        public async Task<string> RecordUserLoginAsync(Guid userId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user != null)
            {
                var sessionId = Guid.NewGuid().ToString();
                user.RecordLogin(sessionId);
                await _userRepository.UpdateAsync(user);
                return sessionId;
            }
            throw new InvalidOperationException("User not found");
        }

        public async Task<string> RecordOwnerLoginAsync(Guid ownerId, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner != null)
            {
                var sessionId = Guid.NewGuid().ToString();
                owner.RecordLogin(sessionId);
                await _ownerRepository.UpdateAsync(owner);
                return sessionId;
            }
            throw new InvalidOperationException("Owner not found");
        }

        public async Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
            {
                return false;
            }
            
            var isValid = user.IsSessionValid(sessionId);
            // Log for debugging
            Console.WriteLine($"[AuthService] ValidateUserSession - UserId: {userId}, SessionId: {sessionId}, CurrentSessionId: {user.CurrentSessionId ?? "null"}, IsValid: {isValid}");
            return isValid;
        }

        public async Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner == null)
            {
                return false;
            }
            
            var isValid = owner.IsSessionValid(sessionId);
            // Log for debugging
            Console.WriteLine($"[AuthService] ValidateOwnerSession - OwnerId: {ownerId}, SessionId: {sessionId}, CurrentSessionId: {owner.CurrentSessionId ?? "null"}, IsValid: {isValid}");
            return isValid;
        }
    }
}
