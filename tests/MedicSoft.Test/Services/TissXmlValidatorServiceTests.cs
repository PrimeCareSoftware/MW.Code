using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using Moq;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class TissXmlValidatorServiceTests
    {
        private readonly Mock<ILogger<TissXmlValidatorService>> _loggerMock;
        private readonly TissXmlValidatorService _service;

        public TissXmlValidatorServiceTests()
        {
            _loggerMock = new Mock<ILogger<TissXmlValidatorService>>();
            _service = new TissXmlValidatorService(_loggerMock.Object);
        }

        [Fact]
        public void GetTissVersion_ShouldReturn_ExpectedVersion()
        {
            // Act
            var version = _service.GetTissVersion();

            // Assert
            version.Should().Be("4.02.00");
        }

        [Fact]
        public async Task ValidateXmlStructureAsync_WithEmptyXml_ShouldReturnInvalid()
        {
            // Arrange
            var xml = string.Empty;

            // Act
            var result = await _service.ValidateXmlStructureAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorCount.Should().BeGreaterThan(0);
            result.Errors[0].Message.Should().Contain("empty");
        }

        [Fact]
        public async Task ValidateXmlStructureAsync_WithMalformedXml_ShouldReturnInvalid()
        {
            // Arrange
            var xml = "<root><unclosed>";

            // Act
            var result = await _service.ValidateXmlStructureAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorCount.Should().BeGreaterThan(0);
            result.Errors[0].Message.Should().Contain("not well-formed");
        }

        [Fact]
        public async Task ValidateXmlStructureAsync_WithValidXml_ShouldReturnValid()
        {
            // Arrange
            var xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><root><child>value</child></root>";

            // Act
            var result = await _service.ValidateXmlStructureAsync(xml);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorCount.Should().Be(0);
        }

        [Fact]
        public async Task ValidateXmlStructureAsync_WithoutXmlDeclaration_ShouldReturnWarning()
        {
            // Arrange
            var xml = "<root><child>value</child></root>";

            // Act
            var result = await _service.ValidateXmlStructureAsync(xml);

            // Assert
            result.IsValid.Should().BeTrue();
            result.WarningCount.Should().Be(1);
            result.Warnings[0].Message.Should().Contain("declaration");
        }

        [Fact]
        public async Task ValidateBatchXmlAsync_WithMissingRequiredElements_ShouldReturnErrors()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <tissLoteGuias xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"" versao=""4.02.00"">
                    <guias></guias>
                </tissLoteGuias>";

            // Act
            var result = await _service.ValidateBatchXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorCount.Should().BeGreaterThan(0);
            result.Errors.Should().Contain(e => e.Message.Contains("cabecalho"));
            result.Errors.Should().Contain(e => e.Message.Contains("rodape"));
        }

        [Fact]
        public async Task ValidateBatchXmlAsync_WithCompleteStructure_ShouldHaveWarningAboutSchema()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <tissLoteGuias xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"" versao=""4.02.00"">
                    <cabecalho>
                        <identificacaoTransacao>
                            <tipoTransacao>ENVIO_LOTE_GUIAS</tipoTransacao>
                            <sequencialTransacao>1</sequencialTransacao>
                            <dataTransacao>2024-01-20</dataTransacao>
                            <horaTransacao>10:30:00</horaTransacao>
                        </identificacaoTransacao>
                        <origem>
                            <codigoPrestador>12345678</codigoPrestador>
                            <nomeContratado>Clinica Test</nomeContratado>
                        </origem>
                        <destino>
                            <registroANS>123456</registroANS>
                            <nomeOperadora>Operadora Test</nomeOperadora>
                        </destino>
                        <versaoPadrao>4.02.00</versaoPadrao>
                    </cabecalho>
                    <guias>
                        <guiaConsulta>
                            <cabecalhoGuia>
                                <registroANS>123456</registroANS>
                                <numeroGuiaPrestador>00001</numeroGuiaPrestador>
                            </cabecalhoGuia>
                            <dadosBeneficiario>
                                <numeroCarteira>123456789</numeroCarteira>
                                <nomeBeneficiario>Test Patient</nomeBeneficiario>
                            </dadosBeneficiario>
                        </guiaConsulta>
                    </guias>
                    <rodape>
                        <quantidadeGuias>1</quantidadeGuias>
                        <valorTotal>150.00</valorTotal>
                    </rodape>
                </tissLoteGuias>";

            // Act
            var result = await _service.ValidateBatchXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorCount.Should().Be(0);
            // Should have a warning about missing schema files
            result.WarningCount.Should().BeGreaterThan(0);
            result.Warnings.Should().Contain(w => w.Message.Contains("schema"));
        }

        [Fact]
        public async Task ValidateBatchXmlAsync_WithWrongRootElement_ShouldReturnError()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <wrongRoot versao=""4.02.00"">
                    <cabecalho></cabecalho>
                </wrongRoot>";

            // Act
            var result = await _service.ValidateBatchXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("tissLoteGuias"));
        }

        [Fact]
        public async Task ValidateBatchXmlAsync_WithWrongVersion_ShouldReturnWarning()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <tissLoteGuias xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"" versao=""3.00.00"">
                    <cabecalho>
                        <identificacaoTransacao>
                            <tipoTransacao>ENVIO_LOTE_GUIAS</tipoTransacao>
                        </identificacaoTransacao>
                        <origem></origem>
                        <destino></destino>
                        <versaoPadrao>3.00.00</versaoPadrao>
                    </cabecalho>
                    <guias><guiaConsulta><cabecalhoGuia></cabecalhoGuia><dadosBeneficiario><numeroCarteira>123</numeroCarteira><nomeBeneficiario>Test</nomeBeneficiario></dadosBeneficiario></guiaConsulta></guias>
                    <rodape>
                        <quantidadeGuias>1</quantidadeGuias>
                        <valorTotal>100</valorTotal>
                    </rodape>
                </tissLoteGuias>";

            // Act
            var result = await _service.ValidateBatchXmlAsync(xml);

            // Assert
            result.Warnings.Should().Contain(w => w.Message.Contains("version") || w.Message.Contains("versão"));
        }

        [Fact]
        public async Task ValidateGuideXmlAsync_WithMissingBeneficiaryData_ShouldReturnError()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <guiaConsulta xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"">
                    <cabecalhoGuia>
                        <registroANS>123456</registroANS>
                    </cabecalhoGuia>
                </guiaConsulta>";

            // Act
            var result = await _service.ValidateGuideXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("dadosBeneficiario"));
        }

        [Fact]
        public async Task ValidateGuideXmlAsync_WithInvalidGuideType_ShouldReturnError()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <invalidGuideType xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"">
                    <cabecalhoGuia></cabecalhoGuia>
                </invalidGuideType>";

            // Act
            var result = await _service.ValidateGuideXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("not a valid TISS guide"));
        }

        [Fact]
        public async Task ValidateGuideXmlAsync_WithValidGuide_ShouldReturnValid()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <guiaConsulta xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"">
                    <cabecalhoGuia>
                        <registroANS>123456</registroANS>
                        <numeroGuiaPrestador>00001</numeroGuiaPrestador>
                    </cabecalhoGuia>
                    <dadosBeneficiario>
                        <numeroCarteira>123456789</numeroCarteira>
                        <nomeBeneficiario>Test Patient</nomeBeneficiario>
                    </dadosBeneficiario>
                </guiaConsulta>";

            // Act
            var result = await _service.ValidateGuideXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorCount.Should().Be(0);
        }

        [Fact]
        public async Task ValidateBatchXmlAsync_WithEmptyGuides_ShouldReturnError()
        {
            // Arrange
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
                <tissLoteGuias xmlns=""http://www.ans.gov.br/padroes/tiss/schemas"" versao=""4.02.00"">
                    <cabecalho>
                        <identificacaoTransacao></identificacaoTransacao>
                        <origem></origem>
                        <destino></destino>
                        <versaoPadrao>4.02.00</versaoPadrao>
                    </cabecalho>
                    <guias></guias>
                    <rodape>
                        <quantidadeGuias>0</quantidadeGuias>
                        <valorTotal>0</valorTotal>
                    </rodape>
                </tissLoteGuias>";

            // Act
            var result = await _service.ValidateBatchXmlAsync(xml);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("at least one guide"));
        }
    }
}
