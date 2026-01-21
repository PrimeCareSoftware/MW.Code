using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa o exame clínico/físico do paciente conforme CFM 1.821/2007
    /// </summary>
    public class ClinicalExamination : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        
        // Navigation properties
        public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
        
        // Sinais vitais obrigatórios (CFM 1.821)
        public decimal? BloodPressureSystolic { get; private set; }
        public decimal? BloodPressureDiastolic { get; private set; }
        public int? HeartRate { get; private set; }
        
        // Sinais vitais recomendados
        public int? RespiratoryRate { get; private set; }
        public decimal? Temperature { get; private set; }
        public decimal? OxygenSaturation { get; private set; }
        
        // Medidas antropométricas
        public decimal? Weight { get; private set; }  // Peso em kg
        public decimal? Height { get; private set; }  // Altura em metros
        
        // IMC calculado automaticamente
        public decimal? BMI => CalculateBMI();
        
        // Exame físico sistemático (obrigatório)
        public string SystematicExamination { get; private set; }
        
        // Estado geral (recomendado)
        public string? GeneralState { get; private set; }
        
        private ClinicalExamination()
        {
            // EF Constructor
            SystematicExamination = null!;
        }
        
        public ClinicalExamination(
            Guid medicalRecordId,
            string tenantId,
            string systematicExamination,
            decimal? bloodPressureSystolic = null,
            decimal? bloodPressureDiastolic = null,
            int? heartRate = null,
            int? respiratoryRate = null,
            decimal? temperature = null,
            decimal? oxygenSaturation = null,
            decimal? weight = null,
            decimal? height = null,
            string? generalState = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (string.IsNullOrWhiteSpace(systematicExamination))
                throw new ArgumentException("Systematic examination is required", nameof(systematicExamination));
            
            if (systematicExamination.Length < 20)
                throw new ArgumentException("Systematic examination must have at least 20 characters", nameof(systematicExamination));
            
            ValidateVitalSigns(bloodPressureSystolic, bloodPressureDiastolic, heartRate, respiratoryRate, temperature, oxygenSaturation);
            ValidateAnthropometricData(weight, height);
            
            MedicalRecordId = medicalRecordId;
            SystematicExamination = systematicExamination.Trim();
            BloodPressureSystolic = bloodPressureSystolic;
            BloodPressureDiastolic = bloodPressureDiastolic;
            HeartRate = heartRate;
            RespiratoryRate = respiratoryRate;
            Temperature = temperature;
            OxygenSaturation = oxygenSaturation;
            Weight = weight;
            Height = height;
            GeneralState = generalState?.Trim();
        }
        
        public void UpdateVitalSigns(
            decimal? bloodPressureSystolic,
            decimal? bloodPressureDiastolic,
            int? heartRate,
            int? respiratoryRate = null,
            decimal? temperature = null,
            decimal? oxygenSaturation = null,
            decimal? weight = null,
            decimal? height = null)
        {
            ValidateVitalSigns(bloodPressureSystolic, bloodPressureDiastolic, heartRate, respiratoryRate, temperature, oxygenSaturation);
            ValidateAnthropometricData(weight, height);
            
            BloodPressureSystolic = bloodPressureSystolic;
            BloodPressureDiastolic = bloodPressureDiastolic;
            HeartRate = heartRate;
            RespiratoryRate = respiratoryRate;
            Temperature = temperature;
            OxygenSaturation = oxygenSaturation;
            Weight = weight;
            Height = height;
            
            UpdateTimestamp();
        }
        
        public void UpdateSystematicExamination(string systematicExamination)
        {
            if (string.IsNullOrWhiteSpace(systematicExamination))
                throw new ArgumentException("Systematic examination is required", nameof(systematicExamination));
            
            if (systematicExamination.Length < 20)
                throw new ArgumentException("Systematic examination must have at least 20 characters", nameof(systematicExamination));
            
            SystematicExamination = systematicExamination.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateGeneralState(string? generalState)
        {
            GeneralState = generalState?.Trim();
            UpdateTimestamp();
        }
        
        private void ValidateVitalSigns(
            decimal? bloodPressureSystolic,
            decimal? bloodPressureDiastolic,
            int? heartRate,
            int? respiratoryRate,
            decimal? temperature,
            decimal? oxygenSaturation)
        {
            if (bloodPressureSystolic.HasValue && (bloodPressureSystolic < 50 || bloodPressureSystolic > 300))
                throw new ArgumentException("Systolic blood pressure must be between 50 and 300 mmHg", nameof(bloodPressureSystolic));
            
            if (bloodPressureDiastolic.HasValue && (bloodPressureDiastolic < 30 || bloodPressureDiastolic > 200))
                throw new ArgumentException("Diastolic blood pressure must be between 30 and 200 mmHg", nameof(bloodPressureDiastolic));
            
            if (heartRate.HasValue && (heartRate < 30 || heartRate > 220))
                throw new ArgumentException("Heart rate must be between 30 and 220 bpm", nameof(heartRate));
            
            if (respiratoryRate.HasValue && (respiratoryRate < 8 || respiratoryRate > 60))
                throw new ArgumentException("Respiratory rate must be between 8 and 60 breaths per minute", nameof(respiratoryRate));
            
            if (temperature.HasValue && (temperature < 32 || temperature > 45))
                throw new ArgumentException("Temperature must be between 32 and 45 °C", nameof(temperature));
            
            if (oxygenSaturation.HasValue && (oxygenSaturation < 0 || oxygenSaturation > 100))
                throw new ArgumentException("Oxygen saturation must be between 0 and 100%", nameof(oxygenSaturation));
        }
        
        private void ValidateAnthropometricData(decimal? weight, decimal? height)
        {
            if (weight.HasValue && (weight < 0.5m || weight > 500m))
                throw new ArgumentException("Weight must be between 0.5 and 500 kg", nameof(weight));
            
            if (height.HasValue && (height < 0.3m || height > 3.0m))
                throw new ArgumentException("Height must be between 0.3 and 3.0 meters", nameof(height));
        }
        
        private decimal? CalculateBMI()
        {
            if (!Weight.HasValue || !Height.HasValue || Height.Value == 0)
                return null;
            
            return Math.Round(Weight.Value / (Height.Value * Height.Value), 2);
        }
    }
}
