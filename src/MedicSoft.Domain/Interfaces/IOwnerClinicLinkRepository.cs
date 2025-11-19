using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IOwnerClinicLinkRepository : IRepository<OwnerClinicLink>
    {
        /// <summary>
        /// Gets all active clinics for a specific owner
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetClinicsByOwnerIdAsync(Guid ownerId);

        /// <summary>
        /// Gets all active owners for a specific clinic
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetOwnersByClinicIdAsync(Guid clinicId);

        /// <summary>
        /// Gets the primary owner of a clinic
        /// </summary>
        Task<OwnerClinicLink?> GetPrimaryOwnerByClinicIdAsync(Guid clinicId);

        /// <summary>
        /// Checks if an owner has access to a specific clinic
        /// </summary>
        Task<bool> HasAccessToClinicAsync(Guid ownerId, Guid clinicId);

        /// <summary>
        /// Gets a specific link between owner and clinic
        /// </summary>
        Task<OwnerClinicLink?> GetLinkAsync(Guid ownerId, Guid clinicId);

        /// <summary>
        /// Checks if a link already exists between owner and clinic
        /// </summary>
        Task<bool> LinkExistsAsync(Guid ownerId, Guid clinicId);

        /// <summary>
        /// Gets all clinics with their subscription status for an owner
        /// </summary>
        Task<IEnumerable<OwnerClinicLink>> GetClinicsWithSubscriptionsAsync(Guid ownerId);
    }
}
