using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing TISS guides
    /// </summary>
    public interface ITissGuideService
    {
        /// <summary>
        /// Creates a new TISS guide
        /// </summary>
        Task<TissGuideDto> CreateAsync(CreateTissGuideDto dto, string tenantId);

        /// <summary>
        /// Adds a procedure to a TISS guide
        /// </summary>
        Task<TissGuideDto> AddProcedureAsync(Guid guideId, AddProcedureToGuideDto dto, string tenantId);

        /// <summary>
        /// Removes a procedure from a TISS guide
        /// </summary>
        Task<TissGuideDto> RemoveProcedureAsync(Guid guideId, Guid procedureId, string tenantId);

        /// <summary>
        /// Finalizes a TISS guide (marks as ready to send)
        /// </summary>
        Task<TissGuideDto> FinalizeAsync(Guid guideId, string tenantId);

        /// <summary>
        /// Gets all TISS guides for a tenant
        /// </summary>
        Task<IEnumerable<TissGuideDto>> GetAllAsync(string tenantId);

        /// <summary>
        /// Gets a TISS guide by ID with procedures
        /// </summary>
        Task<TissGuideDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets TISS guides by batch ID
        /// </summary>
        Task<IEnumerable<TissGuideDto>> GetByBatchIdAsync(Guid batchId, string tenantId);

        /// <summary>
        /// Gets TISS guides by appointment ID
        /// </summary>
        Task<IEnumerable<TissGuideDto>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);

        /// <summary>
        /// Gets TISS guides by status
        /// </summary>
        Task<IEnumerable<TissGuideDto>> GetByStatusAsync(string status, string tenantId);

        /// <summary>
        /// Gets a TISS guide by guide number
        /// </summary>
        Task<TissGuideDto?> GetByGuideNumberAsync(string guideNumber, string tenantId);

        /// <summary>
        /// Processes operator response for a guide
        /// </summary>
        Task<TissGuideDto> ProcessResponseAsync(Guid guideId, ProcessGuideResponseDto dto, string tenantId);

        /// <summary>
        /// Marks a guide as paid
        /// </summary>
        Task<TissGuideDto> MarkAsPaidAsync(Guid guideId, string tenantId);
    }
}
