using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for generating PDF documents for digital prescriptions.
    /// Implements CFM 1.643/2002 and ANVISA 344/98 compliant prescription templates.
    /// </summary>
    public class PrescriptionPdfService : IPrescriptionPdfService
    {
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<PrescriptionPdfService> _logger;

        public PrescriptionPdfService(
            IDigitalPrescriptionRepository prescriptionRepository,
            ILogger<PrescriptionPdfService> logger)
        {
            _prescriptionRepository = prescriptionRepository;
            _logger = logger;

            // Set QuestPDF license (Community license for open source projects)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> GeneratePdfAsync(Guid prescriptionId, PrescriptionPdfOptions? options = null)
        {
            options ??= new PrescriptionPdfOptions();

            var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId);
            if (prescription == null)
            {
                throw new InvalidOperationException($"Prescription {prescriptionId} not found");
            }

            _logger.LogInformation($"Generating PDF for prescription {prescriptionId}, type: {prescription.Type}");

            return prescription.Type switch
            {
                PrescriptionType.Simple => await GenerateSimplePrescriptionPdfAsync(prescription, options),
                PrescriptionType.SpecialControlA or 
                PrescriptionType.SpecialControlB or 
                PrescriptionType.SpecialControlC1 => await GenerateControlledPrescriptionPdfAsync(prescription, options),
                PrescriptionType.Antimicrobial => await GenerateAntimicrobialPrescriptionPdfAsync(prescription, options),
                _ => throw new InvalidOperationException($"Unsupported prescription type: {prescription.Type}")
            };
        }

        public Task<byte[]> GenerateSimplePrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(GetPageSize(options.PaperSize));
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    if (options.IncludeHeader)
                    {
                        page.Header().Element(c => ComposeHeader(c, prescription, options));
                    }

                    page.Content().Element(c => ComposeSimplePrescriptionContent(c, prescription, options));

                    if (options.IncludeFooter)
                    {
                        page.Footer().Element(c => ComposeFooter(c, prescription, options));
                    }
                });
            });

            var pdfBytes = document.GeneratePdf();
            _logger.LogInformation($"Generated simple prescription PDF for {prescription.Id}, size: {pdfBytes.Length} bytes");
            return Task.FromResult(pdfBytes);
        }

        public Task<byte[]> GenerateControlledPrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(GetPageSize(options.PaperSize));
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Watermark for controlled prescriptions
                    if (options.IncludeWatermark)
                    {
                        page.Background()
                            .AlignCenter()
                            .AlignMiddle()
                            .Text("RECEITA CONTROLADA")
                            .FontSize(60)
                            .FontColor(Colors.Grey.Lighten4)
                            .Bold();
                    }

                    if (options.IncludeHeader)
                    {
                        page.Header().Element(c => ComposeHeader(c, prescription, options));
                    }

                    page.Content().Element(c => ComposeControlledPrescriptionContent(c, prescription, options));

                    if (options.IncludeFooter)
                    {
                        page.Footer().Element(c => ComposeFooter(c, prescription, options));
                    }
                });
            });

            var pdfBytes = document.GeneratePdf();
            _logger.LogInformation($"Generated controlled prescription PDF for {prescription.Id}, type: {prescription.Type}");
            return Task.FromResult(pdfBytes);
        }

        public Task<byte[]> GenerateAntimicrobialPrescriptionPdfAsync(DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(GetPageSize(options.PaperSize));
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    // Watermark for antimicrobial prescriptions
                    if (options.IncludeWatermark)
                    {
                        page.Background()
                            .AlignCenter()
                            .AlignMiddle()
                            .Text("USO SOB ORIENTAÇÃO MÉDICA")
                            .FontSize(50)
                            .FontColor(Colors.Grey.Lighten4)
                            .Bold();
                    }

                    if (options.IncludeHeader)
                    {
                        page.Header().Element(c => ComposeHeader(c, prescription, options));
                    }

                    page.Content().Element(c => ComposeAntimicrobialPrescriptionContent(c, prescription, options));

                    if (options.IncludeFooter)
                    {
                        page.Footer().Element(c => ComposeFooter(c, prescription, options));
                    }
                });
            });

            var pdfBytes = document.GeneratePdf();
            _logger.LogInformation($"Generated antimicrobial prescription PDF for {prescription.Id}");
            return Task.FromResult(pdfBytes);
        }

        private void ComposeHeader(IContainer container, DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            container.Row(row =>
            {
                // Clinic information
                row.RelativeItem(3).Column(column =>
                {
                    if (!string.IsNullOrEmpty(options.ClinicName))
                    {
                        column.Item().Text(options.ClinicName).FontSize(16).Bold();
                        
                        if (!string.IsNullOrEmpty(options.ClinicAddress))
                        {
                            column.Item().Text(options.ClinicAddress).FontSize(9);
                        }
                        
                        if (!string.IsNullOrEmpty(options.ClinicPhone))
                        {
                            column.Item().Text($"Tel: {options.ClinicPhone}").FontSize(9);
                        }
                    }
                    else
                    {
                        column.Item().Text("PRESCRIÇÃO MÉDICA").FontSize(16).Bold();
                    }
                });

                // QR Code
                if (options.IncludeQRCode && !string.IsNullOrEmpty(prescription.VerificationCode))
                {
                    row.RelativeItem(1).AlignRight().Height(60).Width(60).Image(GenerateQRCodeImage(prescription.VerificationCode));
                }
            });

            container.PaddingTop(10).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
        }

        private void ComposeSimplePrescriptionContent(IContainer container, DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            container.PaddingVertical(15).Column(column =>
            {
                // Patient information
                column.Item().Text("Dados do Paciente").FontSize(12).Bold().Underline();
                column.Item().PaddingTop(5).Text($"Nome: {prescription.PatientName}").FontSize(11);
                column.Item().Text($"Documento: {prescription.PatientDocument}").FontSize(10);
                column.Item().Text($"Data: {prescription.IssuedAt:dd/MM/yyyy}").FontSize(10);

                // Medications
                column.Item().PaddingTop(20).Text("Medicamentos Prescritos:").FontSize(12).Bold().Underline();

                foreach (var item in prescription.Items)
                {
                    column.Item().PaddingTop(15).Column(medColumn =>
                    {
                        medColumn.Item().Text($"• {item.MedicationName}").FontSize(11).Bold();
                        medColumn.Item().PaddingTop(3).Text($"Dosagem: {item.Dosage}").FontSize(10);
                        medColumn.Item().Text($"Forma Farmacêutica: {item.PharmaceuticalForm}").FontSize(10);
                        medColumn.Item().Text($"Quantidade: {item.Quantity}").FontSize(10);
                        
                        if (!string.IsNullOrEmpty(item.Frequency))
                        {
                            medColumn.Item().Text($"Frequência: {item.Frequency}").FontSize(10);
                        }
                        
                        if (item.DurationDays > 0)
                        {
                            medColumn.Item().Text($"Duração: {item.DurationDays} dias").FontSize(10);
                        }
                        
                        if (!string.IsNullOrEmpty(item.Instructions))
                        {
                            medColumn.Item().PaddingTop(3).Text($"Instruções: {item.Instructions}").FontSize(10).Italic();
                        }
                    });
                }
            });
        }

        private void ComposeControlledPrescriptionContent(IContainer container, DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            container.PaddingVertical(15).Column(column =>
            {
                // Notification number
                column.Item().AlignCenter().Text($"NOTIFICAÇÃO DE RECEITA: {prescription.SequenceNumber ?? "N/A"}")
                    .FontSize(14).Bold().FontColor(Colors.Red.Darken2);

                column.Item().AlignCenter().Text($"TIPO: {GetControlTypeName(prescription.Type)}")
                    .FontSize(12).Bold();

                column.Item().PaddingTop(15);

                // Doctor identification
                column.Item().Text("IDENTIFICAÇÃO DO EMITENTE").FontSize(12).Bold().Underline();
                column.Item().PaddingTop(5).Text($"Nome: {prescription.DoctorName}").FontSize(10);
                column.Item().Text($"CRM: {prescription.DoctorCRM}/{prescription.DoctorCRMState}").FontSize(10);
                
                if (!string.IsNullOrEmpty(options.ClinicAddress))
                {
                    column.Item().Text($"Endereço: {options.ClinicAddress}").FontSize(10);
                }
                
                if (!string.IsNullOrEmpty(options.ClinicPhone))
                {
                    column.Item().Text($"Telefone: {options.ClinicPhone}").FontSize(10);
                }

                // Patient identification
                column.Item().PaddingTop(15).Text("IDENTIFICAÇÃO DO PACIENTE").FontSize(12).Bold().Underline();
                column.Item().PaddingTop(5).Text($"Nome: {prescription.PatientName}").FontSize(10);
                column.Item().Text($"Documento: {prescription.PatientDocument}").FontSize(10);

                // Medication (only 1 per controlled prescription as per ANVISA)
                column.Item().PaddingTop(15).Text("MEDICAMENTO PRESCRITO").FontSize(12).Bold().Underline();
                
                var medication = prescription.Items.FirstOrDefault();
                if (medication != null)
                {
                    column.Item().PaddingTop(5).Text($"{medication.MedicationName}").FontSize(13).Bold();
                    column.Item().Text($"Quantidade: {medication.Quantity}").FontSize(11);
                    column.Item().Text($"Dosagem: {medication.Dosage}").FontSize(11);
                    
                    if (!string.IsNullOrEmpty(medication.Instructions))
                    {
                        column.Item().Text($"Posologia: {medication.Instructions}").FontSize(11);
                    }
                }

                // Date and validity
                column.Item().PaddingTop(15).Text($"Data de Emissão: {prescription.IssuedAt:dd/MM/yyyy}").FontSize(11);
                column.Item().Text($"Validade: {prescription.ExpiresAt:dd/MM/yyyy} ({prescription.DaysUntilExpiration()} dias)")
                    .FontSize(11).Bold().FontColor(Colors.Red.Medium);
            });
        }

        private void ComposeAntimicrobialPrescriptionContent(IContainer container, DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            container.PaddingVertical(15).Column(column =>
            {
                // Title
                column.Item().AlignCenter().Text("RECEITA DE ANTIMICROBIANO").FontSize(14).Bold();
                column.Item().AlignCenter().Text("RDC 20/2011 ANVISA").FontSize(10).Italic();
                column.Item().PaddingTop(15);

                // Patient information
                column.Item().Text("Dados do Paciente").FontSize(12).Bold().Underline();
                column.Item().PaddingTop(5).Text($"Nome: {prescription.PatientName}").FontSize(11);
                column.Item().Text($"Documento: {prescription.PatientDocument}").FontSize(10);
                column.Item().Text($"Data: {prescription.IssuedAt:dd/MM/yyyy}").FontSize(10);

                // Medications
                column.Item().PaddingTop(15).Text("Medicamentos Prescritos:").FontSize(12).Bold().Underline();

                foreach (var item in prescription.Items)
                {
                    column.Item().PaddingTop(10).Column(medColumn =>
                    {
                        medColumn.Item().Text($"• {item.MedicationName}").FontSize(11).Bold();
                        medColumn.Item().PaddingTop(3).Text($"Dosagem: {item.Dosage}").FontSize(10);
                        medColumn.Item().Text($"Quantidade: {item.Quantity}").FontSize(10);
                        
                        if (!string.IsNullOrEmpty(item.Frequency))
                        {
                            medColumn.Item().Text($"Frequência: {item.Frequency}").FontSize(10);
                        }
                        
                        if (!string.IsNullOrEmpty(item.Instructions))
                        {
                            medColumn.Item().PaddingTop(3).Text($"Instruções: {item.Instructions}").FontSize(10).Italic();
                        }
                    });
                }

                // Mandatory warnings
                column.Item().PaddingTop(20).Background(Colors.Yellow.Lighten4).Padding(10).Column(warningColumn =>
                {
                    warningColumn.Item().Text("ATENÇÃO:").FontSize(11).Bold();
                    warningColumn.Item().Text($"• Validade: {prescription.DaysUntilExpiration()} dias a partir da data de emissão").FontSize(9);
                    warningColumn.Item().Text("• Retenção da 2ª via pela farmácia é obrigatória").FontSize(9);
                    warningColumn.Item().Text("• Não compartilhe este medicamento").FontSize(9);
                });
            });
        }

        private void ComposeFooter(IContainer container, DigitalPrescription prescription, PrescriptionPdfOptions options)
        {
            container.AlignCenter().Column(column =>
            {
                column.Item().PaddingTop(20).BorderTop(1).BorderColor(Colors.Grey.Lighten2);

                column.Item().PaddingTop(10).Text("Assinatura do Médico").FontSize(10);

                column.Item().PaddingTop(30).Width(200).BorderBottom(1).BorderColor(Colors.Black);

                column.Item().PaddingTop(5).Text($"Dr(a). {prescription.DoctorName}").FontSize(10).Bold();
                column.Item().Text($"CRM: {prescription.DoctorCRM} - {prescription.DoctorCRMState}").FontSize(9);

                if (!string.IsNullOrEmpty(prescription.VerificationCode))
                {
                    column.Item().PaddingTop(10).Text($"Código de Verificação: {prescription.VerificationCode}").FontSize(8).Italic();
                }

                if (prescription.SignedAt.HasValue)
                {
                    column.Item().Text($"Assinado digitalmente em: {prescription.SignedAt.Value:dd/MM/yyyy HH:mm}").FontSize(8).Italic();
                }
            });
        }

        private byte[] GenerateQRCodeImage(string data)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
                using var qrCode = new PngByteQRCode(qrCodeData);
                return qrCode.GetGraphic(5);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error generating QR code for data: {data}");
                // Return a small transparent PNG if QR code generation fails
                return new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }; // PNG header
            }
        }

        private PageSize GetPageSize(PaperSize size)
        {
            return size switch
            {
                PaperSize.A4 => PageSizes.A4,
                PaperSize.Letter => PageSizes.Letter,
                PaperSize.HalfPage => new PageSize(PageSizes.A4.Width, PageSizes.A4.Height / 2),
                _ => PageSizes.A4
            };
        }

        private string GetControlTypeName(PrescriptionType type)
        {
            return type switch
            {
                PrescriptionType.SpecialControlA => "CONTROLE ESPECIAL A (Entorpecentes)",
                PrescriptionType.SpecialControlB => "CONTROLE ESPECIAL B (Psicotrópicos)",
                PrescriptionType.SpecialControlC1 => "CONTROLE ESPECIAL C1 (Outros Controlados)",
                _ => "CONTROLADO"
            };
        }
    }
}
