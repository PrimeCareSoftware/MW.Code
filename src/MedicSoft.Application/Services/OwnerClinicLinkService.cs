using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IOwnerClinicLinkService
    {
        /// <summary>
        /// Links an owner to a clinic
        /// </summary>
        Task<OwnerClinicLink> LinkOwnerToClinicAsync(Guid ownerId, Guid clinicId, string tenantId, 
            bool isPrimaryOwner = true, string? role = null, decimal? ownershipPercentage = null);

        /// <summary>
        /// Gets all clinics for a specific owner
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetOwnerClinicsAsync(Guid ownerId);

        /// <summary>
        /// Gets all owners for a specific clinic
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetClinicOwnersAsync(Guid clinicId);

        /// <summary>
        /// Gets the primary owner of a clinic
        /// </summary>
        Task<OwnerClinicLink?> GetPrimaryOwnerAsync(Guid clinicId);

        /// <summary>
        /// Checks if an owner has access to a clinic
        /// </summary>
        Task<bool> HasAccessToClinicAsync(Guid ownerId, Guid clinicId);

        /// <summary>
        /// Transfers primary ownership to another owner
        /// </summary>
        Task TransferPrimaryOwnershipAsync(Guid clinicId, Guid currentPrimaryOwnerId, Guid newPrimaryOwnerId);

        /// <summary>
        /// Updates the role and ownership percentage of an owner-clinic link
        /// </summary>
        Task UpdateLinkAsync(Guid linkId, string tenantId, string? role, decimal? ownershipPercentage);

        /// <summary>
        /// Deactivates a link between owner and clinic
        /// </summary>
        Task DeactivateLinkAsync(Guid linkId, string tenantId, string reason);

        /// <summary>
        /// Reactivates a link between owner and clinic
        /// </summary>
        Task ReactivateLinkAsync(Guid linkId, string tenantId);

        /// <summary>
        /// Gets all clinics with subscription information for an owner
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetClinicsWithSubscriptionsAsync(Guid ownerId);
    }

    public class OwnerClinicLinkService : IOwnerClinicLinkService
    {
        private readonly IOwnerClinicLinkRepository _linkRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IClinicRepository _clinicRepository;

        public OwnerClinicLinkService(
            IOwnerClinicLinkRepository linkRepository,
            IOwnerRepository ownerRepository,
            IClinicRepository clinicRepository)
        {
            _linkRepository = linkRepository;
            _ownerRepository = ownerRepository;
            _clinicRepository = clinicRepository;
        }

        public async Task<OwnerClinicLink> LinkOwnerToClinicAsync(Guid ownerId, Guid clinicId, string tenantId,
            bool isPrimaryOwner = true, string? role = null, decimal? ownershipPercentage = null)
        {
            // Verify owner exists
            var owner = await _ownerRepository.GetByIdAsync(ownerId, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            // Verify clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            // Check if link already exists
            if (await _linkRepository.LinkExistsAsync(ownerId, clinicId))
                throw new InvalidOperationException("Owner is already linked to this clinic");

            // If setting as primary owner, verify no other primary owner exists
            if (isPrimaryOwner)
            {
                var existingPrimary = await _linkRepository.GetPrimaryOwnerByClinicIdAsync(clinicId);
                if (existingPrimary != null)
                    throw new InvalidOperationException("Clinic already has a primary owner");
            }

            // Create link
            var link = new OwnerClinicLink(ownerId, clinicId, tenantId, isPrimaryOwner, role, ownershipPercentage);
            await _linkRepository.AddAsync(link);

            return link;
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetOwnerClinicsAsync(Guid ownerId)
        {
            return await _linkRepository.GetClinicsByOwnerIdAsync(ownerId);
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetClinicOwnersAsync(Guid clinicId)
        {
            return await _linkRepository.GetOwnersByClinicIdAsync(clinicId);
        }

        public async Task<OwnerClinicLink?> GetPrimaryOwnerAsync(Guid clinicId)
        {
            return await _linkRepository.GetPrimaryOwnerByClinicIdAsync(clinicId);
        }

        public async Task<bool> HasAccessToClinicAsync(Guid ownerId, Guid clinicId)
        {
            return await _linkRepository.HasAccessToClinicAsync(ownerId, clinicId);
        }

        public async Task TransferPrimaryOwnershipAsync(Guid clinicId, Guid currentPrimaryOwnerId, Guid newPrimaryOwnerId)
        {
            await _linkRepository.ExecuteInTransactionAsync(async () =>
            {
                // Get current primary owner link
                var currentPrimaryLink = await _linkRepository.GetPrimaryOwnerByClinicIdAsync(clinicId);
                if (currentPrimaryLink == null || currentPrimaryLink.OwnerId != currentPrimaryOwnerId)
                    throw new InvalidOperationException("Current user is not the primary owner");

                // Get new primary owner link
                var newPrimaryLink = await _linkRepository.GetLinkAsync(newPrimaryOwnerId, clinicId);
                if (newPrimaryLink == null || !newPrimaryLink.IsActive)
                    throw new InvalidOperationException("New owner does not have access to this clinic");

                // Transfer ownership
                currentPrimaryLink.RemoveAsPrimary();
                newPrimaryLink.SetAsPrimary();

                await _linkRepository.UpdateAsync(currentPrimaryLink);
                await _linkRepository.UpdateAsync(newPrimaryLink);
            });
        }

        public async Task UpdateLinkAsync(Guid linkId, string tenantId, string? role, decimal? ownershipPercentage)
        {
            var allLinks = await _linkRepository.GetAllAsync(tenantId);
            var link = allLinks.FirstOrDefault(l => l.Id == linkId);
            if (link == null)
                throw new InvalidOperationException("Link not found");

            if (role != null)
                link.UpdateRole(role);

            if (ownershipPercentage.HasValue)
                link.UpdateOwnershipPercentage(ownershipPercentage);

            await _linkRepository.UpdateAsync(link);
        }

        public async Task DeactivateLinkAsync(Guid linkId, string tenantId, string reason)
        {
            var allLinks = await _linkRepository.GetAllAsync(tenantId);
            var link = allLinks.FirstOrDefault(l => l.Id == linkId);
            if (link == null)
                throw new InvalidOperationException("Link not found");

            // Prevent deactivation if this is the only active link for the owner
            var ownerLinks = await _linkRepository.GetClinicsByOwnerIdAsync(link.OwnerId);
            var activeLinks = ownerLinks.Where(l => l.IsActive).ToList();
            if (activeLinks.Count == 1 && activeLinks[0].Id == linkId)
                throw new InvalidOperationException("Cannot deactivate the last clinic link. Owner must have at least one active clinic.");

            // Prevent deactivation of primary owner if there are other owners
            if (link.IsPrimaryOwner)
            {
                var clinicOwners = await _linkRepository.GetOwnersByClinicIdAsync(link.ClinicId);
                var activeOwners = clinicOwners.Where(l => l.IsActive && l.Id != linkId).ToList();
                if (activeOwners.Any())
                    throw new InvalidOperationException("Cannot deactivate primary owner while other owners exist. Transfer ownership first.");
            }

            link.Deactivate(reason);
            await _linkRepository.UpdateAsync(link);
        }

        public async Task ReactivateLinkAsync(Guid linkId, string tenantId)
        {
            var allLinks = await _linkRepository.GetAllAsync(tenantId);
            var link = allLinks.FirstOrDefault(l => l.Id == linkId);
            if (link == null)
                throw new InvalidOperationException("Link not found");

            link.Reactivate();
            await _linkRepository.UpdateAsync(link);
        }

        public async Task<IEnumerable<OwnerClinicLink>> GetClinicsWithSubscriptionsAsync(Guid ownerId)
        {
            return await _linkRepository.GetClinicsWithSubscriptionsAsync(ownerId);
        }
    }
}
