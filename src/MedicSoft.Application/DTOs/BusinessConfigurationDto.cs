using System;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public class BusinessConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public BusinessType BusinessType { get; set; }
        public ProfessionalSpecialty PrimarySpecialty { get; set; }
        
        // Clinical Features
        public bool ElectronicPrescription { get; set; }
        public bool LabIntegration { get; set; }
        public bool VaccineControl { get; set; }
        public bool InventoryManagement { get; set; }
        
        // Administrative Features
        public bool MultiRoom { get; set; }
        public bool ReceptionQueue { get; set; }
        public bool FinancialModule { get; set; }
        public bool HealthInsurance { get; set; }
        
        // Consultation Features
        public bool Telemedicine { get; set; }
        public bool HomeVisit { get; set; }
        public bool GroupSessions { get; set; }
        
        // Marketing Features
        public bool PublicProfile { get; set; }
        public bool OnlineBooking { get; set; }
        public bool PatientReviews { get; set; }
        
        // Advanced Features
        public bool BiReports { get; set; }
        public bool ApiAccess { get; set; }
        public bool WhiteLabel { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    public class CreateBusinessConfigurationDto
    {
        public Guid ClinicId { get; set; }
        public BusinessType BusinessType { get; set; }
        public ProfessionalSpecialty PrimarySpecialty { get; set; }
    }
    
    public class UpdateBusinessTypeDto
    {
        public BusinessType BusinessType { get; set; }
    }
    
    public class UpdatePrimarySpecialtyDto
    {
        public ProfessionalSpecialty PrimarySpecialty { get; set; }
    }
    
    public class UpdateFeatureDto
    {
        public string FeatureName { get; set; } = string.Empty;
        public bool Enabled { get; set; }
    }
}
