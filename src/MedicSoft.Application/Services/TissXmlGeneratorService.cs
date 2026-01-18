using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for generating TISS XML files
    /// TISS Standard Version: 4.02.00
    /// </summary>
    public class TissXmlGeneratorService : ITissXmlGeneratorService
    {
        private const string TissVersion = "4.02.00";
        private const string TissNamespace = "http://www.ans.gov.br/padroes/tiss/schemas";

        public string GetTissVersion() => TissVersion;

        public async Task<string> GenerateBatchXmlAsync(TissBatch batch, string outputPath)
        {
            if (batch == null)
                throw new ArgumentNullException(nameof(batch));

            if (string.IsNullOrWhiteSpace(outputPath))
                throw new ArgumentException("Output path cannot be empty", nameof(outputPath));

            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            var xmlFileName = $"TISS_Batch_{batch.BatchNumber}_{DateTime.UtcNow:yyyyMMddHHmmss}.xml";
            var xmlFilePath = Path.Combine(outputPath, xmlFileName);

            // Create XML document
            var doc = new XDocument(
                new XDeclaration("1.0", "UTF-8", "yes"),
                CreateTissLoteGuias(batch)
            );

            // Save with proper formatting
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                Encoding = Encoding.UTF8,
                OmitXmlDeclaration = false
            };

            await using var writer = XmlWriter.Create(xmlFilePath, settings);
            await doc.SaveAsync(writer, default);

            return xmlFilePath;
        }

        private XElement CreateTissLoteGuias(TissBatch batch)
        {
            var ns = XNamespace.Get(TissNamespace);
            
            return new XElement(ns + "tissLoteGuias",
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute("versao", TissVersion),
                
                // Cabeçalho do lote
                CreateCabecalho(batch),
                
                // Guias
                CreateGuias(batch),
                
                // Rodapé
                CreateRodape(batch)
            );
        }

        private XElement CreateCabecalho(TissBatch batch)
        {
            return new XElement("cabecalho",
                new XElement("identificacaoTransacao",
                    new XElement("tipoTransacao", "ENVIO_LOTE_GUIAS"),
                    new XElement("sequencialTransacao", batch.BatchNumber),
                    new XElement("dataTransacao", DateTime.UtcNow.ToString("yyyy-MM-dd")),
                    new XElement("horaTransacao", DateTime.UtcNow.ToString("HH:mm:ss"))
                ),
                new XElement("origem",
                    new XElement("codigoPrestador", batch.Clinic?.Document ?? ""),
                    new XElement("nomeContratado", batch.Clinic?.Name ?? "")
                ),
                new XElement("destino",
                    new XElement("registroANS", batch.Operator?.RegisterNumber ?? ""),
                    new XElement("nomeOperadora", batch.Operator?.TradeName ?? "")
                ),
                new XElement("versaoPadrao", TissVersion)
            );
        }

        private XElement CreateGuias(TissBatch batch)
        {
            var guiasElement = new XElement("guias");

            foreach (var guide in batch.Guides)
            {
                guiasElement.Add(CreateGuia(guide));
            }

            return guiasElement;
        }

        private XElement CreateGuia(TissGuide guide)
        {
            // Different guide types have different XML structures
            // This is a simplified version for consultation guides
            return guide.GuideType switch
            {
                TissGuideType.Consultation => CreateGuiaConsulta(guide),
                TissGuideType.SPSADT => CreateGuiaSPSADT(guide),
                _ => CreateGuiaConsulta(guide) // Default to consultation
            };
        }

        private XElement CreateGuiaConsulta(TissGuide guide)
        {
            return new XElement("guiaConsulta",
                new XElement("cabecalhoGuia",
                    new XElement("registroANS", guide.PatientHealthInsurance?.HealthInsurancePlan?.RegisterNumber ?? ""),
                    new XElement("numeroGuiaPrestador", guide.GuideNumber),
                    new XElement("numeroGuiaOperadora", guide.AuthorizationNumber ?? "")
                ),
                new XElement("dadosBeneficiario",
                    new XElement("numeroCarteira", guide.PatientHealthInsurance?.CardNumber ?? ""),
                    new XElement("nomeBeneficiario", guide.PatientHealthInsurance?.Patient?.Name ?? ""),
                    new XElement("validadeCarteira", guide.PatientHealthInsurance?.ValidUntil?.ToString("yyyy-MM-dd") ?? "")
                ),
                new XElement("dadosAtendimento",
                    new XElement("dataAtendimento", guide.ServiceDate.ToString("yyyy-MM-dd")),
                    new XElement("tipoConsulta", "1"), // 1 = Primeira consulta
                    new XElement("indicacaoAcidente", "0") // 0 = Não
                ),
                CreateProcedimentosRealizados(guide),
                new XElement("valorTotal",
                    new XElement("valorProcedimentos", guide.TotalAmount.ToString("F2")),
                    new XElement("valorTotal", guide.TotalAmount.ToString("F2"))
                )
            );
        }

        private XElement CreateGuiaSPSADT(TissGuide guide)
        {
            return new XElement("guiaSP-SADT",
                new XElement("cabecalhoGuia",
                    new XElement("registroANS", guide.PatientHealthInsurance?.HealthInsurancePlan?.RegisterNumber ?? ""),
                    new XElement("numeroGuiaPrestador", guide.GuideNumber),
                    new XElement("numeroGuiaOperadora", guide.AuthorizationNumber ?? "")
                ),
                new XElement("dadosBeneficiario",
                    new XElement("numeroCarteira", guide.PatientHealthInsurance?.CardNumber ?? ""),
                    new XElement("nomeBeneficiario", guide.PatientHealthInsurance?.Patient?.Name ?? ""),
                    new XElement("validadeCarteira", guide.PatientHealthInsurance?.ValidUntil?.ToString("yyyy-MM-dd") ?? "")
                ),
                new XElement("dadosSolicitacao",
                    new XElement("dataAutorizacao", guide.ServiceDate.ToString("yyyy-MM-dd")),
                    new XElement("numeroGuiaSolicitacao", guide.AuthorizationNumber ?? guide.GuideNumber)
                ),
                CreateProcedimentosRealizados(guide),
                new XElement("valorTotal",
                    new XElement("valorProcedimentos", guide.TotalAmount.ToString("F2")),
                    new XElement("valorTotal", guide.TotalAmount.ToString("F2"))
                )
            );
        }

        private XElement CreateProcedimentosRealizados(TissGuide guide)
        {
            var procedimentosElement = new XElement("procedimentosRealizados");

            foreach (var procedure in guide.Procedures)
            {
                procedimentosElement.Add(
                    new XElement("procedimento",
                        new XElement("data", guide.ServiceDate.ToString("yyyy-MM-dd")),
                        new XElement("codigoProcedimento",
                            new XElement("codigoTabela", "22"), // 22 = TUSS
                            new XElement("codigoProcedimento", procedure.ProcedureCode)
                        ),
                        new XElement("descricao", procedure.ProcedureDescription),
                        new XElement("quantidadeExecutada", procedure.Quantity),
                        new XElement("valorUnitario", procedure.UnitPrice.ToString("F2")),
                        new XElement("valorTotal", procedure.TotalPrice.ToString("F2"))
                    )
                );
            }

            return procedimentosElement;
        }

        private XElement CreateRodape(TissBatch batch)
        {
            return new XElement("rodape",
                new XElement("hash", GenerateHash(batch)),
                new XElement("quantidadeGuias", batch.GetGuideCount()),
                new XElement("valorTotal", batch.GetTotalAmount().ToString("F2"))
            );
        }

        private string GenerateHash(TissBatch batch)
        {
            // Simplified hash generation - in production, should use proper cryptographic hash
            var content = $"{batch.BatchNumber}{batch.CreatedDate:yyyyMMddHHmmss}{batch.GetGuideCount()}{batch.GetTotalAmount()}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hashBytes);
        }

        public async Task<bool> ValidateXmlAsync(string xmlPath)
        {
            try
            {
                if (!File.Exists(xmlPath))
                    return false;

                // Load and validate XML structure
                var doc = await Task.Run(() => XDocument.Load(xmlPath));
                
                // Basic validation: check root element
                var root = doc.Root;
                if (root == null || !root.Name.LocalName.Contains("tissLoteGuias"))
                    return false;

                // Check for required elements
                var hasHeader = root.Element("cabecalho") != null;
                var hasGuides = root.Element("guias") != null;
                var hasFooter = root.Element("rodape") != null;

                return hasHeader && hasGuides && hasFooter;
            }
            catch
            {
                return false;
            }
        }

        public async Task<TissXmlValidationResult> ValidateXmlContentAsync(string xmlContent)
        {
            var result = new TissXmlValidationResult
            {
                IsValid = true,
                ValidationErrors = new System.Collections.Generic.List<string>()
            };

            try
            {
                if (string.IsNullOrWhiteSpace(xmlContent))
                {
                    result.IsValid = false;
                    result.ErrorMessage = "XML content is empty";
                    return result;
                }

                // Parse XML
                var doc = await Task.Run(() => XDocument.Parse(xmlContent));
                
                // Validate structure
                var root = doc.Root;
                if (root == null)
                {
                    result.IsValid = false;
                    result.ErrorMessage = "Invalid XML: no root element";
                    return result;
                }

                if (!root.Name.LocalName.Contains("tissLoteGuias"))
                {
                    result.ValidationErrors.Add("Root element should be 'tissLoteGuias'");
                    result.IsValid = false;
                }

                // Validate required sections
                if (root.Element("cabecalho") == null)
                {
                    result.ValidationErrors.Add("Missing required element: cabecalho");
                    result.IsValid = false;
                }

                if (root.Element("guias") == null)
                {
                    result.ValidationErrors.Add("Missing required element: guias");
                    result.IsValid = false;
                }

                if (root.Element("rodape") == null)
                {
                    result.ValidationErrors.Add("Missing required element: rodape");
                    result.IsValid = false;
                }

                if (!result.IsValid)
                {
                    result.ErrorMessage = "XML validation failed. See ValidationErrors for details.";
                }
            }
            catch (Exception ex)
            {
                result.IsValid = false;
                result.ErrorMessage = $"XML parsing error: {ex.Message}";
            }

            return result;
        }
    }
}
