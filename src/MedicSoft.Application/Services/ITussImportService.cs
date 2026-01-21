using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for importing TUSS procedures from official ANS tables
    /// 
    /// TUSS (Terminologia Unificada da Saúde Suplementar) is the standardized healthcare procedure terminology
    /// maintained by ANS (Agência Nacional de Saúde Suplementar).
    /// 
    /// Official TUSS table can be downloaded from:
    /// https://www.gov.br/ans/pt-br/assuntos/prestadores/banco-de-dados-de-procedimentos-tuss
    /// </summary>
    public interface ITussImportService
    {
        /// <summary>
        /// Imports TUSS procedures from a CSV file
        /// </summary>
        /// <param name="csvStream">CSV file stream</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Import result with statistics</returns>
        Task<ImportResult> ImportFromCsvAsync(Stream csvStream, string tenantId);

        /// <summary>
        /// Imports TUSS procedures from an Excel file
        /// </summary>
        /// <param name="excelStream">Excel file stream</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Import result with statistics</returns>
        Task<ImportResult> ImportFromExcelAsync(Stream excelStream, string tenantId);

        /// <summary>
        /// Gets the last import status for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Last import result or null if no imports yet</returns>
        Task<ImportResult?> GetLastImportStatusAsync(string tenantId);
    }

    /// <summary>
    /// Result of TUSS import operation
    /// </summary>
    public class ImportResult
    {
        public bool Success { get; set; }
        public int TotalRecords { get; set; }
        public int SuccessfulImports { get; set; }
        public int UpdatedRecords { get; set; }
        public int FailedImports { get; set; }
        public List<ImportError> Errors { get; set; } = new();
        public TimeSpan Duration { get; set; }
        public DateTime ImportedAt { get; set; }
        public string TenantId { get; set; } = string.Empty;

        /// <summary>
        /// Summary message of the import operation
        /// </summary>
        public string GetSummary()
        {
            if (Success && FailedImports == 0)
            {
                return $"Import successful: {SuccessfulImports} procedures imported, {UpdatedRecords} updated in {Duration.TotalSeconds:F2}s";
            }
            else if (Success && FailedImports > 0)
            {
                return $"Import completed with warnings: {SuccessfulImports} imported, {UpdatedRecords} updated, {FailedImports} failed in {Duration.TotalSeconds:F2}s";
            }
            else
            {
                return $"Import failed: {Errors.Count} error(s) occurred";
            }
        }
    }

    /// <summary>
    /// Represents an error during import
    /// </summary>
    public class ImportError
    {
        public int LineNumber { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }

        public override string ToString()
        {
            var result = $"Line {LineNumber}";
            if (!string.IsNullOrEmpty(Code))
                result += $" (Code: {Code})";
            result += $": {Message}";
            if (!string.IsNullOrEmpty(Details))
                result += $" - {Details}";
            return result;
        }
    }
}
