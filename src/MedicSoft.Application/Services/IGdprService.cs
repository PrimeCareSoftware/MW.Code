using System;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    public interface IGdprService
    {
        Task<byte[]> ExportClinicDataAsync(Guid clinicId, string tenantId);
        Task AnonymizeClinicAsync(Guid clinicId, string tenantId, string userId);
        Task<byte[]> ExportUserDataAsync(string userId, string tenantId);
        Task AnonymizeUserDataAsync(string userId, string tenantId, string requestedByUserId);
    }
}
