using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for generating TISS XML files
    /// </summary>
    public interface ITissXmlGeneratorService
    {
        /// <summary>
        /// Generates TISS 4.02.00 XML file for a batch
        /// </summary>
        /// <param name="batch">The batch to generate XML for</param>
        /// <param name="outputPath">The directory to save the XML file</param>
        /// <returns>The full path to the generated XML file</returns>
        Task<string> GenerateBatchXmlAsync(TissBatch batch, string outputPath);

        /// <summary>
        /// Validates an XML file against TISS schema
        /// </summary>
        /// <param name="xmlPath">Path to the XML file</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> ValidateXmlAsync(string xmlPath);

        /// <summary>
        /// Validates XML content against TISS schema
        /// </summary>
        /// <param name="xmlContent">XML content as string</param>
        /// <returns>Validation result with errors if any</returns>
        Task<TissXmlValidationResult> ValidateXmlContentAsync(string xmlContent);

        /// <summary>
        /// Gets the TISS version being used
        /// </summary>
        string GetTissVersion();
    }

    /// <summary>
    /// Result of TISS XML validation
    /// </summary>
    public class TissXmlValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public System.Collections.Generic.List<string> ValidationErrors { get; set; } = new();
    }
}
