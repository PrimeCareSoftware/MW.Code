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
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IOwnerSessionRepository _ownerSessionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public AuthService(
            IUserRepository userRepository, 
            IOwnerRepository ownerRepository,
            IUserSessionRepository userSessionRepository,
            IOwnerSessionRepository ownerSessionRepository,
            IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _ownerRepository = ownerRepository;
            _userSessionRepository = userSessionRepository;
            _ownerSessionRepository = ownerSessionRepository;
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
            if (user == null)
                throw new InvalidOperationException("User not found");

            var sessionId = Guid.NewGuid().ToString();
            
            // IMPORTANT: Invalidate ALL previous sessions (single session enforcement)
            await _userSessionRepository.DeleteAllUserSessionsAsync(userId, tenantId);
            
            // Update legacy field for backward compatibility
            user.RecordLogin(sessionId);
            await _userRepository.UpdateAsync(user);

            // Create new session record in database
            var userSession = new UserSession(userId, sessionId, tenantId, expiryHours: 24);
            await _userSessionRepository.AddAsync(userSession);
            await _userSessionRepository.SaveChangesAsync();

            return sessionId;
        }

        public async Task<string> RecordOwnerLoginAsync(Guid ownerId, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            var sessionId = Guid.NewGuid().ToString();
            
            // IMPORTANT: Invalidate ALL previous sessions (single session enforcement)
            await _ownerSessionRepository.DeleteAllOwnerSessionsAsync(ownerId, tenantId);
            
            // Update legacy field for backward compatibility
            owner.RecordLogin(sessionId);
            await _ownerRepository.UpdateAsync(owner);

            // Create new session record in database
            var ownerSession = new OwnerSession(ownerId, sessionId, tenantId, expiryHours: 24);
            await _ownerSessionRepository.AddAsync(ownerSession);
            await _ownerSessionRepository.SaveChangesAsync();

            return sessionId;
        }

        public async Task<bool> ValidateUserSessionAsync(Guid userId, string sessionId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
                return false;

            // Check if session exists in the UserSessions table
            var isValid = await _userSessionRepository.IsSessionValidAsync(userId, sessionId, tenantId);
            
            if (isValid)
                return true;

            // Fallback to legacy SessionId field for backward compatibility
            // This allows sessions created before the migration to still work
            var legacySessionValid = user.IsSessionValid(sessionId);
            return legacySessionValid;
        }

        public async Task<bool> ValidateOwnerSessionAsync(Guid ownerId, string sessionId, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner == null)
                return false;

            // Check if session exists in the OwnerSessions table
            var isValid = await _ownerSessionRepository.IsSessionValidAsync(ownerId, sessionId, tenantId);
            
            if (isValid)
                return true;

            // Fallback to legacy SessionId field for backward compatibility
            // This allows sessions created before the migration to still work
            var legacySessionValid = owner.IsSessionValid(sessionId);
            return legacySessionValid;
        }
    }
}
