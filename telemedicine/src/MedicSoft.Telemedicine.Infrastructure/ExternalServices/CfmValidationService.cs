using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MedicSoft.Telemedicine.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Telemedicine.Infrastructure.ExternalServices;

/// <summary>
/// CFM (Conselho Federal de Medicina) API validation service implementation
/// API Reference: https://siem-servicos-api.cfm.org.br/swagger-ui/index.html
/// </summary>
public class CfmValidationService : ICfmValidationService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<CfmValidationService> _logger;
    private const string BaseUrl = "https://siem-servicos-api.cfm.org.br";

    public CfmValidationService(
        HttpClient httpClient, 
        IConfiguration configuration,
        ILogger<CfmValidationService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<CfmCrmValidationResult> ValidateCrmAsync(string crmNumber, string crmState)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(crmNumber))
            {
                return new CfmCrmValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "CRM number is required"
                };
            }

            if (string.IsNullOrWhiteSpace(crmState))
            {
                return new CfmCrmValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "CRM state is required"
                };
            }

            // Clean CRM number (remove non-numeric characters)
            var cleanCrmNumber = new string(crmNumber.Where(char.IsDigit).ToArray());
            var cleanCrmState = crmState.Trim().ToUpperInvariant();

            _logger.LogInformation("Validating CRM {CrmNumber}-{CrmState} with CFM API", cleanCrmNumber, cleanCrmState);

            // CFM API endpoint for CRM validation
            // Note: The actual CFM API endpoint structure should be confirmed with official documentation
            // at https://siem-servicos-api.cfm.org.br/swagger-ui/index.html
            // This implementation uses a common pattern for Brazilian government APIs
            // TODO: Verify and update endpoint path once CFM API documentation is accessible
            var url = $"/api/consulta-crm?numero={cleanCrmNumber}&uf={cleanCrmState}";
            
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("CFM API returned status {StatusCode} for CRM {CrmNumber}-{CrmState}", 
                    response.StatusCode, cleanCrmNumber, cleanCrmState);
                
                // If the API returns 404, it means the CRM is not found (invalid)
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new CfmCrmValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "CRM not found in CFM database"
                    };
                }
                
                return new CfmCrmValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"CFM API error: {response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<CfmCrmApiResponse>();
            
            if (result == null)
            {
                return new CfmCrmValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Failed to parse CFM API response"
                };
            }

            _logger.LogInformation("CRM {CrmNumber}-{CrmState} validated successfully. Doctor: {DoctorName}", 
                cleanCrmNumber, cleanCrmState, result.Nome);

            return new CfmCrmValidationResult
            {
                IsValid = true,
                DoctorName = result.Nome,
                CrmNumber = cleanCrmNumber,
                CrmState = cleanCrmState,
                Specialty = result.Especialidade,
                Status = result.Situacao,
                RegistrationDate = result.DataInscricao
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error validating CRM {CrmNumber}-{CrmState}", crmNumber, crmState);
            return new CfmCrmValidationResult
            {
                IsValid = false,
                ErrorMessage = "Failed to connect to CFM API"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CRM {CrmNumber}-{CrmState}", crmNumber, crmState);
            return new CfmCrmValidationResult
            {
                IsValid = false,
                ErrorMessage = "Internal error during validation"
            };
        }
    }

    public async Task<CfmCpfValidationResult> ValidateCpfAsync(string cpf)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return new CfmCpfValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "CPF is required"
                };
            }

            // Clean CPF (remove non-numeric characters)
            var cleanCpf = new string(cpf.Where(char.IsDigit).ToArray());

            if (cleanCpf.Length != 11)
            {
                return new CfmCpfValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "CPF must have 11 digits"
                };
            }

            _logger.LogInformation("Validating CPF with CFM API (masked for security)");

            // CFM API endpoint for CPF validation
            // Note: The actual CFM API endpoint structure should be confirmed with official documentation
            // at https://siem-servicos-api.cfm.org.br/swagger-ui/index.html
            // TODO: Verify and update endpoint path once CFM API documentation is accessible
            var url = $"/api/consulta-cpf?cpf={cleanCpf}";
            
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("CFM API returned status {StatusCode} for CPF validation", response.StatusCode);
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return new CfmCpfValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "CPF not found in CFM database"
                    };
                }
                
                return new CfmCpfValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"CFM API error: {response.StatusCode}"
                };
            }

            var result = await response.Content.ReadFromJsonAsync<CfmCpfApiResponse>();
            
            if (result == null)
            {
                return new CfmCpfValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Failed to parse CFM API response"
                };
            }

            _logger.LogInformation("CPF validated successfully with CFM API");

            return new CfmCpfValidationResult
            {
                IsValid = result.Valido,
                Cpf = cleanCpf
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error validating CPF");
            return new CfmCpfValidationResult
            {
                IsValid = false,
                ErrorMessage = "Failed to connect to CFM API"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CPF");
            return new CfmCpfValidationResult
            {
                IsValid = false,
                ErrorMessage = "Internal error during validation"
            };
        }
    }

    #region Response DTOs

    private class CfmCrmApiResponse
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; } = string.Empty;

        [JsonPropertyName("numero")]
        public string Numero { get; set; } = string.Empty;

        [JsonPropertyName("uf")]
        public string Uf { get; set; } = string.Empty;

        [JsonPropertyName("especialidade")]
        public string? Especialidade { get; set; }

        [JsonPropertyName("situacao")]
        public string? Situacao { get; set; }

        [JsonPropertyName("dataInscricao")]
        public DateTime? DataInscricao { get; set; }
    }

    private class CfmCpfApiResponse
    {
        [JsonPropertyName("valido")]
        public bool Valido { get; set; }

        [JsonPropertyName("cpf")]
        public string? Cpf { get; set; }
    }

    #endregion
}
