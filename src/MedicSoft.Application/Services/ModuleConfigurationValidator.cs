using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MedicSoft.Application.DTOs.ModuleConfigurations;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Interface for module configuration validators
    /// </summary>
    public interface IModuleConfigurationValidator
    {
        /// <summary>
        /// Validates a module configuration JSON string
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <param name="configurationJson">Configuration JSON to validate</param>
        /// <returns>Validation result with errors if any</returns>
        ConfigurationValidationResult ValidateConfiguration(string moduleName, string? configurationJson);

        /// <summary>
        /// Gets the default configuration for a module
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>Default configuration as JSON string</returns>
        string? GetDefaultConfiguration(string moduleName);
    }

    /// <summary>
    /// Result of a configuration validation
    /// </summary>
    public class ConfigurationValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();

        public ConfigurationValidationResult()
        {
            IsValid = true;
        }

        public ConfigurationValidationResult(bool isValid, params string[] errors)
        {
            IsValid = isValid;
            Errors.AddRange(errors);
        }
    }

    /// <summary>
    /// Service for validating module configurations
    /// </summary>
    public class ModuleConfigurationValidator : IModuleConfigurationValidator
    {
        public ConfigurationValidationResult ValidateConfiguration(string moduleName, string? configurationJson)
        {
            var result = new ConfigurationValidationResult();

            // If module doesn't require configuration, empty config is valid
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);
            if (!moduleInfo.RequiresConfiguration)
            {
                return result;
            }

            // If configuration is required but not provided, that's an error
            if (string.IsNullOrWhiteSpace(configurationJson))
            {
                result.IsValid = false;
                result.Errors.Add($"Configuração é obrigatória para o módulo {moduleInfo.DisplayName}");
                return result;
            }

            // Try to deserialize and validate the configuration
            try
            {
                var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                switch (moduleName)
                {
                    case SystemModules.WhatsAppIntegration:
                        validationResults = ValidateConfig<WhatsAppIntegrationConfig>(configurationJson);
                        break;

                    case SystemModules.SMSNotifications:
                        validationResults = ValidateConfig<SMSNotificationsConfig>(configurationJson);
                        break;

                    case SystemModules.TissExport:
                        validationResults = ValidateConfig<TissExportConfig>(configurationJson);
                        break;

                    case SystemModules.InventoryManagement:
                        validationResults = ValidateConfig<InventoryManagementConfig>(configurationJson);
                        break;

                    case SystemModules.Reports:
                        validationResults = ValidateConfig<ReportsConfig>(configurationJson);
                        break;

                    case SystemModules.WaitingQueue:
                        validationResults = ValidateConfig<WaitingQueueConfig>(configurationJson);
                        break;

                    case SystemModules.DoctorFieldsConfig:
                        validationResults = ValidateConfig<DoctorFieldsConfigOptions>(configurationJson);
                        break;

                    case SystemModules.Chat:
                        validationResults = ValidateConfig<ChatConfig>(configurationJson);
                        break;

                    default:
                        // For modules without specific validation, just check if it's valid JSON
                        JsonDocument.Parse(configurationJson);
                        break;
                }

                // Add validation errors to result
                foreach (var validationResult in validationResults)
                {
                    result.IsValid = false;
                    result.Errors.Add(validationResult.ErrorMessage ?? "Erro de validação desconhecido");
                }
            }
            catch (JsonException ex)
            {
                result.IsValid = false;
                result.Errors.Add($"JSON inválido: {ex.Message}");
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.Errors.Add($"Erro ao validar configuração: {ex.Message}");
            }

            return result;
        }

        private List<System.ComponentModel.DataAnnotations.ValidationResult> ValidateConfig<T>(string configurationJson) where T : ModuleConfigurationBase
        {
            var config = JsonSerializer.Deserialize<T>(configurationJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (config == null)
            {
                return new List<System.ComponentModel.DataAnnotations.ValidationResult> 
                { 
                    new System.ComponentModel.DataAnnotations.ValidationResult("Falha ao deserializar configuração") 
                };
            }

            var validationContext = new ValidationContext(config);
            return new List<System.ComponentModel.DataAnnotations.ValidationResult>(config.Validate(validationContext));
        }

        public string? GetDefaultConfiguration(string moduleName)
        {
            switch (moduleName)
            {
                case SystemModules.WhatsAppIntegration:
                    return SerializeDefault(new WhatsAppIntegrationConfig
                    {
                        ApiKey = "",
                        PhoneNumber = "",
                        EnableAppointmentReminders = true,
                        ReminderHoursBefore = 24
                    });

                case SystemModules.SMSNotifications:
                    return SerializeDefault(new SMSNotificationsConfig
                    {
                        Provider = "Twilio",
                        ApiKey = "",
                        AuthToken = "",
                        SenderId = "",
                        EnableAppointmentReminders = true,
                        DailyLimit = 500
                    });

                case SystemModules.TissExport:
                    return SerializeDefault(new TissExportConfig
                    {
                        AnsCode = "",
                        TissVersion = "3.05.00",
                        ExportPath = "",
                        GenerateXml = true,
                        GeneratePdf = true,
                        AutoSignXml = false
                    });

                case SystemModules.InventoryManagement:
                    return SerializeDefault(new InventoryManagementConfig
                    {
                        LowStockThresholdPercent = 20,
                        EnableLowStockAlerts = true,
                        TrackExpirationDates = true,
                        ExpirationAlertDays = 30
                    });

                case SystemModules.Reports:
                    return SerializeDefault(new ReportsConfig
                    {
                        DefaultFormat = "PDF",
                        EnableAutomaticGeneration = false,
                        EnableCustomTemplates = true,
                        RetentionDays = 90
                    });

                case SystemModules.WaitingQueue:
                    return SerializeDefault(new WaitingQueueConfig
                    {
                        EnableAutoProgression = true,
                        EnableDisplayScreen = true,
                        DisplayRefreshSeconds = 30,
                        MaxWaitingMinutes = 120,
                        EnablePriorityQueues = true
                    });

                case SystemModules.DoctorFieldsConfig:
                    return SerializeDefault(new DoctorFieldsConfigOptions
                    {
                        EnableCustomFields = true,
                        MaxCustomFieldsPerSpecialty = 20,
                        EnableFieldTemplates = true,
                        EnableConditionalFields = false,
                        EnableValidationRules = true
                    });

                case SystemModules.Chat:
                    return SerializeDefault(new ChatConfig
                    {
                        EnableFileSharing = true,
                        MaxFileSizeMB = 10,
                        EnableMessageHistory = true,
                        MessageRetentionDays = 90,
                        EnableGroupChats = true,
                        MaxGroupMembers = 20
                    });

                default:
                    return null;
            }
        }

        private string SerializeDefault<T>(T config)
        {
            return JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }
    }
}
