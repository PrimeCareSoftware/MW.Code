using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Interface for ANVISA SNGPC webservice client.
    /// Handles communication with ANVISA servers for controlled medication reporting.
    /// Reference: ANVISA RDC 27/2007
    /// </summary>
    public interface IAnvisaSngpcClient
    {
        /// <summary>
        /// Sends SNGPC XML data to ANVISA webservice.
        /// </summary>
        /// <param name="xmlContent">The SNGPC XML content conforming to ANVISA schema v2.1</param>
        /// <returns>Response from ANVISA including protocol number if successful</returns>
        Task<SngpcSendResponse> SendSngpcXmlAsync(string xmlContent);

        /// <summary>
        /// Checks the processing status of a previously submitted SNGPC report.
        /// </summary>
        /// <param name="protocolNumber">The protocol number received from ANVISA</param>
        /// <returns>Current processing status of the report</returns>
        Task<SngpcStatusResponse> CheckProtocolStatusAsync(string protocolNumber);

        /// <summary>
        /// Validates XML content against ANVISA's official XSD schema.
        /// </summary>
        /// <param name="xmlContent">The XML content to validate</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> ValidateXmlAsync(string xmlContent);
    }

    /// <summary>
    /// Response from ANVISA SNGPC submission
    /// </summary>
    public class SngpcSendResponse
    {
        public bool Success { get; set; }
        public string? ProtocolNumber { get; set; }
        public string? Message { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public int? HttpStatusCode { get; set; }
    }

    /// <summary>
    /// Status response for a submitted SNGPC report
    /// </summary>
    public class SngpcStatusResponse
    {
        public bool IsConfirmed { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
        public string? ProcessingDate { get; set; }
    }
}
