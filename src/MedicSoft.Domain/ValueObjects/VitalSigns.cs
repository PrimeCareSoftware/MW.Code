using System;

namespace MedicSoft.Domain.ValueObjects
{
    public class VitalSigns
    {
        public int? SystolicBP { get; private set; }  // mmHg
        public int? DiastolicBP { get; private set; }  // mmHg
        public int? HeartRate { get; private set; }  // bpm
        public int? RespiratoryRate { get; private set; }  // rpm
        public decimal? Temperature { get; private set; }  // °C
        public int? OxygenSaturation { get; private set; }  // %
        public decimal? Weight { get; private set; }  // kg
        public decimal? Height { get; private set; }  // cm
        public decimal? BMI { get; private set; }  // calculado
        public int? Pain { get; private set; }  // Escala 0-10

        private VitalSigns() { }

        public VitalSigns(int? systolicBP = null, int? diastolicBP = null, 
            int? heartRate = null, int? respiratoryRate = null,
            decimal? temperature = null, int? oxygenSaturation = null,
            decimal? weight = null, decimal? height = null, int? pain = null)
        {
            ValidateVitalSigns(systolicBP, diastolicBP, heartRate, respiratoryRate, 
                temperature, oxygenSaturation, weight, height, pain);

            SystolicBP = systolicBP;
            DiastolicBP = diastolicBP;
            HeartRate = heartRate;
            RespiratoryRate = respiratoryRate;
            Temperature = temperature;
            OxygenSaturation = oxygenSaturation;
            Weight = weight;
            Height = height;
            Pain = pain;
            
            CalculateBMI();
        }

        private void ValidateVitalSigns(int? systolicBP, int? diastolicBP, 
            int? heartRate, int? respiratoryRate,
            decimal? temperature, int? oxygenSaturation,
            decimal? weight, decimal? height, int? pain)
        {
            if (systolicBP.HasValue && (systolicBP < 0 || systolicBP > 300))
                throw new ArgumentException("Systolic BP must be between 0 and 300 mmHg");
            
            if (diastolicBP.HasValue && (diastolicBP < 0 || diastolicBP > 200))
                throw new ArgumentException("Diastolic BP must be between 0 and 200 mmHg");
            
            if (heartRate.HasValue && (heartRate < 0 || heartRate > 300))
                throw new ArgumentException("Heart rate must be between 0 and 300 bpm");
            
            if (respiratoryRate.HasValue && (respiratoryRate < 0 || respiratoryRate > 100))
                throw new ArgumentException("Respiratory rate must be between 0 and 100 rpm");
            
            if (temperature.HasValue && (temperature < 32 || temperature > 45))
                throw new ArgumentException("Temperature must be between 32 and 45 °C");
            
            if (oxygenSaturation.HasValue && (oxygenSaturation < 0 || oxygenSaturation > 100))
                throw new ArgumentException("Oxygen saturation must be between 0 and 100 %");
            
            if (weight.HasValue && (weight < 0 || weight > 500))
                throw new ArgumentException("Weight must be between 0 and 500 kg");
            
            if (height.HasValue && (height < 0 || height > 300))
                throw new ArgumentException("Height must be between 0 and 300 cm");
            
            if (pain.HasValue && (pain < 0 || pain > 10))
                throw new ArgumentException("Pain scale must be between 0 and 10");
        }

        private void CalculateBMI()
        {
            if (Weight.HasValue && Height.HasValue && Height.Value > 0)
            {
                var heightInMeters = Height.Value / 100;
                BMI = Math.Round(Weight.Value / (heightInMeters * heightInMeters), 1);
            }
        }

        public override bool Equals(object? obj)
        {
            if (obj is not VitalSigns other)
                return false;

            return SystolicBP == other.SystolicBP &&
                   DiastolicBP == other.DiastolicBP &&
                   HeartRate == other.HeartRate &&
                   RespiratoryRate == other.RespiratoryRate &&
                   Temperature == other.Temperature &&
                   OxygenSaturation == other.OxygenSaturation &&
                   Weight == other.Weight &&
                   Height == other.Height &&
                   Pain == other.Pain;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(SystolicBP);
            hash.Add(DiastolicBP);
            hash.Add(HeartRate);
            hash.Add(RespiratoryRate);
            hash.Add(Temperature);
            hash.Add(OxygenSaturation);
            hash.Add(Weight);
            hash.Add(Height);
            hash.Add(Pain);
            return hash.ToHashCode();
        }
    }
}
