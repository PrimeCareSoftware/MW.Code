using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Controls sequential numbering for controlled substance prescriptions.
    /// Required by ANVISA for tracking controlled medication prescriptions.
    /// Each clinic must maintain independent sequence numbers per prescription type.
    /// </summary>
    public class PrescriptionSequenceControl : BaseEntity
    {
        public PrescriptionType Type { get; private set; }
        public int CurrentSequence { get; private set; }
        public int Year { get; private set; }
        public string Prefix { get; private set; } // Optional prefix for sequence (e.g., clinic code)
        
        // Audit
        public DateTime LastGeneratedAt { get; private set; }
        public int TotalGenerated { get; private set; }

        private PrescriptionSequenceControl()
        {
            // EF Constructor
            Prefix = null!;
        }

        public PrescriptionSequenceControl(
            PrescriptionType type,
            string tenantId,
            string? prefix = null) : base(tenantId)
        {
            Type = type;
            CurrentSequence = 0;
            Year = DateTime.UtcNow.Year;
            Prefix = prefix?.Trim() ?? string.Empty;
            LastGeneratedAt = DateTime.UtcNow;
            TotalGenerated = 0;
        }

        /// <summary>
        /// Generates the next sequence number and returns it formatted.
        /// Format: PREFIX-YEAR-TYPE-SEQUENCE
        /// Example: "CL001-2024-B-0001234"
        /// </summary>
        public string GenerateNext()
        {
            var currentYear = DateTime.UtcNow.Year;
            
            // Reset sequence if year changed
            if (currentYear != Year)
            {
                Year = currentYear;
                CurrentSequence = 0;
            }
            
            CurrentSequence++;
            TotalGenerated++;
            LastGeneratedAt = DateTime.UtcNow;
            UpdateTimestamp();
            
            return FormatSequenceNumber(CurrentSequence);
        }

        private string FormatSequenceNumber(int sequence)
        {
            // Type code mapping
            var typeCode = Type switch
            {
                PrescriptionType.Simple => "S",
                PrescriptionType.SpecialControlB => "B",
                PrescriptionType.SpecialControlA => "A",
                PrescriptionType.Antimicrobial => "ATB",
                PrescriptionType.SpecialControlC1 => "C1",
                _ => "X"
            };
            
            // Format: PREFIX-YEAR-TYPE-SEQUENCE (7 digits)
            var parts = new[]
            {
                Prefix,
                Year.ToString(),
                typeCode,
                sequence.ToString("D7")
            }.Where(p => !string.IsNullOrWhiteSpace(p));
            
            return string.Join("-", parts);
        }

        /// <summary>
        /// Gets the next sequence number without incrementing (preview).
        /// </summary>
        public string PreviewNext()
        {
            var nextSequence = CurrentSequence + 1;
            var currentYear = DateTime.UtcNow.Year;
            
            if (currentYear != Year)
            {
                nextSequence = 1;
            }
            
            return FormatSequenceNumber(nextSequence);
        }

        /// <summary>
        /// Resets the sequence (should only be used in specific scenarios like data migration).
        /// </summary>
        public void ResetSequence(string justification)
        {
            if (string.IsNullOrWhiteSpace(justification))
                throw new ArgumentException("Justification is required for sequence reset", nameof(justification));
            
            CurrentSequence = 0;
            UpdateTimestamp();
            
            // TODO: Log this action for audit purposes
        }
    }
}
