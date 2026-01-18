using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAuthorizationRequestRepository : IRepository<AuthorizationRequest>
    {
        Task<AuthorizationRequest?> GetByRequestNumberAsync(string requestNumber, string tenantId);
        Task<IEnumerable<AuthorizationRequest>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<AuthorizationRequest>> GetByStatusAsync(AuthorizationStatus status, string tenantId);
        Task<IEnumerable<AuthorizationRequest>> GetPendingAuthorizationsAsync(string tenantId);
        Task<IEnumerable<AuthorizationRequest>> GetExpiredAuthorizationsAsync(string tenantId);
        Task<AuthorizationRequest?> GetByAuthorizationNumberAsync(string authorizationNumber, string tenantId);
        Task<AuthorizationRequest?> GetByIdWithDetailsAsync(Guid id, string tenantId);
        Task<IEnumerable<AuthorizationRequest>> GetAllWithDetailsAsync(string tenantId);
    }
}
