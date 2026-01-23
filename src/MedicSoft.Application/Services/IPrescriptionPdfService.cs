using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for generating PDF documents for digital prescriptions.
    /// Compliant with CFM 1.643/2002 and ANVISA 344/98 regulations.
    /// </summary>
    public interface IPrescriptionPdfService
    {
        /// <summary>
        /// Generates a PDF for the specified prescription with default options.
        /// </summary>
        Task<byte[]> GeneratePdfAsync(Guid prescriptionId, PrescriptionPdfOptions? options = null);

        /// <summary>
        /// Generates a simple prescription PDF.
        /// </summary>
        Task<byte[]> GenerateSimplePrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options);

        /// <summary>
        /// Generates a controlled substance prescription PDF (Special Control A/B/C1).
        /// </summary>
        Task<byte[]> GenerateControlledPrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options);

        /// <summary>
        /// Generates an antimicrobial prescription PDF.
        /// </summary>
        Task<byte[]> GenerateAntimicrobialPrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options);
    }

    /// <summary>
    /// Options for PDF generation.
    /// </summary>
    public class PrescriptionPdfOptions
    {
        /// <summary>
        /// Include header with clinic information.
        /// </summary>
        public bool IncludeHeader { get; set; } = true;

        /// <summary>
        /// Include footer with doctor signature area.
        /// </summary>
        public bool IncludeFooter { get; set; } = true;

        /// <summary>
        /// Include QR Code for verification.
        /// </summary>
        public bool IncludeQRCode { get; set; } = true;

        /// <summary>
        /// Include watermark.
        /// </summary>
        public bool IncludeWatermark { get; set; } = true;

        /// <summary>
        /// Paper size (A4 is default).
        /// </summary>
        public PaperSize PaperSize { get; set; } = PaperSize.A4;

        /// <summary>
        /// Generate patient copy (2 vias for controlled prescriptions).
        /// </summary>
        public bool IncludePatientCopy { get; set; } = false;

        /// <summary>
        /// Clinic name to display in header.
        /// </summary>
        public string? ClinicName { get; set; }

        /// <summary>
        /// Clinic address to display in header.
        /// </summary>
        public string? ClinicAddress { get; set; }

        /// <summary>
        /// Clinic phone to display in header.
        /// </summary>
        public string? ClinicPhone { get; set; }

        /// <summary>
        /// Clinic logo image (base64 encoded).
        /// </summary>
        public string? ClinicLogo { get; set; }
    }

    /// <summary>
    /// Paper sizes for PDF generation.
    /// </summary>
    public enum PaperSize
    {
        A4,
        Letter,
        HalfPage
    }
}
