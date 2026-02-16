using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents business configuration and feature flags for a clinic/tenant
    /// </summary>
    public class BusinessConfiguration : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public BusinessType BusinessType { get; private set; }
        public ProfessionalSpecialty PrimarySpecialty { get; private set; }
        
        // Clinical Features
        public bool ElectronicPrescription { get; private set; }
        public bool LabIntegration { get; private set; }
        public bool VaccineControl { get; private set; }
        public bool InventoryManagement { get; private set; }
        
        // Administrative Features
        public bool MultiRoom { get; private set; }
        public bool ReceptionQueue { get; private set; }
        public bool FinancialModule { get; private set; }
        public bool HealthInsurance { get; private set; }
        
        // Consultation Features
        public bool Telemedicine { get; private set; }
        public bool HomeVisit { get; private set; }
        public bool GroupSessions { get; private set; }
        
        // Marketing Features
        public bool PublicProfile { get; private set; }
        public bool OnlineBooking { get; private set; }
        public bool PatientReviews { get; private set; }
        
        // Advanced Features
        public bool BiReports { get; private set; }
        public bool ApiAccess { get; private set; }
        public bool WhiteLabel { get; private set; }
        
        // Payment Features
        public bool CreditCardPayments { get; private set; }
        
        // Navigation properties
        public Clinic? Clinic { get; private set; }
        
        private BusinessConfiguration()
        {
            // EF Constructor
        }
        
        public BusinessConfiguration(
            Guid clinicId,
            BusinessType businessType,
            ProfessionalSpecialty primarySpecialty,
            string tenantId) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("ClinicId cannot be empty", nameof(clinicId));
                
            ClinicId = clinicId;
            BusinessType = businessType;
            PrimarySpecialty = primarySpecialty;
            
            // Set default features based on business type and specialty
            SetDefaultFeatures();

            // Ensure UpdatedAt is initialized for not-null database constraint
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Sets default feature configuration based on business type and specialty
        /// </summary>
        private void SetDefaultFeatures()
        {
            // Common defaults
            FinancialModule = true;
            OnlineBooking = true;
            PublicProfile = true;
            CreditCardPayments = true;
            
            // Business type specific defaults
            switch (BusinessType)
            {
                case BusinessType.SoloPractitioner:
                    MultiRoom = false;
                    ReceptionQueue = false;
                    BiReports = false;
                    ApiAccess = false;
                    WhiteLabel = false;
                    InventoryManagement = false;
                    break;
                    
                case BusinessType.SmallClinic:
                    MultiRoom = true;
                    ReceptionQueue = true;
                    BiReports = false;
                    ApiAccess = false;
                    WhiteLabel = false;
                    InventoryManagement = true;
                    break;
                    
                case BusinessType.MediumClinic:
                    MultiRoom = true;
                    ReceptionQueue = true;
                    BiReports = true;
                    ApiAccess = false;
                    WhiteLabel = false;
                    InventoryManagement = true;
                    break;
                    
                case BusinessType.LargeClinic:
                    MultiRoom = true;
                    ReceptionQueue = true;
                    BiReports = true;
                    ApiAccess = true;
                    WhiteLabel = true;
                    InventoryManagement = true;
                    break;
            }
            
            // Specialty specific defaults
            switch (PrimarySpecialty)
            {
                case ProfessionalSpecialty.Psicologo:
                    ElectronicPrescription = false;
                    LabIntegration = false;
                    VaccineControl = false;
                    Telemedicine = true;
                    HomeVisit = false;
                    GroupSessions = true;
                    HealthInsurance = false;
                    PatientReviews = true;
                    break;
                    
                case ProfessionalSpecialty.Nutricionista:
                    ElectronicPrescription = false;
                    LabIntegration = true;
                    VaccineControl = false;
                    Telemedicine = true;
                    HomeVisit = true;
                    GroupSessions = false;
                    HealthInsurance = true;
                    PatientReviews = true;
                    break;
                    
                case ProfessionalSpecialty.Dentista:
                    ElectronicPrescription = true;
                    LabIntegration = true;
                    VaccineControl = false;
                    Telemedicine = false;
                    HomeVisit = false;
                    GroupSessions = false;
                    HealthInsurance = true;
                    PatientReviews = true;
                    break;
                    
                case ProfessionalSpecialty.Fisioterapeuta:
                    ElectronicPrescription = false;
                    LabIntegration = false;
                    VaccineControl = false;
                    Telemedicine = true;
                    HomeVisit = true;
                    GroupSessions = false;
                    HealthInsurance = true;
                    PatientReviews = true;
                    break;
                    
                case ProfessionalSpecialty.Medico:
                    ElectronicPrescription = true;
                    LabIntegration = true;
                    VaccineControl = true;
                    Telemedicine = true;
                    HomeVisit = true;
                    GroupSessions = false;
                    HealthInsurance = true;
                    PatientReviews = true;
                    break;
                    
                default:
                    // Generic defaults for other specialties
                    ElectronicPrescription = true;
                    LabIntegration = true;
                    VaccineControl = false;
                    Telemedicine = true;
                    HomeVisit = false;
                    GroupSessions = false;
                    HealthInsurance = true;
                    PatientReviews = true;
                    break;
            }
        }
        
        public void UpdateBusinessType(BusinessType businessType)
        {
            BusinessType = businessType;
            SetDefaultFeatures();
            UpdateTimestamp();
        }
        
        public void UpdatePrimarySpecialty(ProfessionalSpecialty primarySpecialty)
        {
            PrimarySpecialty = primarySpecialty;
            SetDefaultFeatures();
            UpdateTimestamp();
        }
        
        public void UpdateFeature(string featureName, bool enabled)
        {
            var property = GetType().GetProperty(featureName);
            if (property == null || property.PropertyType != typeof(bool))
                throw new ArgumentException($"Invalid feature name: {featureName}", nameof(featureName));
                
            property.SetValue(this, enabled);
            UpdateTimestamp();
        }
        
        public void EnableFeature(string featureName) => UpdateFeature(featureName, true);
        
        public void DisableFeature(string featureName) => UpdateFeature(featureName, false);
        
        public bool IsFeatureEnabled(string featureName)
        {
            var property = GetType().GetProperty(featureName);
            if (property == null || property.PropertyType != typeof(bool))
                return false;
                
            return (bool)(property.GetValue(this) ?? false);
        }
    }
}
