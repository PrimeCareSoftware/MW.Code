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
        Task RecordUserLoginAsync(Guid userId, string tenantId);
        Task RecordOwnerLoginAsync(Guid ownerId, string tenantId);
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

        public async Task RecordUserLoginAsync(Guid userId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user != null)
            {
                user.RecordLogin();
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task RecordOwnerLoginAsync(Guid ownerId, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner != null)
            {
                owner.RecordLogin();
                await _ownerRepository.UpdateAsync(owner);
            }
        }
    }
}
