using System;

namespace MedicSoft.Domain.ValueObjects
{
    public class ObjectiveData
    {
        // Sinais Vitais
        public VitalSigns? VitalSigns { get; private set; }
        
        // Exame Físico
        public PhysicalExamination? PhysicalExam { get; private set; }
        
        // Exames Complementares
        public string? LabResults { get; private set; }
        public string? ImagingResults { get; private set; }
        public string? OtherExamResults { get; private set; }

        private ObjectiveData() { }

        public ObjectiveData(
            VitalSigns? vitalSigns = null,
            PhysicalExamination? physicalExam = null,
            string? labResults = null,
            string? imagingResults = null,
            string? otherExamResults = null)
        {
            VitalSigns = vitalSigns;
            PhysicalExam = physicalExam;
            LabResults = labResults?.Trim();
            ImagingResults = imagingResults?.Trim();
            OtherExamResults = otherExamResults?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ObjectiveData other)
                return false;

            return Equals(VitalSigns, other.VitalSigns) &&
                   Equals(PhysicalExam, other.PhysicalExam) &&
                   LabResults == other.LabResults &&
                   ImagingResults == other.ImagingResults &&
                   OtherExamResults == other.OtherExamResults;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(VitalSigns, PhysicalExam, LabResults, 
                ImagingResults, OtherExamResults);
        }
    }
}
