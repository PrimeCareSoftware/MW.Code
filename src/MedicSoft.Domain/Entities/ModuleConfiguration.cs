using System;
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

        public static string[] GetAllModules() => new[]
        {
            PatientManagement,
            AppointmentScheduling,
            MedicalRecords,
            Prescriptions,
            FinancialManagement,
            Reports,
            WhatsAppIntegration,
            SMSNotifications,
            TissExport,
            InventoryManagement,
            UserManagement,
            WaitingQueue
        };

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
}
