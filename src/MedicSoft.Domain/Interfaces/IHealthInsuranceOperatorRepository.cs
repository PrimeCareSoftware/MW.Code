using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IHealthInsuranceOperatorRepository : IRepository<HealthInsuranceOperator>
    {
        Task<HealthInsuranceOperator?> GetByRegisterNumberAsync(string registerNumber, string tenantId);
        Task<HealthInsuranceOperator?> GetByDocumentAsync(string document, string tenantId);
        Task<IEnumerable<HealthInsuranceOperator>> SearchByNameAsync(string name, string tenantId);
        Task<IEnumerable<HealthInsuranceOperator>> GetActiveOperatorsAsync(string tenantId);
        Task<bool> IsRegisterNumberUniqueAsync(string registerNumber, string tenantId, Guid? excludeId = null);
    }
}
