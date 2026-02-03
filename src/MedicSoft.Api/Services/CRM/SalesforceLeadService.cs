using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// Implementation of Salesforce lead integration service
    /// </summary>
    public class SalesforceLeadService : ISalesforceLeadService
    {
        private readonly IRepository<SalesforceLead> _leadRepository;
        private readonly IRepository<SalesFunnelMetric> _funnelRepository;
        private readonly SalesforceConfiguration _config;
        private readonly ILogger<SalesforceLeadService> _logger;
        private readonly HttpClient _httpClient;
        private string? _accessToken;
        private DateTime? _tokenExpiresAt;

        public SalesforceLeadService(
            IRepository<SalesforceLead> leadRepository,
            IRepository<SalesFunnelMetric> funnelRepository,
            IOptions<SalesforceConfiguration> config,
            ILogger<SalesforceLeadService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _leadRepository = leadRepository;
            _funnelRepository = funnelRepository;
            _config = config.Value;
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("Salesforce");
        }

        public async Task<SalesforceLead> CreateLeadFromFunnelAsync(string sessionId)
        {
            try
            {
                // Get all funnel metrics for this session
                var funnelMetrics = await _funnelRepository
                    .FindAsync(m => m.SessionId == sessionId);

                if (!funnelMetrics.Any())
                {
                    throw new InvalidOperationException($"No funnel metrics found for session {sessionId}");
                }

                // Check if lead already exists
                var existingLead = await _leadRepository
                    .FindFirstAsync(l => l.SessionId == sessionId);

                if (existingLead != null)
                {
                    _logger.LogInformation("Lead already exists for session {SessionId}", sessionId);
                    return existingLead;
                }

                // Get the last step reached
                var lastStep = funnelMetrics.Max(m => m.Step);
                var lastMetric = funnelMetrics.First(m => m.Step == lastStep);

                // Parse captured data to extract contact information
                var capturedData = ParseCapturedData(funnelMetrics);

                // Create lead entity
                var lead = new SalesforceLead(
                    sessionId: sessionId,
                    leadSource: "Website Registration",
                    lastStepReached: lastStep,
                    companyName: capturedData.GetValueOrDefault("companyName"),
                    contactName: capturedData.GetValueOrDefault("contactName"),
                    email: capturedData.GetValueOrDefault("email"),
                    phone: capturedData.GetValueOrDefault("phone"),
                    city: capturedData.GetValueOrDefault("city"),
                    state: capturedData.GetValueOrDefault("state"),
                    planId: lastMetric.PlanId,
                    planName: capturedData.GetValueOrDefault("planName"),
                    referrer: lastMetric.Referrer,
                    utmCampaign: ParseUtmParameter(lastMetric.Metadata, "utm_campaign"),
                    utmSource: ParseUtmParameter(lastMetric.Metadata, "utm_source"),
                    utmMedium: ParseUtmParameter(lastMetric.Metadata, "utm_medium"),
                    metadata: lastMetric.Metadata
                );

                await _leadRepository.AddAsync(lead);
                _logger.LogInformation("Created lead {LeadId} from session {SessionId}", lead.Id, sessionId);

                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lead from funnel session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> SyncLeadToSalesforceAsync(Guid leadId)
        {
            if (!_config.Enabled)
            {
                _logger.LogWarning("Salesforce integration is disabled");
                return false;
            }

            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                // Check if already synced
                if (lead.IsSyncedToSalesforce && !string.IsNullOrEmpty(lead.SalesforceLeadId))
                {
                    _logger.LogInformation("Lead {LeadId} already synced to Salesforce", leadId);
                    return true;
                }

                // Check max sync attempts
                if (lead.SyncAttempts >= _config.MaxSyncAttempts)
                {
                    _logger.LogWarning("Lead {LeadId} exceeded max sync attempts", leadId);
                    return false;
                }

                // Authenticate and sync
                await EnsureAuthenticatedAsync();

                var salesforceLeadId = await CreateSalesforceLeadAsync(lead);
                
                if (!string.IsNullOrEmpty(salesforceLeadId))
                {
                    lead.MarkAsSynced(salesforceLeadId);
                    await _leadRepository.UpdateAsync(lead);
                    _logger.LogInformation("Successfully synced lead {LeadId} to Salesforce as {SalesforceLeadId}", 
                        leadId, salesforceLeadId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing lead {LeadId} to Salesforce", leadId);
                
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead != null)
                {
                    lead.RecordSyncFailure(ex.Message);
                    await _leadRepository.UpdateAsync(lead);
                }
                
                return false;
            }
        }

        public async Task<SyncResult> SyncAllLeadsAsync()
        {
            var result = new SyncResult();

            try
            {
                var unsyncedLeads = await GetUnsyncedLeadsAsync();
                result.TotalLeads = unsyncedLeads.Count();

                foreach (var lead in unsyncedLeads)
                {
                    var success = await SyncLeadToSalesforceAsync(lead.Id);
                    if (success)
                    {
                        result.SuccessfulSyncs++;
                    }
                    else
                    {
                        result.FailedSyncs++;
                        result.Errors.Add($"Failed to sync lead {lead.Id}");
                    }
                }

                _logger.LogInformation("Sync completed: {Success}/{Total} leads synced successfully",
                    result.SuccessfulSyncs, result.TotalLeads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk sync operation");
                result.Errors.Add($"Bulk sync error: {ex.Message}");
            }

            return result;
        }

        public async Task<IEnumerable<SalesforceLead>> GetUnsyncedLeadsAsync()
        {
            return await _leadRepository.FindAsync(l => 
                !l.IsSyncedToSalesforce && l.SyncAttempts < _config.MaxSyncAttempts);
        }

        public async Task<IEnumerable<SalesforceLead>> GetLeadsByStatusAsync(LeadStatus status)
        {
            return await _leadRepository.FindAsync(l => l.Status == status);
        }

        public async Task<bool> UpdateLeadStatusAsync(Guid leadId, LeadStatus newStatus)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null)
                {
                    return false;
                }

                lead.UpdateStatus(newStatus);
                await _leadRepository.UpdateAsync(lead);
                
                _logger.LogInformation("Updated lead {LeadId} status to {Status}", leadId, newStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead {LeadId} status", leadId);
                return false;
            }
        }

        public async Task<LeadStatistics> GetLeadStatisticsAsync()
        {
            var allLeads = await _leadRepository.GetAllAsync();
            
            var stats = new LeadStatistics
            {
                TotalLeads = allLeads.Count(),
                NewLeads = allLeads.Count(l => l.Status == LeadStatus.New),
                ContactedLeads = allLeads.Count(l => l.Status == LeadStatus.Contacted),
                QualifiedLeads = allLeads.Count(l => l.Status == LeadStatus.Qualified),
                ConvertedLeads = allLeads.Count(l => l.Status == LeadStatus.Converted),
                LostLeads = allLeads.Count(l => l.Status == LeadStatus.Lost),
                SyncedLeads = allLeads.Count(l => l.IsSyncedToSalesforce),
                UnsyncedLeads = allLeads.Count(l => !l.IsSyncedToSalesforce),
                LeadsByStep = allLeads.GroupBy(l => l.LastStepReached)
                    .ToDictionary(g => g.Key, g => g.Count())
            };

            return stats;
        }

        public async Task<bool> TestConnectionAsync()
        {
            if (!_config.Enabled)
            {
                return false;
            }

            try
            {
                await EnsureAuthenticatedAsync();
                return !string.IsNullOrEmpty(_accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Salesforce connection test failed");
                return false;
            }
        }

        #region Private Methods

        private async Task EnsureAuthenticatedAsync()
        {
            // Check if token is still valid
            if (!string.IsNullOrEmpty(_accessToken) && 
                _tokenExpiresAt.HasValue && 
                _tokenExpiresAt.Value > DateTime.UtcNow.AddMinutes(5))
            {
                return;
            }

            // Authenticate using OAuth 2.0 Password Flow
            var tokenEndpoint = $"{_config.InstanceUrl}/services/oauth2/token";
            
            // SECURITY NOTE: Credentials are in FormUrlEncodedContent which is not logged
            // HttpClient does not log request bodies by default
            // For additional security, ensure HTTP logging middleware excludes oauth2/token endpoints
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("client_id", _config.ClientId ?? ""),
                new KeyValuePair<string, string>("client_secret", _config.ClientSecret ?? ""),
                new KeyValuePair<string, string>("username", _config.Username ?? ""),
                new KeyValuePair<string, string>("password", $"{_config.Password}{_config.SecurityToken}")
            });

            var response = await _httpClient.PostAsync(tokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            var tokenResponse = await response.Content.ReadFromJsonAsync<SalesforceTokenResponse>();
            
            if (tokenResponse == null)
            {
                throw new InvalidOperationException("Failed to parse Salesforce token response");
            }

            _accessToken = tokenResponse.AccessToken;
            _tokenExpiresAt = DateTime.UtcNow.AddHours(2); // Salesforce tokens typically valid for 2 hours
            
            _logger.LogInformation("Successfully authenticated with Salesforce");
        }

        private async Task<string?> CreateSalesforceLeadAsync(SalesforceLead lead)
        {
            var leadData = new
            {
                LastName = lead.ContactName ?? "Unknown",
                Company = lead.CompanyName ?? "Unknown Company",
                Email = lead.Email,
                Phone = lead.Phone,
                City = lead.City,
                State = lead.State,
                LeadSource = lead.LeadSource,
                Status = "Open - Not Contacted",
                Description = $"Abandoned registration at step {lead.LastStepReached}/6. Plan: {lead.PlanName ?? "Not selected"}",
                // Custom fields
                Registration_Step__c = lead.LastStepReached,
                Selected_Plan__c = lead.PlanName,
                UTM_Campaign__c = lead.UtmCampaign,
                UTM_Source__c = lead.UtmSource,
                UTM_Medium__c = lead.UtmMedium,
                Session_ID__c = lead.SessionId
            };

            var json = JsonSerializer.Serialize(leadData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_accessToken}");

            var createUrl = $"{_config.InstanceUrl}/services/data/{_config.ApiVersion}/sobjects/Lead";
            var response = await _httpClient.PostAsync(createUrl, content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<SalesforceCreateResponse>();
                return result?.Id;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("Salesforce API error: {Error}", errorContent);
            return null;
        }

        private Dictionary<string, string?> ParseCapturedData(IEnumerable<SalesFunnelMetric> metrics)
        {
            var result = new Dictionary<string, string?>();

            foreach (var metric in metrics.Where(m => !string.IsNullOrEmpty(m.CapturedData)))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(metric.CapturedData!);
                    if (data != null)
                    {
                        foreach (var kvp in data)
                        {
                            var key = kvp.Key;
                            var value = kvp.Value.ValueKind == JsonValueKind.String 
                                ? kvp.Value.GetString() 
                                : kvp.Value.ToString();
                            
                            if (!result.ContainsKey(key) && !string.IsNullOrEmpty(value))
                            {
                                result[key] = value;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing captured data: {Data}", metric.CapturedData);
                }
            }

            return result;
        }

        private string? ParseUtmParameter(string? metadata, string parameterName)
        {
            if (string.IsNullOrEmpty(metadata))
            {
                return null;
            }

            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(metadata);
                if (data != null && data.TryGetValue(parameterName, out var value))
                {
                    return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing UTM parameter {Parameter}", parameterName);
            }

            return null;
        }

        #endregion

        #region Salesforce API Response Models

        private class SalesforceTokenResponse
        {
            public string AccessToken { get; set; } = null!;
            public string InstanceUrl { get; set; } = null!;
        }

        private class SalesforceCreateResponse
        {
            public string Id { get; set; } = null!;
            public bool Success { get; set; }
        }

        #endregion
    }
}
