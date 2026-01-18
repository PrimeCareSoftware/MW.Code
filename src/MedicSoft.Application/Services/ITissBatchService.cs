using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing TISS batches
    /// </summary>
    public interface ITissBatchService
    {
        /// <summary>
        /// Creates a new TISS batch
        /// </summary>
        Task<TissBatchDto> CreateAsync(CreateTissBatchDto dto, string tenantId);

        /// <summary>
        /// Adds a guide to a batch
        /// </summary>
        Task<TissBatchDto> AddGuideAsync(Guid batchId, Guid guideId, string tenantId);

        /// <summary>
        /// Removes a guide from a batch
        /// </summary>
        Task<TissBatchDto> RemoveGuideAsync(Guid batchId, Guid guideId, string tenantId);

        /// <summary>
        /// Generates XML for a batch
        /// </summary>
        Task<TissXmlGenerationResultDto> GenerateXmlAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Marks batch as ready to send
        /// </summary>
        Task<TissBatchDto> MarkAsReadyToSendAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Submits a batch to the operator
        /// </summary>
        Task<TissBatchDto> SubmitAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Processes operator response for a batch
        /// </summary>
        Task<TissBatchDto> ProcessResponseAsync(Guid batchId, ProcessBatchResponseDto dto, string tenantId);

        /// <summary>
        /// Marks a batch as paid
        /// </summary>
        Task<TissBatchDto> MarkAsPaidAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Rejects a batch
        /// </summary>
        Task<TissBatchDto> RejectAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Gets all batches for a tenant
        /// </summary>
        Task<IEnumerable<TissBatchDto>> GetAllAsync(string tenantId);

        /// <summary>
        /// Gets a batch by ID with guides
        /// </summary>
        Task<TissBatchDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets batches by clinic ID
        /// </summary>
        Task<IEnumerable<TissBatchDto>> GetByClinicIdAsync(Guid clinicId, string tenantId);

        /// <summary>
        /// Gets batches by operator ID
        /// </summary>
        Task<IEnumerable<TissBatchDto>> GetByOperatorIdAsync(Guid operatorId, string tenantId);

        /// <summary>
        /// Gets batches by status
        /// </summary>
        Task<IEnumerable<TissBatchDto>> GetByStatusAsync(string status, string tenantId);

        /// <summary>
        /// Downloads the XML file for a batch
        /// </summary>
        Task<byte[]?> DownloadXmlAsync(Guid batchId, string tenantId);
    }
}
