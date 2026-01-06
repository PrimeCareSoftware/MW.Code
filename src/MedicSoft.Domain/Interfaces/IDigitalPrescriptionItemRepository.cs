using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IDigitalPrescriptionItemRepository : IRepository<DigitalPrescriptionItem>
    {
        /// <summary>
        /// Gets all items for a specific prescription.
        /// </summary>
        Task<IEnumerable<DigitalPrescriptionItem>> GetByPrescriptionIdAsync(Guid prescriptionId, string tenantId);

        /// <summary>
        /// Gets all items containing a specific medication.
        /// </summary>
        Task<IEnumerable<DigitalPrescriptionItem>> GetByMedicationIdAsync(Guid medicationId, string tenantId);

        /// <summary>
        /// Gets all controlled substance items within a date range (for SNGPC reporting).
        /// </summary>
        Task<IEnumerable<DigitalPrescriptionItem>> GetControlledSubstancesAsync(DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets items by controlled substance list (for ANVISA reporting).
        /// </summary>
        Task<IEnumerable<DigitalPrescriptionItem>> GetByControlledListAsync(ControlledSubstanceList controlledList, DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets statistics on medication usage for a clinic.
        /// </summary>
        Task<Dictionary<Guid, int>> GetMedicationUsageStatsAsync(DateTime startDate, DateTime endDate, string tenantId);
    }
}
