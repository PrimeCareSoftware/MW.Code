using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient?> GetByDocumentAsync(string document, string tenantId);
        Task<Patient?> GetByDocumentGlobalAsync(string document);
        Task<Patient?> GetByEmailAsync(string email, string tenantId);
        Task<IEnumerable<Patient>> SearchByNameAsync(string name, string tenantId);
        Task<IEnumerable<Patient>> SearchByPhoneAsync(string phoneNumber, string tenantId);
        Task<IEnumerable<Patient>> SearchAsync(string searchTerm, string tenantId);
        Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, string tenantId, Guid? excludeId = null);
    }
}