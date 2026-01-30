using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for generating SNGPC XML files according to ANVISA schema v2.1
    /// RDC 22/2014 - Sistema Nacional de Gerenciamento de Produtos Controlados
    /// </summary>
    public interface ISNGPCXmlGeneratorService
    {
        Task<string> GenerateXmlAsync(SNGPCReport report, IEnumerable<DigitalPrescription> prescriptions);
        Task<string> SignXmlAsync(string xmlContent, X509Certificate2 certificate);
    }

    public class SNGPCXmlGeneratorService : ISNGPCXmlGeneratorService
    {
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private const string SNGPC_NAMESPACE = "http://www.anvisa.gov.br/sngpc/v2.1";
        private const string SCHEMA_VERSION = "2.1";

        public SNGPCXmlGeneratorService(IDigitalPrescriptionRepository prescriptionRepository)
        {
            _prescriptionRepository = prescriptionRepository;
        }

        public Task<string> GenerateXmlAsync(SNGPCReport report, IEnumerable<DigitalPrescription> prescriptions)
        {
            if (report == null)
                throw new ArgumentNullException(nameof(report));
            
            if (prescriptions == null || !prescriptions.Any())
                throw new ArgumentException("At least one prescription is required to generate SNGPC XML", nameof(prescriptions));

            var xmlDocument = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                CreateRootElement(report, prescriptions)
            );

            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\n",
                NewLineHandling = NewLineHandling.Replace,
                OmitXmlDeclaration = false
            };

            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter, settings);
            xmlDocument.Save(xmlWriter);
            xmlWriter.Flush();

            return Task.FromResult(stringWriter.ToString());
        }

        private XElement CreateRootElement(SNGPCReport report, IEnumerable<DigitalPrescription> prescriptions)
        {
            var ns = XNamespace.Get(SNGPC_NAMESPACE);
            var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

            var root = new XElement(ns + "SNGPC",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(xsi + "schemaLocation", $"{SNGPC_NAMESPACE} SNGPC_v{SCHEMA_VERSION}.xsd"),
                new XAttribute("versao", SCHEMA_VERSION),
                CreateHeaderElement(ns, report),
                CreatePrescriptionsElement(ns, prescriptions)
            );

            return root;
        }

        private XElement CreateHeaderElement(XNamespace ns, SNGPCReport report)
        {
            return new XElement(ns + "Cabecalho",
                new XElement(ns + "Versao", SCHEMA_VERSION),
                new XElement(ns + "TipoDocumento", "ESCRITURACAO"),
                new XElement(ns + "PeriodoInicio", report.ReportPeriodStart.ToString("yyyy-MM-dd")),
                new XElement(ns + "PeriodoFim", report.ReportPeriodEnd.ToString("yyyy-MM-dd")),
                new XElement(ns + "DataGeracao", report.GeneratedAt.ToString("yyyy-MM-ddTHH:mm:ss")),
                new XElement(ns + "MesReferencia", report.Month.ToString("D2")),
                new XElement(ns + "AnoReferencia", report.Year),
                new XElement(ns + "QuantidadeReceitas", report.TotalPrescriptions),
                new XElement(ns + "QuantidadeItens", report.TotalItems)
            );
        }

        private XElement CreatePrescriptionsElement(XNamespace ns, IEnumerable<DigitalPrescription> prescriptions)
        {
            var prescriptionsElement = new XElement(ns + "Receitas");

            foreach (var prescription in prescriptions)
            {
                prescriptionsElement.Add(CreatePrescriptionElement(ns, prescription));
            }

            return prescriptionsElement;
        }

        private XElement CreatePrescriptionElement(XNamespace ns, DigitalPrescription prescription)
        {
            return new XElement(ns + "Receita",
                new XElement(ns + "NumeroReceita", prescription.SequenceNumber ?? prescription.Id.ToString()),
                new XElement(ns + "TipoReceituario", MapPrescriptionType(prescription.Type)),
                new XElement(ns + "DataEmissao", prescription.IssuedAt.ToString("yyyy-MM-dd")),
                CreatePrescribingDoctorElement(ns, prescription),
                CreatePatientElement(ns, prescription),
                CreateItemsElement(ns, prescription)
            );
        }

        private XElement CreatePrescribingDoctorElement(XNamespace ns, DigitalPrescription prescription)
        {
            return new XElement(ns + "Prescritor",
                new XElement(ns + "Nome", SanitizeText(prescription.DoctorName)),
                new XElement(ns + "CRM", prescription.DoctorCRM),
                new XElement(ns + "UF", prescription.DoctorCRMState)
            );
        }

        private XElement CreatePatientElement(XNamespace ns, DigitalPrescription prescription)
        {
            var patientElement = new XElement(ns + "Paciente",
                new XElement(ns + "Nome", SanitizeText(prescription.PatientName))
            );

            // Add CPF if available (remove formatting)
            var document = prescription.PatientDocument?.Replace(".", "").Replace("-", "").Trim();
            if (!string.IsNullOrEmpty(document) && document.Length == 11)
            {
                patientElement.Add(new XElement(ns + "CPF", document));
            }
            else if (!string.IsNullOrEmpty(document))
            {
                // If not CPF, add as RG
                patientElement.Add(new XElement(ns + "RG", SanitizeText(document)));
            }

            return patientElement;
        }

        private XElement CreateItemsElement(XNamespace ns, DigitalPrescription prescription)
        {
            var itemsElement = new XElement(ns + "Itens");

            foreach (var item in prescription.Items)
            {
                if (item.IsControlledSubstance)
                {
                    itemsElement.Add(CreateItemElement(ns, item));
                }
            }

            return itemsElement;
        }

        private XElement CreateItemElement(XNamespace ns, DigitalPrescriptionItem item)
        {
            var itemElement = new XElement(ns + "Item",
                new XElement(ns + "Medicamento", SanitizeText(item.MedicationName)),
                new XElement(ns + "Quantidade", item.Quantity),
                new XElement(ns + "Unidade", "UN") // Default unit
            );

            // Add DCB/DCI (generic name) if available
            if (!string.IsNullOrWhiteSpace(item.GenericName))
            {
                itemElement.Add(new XElement(ns + "NomeGenerico", SanitizeText(item.GenericName)));
            }

            // Add active ingredient if available
            if (!string.IsNullOrWhiteSpace(item.ActiveIngredient))
            {
                itemElement.Add(new XElement(ns + "PrincipioAtivo", SanitizeText(item.ActiveIngredient)));
            }

            // Add ANVISA registration if available
            if (!string.IsNullOrWhiteSpace(item.AnvisaRegistration))
            {
                itemElement.Add(new XElement(ns + "RegistroANVISA", SanitizeText(item.AnvisaRegistration)));
            }

            // Add controlled substance list classification
            if (item.ControlledList.HasValue)
            {
                itemElement.Add(new XElement(ns + "ListaControlada", MapControlledList(item.ControlledList.Value)));
            }

            // Add dosage information
            if (!string.IsNullOrWhiteSpace(item.Dosage))
            {
                itemElement.Add(new XElement(ns + "Dosagem", SanitizeText(item.Dosage)));
            }

            // Add pharmaceutical form
            if (!string.IsNullOrWhiteSpace(item.PharmaceuticalForm))
            {
                itemElement.Add(new XElement(ns + "FormaFarmaceutica", SanitizeText(item.PharmaceuticalForm)));
            }

            // Add posology
            var posology = BuildPosologyText(item);
            if (!string.IsNullOrEmpty(posology))
            {
                itemElement.Add(new XElement(ns + "Posologia", SanitizeText(posology)));
            }

            return itemElement;
        }

        private string BuildPosologyText(DigitalPrescriptionItem item)
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(item.Frequency))
            {
                parts.Add(item.Frequency);
            }

            if (item.DurationDays > 0)
            {
                parts.Add($"por {item.DurationDays} dias");
            }

            if (!string.IsNullOrWhiteSpace(item.AdministrationRoute))
            {
                parts.Add($"via {item.AdministrationRoute}");
            }

            return parts.Any() ? string.Join(", ", parts) : string.Empty;
        }

        private string MapPrescriptionType(PrescriptionType type)
        {
            return type switch
            {
                PrescriptionType.Simple => "SIMPLES",
                PrescriptionType.SpecialControlA => "CONTROLE_ESPECIAL_A",
                PrescriptionType.SpecialControlB => "CONTROLE_ESPECIAL_B",
                PrescriptionType.SpecialControlC1 => "CONTROLE_ESPECIAL_C1",
                PrescriptionType.Antimicrobial => "ANTIMICROBIANO",
                _ => "SIMPLES"
            };
        }

        private string MapControlledList(ControlledSubstanceList list)
        {
            return list switch
            {
                ControlledSubstanceList.A1_Narcotics => "A1",
                ControlledSubstanceList.A2_Psychotropics => "A2",
                ControlledSubstanceList.A3_Psychotropics => "A3",
                ControlledSubstanceList.B1_Psychotropics => "B1",
                ControlledSubstanceList.B2_Anorexigenics => "B2",
                ControlledSubstanceList.C1_OtherControlled => "C1",
                ControlledSubstanceList.C2_Retinoids => "C2",
                ControlledSubstanceList.C3_Immunosuppressants => "C3",
                ControlledSubstanceList.C4_Antiretrovirals => "C4",
                ControlledSubstanceList.C5_Anabolics => "C5",
                _ => "C1"
            };
        }

        private string SanitizeText(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Remove XML invalid characters and trim
            var sanitized = text.Trim();
            sanitized = System.Text.RegularExpressions.Regex.Replace(sanitized, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");
            
            return sanitized;
        }

        /// <summary>
        /// Signs XML content using X509 certificate for ANVISA compliance.
        /// Implements XML-DSig standard required for SNGPC transmission.
        /// </summary>
        /// <param name="xmlContent">XML content to sign</param>
        /// <param name="certificate">X509 certificate for signing</param>
        /// <returns>Signed XML content</returns>
        public Task<string> SignXmlAsync(string xmlContent, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new ArgumentException("XML content cannot be empty", nameof(xmlContent));

            if (certificate == null)
                throw new ArgumentNullException(nameof(certificate));

            if (!certificate.HasPrivateKey)
                throw new InvalidOperationException("Certificate must have a private key for signing");

            try
            {
                // Load XML document
                var xmlDoc = new XmlDocument();
                xmlDoc.PreserveWhitespace = true;
                xmlDoc.LoadXml(xmlContent);

                // Create signed XML object
                var signedXml = new SignedXml(xmlDoc);
                signedXml.SigningKey = certificate.GetRSAPrivateKey();

                // Create reference to sign entire document
                var reference = new Reference();
                reference.Uri = ""; // Empty URI signs the whole document
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                
                // Add C14N transformation (Canonical XML 1.0)
                reference.AddTransform(new XmlDsigC14NTransform());
                
                signedXml.AddReference(reference);

                // Add certificate information to signature
                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
                signedXml.KeyInfo = keyInfo;

                // Compute signature
                signedXml.ComputeSignature();

                // Get signature XML element
                var signatureElement = signedXml.GetXml();

                // Append signature to document root
                xmlDoc.DocumentElement?.AppendChild(xmlDoc.ImportNode(signatureElement, true));

                // Return signed XML as string
                using var stringWriter = new StringWriter();
                using var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
                {
                    Encoding = Encoding.UTF8,
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\n",
                    NewLineHandling = NewLineHandling.Replace
                });
                xmlDoc.Save(xmlWriter);
                xmlWriter.Flush();

                return Task.FromResult(stringWriter.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to sign XML: {ex.Message}", ex);
            }
        }
    }
}
