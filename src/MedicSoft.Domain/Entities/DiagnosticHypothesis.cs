using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Tipo de diagnóstico conforme CFM 1.821/2007
    /// </summary>
    public enum DiagnosisType
    {
        /// <summary>
        /// Diagnóstico principal
        /// </summary>
        Principal = 1,
        
        /// <summary>
        /// Diagnóstico secundário
        /// </summary>
        Secondary = 2
    }
    
    /// <summary>
    /// Representa uma hipótese diagnóstica conforme CFM 1.821/2007
    /// </summary>
    public class DiagnosticHypothesis : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        
        // Navigation properties
        public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
        
        // Descrição da hipótese diagnóstica
        public string Description { get; private set; }
        
        // Código CID-10 (obrigatório CFM 1.821)
        public string ICD10Code { get; private set; }
        
        // Tipo do diagnóstico (principal ou secundário)
        public DiagnosisType Type { get; private set; }
        
        // Data do diagnóstico
        public DateTime DiagnosedAt { get; private set; }
        
        private DiagnosticHypothesis()
        {
            // EF Constructor
            Description = null!;
            ICD10Code = null!;
        }
        
        public DiagnosticHypothesis(
            Guid medicalRecordId,
            string tenantId,
            string description,
            string icd10Code,
            DiagnosisType type = DiagnosisType.Principal) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));
            
            if (string.IsNullOrWhiteSpace(icd10Code))
                throw new ArgumentException("ICD-10 code is required", nameof(icd10Code));
            
            ValidateICD10Code(icd10Code);
            
            MedicalRecordId = medicalRecordId;
            Description = description.Trim();
            ICD10Code = icd10Code.Trim().ToUpperInvariant();
            Type = type;
            DiagnosedAt = DateTime.UtcNow;
        }
        
        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required", nameof(description));
            
            Description = description.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateICD10Code(string icd10Code)
        {
            if (string.IsNullOrWhiteSpace(icd10Code))
                throw new ArgumentException("ICD-10 code is required", nameof(icd10Code));
            
            ValidateICD10Code(icd10Code);
            
            ICD10Code = icd10Code.Trim().ToUpperInvariant();
            UpdateTimestamp();
        }
        
        public void UpdateType(DiagnosisType type)
        {
            Type = type;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Valida o formato do código CID-10
        /// Formato: Letra seguida de 2 dígitos, opcionalmente seguido de ponto e 1-2 dígitos
        /// Exemplos válidos: A00, A00.0, A00.01, Z99.9
        /// </summary>
        private void ValidateICD10Code(string icd10Code)
        {
            if (string.IsNullOrWhiteSpace(icd10Code))
                throw new ArgumentException("ICD-10 code cannot be empty", nameof(icd10Code));
            
            var cleanCode = icd10Code.Trim().ToUpperInvariant();
            
            // Formato básico: Letra + 2 dígitos
            if (cleanCode.Length < 3)
                throw new ArgumentException("ICD-10 code must have at least 3 characters (e.g., A00)", nameof(icd10Code));
            
            // Primeiro caractere deve ser uma letra (A-Z)
            if (!char.IsLetter(cleanCode[0]))
                throw new ArgumentException("ICD-10 code must start with a letter", nameof(icd10Code));
            
            // Segundo e terceiro caracteres devem ser dígitos
            if (!char.IsDigit(cleanCode[1]) || !char.IsDigit(cleanCode[2]))
                throw new ArgumentException("ICD-10 code must have two digits after the letter (e.g., A00)", nameof(icd10Code));
            
            // Se houver mais caracteres, deve ser ponto seguido de 1-2 dígitos
            if (cleanCode.Length > 3)
            {
                if (cleanCode[3] != '.')
                    throw new ArgumentException("ICD-10 code subcategory must start with a dot (e.g., A00.0)", nameof(icd10Code));
                
                if (cleanCode.Length < 5)
                    throw new ArgumentException("ICD-10 code subcategory must have at least one digit after the dot", nameof(icd10Code));
                
                // Validar dígitos após o ponto
                for (int i = 4; i < cleanCode.Length; i++)
                {
                    if (!char.IsDigit(cleanCode[i]))
                        throw new ArgumentException("ICD-10 code subcategory must contain only digits after the dot", nameof(icd10Code));
                }
                
                if (cleanCode.Length > 6)
                    throw new ArgumentException("ICD-10 code subcategory can have at most 2 digits after the dot", nameof(icd10Code));
            }
        }
    }
}
