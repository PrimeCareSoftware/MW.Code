using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents module permissions and access configuration for clinics
    /// </summary>
    public class ModuleConfiguration : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public string ModuleName { get; private set; }
        public bool IsEnabled { get; private set; }
        public string? Configuration { get; private set; } // JSON configuration

        // Navigation properties
        public Clinic? Clinic { get; private set; }

        private ModuleConfiguration()
        {
            // EF Constructor
            ModuleName = null!;
        }

        public ModuleConfiguration(Guid clinicId, string moduleName, string tenantId,
            bool isEnabled = true, string? configuration = null) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name cannot be empty", nameof(moduleName));

            ClinicId = clinicId;
            ModuleName = moduleName.Trim();
            IsEnabled = isEnabled;
            Configuration = configuration;
        }

        public void Enable()
        {
            IsEnabled = true;
            UpdateTimestamp();
        }

        public void Disable()
        {
            IsEnabled = false;
            UpdateTimestamp();
        }

        public void UpdateConfiguration(string? configuration)
        {
            Configuration = configuration;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Available modules in the system
    /// </summary>
    public static class SystemModules
    {
        public const string PatientManagement = "PatientManagement";
        public const string AppointmentScheduling = "AppointmentScheduling";
        public const string MedicalRecords = "MedicalRecords";
        public const string Prescriptions = "Prescriptions";
        public const string FinancialManagement = "FinancialManagement";
        public const string Reports = "Reports";
        public const string WhatsAppIntegration = "WhatsAppIntegration";
        public const string SMSNotifications = "SMSNotifications";
        public const string TissExport = "TissExport";
        public const string InventoryManagement = "InventoryManagement";
        public const string UserManagement = "UserManagement";
        public const string WaitingQueue = "WaitingQueue";
        public const string DoctorFieldsConfig = "DoctorFieldsConfig";
        public const string Chat = "Chat";

        /// <summary>
        /// Get detailed information about all available modules
        /// </summary>
        public static Dictionary<string, ModuleInfo> GetModulesInfo() => new()
        {
            [PatientManagement] = new ModuleInfo
            {
                Name = PatientManagement,
                DisplayName = "Gestão de Pacientes",
                Description = "Cadastro, edição e consulta de pacientes",
                Category = "Core",
                Icon = "people",
                IsCore = true,
                RequiredModules = new[] { UserManagement },
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [AppointmentScheduling] = new ModuleInfo
            {
                Name = AppointmentScheduling,
                DisplayName = "Agendamento de Consultas",
                Description = "Sistema de agendamento e controle de horários",
                Category = "Core",
                Icon = "calendar_today",
                IsCore = true,
                RequiredModules = new[] { PatientManagement },
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [MedicalRecords] = new ModuleInfo
            {
                Name = MedicalRecords,
                DisplayName = "Prontuário Eletrônico",
                Description = "Gerenciamento de prontuários médicos eletrônicos",
                Category = "Core",
                Icon = "description",
                IsCore = true,
                RequiredModules = new[] { PatientManagement },
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [Prescriptions] = new ModuleInfo
            {
                Name = Prescriptions,
                DisplayName = "Prescrições Médicas",
                Description = "Geração e gerenciamento de prescrições médicas",
                Category = "Core",
                Icon = "local_pharmacy",
                IsCore = true,
                RequiredModules = new[] { MedicalRecords },
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [FinancialManagement] = new ModuleInfo
            {
                Name = FinancialManagement,
                DisplayName = "Gestão Financeira",
                Description = "Controle financeiro, faturamento e pagamentos",
                Category = "Core",
                Icon = "attach_money",
                IsCore = true,
                RequiredModules = Array.Empty<string>(),
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [UserManagement] = new ModuleInfo
            {
                Name = UserManagement,
                DisplayName = "Gestão de Usuários",
                Description = "Gerenciamento de usuários e permissões",
                Category = "Core",
                Icon = "group",
                IsCore = true,
                RequiredModules = Array.Empty<string>(),
                MinimumPlan = SubscriptionPlanType.Basic
            },
            [Reports] = new ModuleInfo
            {
                Name = Reports,
                DisplayName = "Relatórios Avançados",
                Description = "Geração de relatórios e dashboards avançados",
                Category = "Analytics",
                Icon = "assessment",
                IsCore = false,
                RequiredModules = Array.Empty<string>(),
                MinimumPlan = SubscriptionPlanType.Standard,
                RequiresConfiguration = true,
                ConfigurationType = "ReportsConfig",
                ConfigurationExample = @"{
  ""defaultFormat"": ""PDF"",
  ""enableAutomaticGeneration"": false,
  ""enableCustomTemplates"": true,
  ""retentionDays"": 90
}",
                ConfigurationHelp = "Configure a geração de relatórios avançados. Defina formato padrão, habilite geração automática e templates personalizados."
            },
            [WhatsAppIntegration] = new ModuleInfo
            {
                Name = WhatsAppIntegration,
                DisplayName = "Integração WhatsApp",
                Description = "Integração com WhatsApp para comunicação com pacientes",
                Category = "Advanced",
                Icon = "chat",
                IsCore = false,
                RequiredModules = new[] { PatientManagement },
                MinimumPlan = SubscriptionPlanType.Standard,
                RequiresConfiguration = true,
                ConfigurationType = "WhatsAppIntegrationConfig",
                ConfigurationExample = @"{
  ""apiKey"": ""sua_api_key_aqui"",
  ""phoneNumber"": ""+5511999999999"",
  ""webhookUrl"": ""https://suaurl.com/webhook"",
  ""enableAppointmentReminders"": true,
  ""reminderHoursBefore"": 24
}",
                ConfigurationHelp = "Configure a API do WhatsApp Business para enviar mensagens automáticas aos pacientes. Você precisará de uma API Key válida e número de telefone verificado."
            },
            [SMSNotifications] = new ModuleInfo
            {
                Name = SMSNotifications,
                DisplayName = "Notificações SMS",
                Description = "Envio de notificações via SMS",
                Category = "Advanced",
                Icon = "sms",
                IsCore = false,
                RequiredModules = new[] { PatientManagement },
                MinimumPlan = SubscriptionPlanType.Standard,
                RequiresConfiguration = true,
                ConfigurationType = "SMSNotificationsConfig",
                ConfigurationExample = @"{
  ""provider"": ""Twilio"",
  ""apiKey"": ""sua_account_sid"",
  ""authToken"": ""seu_auth_token"",
  ""senderId"": ""+5511888888888"",
  ""enableAppointmentReminders"": true,
  ""dailyLimit"": 500
}",
                ConfigurationHelp = "Configure um provedor de SMS (Twilio, Nexmo, AWS SNS) para enviar notificações aos pacientes. Você precisará de credenciais da API do provedor escolhido."
            },
            [TissExport] = new ModuleInfo
            {
                Name = TissExport,
                DisplayName = "Exportação TISS",
                Description = "Exportação de guias no padrão TISS",
                Category = "Premium",
                Icon = "upload_file",
                IsCore = false,
                RequiredModules = new[] { FinancialManagement },
                MinimumPlan = SubscriptionPlanType.Premium,
                RequiresConfiguration = true,
                ConfigurationType = "TissExportConfig",
                ConfigurationExample = @"{
  ""ansCode"": ""123456"",
  ""tissVersion"": ""3.05.00"",
  ""exportPath"": ""C:\\TISS\\Export"",
  ""generateXml"": true,
  ""generatePdf"": true,
  ""autoSignXml"": false
}",
                ConfigurationHelp = "Configure a exportação de guias TISS com seu código ANS e diretório de exportação. Opcionalmente, configure assinatura digital automática dos arquivos XML."
            },
            [InventoryManagement] = new ModuleInfo
            {
                Name = InventoryManagement,
                DisplayName = "Gestão de Estoque",
                Description = "Controle de estoque de medicamentos e materiais",
                Category = "Advanced",
                Icon = "inventory",
                IsCore = false,
                RequiredModules = Array.Empty<string>(),
                MinimumPlan = SubscriptionPlanType.Standard,
                RequiresConfiguration = true,
                ConfigurationType = "InventoryManagementConfig",
                ConfigurationExample = @"{
  ""lowStockThresholdPercent"": 20,
  ""enableLowStockAlerts"": true,
  ""alertEmails"": ""estoque@clinica.com,gerente@clinica.com"",
  ""trackExpirationDates"": true,
  ""expirationAlertDays"": 30
}",
                ConfigurationHelp = "Configure alertas de estoque baixo e controle de validade. Defina o percentual mínimo de estoque e os emails para receber notificações."
            },
            [WaitingQueue] = new ModuleInfo
            {
                Name = WaitingQueue,
                DisplayName = "Fila de Espera",
                Description = "Gerenciamento de fila de espera de pacientes",
                Category = "Advanced",
                Icon = "queue",
                IsCore = false,
                RequiredModules = new[] { AppointmentScheduling },
                MinimumPlan = SubscriptionPlanType.Standard,
                RequiresConfiguration = true,
                ConfigurationType = "WaitingQueueConfig",
                ConfigurationExample = @"{
  ""enableAutoProgression"": true,
  ""enableDisplayScreen"": true,
  ""displayRefreshSeconds"": 30,
  ""maxWaitingMinutes"": 120,
  ""enablePriorityQueues"": true
}",
                ConfigurationHelp = "Configure a fila de espera com progressão automática e tela de exibição. Defina o tempo máximo de espera e intervalo de atualização da tela."
            },
            [DoctorFieldsConfig] = new ModuleInfo
            {
                Name = DoctorFieldsConfig,
                DisplayName = "Configuração de Campos",
                Description = "Configuração personalizada de campos do prontuário",
                Category = "Premium",
                Icon = "settings",
                IsCore = false,
                RequiredModules = new[] { MedicalRecords },
                MinimumPlan = SubscriptionPlanType.Premium,
                RequiresConfiguration = true,
                ConfigurationType = "DoctorFieldsConfigOptions",
                ConfigurationExample = @"{
  ""enableCustomFields"": true,
  ""maxCustomFieldsPerSpecialty"": 20,
  ""enableFieldTemplates"": true,
  ""enableConditionalFields"": false,
  ""enableValidationRules"": true
}",
                ConfigurationHelp = "Configure campos personalizados no prontuário eletrônico. Defina limites, habilite templates e regras de validação para os campos customizados."
            },
            [Chat] = new ModuleInfo
            {
                Name = Chat,
                DisplayName = "Chat Interno",
                Description = "Sistema de mensagens instantâneas para comunicação entre usuários da clínica",
                Category = "Core",
                Icon = "chat",
                IsCore = false,
                RequiredModules = Array.Empty<string>(),
                MinimumPlan = SubscriptionPlanType.Basic,
                RequiresConfiguration = true,
                ConfigurationType = "ChatConfig",
                ConfigurationExample = @"{
  ""enableFileSharing"": true,
  ""maxFileSizeMB"": 10,
  ""enableMessageHistory"": true,
  ""messageRetentionDays"": 90,
  ""enableGroupChats"": true,
  ""maxGroupMembers"": 20
}",
                ConfigurationHelp = "Configure o chat interno da clínica. Defina limites de tamanho de arquivo, retenção de mensagens e número máximo de membros em grupos."
            }
        };

        public static string[] GetAllModules() => 
            GetModulesInfo().Keys.ToArray();

        public static ModuleInfo GetModuleInfo(string moduleName)
        {
            if (!GetModulesInfo().TryGetValue(moduleName, out var info))
                throw new ArgumentException($"Module {moduleName} not found", nameof(moduleName));
            
            return info;
        }

        public static bool IsModuleAvailableInPlan(string moduleName, SubscriptionPlan plan)
        {
            return moduleName switch
            {
                Reports => plan.HasReports,
                WhatsAppIntegration => plan.HasWhatsAppIntegration,
                SMSNotifications => plan.HasSMSNotifications,
                TissExport => plan.HasTissExport,
                _ => true // Basic modules available in all plans
            };
        }
    }

    /// <summary>
    /// Detailed information about a module
    /// </summary>
    public class ModuleInfo
    {
        public string Name { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "Core", "Advanced", "Premium", "Analytics"
        public string Icon { get; set; } = string.Empty; // Material icon name
        public bool IsCore { get; set; } // If true, cannot be disabled
        public string[] RequiredModules { get; set; } = Array.Empty<string>();
        public SubscriptionPlanType MinimumPlan { get; set; }
        
        /// <summary>
        /// Indicates if this module requires specific configuration
        /// </summary>
        public bool RequiresConfiguration { get; set; }
        
        /// <summary>
        /// Type name of the configuration DTO for this module (e.g., "WhatsAppIntegrationConfig")
        /// </summary>
        public string? ConfigurationType { get; set; }
        
        /// <summary>
        /// Example configuration JSON to help users understand the required format
        /// </summary>
        public string? ConfigurationExample { get; set; }
        
        /// <summary>
        /// Help text describing what this module does and how to configure it
        /// </summary>
        public string? ConfigurationHelp { get; set; }
    }
}
