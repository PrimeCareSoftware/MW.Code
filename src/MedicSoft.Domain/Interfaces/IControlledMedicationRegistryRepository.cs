using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IControlledMedicationRegistryRepository : IRepository<ControlledMedicationRegistry>
    {
        /// <summary>
        /// Gets all registry entries for a specific period.
        /// </summary>
        Task<IEnumerable<ControlledMedicationRegistry>> GetByPeriodAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId);

        /// <summary>
        /// Gets registry entries for a specific medication.
        /// </summary>
        Task<IEnumerable<ControlledMedicationRegistry>> GetByMedicationAsync(
            string medicationName,
            string tenantId);

        /// <summary>
        /// Gets registry entries for a specific ANVISA list (A1, B1, C1, etc.).
        /// </summary>
        Task<IEnumerable<ControlledMedicationRegistry>> GetByAnvisaListAsync(
            string anvisaList,
            string tenantId);

        /// <summary>
        /// Gets the latest balance for a specific medication.
        /// </summary>
        Task<decimal> GetLatestBalanceAsync(
            string medicationName,
            string tenantId);

        /// <summary>
        /// Gets registry entries by prescription ID.
        /// </summary>
        Task<ControlledMedicationRegistry?> GetByPrescriptionIdAsync(
            Guid prescriptionId,
            string tenantId);

        /// <summary>
        /// Gets all inbound entries (stock entries) for a period.
        /// </summary>
        Task<IEnumerable<ControlledMedicationRegistry>> GetInboundEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId);

        /// <summary>
        /// Gets all outbound entries (prescriptions) for a period.
        /// </summary>
        Task<IEnumerable<ControlledMedicationRegistry>> GetOutboundEntriesAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId);

        /// <summary>
        /// Gets all medications with controlled substances.
        /// </summary>
        Task<IEnumerable<string>> GetControlledMedicationsAsync(string tenantId);

        /// <summary>
        /// Checks if a prescription has already been registered.
        /// </summary>
        Task<bool> IsPrescriptionRegisteredAsync(Guid prescriptionId, string tenantId);
    }
}
