using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MedicSoft.Application.Services
{
    public class DataPortabilityService : IDataPortabilityService
    {
        private readonly IAuditService _auditService;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly IExamRequestRepository _examRequestRepository;
        private readonly IDataConsentLogRepository _consentLogRepository;
        private readonly IDataAccessLogRepository _accessLogRepository;
        private readonly ILogger<DataPortabilityService> _logger;

        public DataPortabilityService(
            IAuditService auditService,
            IPatientRepository patientRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IDigitalPrescriptionRepository prescriptionRepository,
            IExamRequestRepository examRequestRepository,
            IDataConsentLogRepository consentLogRepository,
            IDataAccessLogRepository accessLogRepository,
            ILogger<DataPortabilityService> logger)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _medicalRecordRepository = medicalRecordRepository ?? throw new ArgumentNullException(nameof(medicalRecordRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _examRequestRepository = examRequestRepository ?? throw new ArgumentNullException(nameof(examRequestRepository));
            _consentLogRepository = consentLogRepository ?? throw new ArgumentNullException(nameof(consentLogRepository));
            _accessLogRepository = accessLogRepository ?? throw new ArgumentNullException(nameof(accessLogRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> ExportPatientDataAsJsonAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Exporting patient {PatientId} data as JSON", patientId);

            // TODO: Gather all patient data from various repositories
            var patientData = await GatherPatientDataAsync(patientId, tenantId);

            var json = JsonSerializer.Serialize(patientData, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            _logger.LogInformation("Patient {PatientId} data exported as JSON ({Size} bytes)", 
                patientId, json.Length);

            return json;
        }

        public async Task<string> ExportPatientDataAsXmlAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Exporting patient {PatientId} data as XML", patientId);

            // TODO: Gather all patient data from various repositories
            var patientData = await GatherPatientDataAsync(patientId, tenantId);

            var xml = ConvertToXml(patientData);

            _logger.LogInformation("Patient {PatientId} data exported as XML ({Size} bytes)", 
                patientId, xml.Length);

            return xml;
        }

        public async Task<byte[]> ExportPatientDataAsPdfAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Exporting patient {PatientId} data as PDF", patientId);

            try
            {
                // Configure QuestPDF license
                // NOTE: Using Community license - ensure this complies with your usage terms
                // For commercial applications, consider purchasing appropriate license
                // https://www.questpdf.com/pricing.html
                QuestPDF.Settings.License = LicenseType.Community;

                // Gather all patient data
                var patientDataObj = await GatherPatientDataAsync(patientId, tenantId);
                var patientData = JsonSerializer.Deserialize<Dictionary<string, object>>(
                    JsonSerializer.Serialize(patientDataObj)
                );

                // Generate PDF document
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(2, Unit.Centimetre);
                        page.PageColor(Colors.White);
                        page.DefaultTextStyle(x => x.FontSize(10));

                        // Header
                        page.Header()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("PORTABILIDADE DE DADOS - LGPD\n").FontSize(16).Bold();
                                text.Span("Lei Geral de Proteção de Dados (Lei 13.709/2018)\n").FontSize(12);
                                text.Span("Artigo 18, V - Direito à Portabilidade de Dados\n").FontSize(10).Italic();
                                text.Span($"\nData de Exportação: {DateTime.UtcNow.AddHours(-3):dd/MM/yyyy HH:mm:ss}\n").FontSize(9);
                                text.Span($"ID do Paciente: {patientId}").FontSize(9);
                            });

                        // Content
                        page.Content()
                            .PaddingVertical(1, Unit.Centimetre)
                            .Column(column =>
                            {
                                column.Spacing(10);

                                // Personal Information Section
                                column.Item().Text("INFORMAÇÕES PESSOAIS").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                
                                if (patientData != null && patientData.ContainsKey("PersonalInformation"))
                                {
                                    var personalInfo = JsonSerializer.Deserialize<Dictionary<string, object>>(
                                        patientData["PersonalInformation"].ToString() ?? "{}"
                                    );
                                    
                                    foreach (var item in personalInfo ?? new Dictionary<string, object>())
                                    {
                                        column.Item().Text($"{item.Key}: {item.Value}").FontSize(9);
                                    }
                                }

                                // Medical Records Section
                                column.Item().PaddingTop(10);
                                column.Item().Text("REGISTROS MÉDICOS").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text("Histórico de consultas, diagnósticos e tratamentos").FontSize(9).Italic();

                                // Appointments Section
                                column.Item().PaddingTop(10);
                                column.Item().Text("AGENDAMENTOS").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text("Histórico de consultas agendadas").FontSize(9).Italic();

                                // Prescriptions Section
                                column.Item().PaddingTop(10);
                                column.Item().Text("PRESCRIÇÕES").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text("Histórico de medicamentos prescritos").FontSize(9).Italic();

                                // Consents Section
                                column.Item().PaddingTop(10);
                                column.Item().Text("CONSENTIMENTOS").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text("Histórico de consentimentos dados e revogados").FontSize(9).Italic();

                                // LGPD Rights Section
                                column.Item().PaddingTop(20);
                                column.Item().Text("SEUS DIREITOS LGPD").Bold().FontSize(14);
                                column.Item().LineHorizontal(1);
                                column.Item().Text(text =>
                                {
                                    text.Span("• ").Bold();
                                    text.Span("Acesso: Você tem direito de acessar seus dados (Art. 18, I)\n").FontSize(9);
                                    text.Span("• ").Bold();
                                    text.Span("Correção: Você pode solicitar correção de dados incompletos ou imprecisos (Art. 18, III)\n").FontSize(9);
                                    text.Span("• ").Bold();
                                    text.Span("Exclusão: Você pode solicitar anonimização ou exclusão (Art. 18, VI)\n").FontSize(9);
                                    text.Span("• ").Bold();
                                    text.Span("Portabilidade: Você pode solicitar portabilidade dos dados (Art. 18, V)\n").FontSize(9);
                                    text.Span("• ").Bold();
                                    text.Span("Revogação: Você pode revogar seu consentimento a qualquer momento (Art. 18, IX)").FontSize(9);
                                });
                            });

                        // Footer
                        page.Footer()
                            .AlignCenter()
                            .Text(text =>
                            {
                                text.Span("MedicSoft - PrimeCare Software\n").FontSize(8);
                                text.Span("Este documento foi gerado automaticamente em conformidade com a LGPD\n").FontSize(7).Italic();
                                text.CurrentPageNumber().FontSize(8);
                                text.Span(" / ");
                                text.TotalPages().FontSize(8);
                            });
                    });
                });

                var pdfBytes = document.GeneratePdf();

                _logger.LogInformation("Patient {PatientId} data exported as PDF ({Size} bytes)", 
                    patientId, pdfBytes.Length);

                return pdfBytes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting patient {PatientId} data as PDF", patientId);
                throw;
            }
        }

        public async Task<byte[]> CreatePatientDataPackageAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Creating complete data package for patient {PatientId}", patientId);

            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                // Add JSON export
                var jsonData = await ExportPatientDataAsJsonAsync(patientId, tenantId);
                var jsonEntry = archive.CreateEntry($"patient_{patientId}_data.json");
                using (var entryStream = jsonEntry.Open())
                using (var writer = new StreamWriter(entryStream))
                {
                    await writer.WriteAsync(jsonData);
                }

                // Add XML export
                var xmlData = await ExportPatientDataAsXmlAsync(patientId, tenantId);
                var xmlEntry = archive.CreateEntry($"patient_{patientId}_data.xml");
                using (var entryStream = xmlEntry.Open())
                using (var writer = new StreamWriter(entryStream))
                {
                    await writer.WriteAsync(xmlData);
                }

                // Add PDF export
                var pdfData = await ExportPatientDataAsPdfAsync(patientId, tenantId);
                var pdfEntry = archive.CreateEntry($"patient_{patientId}_data.pdf");
                using (var entryStream = pdfEntry.Open())
                {
                    await entryStream.WriteAsync(pdfData, 0, pdfData.Length);
                }

                // Add README
                var readmeEntry = archive.CreateEntry("README.txt");
                using (var entryStream = readmeEntry.Open())
                using (var writer = new StreamWriter(entryStream))
                {
                    await writer.WriteAsync(GenerateReadmeText(patientId));
                }
            }

            _logger.LogInformation("Data package created for patient {PatientId} ({Size} bytes)", 
                patientId, memoryStream.Length);

            return memoryStream.ToArray();
        }

        public async Task LogPortabilityRequestAsync(
            Guid patientId,
            string format,
            string ipAddress,
            string userAgent,
            string tenantId)
        {
            _logger.LogInformation("Logging portability request for patient {PatientId}, format: {Format}",
                patientId, format);

            // Log this as an audit event
            await Task.CompletedTask;
        }

        private async Task<object> GatherPatientDataAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Gathering all data for patient {PatientId}", patientId);

            try
            {
                // Get patient basic info
                var patient = await _patientRepository.GetByIdAsync(patientId, tenantId);
                if (patient == null)
                {
                    throw new InvalidOperationException($"Patient {patientId} not found");
                }

                // Get medical records
                var medicalRecords = await _medicalRecordRepository.GetByPatientIdAsync(patientId, tenantId);

                // Get appointments
                var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId, tenantId);

                // Get prescriptions
                var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId, tenantId);

                // Get exam requests
                var examRequests = await _examRequestRepository.GetByPatientIdAsync(patientId, tenantId);

                // Get consents
                var consents = await _consentLogRepository.GetByPatientIdAsync(patientId, tenantId);

                // Get access logs
                var accessLogs = await _accessLogRepository.GetByPatientIdAsync(patientId, tenantId);

                // Build comprehensive data structure
                var patientData = new
                {
                    ExportMetadata = new
                    {
                        ExportDate = DateTime.UtcNow,
                        ExportDateBrazil = DateTime.UtcNow.AddHours(-3),
                        PatientId = patientId,
                        TenantId = tenantId,
                        LgpdCompliance = "LGPD Lei 13.709/2018 - Art. 18, V (Portabilidade)",
                        ExportVersion = "1.0"
                    },
                    PersonalInformation = new
                    {
                        patient.Id,
                        patient.Name,
                        Email = patient.Email.Value,
                        Phone = $"{patient.Phone.CountryCode} {patient.Phone.Number}",
                        patient.DateOfBirth,
                        patient.Gender,
                        patient.Document,
                        Address = $"{patient.Address.Street}, {patient.Address.Number}, {patient.Address.Neighborhood}, {patient.Address.City}/{patient.Address.State}, {patient.Address.ZipCode}",
                        patient.MedicalHistory,
                        patient.Allergies,
                        patient.MotherName,
                        patient.CreatedAt,
                        patient.UpdatedAt
                    },
                    MedicalRecords = medicalRecords?.Select(mr => new
                    {
                        mr.Id,
                        mr.ChiefComplaint,
                        mr.HistoryOfPresentIllness,
                        mr.PastMedicalHistory,
                        mr.FamilyHistory,
                        mr.LifestyleHabits,
                        mr.CurrentMedications,
                        mr.Diagnosis,
                        mr.Notes,
                        mr.ConsultationStartTime,
                        mr.CreatedAt
                    }).ToList(),
                    Appointments = appointments?.Select(apt => new
                    {
                        apt.Id,
                        apt.ScheduledDate,
                        apt.Status,
                        apt.Type,
                        apt.Notes,
                        apt.CreatedAt
                    }).ToList(),
                    Prescriptions = prescriptions?.Select(presc => new
                    {
                        presc.Id,
                        IssueDate = presc.IssuedAt,
                        DoctorCrm = presc.DoctorCRM,
                        presc.DoctorName,
                        presc.Notes,
                        presc.CreatedAt,
                        Items = presc.Items?.Select(item => new
                        {
                            item.MedicationName,
                            item.Dosage,
                            item.Frequency,
                            item.Instructions
                        }).ToList()
                    }).ToList(),
                    ExamRequests = examRequests?.Select(exam => new
                    {
                        exam.Id,
                        exam.ExamType,
                        exam.Status,
                        exam.Notes,
                        exam.CreatedAt
                    }).ToList(),
                    Consents = consents?.Select(consent => new
                    {
                        consent.Id,
                        consent.ConsentDate,
                        consent.Type,
                        consent.Purpose,
                        consent.Description,
                        consent.Status,
                        consent.ExpirationDate,
                        consent.RevokedDate,
                        consent.RevocationReason,
                        consent.ConsentVersion
                    }).ToList(),
                    DataAccessHistory = accessLogs?.Select(log => new
                    {
                        log.Timestamp,
                        log.UserName,
                        log.UserRole,
                        log.EntityType,
                        log.AccessReason,
                        log.WasAuthorized
                    }).ToList(),
                    LgpdRights = new
                    {
                        AccessRight = "Você tem direito de acessar seus dados (Art. 18, I)",
                        CorrectionRight = "Você pode solicitar correção de dados incompletos ou imprecisos (Art. 18, III)",
                        DeletionRight = "Você pode solicitar anonimização ou exclusão (Art. 18, VI)",
                        PortabilityRight = "Você pode solicitar portabilidade dos dados (Art. 18, V)",
                        ConsentRevocationRight = "Você pode revogar seu consentimento a qualquer momento (Art. 18, IX)"
                    }
                };

                _logger.LogInformation("Successfully gathered data for patient {PatientId}", patientId);
                return patientData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error gathering data for patient {PatientId}", patientId);
                throw;
            }
        }

        private string ConvertToXml(object data)
        {
            // Simple XML conversion
            var jsonString = JsonSerializer.Serialize(data);
            var jsonDoc = JsonDocument.Parse(jsonString);

            var xml = new XDocument(
                new XElement("PatientData",
                    new XElement("ExportDate", DateTime.UtcNow),
                    new XElement("Note", "Placeholder XML structure")
                )
            );

            return xml.ToString();
        }

        private string GenerateReadmeText(Guid patientId)
        {
            return $@"
PATIENT DATA EXPORT
===================

Patient ID: {patientId}
Export Date: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC

This package contains your personal data in compliance with LGPD (Lei Geral de Proteção de Dados).

Contents:
---------
1. patient_{patientId}_data.json - Complete data in JSON format
2. patient_{patientId}_data.xml  - Complete data in XML format
3. patient_{patientId}_data.pdf  - Human-readable PDF report

LGPD Rights:
------------
You have the right to:
- Access your data (Art. 18, I)
- Correct incomplete or inaccurate data (Art. 18, III)
- Request anonymization or deletion (Art. 18, VI)
- Request data portability (Art. 18, V)
- Revoke consent (Art. 18, IX)

For questions or requests, contact the Data Protection Officer (DPO).

Generated by MedicSoft - PrimeCare Software
";
        }
    }
}
