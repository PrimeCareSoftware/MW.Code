using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IClinicSelectionService
    {
        Task<IEnumerable<UserClinicDto>> GetUserClinicsAsync(Guid userId, string tenantId);
        Task<SwitchClinicResponse> SwitchClinicAsync(Guid userId, Guid clinicId, string tenantId);
        Task<UserClinicDto?> GetCurrentClinicAsync(Guid userId, string tenantId);
    }

    public class ClinicSelectionService : IClinicSelectionService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserClinicLinkRepository _userClinicLinkRepository;
        private readonly IClinicRepository _clinicRepository;

        public ClinicSelectionService(
            IUserRepository userRepository,
            IUserClinicLinkRepository userClinicLinkRepository,
            IClinicRepository clinicRepository)
        {
            _userRepository = userRepository;
            _userClinicLinkRepository = userClinicLinkRepository;
            _clinicRepository = clinicRepository;
        }

        public async Task<IEnumerable<UserClinicDto>> GetUserClinicsAsync(Guid userId, string tenantId)
        {
            // Get all clinic links for the user
            var clinicLinks = await _userClinicLinkRepository.GetByUserIdAsync(userId, tenantId);
            
            // Get clinic details
            var clinicDtos = new List<UserClinicDto>();
            foreach (var link in clinicLinks.Where(l => l.IsActive))
            {
                var clinic = await _clinicRepository.GetByIdAsync(link.ClinicId, tenantId);
                if (clinic != null && clinic.IsActive)
                {
                    clinicDtos.Add(new UserClinicDto
                    {
                        ClinicId = clinic.Id,
                        ClinicName = clinic.Name,
                        ClinicAddress = clinic.Address,
                        IsPreferred = link.IsPreferredClinic,
                        IsActive = link.IsActive,
                        LinkedDate = link.LinkedDate
                    });
                }
            }

            // If no clinic links found, check legacy ClinicId for backward compatibility
            if (!clinicDtos.Any())
            {
                var user = await _userRepository.GetByIdAsync(userId, tenantId);
                if (user?.ClinicId != null)
                {
                    var clinic = await _clinicRepository.GetByIdAsync(user.ClinicId.Value, tenantId);
                    if (clinic != null && clinic.IsActive)
                    {
                        clinicDtos.Add(new UserClinicDto
                        {
                            ClinicId = clinic.Id,
                            ClinicName = clinic.Name,
                            ClinicAddress = clinic.Address,
                            IsPreferred = true,
                            IsActive = true,
                            LinkedDate = user.CreatedAt
                        });
                    }
                }
            }

            return clinicDtos.OrderByDescending(c => c.IsPreferred).ThenBy(c => c.ClinicName);
        }

        public async Task<SwitchClinicResponse> SwitchClinicAsync(Guid userId, Guid clinicId, string tenantId)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
            {
                return new SwitchClinicResponse
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Verify user has access to the clinic
            var hasAccess = await _userClinicLinkRepository.UserHasAccessToClinicAsync(userId, clinicId, tenantId);
            
            // Also check legacy ClinicId for backward compatibility
            if (!hasAccess && user.ClinicId == clinicId)
            {
                hasAccess = true;
            }

            if (!hasAccess)
            {
                return new SwitchClinicResponse
                {
                    Success = false,
                    Message = "User does not have access to this clinic"
                };
            }

            // Verify clinic exists and is active
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null || !clinic.IsActive)
            {
                return new SwitchClinicResponse
                {
                    Success = false,
                    Message = "Clinic not found or inactive"
                };
            }

            // Update user's current clinic
            user.SetCurrentClinic(clinicId);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return new SwitchClinicResponse
            {
                Success = true,
                Message = "Clinic switched successfully",
                CurrentClinicId = clinicId,
                CurrentClinicName = clinic.Name
            };
        }

        public async Task<UserClinicDto?> GetCurrentClinicAsync(Guid userId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
            {
                return null;
            }

            Guid? currentClinicId = user.CurrentClinicId ?? user.GetPreferredClinicId() ?? user.ClinicId;
            
            if (currentClinicId == null)
            {
                return null;
            }

            var clinic = await _clinicRepository.GetByIdAsync(currentClinicId.Value, tenantId);
            if (clinic == null)
            {
                return null;
            }

            // Check if there's a clinic link
            var link = await _userClinicLinkRepository.GetByUserAndClinicAsync(userId, currentClinicId.Value, tenantId);

            return new UserClinicDto
            {
                ClinicId = clinic.Id,
                ClinicName = clinic.Name,
                ClinicAddress = clinic.Address,
                IsPreferred = link?.IsPreferredClinic ?? (user.ClinicId == currentClinicId),
                IsActive = link?.IsActive ?? true,
                LinkedDate = link?.LinkedDate ?? user.CreatedAt
            };
        }
    }
}
