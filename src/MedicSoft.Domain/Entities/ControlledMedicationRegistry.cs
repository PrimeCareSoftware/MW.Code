using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an entry in the controlled medication registry (Livro de Registro).
    /// Required by ANVISA RDC 27/2007 for tracking all controlled substance movements.
    /// </summary>
    public class ControlledMedicationRegistry : BaseEntity
    {
        public DateTime Date { get; private set; }
        public RegistryType RegistryType { get; private set; }
        
        // Medication Information
        public string MedicationName { get; private set; }
        public string ActiveIngredient { get; private set; }
        public string AnvisaList { get; private set; } // A1, A2, A3, B1, B2, C1, C2, C3, C4, C5
        public string Concentration { get; private set; }
        public string PharmaceuticalForm { get; private set; }
        
        // Movement Tracking
        public decimal QuantityIn { get; private set; }
        public decimal QuantityOut { get; private set; }
        public decimal Balance { get; private set; }
        
        // Document Information
        public string DocumentType { get; private set; } // "Nota Fiscal", "Receita Médica", "Devolução", "Transferência"
        public string DocumentNumber { get; private set; }
        public DateTime? DocumentDate { get; private set; }
        
        // Prescription Information (for outbound movements)
        public Guid? PrescriptionId { get; private set; }
        public string? PatientName { get; private set; }
        public string? PatientCPF { get; private set; }
        public string? DoctorName { get; private set; }
        public string? DoctorCRM { get; private set; }
        
        // Supplier Information (for inbound movements)
        public string? SupplierName { get; private set; }
        public string? SupplierCNPJ { get; private set; }
        
        // Audit Trail
        public Guid RegisteredByUserId { get; private set; }
        public DateTime RegisteredAt { get; private set; }
        
        // Navigation Properties
        public DigitalPrescription? Prescription { get; private set; }
        public User? RegisteredBy { get; private set; }

        private ControlledMedicationRegistry()
        {
            // EF Constructor
            MedicationName = string.Empty;
            ActiveIngredient = string.Empty;
            AnvisaList = string.Empty;
            Concentration = string.Empty;
            PharmaceuticalForm = string.Empty;
            DocumentType = string.Empty;
            DocumentNumber = string.Empty;
        }

        /// <summary>
        /// Creates a new registry entry for outbound movement (prescription dispensing)
        /// </summary>
        public static ControlledMedicationRegistry CreatePrescriptionEntry(
            string tenantId,
            Guid prescriptionId,
            DateTime date,
            string medicationName,
            string activeIngredient,
            string anvisaList,
            string concentration,
            string pharmaceuticalForm,
            decimal quantity,
            decimal previousBalance,
            string documentNumber,
            DateTime documentDate,
            string patientName,
            string patientCPF,
            string doctorName,
            string doctorCRM,
            Guid registeredByUserId)
        {
            ValidateCommonFields(medicationName, activeIngredient, anvisaList, concentration, pharmaceuticalForm, quantity, documentNumber);
            
            if (prescriptionId == Guid.Empty)
                throw new ArgumentException("Prescription ID cannot be empty", nameof(prescriptionId));
            
            if (string.IsNullOrWhiteSpace(patientName))
                throw new ArgumentException("Patient name cannot be empty", nameof(patientName));
            
            if (string.IsNullOrWhiteSpace(doctorName))
                throw new ArgumentException("Doctor name cannot be empty", nameof(doctorName));
            
            if (string.IsNullOrWhiteSpace(doctorCRM))
                throw new ArgumentException("Doctor CRM cannot be empty", nameof(doctorCRM));

            return new ControlledMedicationRegistry
            {
                TenantId = tenantId,
                Date = date,
                RegistryType = RegistryType.Outbound,
                MedicationName = medicationName.Trim(),
                ActiveIngredient = activeIngredient.Trim(),
                AnvisaList = anvisaList.Trim().ToUpperInvariant(),
                Concentration = concentration.Trim(),
                PharmaceuticalForm = pharmaceuticalForm.Trim(),
                QuantityIn = 0,
                QuantityOut = quantity,
                Balance = previousBalance - quantity,
                DocumentType = "Receita Médica",
                DocumentNumber = documentNumber.Trim(),
                DocumentDate = documentDate,
                PrescriptionId = prescriptionId,
                PatientName = patientName.Trim(),
                PatientCPF = patientCPF?.Trim(),
                DoctorName = doctorName.Trim(),
                DoctorCRM = doctorCRM.Trim(),
                RegisteredByUserId = registeredByUserId,
                RegisteredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Creates a new registry entry for inbound movement (stock entry)
        /// </summary>
        public static ControlledMedicationRegistry CreateStockEntry(
            string tenantId,
            DateTime date,
            string medicationName,
            string activeIngredient,
            string anvisaList,
            string concentration,
            string pharmaceuticalForm,
            decimal quantity,
            decimal previousBalance,
            string documentType,
            string documentNumber,
            DateTime documentDate,
            string? supplierName,
            string? supplierCNPJ,
            Guid registeredByUserId)
        {
            ValidateCommonFields(medicationName, activeIngredient, anvisaList, concentration, pharmaceuticalForm, quantity, documentNumber);
            
            if (string.IsNullOrWhiteSpace(documentType))
                throw new ArgumentException("Document type cannot be empty", nameof(documentType));

            return new ControlledMedicationRegistry
            {
                TenantId = tenantId,
                Date = date,
                RegistryType = RegistryType.Inbound,
                MedicationName = medicationName.Trim(),
                ActiveIngredient = activeIngredient.Trim(),
                AnvisaList = anvisaList.Trim().ToUpperInvariant(),
                Concentration = concentration.Trim(),
                PharmaceuticalForm = pharmaceuticalForm.Trim(),
                QuantityIn = quantity,
                QuantityOut = 0,
                Balance = previousBalance + quantity,
                DocumentType = documentType.Trim(),
                DocumentNumber = documentNumber.Trim(),
                DocumentDate = documentDate,
                SupplierName = supplierName?.Trim(),
                SupplierCNPJ = supplierCNPJ?.Trim(),
                RegisteredByUserId = registeredByUserId,
                RegisteredAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };
        }

        private static void ValidateCommonFields(
            string medicationName,
            string activeIngredient,
            string anvisaList,
            string concentration,
            string pharmaceuticalForm,
            decimal quantity,
            string documentNumber)
        {
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name cannot be empty", nameof(medicationName));
            
            if (string.IsNullOrWhiteSpace(activeIngredient))
                throw new ArgumentException("Active ingredient cannot be empty", nameof(activeIngredient));
            
            if (string.IsNullOrWhiteSpace(anvisaList))
                throw new ArgumentException("ANVISA list cannot be empty", nameof(anvisaList));
            
            if (string.IsNullOrWhiteSpace(concentration))
                throw new ArgumentException("Concentration cannot be empty", nameof(concentration));
            
            if (string.IsNullOrWhiteSpace(pharmaceuticalForm))
                throw new ArgumentException("Pharmaceutical form cannot be empty", nameof(pharmaceuticalForm));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number cannot be empty", nameof(documentNumber));
        }

        public void UpdateBalance(decimal newBalance)
        {
            Balance = newBalance;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Type of registry entry
    /// </summary>
    public enum RegistryType
    {
        /// <summary>
        /// Stock entry (purchase, return, transfer in)
        /// </summary>
        Inbound = 1,

        /// <summary>
        /// Stock exit (prescription dispensing, loss, transfer out)
        /// </summary>
        Outbound = 2,

        /// <summary>
        /// Physical inventory count
        /// </summary>
        Balance = 3
    }
}
