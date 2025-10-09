using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPrescriptionItemRepository : IRepository<PrescriptionItem>
    {
        Task<IEnumerable<PrescriptionItem>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
        Task<IEnumerable<PrescriptionItem>> GetByMedicationIdAsync(Guid medicationId, string tenantId);
    }
}
