using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPrescriptionSequenceControlRepository : IRepository<PrescriptionSequenceControl>
    {
        /// <summary>
        /// Gets the sequence control for a specific prescription type and tenant.
        /// </summary>
        Task<PrescriptionSequenceControl?> GetByTypeAsync(PrescriptionType type, string tenantId);

        /// <summary>
        /// Gets or creates the sequence control for a prescription type.
        /// This ensures a sequence control always exists for generation.
        /// </summary>
        Task<PrescriptionSequenceControl> GetOrCreateByTypeAsync(PrescriptionType type, string tenantId, string? prefix = null);

        /// <summary>
        /// Generates the next sequence number for a prescription type atomically.
        /// This method handles concurrency to prevent duplicate sequence numbers.
        /// </summary>
        Task<string> GenerateNextSequenceAsync(PrescriptionType type, string tenantId, string? prefix = null);

        /// <summary>
        /// Previews the next sequence number without incrementing.
        /// </summary>
        Task<string> PreviewNextSequenceAsync(PrescriptionType type, string tenantId);
    }
}
