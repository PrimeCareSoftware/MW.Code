using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for manual stock entry of controlled medications.
    /// Used for registering incoming controlled substances (purchases, transfers, returns).
    /// </summary>
    public class StockEntryDto
    {
        /// <summary>
        /// Date of the stock entry.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Name of the controlled medication.
        /// </summary>
        public string MedicationName { get; set; } = string.Empty;

        /// <summary>
        /// Active pharmaceutical ingredient.
        /// </summary>
        public string ActiveIngredient { get; set; } = string.Empty;

        /// <summary>
        /// ANVISA controlled list classification (A1, A2, A3, B1, B2, C1, C2, C3, C4, C5).
        /// </summary>
        public string AnvisaList { get; set; } = string.Empty;

        /// <summary>
        /// Medication concentration (e.g., "10mg/ml", "500mg").
        /// </summary>
        public string Concentration { get; set; } = string.Empty;

        /// <summary>
        /// Pharmaceutical form (e.g., "Comprimido", "Solução Oral", "Ampola").
        /// </summary>
        public string PharmaceuticalForm { get; set; } = string.Empty;

        /// <summary>
        /// Quantity of units being added to stock.
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// Type of document (e.g., "Nota Fiscal", "Transferência", "Devolução").
        /// </summary>
        public string DocumentType { get; set; } = string.Empty;

        /// <summary>
        /// Document identification number.
        /// </summary>
        public string DocumentNumber { get; set; } = string.Empty;

        /// <summary>
        /// Date of the document.
        /// </summary>
        public DateTime DocumentDate { get; set; }

        /// <summary>
        /// Name of the supplier (optional).
        /// </summary>
        public string? SupplierName { get; set; }

        /// <summary>
        /// CNPJ of the supplier (optional).
        /// </summary>
        public string? SupplierCNPJ { get; set; }
    }
}
