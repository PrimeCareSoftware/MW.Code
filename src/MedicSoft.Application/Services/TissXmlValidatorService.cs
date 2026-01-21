using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for validating TISS XML files against ANS schemas
    /// TISS Standard Version: 4.02.00
    /// 
    /// ANS Schema Information:
    /// - Official TISS schemas can be downloaded from ANS website: https://www.gov.br/ans/pt-br/assuntos/prestadores/padrao-para-troca-de-informacao-de-saude-suplementar-2013-tiss
    /// - TISS 4.02.00 schemas should be placed in: wwwroot/schemas/tiss/4.02.00/
    /// - Main schema files: tissLoteGuias_4_02_00.xsd, tissGuiaConsulta_4_02_00.xsd, tissGuiaSPSADT_4_02_00.xsd
    /// </summary>
    public class TissXmlValidatorService : ITissXmlValidatorService
    {
        private const string TissVersion = "4.02.00";
        private const string TissNamespace = "http://www.ans.gov.br/padroes/tiss/schemas";
        
        private readonly ILogger<TissXmlValidatorService> _logger;
        private readonly string _schemaPath;
        private XmlSchemaSet? _schemaSet;

        public TissXmlValidatorService(ILogger<TissXmlValidatorService> logger)
        {
            _logger = logger;
            // Schema path relative to application root
            _schemaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "schemas", "tiss", TissVersion);
        }

        public string GetTissVersion() => TissVersion;

        public async Task<ValidationResult> ValidateGuideXmlAsync(string xml)
        {
            _logger.LogInformation("Starting guide XML validation");
            
            var result = new ValidationResult { IsValid = true };

            try
            {
                // First validate XML structure
                var structureResult = await ValidateXmlStructureAsync(xml);
                if (!structureResult.IsValid)
                {
                    return structureResult;
                }

                // Parse XML
                var doc = XDocument.Parse(xml);
                var root = doc.Root;

                if (root == null)
                {
                    result.AddError("XML root element is missing");
                    return result;
                }

                // Validate guide-specific elements
                ValidateGuideElements(root, result);

                // Validate against schema if available
                if (IsSchemaAvailable())
                {
                    await ValidateAgainstSchemaAsync(xml, result, "guide");
                }
                else
                {
                    result.AddWarning("ANS schema files not found. Only basic validation performed. " +
                        "Download TISS schemas from ANS and place in: " + _schemaPath);
                }

                _logger.LogInformation("Guide XML validation completed. Valid: {IsValid}, Errors: {ErrorCount}, Warnings: {WarningCount}",
                    result.IsValid, result.ErrorCount, result.WarningCount);
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error during guide validation");
                result.AddError($"XML parsing error: {ex.Message}", ex.LineNumber.ToString(), ex.LinePosition.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during guide XML validation");
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        public async Task<ValidationResult> ValidateBatchXmlAsync(string xml)
        {
            _logger.LogInformation("Starting batch XML validation");
            
            var result = new ValidationResult { IsValid = true };

            try
            {
                // First validate XML structure
                var structureResult = await ValidateXmlStructureAsync(xml);
                if (!structureResult.IsValid)
                {
                    return structureResult;
                }

                // Parse XML
                var doc = XDocument.Parse(xml);
                var root = doc.Root;

                if (root == null)
                {
                    result.AddError("XML root element is missing");
                    return result;
                }

                // Validate batch-specific elements
                ValidateBatchElements(root, result);

                // Validate against schema if available
                if (IsSchemaAvailable())
                {
                    await ValidateAgainstSchemaAsync(xml, result, "batch");
                }
                else
                {
                    result.AddWarning("ANS schema files not found. Only basic validation performed. " +
                        "Download TISS 4.02.00 schemas from ANS and place in: " + _schemaPath);
                }

                _logger.LogInformation("Batch XML validation completed. Valid: {IsValid}, Errors: {ErrorCount}, Warnings: {WarningCount}",
                    result.IsValid, result.ErrorCount, result.WarningCount);
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML parsing error during batch validation");
                result.AddError($"XML parsing error: {ex.Message}", ex.LineNumber.ToString(), ex.LinePosition.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during batch XML validation");
                result.AddError($"Validation error: {ex.Message}");
            }

            return result;
        }

        public async Task<ValidationResult> ValidateXmlStructureAsync(string xml)
        {
            var result = new ValidationResult { IsValid = true };

            try
            {
                if (string.IsNullOrWhiteSpace(xml))
                {
                    result.AddError("XML content is empty");
                    return result;
                }

                // Validate well-formedness by parsing
                var doc = await Task.Run(() => XDocument.Parse(xml));
                
                // Basic structure validation
                if (doc.Root == null)
                {
                    result.AddError("XML document has no root element");
                    return result;
                }

                // Check for XML declaration
                if (doc.Declaration == null)
                {
                    result.AddWarning("XML declaration is missing. Recommended: <?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                }
                else if (doc.Declaration.Encoding?.ToUpperInvariant() != "UTF-8")
                {
                    result.AddWarning($"XML encoding is '{doc.Declaration.Encoding}'. Recommended: UTF-8");
                }

                _logger.LogDebug("XML structure validation passed");
            }
            catch (XmlException ex)
            {
                _logger.LogError(ex, "XML structure validation failed");
                result.AddError($"XML is not well-formed: {ex.Message}", ex.LineNumber.ToString(), ex.LinePosition.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during structure validation");
                result.AddError($"Structure validation error: {ex.Message}");
            }

            return result;
        }

        private void ValidateGuideElements(XElement root, ValidationResult result)
        {
            var localName = root.Name.LocalName;

            // Check if it's a valid guide type
            if (!localName.StartsWith("guia", StringComparison.OrdinalIgnoreCase))
            {
                result.AddError($"Root element '{localName}' is not a valid TISS guide element. Expected: guiaConsulta, guiaSP-SADT, etc.");
                return;
            }

            // Validate required guide sections
            ValidateRequiredElement(root, "cabecalhoGuia", result);
            ValidateRequiredElement(root, "dadosBeneficiario", result);
            
            // Validate beneficiary data
            var beneficiary = root.Element("dadosBeneficiario");
            if (beneficiary != null)
            {
                ValidateRequiredElement(beneficiary, "numeroCarteira", result);
                ValidateRequiredElement(beneficiary, "nomeBeneficiario", result);
            }

            // Check for namespace
            if (root.Name.NamespaceName != TissNamespace)
            {
                result.AddWarning($"Guide uses namespace '{root.Name.NamespaceName}'. Expected TISS namespace: {TissNamespace}");
            }
        }

        private void ValidateBatchElements(XElement root, ValidationResult result)
        {
            var localName = root.Name.LocalName;

            // Validate root element
            if (!localName.Contains("tissLoteGuias"))
            {
                result.AddError($"Root element '{localName}' is not 'tissLoteGuias'");
                return;
            }

            // Validate TISS version
            var versionAttr = root.Attribute("versao");
            if (versionAttr == null)
            {
                result.AddError("Missing required attribute 'versao' on root element");
            }
            else if (versionAttr.Value != TissVersion)
            {
                result.AddWarning($"TISS version is '{versionAttr.Value}'. Expected: {TissVersion}");
            }

            // Validate required sections
            ValidateRequiredElement(root, "cabecalho", result);
            ValidateRequiredElement(root, "guias", result);
            ValidateRequiredElement(root, "rodape", result);

            // Validate header
            var header = root.Element("cabecalho");
            if (header != null)
            {
                ValidateRequiredElement(header, "identificacaoTransacao", result);
                ValidateRequiredElement(header, "origem", result);
                ValidateRequiredElement(header, "destino", result);
                ValidateRequiredElement(header, "versaoPadrao", result);
            }

            // Validate guides exist
            var guides = root.Element("guias");
            if (guides != null && !guides.HasElements)
            {
                result.AddError("Element 'guias' must contain at least one guide");
            }

            // Validate footer
            var footer = root.Element("rodape");
            if (footer != null)
            {
                ValidateRequiredElement(footer, "quantidadeGuias", result);
                ValidateRequiredElement(footer, "valorTotal", result);
            }

            // Check for namespace
            if (root.Name.NamespaceName != TissNamespace)
            {
                result.AddWarning($"Batch uses namespace '{root.Name.NamespaceName}'. Expected TISS namespace: {TissNamespace}");
            }
        }

        private void ValidateRequiredElement(XElement parent, string elementName, ValidationResult result)
        {
            if (parent.Element(elementName) == null)
            {
                result.AddError($"Missing required element '{elementName}' in '{parent.Name.LocalName}'");
            }
        }

        private bool IsSchemaAvailable()
        {
            if (!Directory.Exists(_schemaPath))
            {
                _logger.LogWarning("Schema directory not found: {SchemaPath}", _schemaPath);
                return false;
            }

            var schemaFiles = Directory.GetFiles(_schemaPath, "*.xsd");
            if (schemaFiles.Length == 0)
            {
                _logger.LogWarning("No XSD schema files found in: {SchemaPath}", _schemaPath);
                return false;
            }

            return true;
        }

        private async Task ValidateAgainstSchemaAsync(string xml, ValidationResult result, string type)
        {
            try
            {
                if (_schemaSet == null)
                {
                    _schemaSet = new XmlSchemaSet();
                    
                    // Load all XSD files from schema directory
                    var schemaFiles = Directory.GetFiles(_schemaPath, "*.xsd");
                    foreach (var schemaFile in schemaFiles)
                    {
                        _logger.LogDebug("Loading schema file: {SchemaFile}", schemaFile);
                        _schemaSet.Add(TissNamespace, schemaFile);
                    }

                    _schemaSet.Compile();
                }

                // Validate XML against schema
                var doc = XDocument.Parse(xml);
                doc.Validate(_schemaSet, (sender, args) =>
                {
                    if (args.Severity == XmlSeverityType.Error)
                    {
                        result.AddError(args.Message, args.Exception?.LineNumber.ToString(), args.Exception?.LinePosition.ToString());
                        _logger.LogError("Schema validation error: {Message}", args.Message);
                    }
                    else
                    {
                        result.AddWarning(args.Message, args.Exception?.LineNumber.ToString());
                        _logger.LogWarning("Schema validation warning: {Message}", args.Message);
                    }
                });

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during schema validation");
                result.AddWarning($"Schema validation could not be completed: {ex.Message}");
            }
        }
    }
}
