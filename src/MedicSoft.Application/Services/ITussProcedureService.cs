using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing TUSS procedures
    /// </summary>
    public interface ITussProcedureService
    {
        /// <summary>
        /// Creates a new TUSS procedure
        /// </summary>
        Task<TussProcedureDto> CreateAsync(CreateTussProcedureDto dto, string tenantId);

        /// <summary>
        /// Updates a TUSS procedure
        /// </summary>
        Task<TussProcedureDto> UpdateAsync(Guid id, UpdateTussProcedureDto dto, string tenantId);

        /// <summary>
        /// Gets all TUSS procedures for a tenant
        /// </summary>
        Task<IEnumerable<TussProcedureDto>> GetAllAsync(string tenantId, bool includeInactive = false);

        /// <summary>
        /// Gets a TUSS procedure by ID
        /// </summary>
        Task<TussProcedureDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets a TUSS procedure by code
        /// </summary>
        Task<TussProcedureDto?> GetByCodeAsync(string code, string tenantId);

        /// <summary>
        /// Searches TUSS procedures by code or description
        /// </summary>
        Task<IEnumerable<TussProcedureDto>> SearchAsync(string query, string tenantId);

        /// <summary>
        /// Gets TUSS procedures by category
        /// </summary>
        Task<IEnumerable<TussProcedureDto>> GetByCategoryAsync(string category, string tenantId);

        /// <summary>
        /// Gets procedures that require authorization
        /// </summary>
        Task<IEnumerable<TussProcedureDto>> GetRequiringAuthorizationAsync(string tenantId);

        /// <summary>
        /// Imports TUSS procedures from a CSV file
        /// </summary>
        Task<TussImportResultDto> ImportFromCsvAsync(string filePath, string tenantId);

        /// <summary>
        /// Activates a TUSS procedure
        /// </summary>
        Task<TussProcedureDto> ActivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deactivates a TUSS procedure
        /// </summary>
        Task<TussProcedureDto> DeactivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deletes a TUSS procedure (soft delete by deactivating)
        /// </summary>
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
