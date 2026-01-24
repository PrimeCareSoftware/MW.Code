using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Implementation of ANVISA SNGPC webservice client.
    /// Handles XML transmission, validation, and status checking.
    /// Reference: ANVISA RDC 27/2007 - Sistema Nacional de Gerenciamento de Produtos Controlados
    /// </summary>
    public class AnvisaSngpcClient : IAnvisaSngpcClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AnvisaSngpcClient> _logger;
        private readonly string _xsdSchemaPath;

        public AnvisaSngpcClient(
            HttpClient httpClient, 
            IConfiguration configuration, 
            ILogger<AnvisaSngpcClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Configure base URL (homologation or production)
            var baseUrl = configuration["Anvisa:Sngpc:BaseUrl"] ?? "https://sngpc.anvisa.gov.br/api";
            _httpClient.BaseAddress = new Uri(baseUrl);

            // Configure timeout
            var timeoutSeconds = configuration.GetValue<int>("Anvisa:Sngpc:TimeoutSeconds", 60);
            _httpClient.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

            // Set up authentication if needed
            var apiKey = configuration["Anvisa:Sngpc:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-Key", apiKey);
            }

            // Path to XSD schema for validation
            _xsdSchemaPath = configuration["Anvisa:Sngpc:XsdSchemaPath"] ?? "docs/schemas/sngpc_v2.1.xsd";

            _logger.LogInformation("ANVISA SNGPC Client initialized with base URL: {BaseUrl}", baseUrl);
        }

        public async Task<SngpcSendResponse> SendSngpcXmlAsync(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                throw new ArgumentException("XML content cannot be empty", nameof(xmlContent));
            }

            _logger.LogInformation("Sending SNGPC XML to ANVISA (Size: {Size} bytes)", xmlContent.Length);

            try
            {
                // Validate XML before sending
                var isValid = await ValidateXmlAsync(xmlContent);
                if (!isValid)
                {
                    _logger.LogError("XML validation failed before sending to ANVISA");
                    return new SngpcSendResponse
                    {
                        Success = false,
                        ErrorMessage = "XML validation failed against ANVISA schema",
                        ErrorCode = "VALIDATION_ERROR"
                    };
                }

                // ANVISA endpoint for SNGPC submission
                var endpoint = _configuration["Anvisa:Sngpc:SubmitEndpoint"] ?? "/sngpc/envio";

                var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");

                var response = await _httpClient.PostAsync(endpoint, content);

                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("SNGPC XML sent successfully to ANVISA");

                    var protocolNumber = ExtractProtocolNumber(responseContent);
                    var message = ExtractMessage(responseContent);

                    return new SngpcSendResponse
                    {
                        Success = true,
                        ProtocolNumber = protocolNumber,
                        Message = message ?? "Enviado com sucesso",
                        HttpStatusCode = (int)response.StatusCode
                    };
                }
                else
                {
                    _logger.LogError(
                        "ANVISA rejected SNGPC XML. Status: {StatusCode}, Response: {Response}",
                        response.StatusCode, responseContent);

                    var errorCode = ExtractErrorCode(responseContent);
                    var errorMessage = ExtractErrorMessage(responseContent);

                    return new SngpcSendResponse
                    {
                        Success = false,
                        ErrorMessage = errorMessage ?? responseContent,
                        ErrorCode = errorCode ?? response.StatusCode.ToString(),
                        HttpStatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while sending SNGPC XML to ANVISA");
                return new SngpcSendResponse
                {
                    Success = false,
                    ErrorMessage = $"Erro de comunicação com ANVISA: {ex.Message}",
                    ErrorCode = "HTTP_ERROR"
                };
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while sending SNGPC XML to ANVISA");
                return new SngpcSendResponse
                {
                    Success = false,
                    ErrorMessage = "Timeout ao enviar dados para ANVISA",
                    ErrorCode = "TIMEOUT"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending SNGPC XML to ANVISA");
                return new SngpcSendResponse
                {
                    Success = false,
                    ErrorMessage = $"Erro inesperado: {ex.Message}",
                    ErrorCode = "UNEXPECTED_ERROR"
                };
            }
        }

        public async Task<SngpcStatusResponse> CheckProtocolStatusAsync(string protocolNumber)
        {
            if (string.IsNullOrWhiteSpace(protocolNumber))
            {
                throw new ArgumentException("Protocol number cannot be empty", nameof(protocolNumber));
            }

            _logger.LogInformation("Checking SNGPC protocol status: {ProtocolNumber}", protocolNumber);

            try
            {
                var endpoint = _configuration["Anvisa:Sngpc:StatusEndpoint"] ?? "/sngpc/consulta";
                var requestUrl = $"{endpoint}/{protocolNumber}";

                var response = await _httpClient.GetAsync(requestUrl);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully retrieved status for protocol {ProtocolNumber}", protocolNumber);

                    var status = ExtractStatus(responseContent);
                    var message = ExtractMessage(responseContent);
                    var processingDate = ExtractProcessingDate(responseContent);

                    return new SngpcStatusResponse
                    {
                        IsConfirmed = status == "PROCESSADO" || status == "CONFIRMADO",
                        Status = status ?? "UNKNOWN",
                        Message = message,
                        ProcessingDate = processingDate
                    };
                }
                else
                {
                    _logger.LogWarning(
                        "Failed to retrieve status for protocol {ProtocolNumber}. Status: {StatusCode}",
                        protocolNumber, response.StatusCode);

                    return new SngpcStatusResponse
                    {
                        IsConfirmed = false,
                        Status = "ERRO",
                        Message = $"Erro ao consultar protocolo: {response.StatusCode}"
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking protocol {ProtocolNumber} status", protocolNumber);
                return new SngpcStatusResponse
                {
                    IsConfirmed = false,
                    Status = "ERRO",
                    Message = $"Erro ao consultar protocolo: {ex.Message}"
                };
            }
        }

        public async Task<bool> ValidateXmlAsync(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
            {
                _logger.LogWarning("XML content is empty, skipping validation");
                return false;
            }

            try
            {
                // Check if XSD schema file exists
                if (!File.Exists(_xsdSchemaPath))
                {
                    _logger.LogWarning(
                        "XSD schema file not found at {SchemaPath}, skipping validation",
                        _xsdSchemaPath);
                    // In production, you might want to return false here
                    // For now, we'll allow it to proceed with basic XML validation
                    return await ValidateBasicXmlStructure(xmlContent);
                }

                var schemas = new XmlSchemaSet();
                schemas.Add(null, _xsdSchemaPath);

                var document = XDocument.Parse(xmlContent);
                var isValid = true;
                var validationErrors = new System.Text.StringBuilder();

                document.Validate(schemas, (sender, args) =>
                {
                    isValid = false;
                    validationErrors.AppendLine($"{args.Severity}: {args.Message}");
                });

                if (!isValid)
                {
                    _logger.LogError("XML validation failed against ANVISA schema:\n{Errors}", validationErrors.ToString());
                }
                else
                {
                    _logger.LogInformation("XML successfully validated against ANVISA schema");
                }

                return isValid;
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error during validation");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during XML validation");
                return false;
            }
        }

        #region Private Helper Methods

        /// <summary>
        /// Performs basic XML structure validation when XSD is not available
        /// </summary>
        private async Task<bool> ValidateBasicXmlStructure(string xmlContent)
        {
            try
            {
                var document = XDocument.Parse(xmlContent);
                
                // Check for root element
                if (document.Root == null)
                {
                    _logger.LogError("XML has no root element");
                    return false;
                }

                // Check for required elements (basic SNGPC structure)
                var ns = document.Root.GetDefaultNamespace();
                var hasHeader = document.Root.Element(ns + "Cabecalho") != null;
                var hasReceipts = document.Root.Element(ns + "Receitas") != null;

                if (!hasHeader || !hasReceipts)
                {
                    _logger.LogError("XML is missing required SNGPC elements (Cabecalho or Receitas)");
                    return false;
                }

                _logger.LogInformation("XML passed basic structure validation");
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during basic XML validation");
                return false;
            }
        }

        /// <summary>
        /// Extracts protocol number from ANVISA XML response
        /// </summary>
        private string? ExtractProtocolNumber(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                // Try different possible element names
                return doc.Descendants("protocolo").FirstOrDefault()?.Value
                    ?? doc.Descendants("protocolNumber").FirstOrDefault()?.Value
                    ?? doc.Descendants("numeroProtocolo").FirstOrDefault()?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract protocol number from response");
                return null;
            }
        }

        /// <summary>
        /// Extracts status from ANVISA XML response
        /// </summary>
        private string? ExtractStatus(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.Descendants("status").FirstOrDefault()?.Value
                    ?? doc.Descendants("situacao").FirstOrDefault()?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract status from response");
                return null;
            }
        }

        /// <summary>
        /// Extracts message from ANVISA XML response
        /// </summary>
        private string? ExtractMessage(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.Descendants("mensagem").FirstOrDefault()?.Value
                    ?? doc.Descendants("message").FirstOrDefault()?.Value
                    ?? doc.Descendants("descricao").FirstOrDefault()?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract message from response");
                return null;
            }
        }

        /// <summary>
        /// Extracts error code from ANVISA XML response
        /// </summary>
        private string? ExtractErrorCode(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.Descendants("codigoErro").FirstOrDefault()?.Value
                    ?? doc.Descendants("errorCode").FirstOrDefault()?.Value
                    ?? doc.Descendants("codigo").FirstOrDefault()?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract error code from response");
                return null;
            }
        }

        /// <summary>
        /// Extracts error message from ANVISA XML response
        /// </summary>
        private string? ExtractErrorMessage(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.Descendants("mensagemErro").FirstOrDefault()?.Value
                    ?? doc.Descendants("errorMessage").FirstOrDefault()?.Value
                    ?? doc.Descendants("erro").FirstOrDefault()?.Value
                    ?? ExtractMessage(xml);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract error message from response");
                return null;
            }
        }

        /// <summary>
        /// Extracts processing date from ANVISA XML response
        /// </summary>
        private string? ExtractProcessingDate(string xml)
        {
            try
            {
                var doc = XDocument.Parse(xml);
                return doc.Descendants("dataProcessamento").FirstOrDefault()?.Value
                    ?? doc.Descendants("processingDate").FirstOrDefault()?.Value
                    ?? doc.Descendants("data").FirstOrDefault()?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract processing date from response");
                return null;
            }
        }

        #endregion
    }
}
