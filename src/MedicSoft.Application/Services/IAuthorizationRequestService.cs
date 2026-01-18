using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing authorization requests
    /// </summary>
    public interface IAuthorizationRequestService
    {
        /// <summary>
        /// Creates a new authorization request
        /// </summary>
        Task<AuthorizationRequestDto> CreateAsync(CreateAuthorizationRequestDto dto, string tenantId);

        /// <summary>
        /// Approves an authorization request
        /// </summary>
        Task<AuthorizationRequestDto> ApproveAsync(Guid id, ApproveAuthorizationDto dto, string tenantId);

        /// <summary>
        /// Denies an authorization request
        /// </summary>
        Task<AuthorizationRequestDto> DenyAsync(Guid id, DenyAuthorizationDto dto, string tenantId);

        /// <summary>
        /// Cancels an authorization request
        /// </summary>
        Task<AuthorizationRequestDto> CancelAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets all authorization requests for a tenant
        /// </summary>
        Task<IEnumerable<AuthorizationRequestDto>> GetAllAsync(string tenantId);

        /// <summary>
        /// Gets an authorization request by ID
        /// </summary>
        Task<AuthorizationRequestDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets authorization requests by patient ID
        /// </summary>
        Task<IEnumerable<AuthorizationRequestDto>> GetByPatientIdAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Gets authorization requests by status
        /// </summary>
        Task<IEnumerable<AuthorizationRequestDto>> GetByStatusAsync(string status, string tenantId);

        /// <summary>
        /// Gets pending authorization requests
        /// </summary>
        Task<IEnumerable<AuthorizationRequestDto>> GetPendingAsync(string tenantId);

        /// <summary>
        /// Gets an authorization request by authorization number
        /// </summary>
        Task<AuthorizationRequestDto?> GetByAuthorizationNumberAsync(string authorizationNumber, string tenantId);

        /// <summary>
        /// Checks and marks expired authorizations
        /// </summary>
        Task<int> MarkExpiredAuthorizationsAsync(string tenantId);
    }
}
