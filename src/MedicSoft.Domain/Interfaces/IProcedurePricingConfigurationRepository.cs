using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IProcedurePricingConfigurationRepository : IRepository<ProcedurePricingConfiguration>
    {
        Task<ProcedurePricingConfiguration?> GetByProcedureAndClinicAsync(Guid procedureId, Guid clinicId, string tenantId);
        Task<IEnumerable<ProcedurePricingConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<ProcedurePricingConfiguration>> GetByProcedureIdAsync(Guid procedureId, string tenantId);
    }
}
