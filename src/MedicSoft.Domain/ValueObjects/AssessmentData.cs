using System;
using System.Collections.Generic;
using System.Linq;

namespace MedicSoft.Domain.ValueObjects
{
    public class DifferentialDiagnosis
    {
        public string Diagnosis { get; private set; }
        public string? Icd10Code { get; private set; }
        public string? Justification { get; private set; }
        public int Priority { get; private set; }  // 1 = mais provável

        private DifferentialDiagnosis() 
        {
            Diagnosis = null!;
        }

        public DifferentialDiagnosis(string diagnosis, string? icd10Code = null, 
            string? justification = null, int priority = 1)
        {
            if (string.IsNullOrWhiteSpace(diagnosis))
                throw new ArgumentException("Diagnosis is required", nameof(diagnosis));
            
            if (priority < 1)
                throw new ArgumentException("Priority must be at least 1", nameof(priority));

            Diagnosis = diagnosis.Trim();
            Icd10Code = icd10Code?.Trim();
            Justification = justification?.Trim();
            Priority = priority;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not DifferentialDiagnosis other)
                return false;

            return Diagnosis == other.Diagnosis &&
                   Icd10Code == other.Icd10Code &&
                   Justification == other.Justification &&
                   Priority == other.Priority;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Diagnosis, Icd10Code, Justification, Priority);
        }
    }

    public class AssessmentData
    {
        public string? PrimaryDiagnosis { get; private set; }
        public string? PrimaryDiagnosisIcd10 { get; private set; }
        
        public List<DifferentialDiagnosis> DifferentialDiagnoses { get; private set; }
        
        public string? ClinicalReasoning { get; private set; }  // Raciocínio clínico
        public string? Prognosis { get; private set; }
        public string? Evolution { get; private set; }  // Evolução do quadro

        private AssessmentData() 
        {
            DifferentialDiagnoses = new List<DifferentialDiagnosis>();
        }

        public AssessmentData(
            string? primaryDiagnosis = null,
            string? primaryDiagnosisIcd10 = null,
            List<DifferentialDiagnosis>? differentialDiagnoses = null,
            string? clinicalReasoning = null,
            string? prognosis = null,
            string? evolution = null)
        {
            PrimaryDiagnosis = primaryDiagnosis?.Trim();
            PrimaryDiagnosisIcd10 = primaryDiagnosisIcd10?.Trim();
            DifferentialDiagnoses = differentialDiagnoses ?? new List<DifferentialDiagnosis>();
            ClinicalReasoning = clinicalReasoning?.Trim();
            Prognosis = prognosis?.Trim();
            Evolution = evolution?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not AssessmentData other)
                return false;

            return PrimaryDiagnosis == other.PrimaryDiagnosis &&
                   PrimaryDiagnosisIcd10 == other.PrimaryDiagnosisIcd10 &&
                   DifferentialDiagnoses.SequenceEqual(other.DifferentialDiagnoses) &&
                   ClinicalReasoning == other.ClinicalReasoning &&
                   Prognosis == other.Prognosis &&
                   Evolution == other.Evolution;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(PrimaryDiagnosis);
            hash.Add(PrimaryDiagnosisIcd10);
            foreach (var diagnosis in DifferentialDiagnoses)
                hash.Add(diagnosis);
            hash.Add(ClinicalReasoning);
            hash.Add(Prognosis);
            hash.Add(Evolution);
            return hash.ToHashCode();
        }
    }
}
