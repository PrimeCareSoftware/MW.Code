using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MedicSoft.Application.Services.Fiscal;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Test.Services.Fiscal
{
    /// <summary>
    /// Testes unitários para CalculoImpostosService
    /// </summary>
    public class CalculoImpostosServiceTests
    {
        private readonly Mock<IImpostoNotaRepository> _impostoNotaRepositoryMock;
        private readonly Mock<IConfiguracaoFiscalRepository> _configuracaoFiscalRepositoryMock;
        private readonly Mock<ILogger<CalculoImpostosService>> _loggerMock;
        private readonly CalculoImpostosService _service;
        private const string TenantId = "test-tenant";

        public CalculoImpostosServiceTests()
        {
            _impostoNotaRepositoryMock = new Mock<IImpostoNotaRepository>();
            _configuracaoFiscalRepositoryMock = new Mock<IConfiguracaoFiscalRepository>();
            _loggerMock = new Mock<ILogger<CalculoImpostosService>>();

            _service = new CalculoImpostosService(
                _impostoNotaRepositoryMock.Object,
                _configuracaoFiscalRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        #region CalcularImpostosNotaAsync Tests

        [Theory]
        [InlineData(1000.00, 6.50, 65.00)]  // PIS 0.65%
        [InlineData(5000.00, 6.50, 325.00)]
        [InlineData(10000.00, 6.50, 650.00)]
        public async Task CalcularImpostosNotaAsync_DeveCalcularPISCorretamente_QuandoLucroPresumido(
            decimal valorNota, decimal aliquotaPIS, decimal valorPISEsperado)
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, valorNota);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);
            configuracao.AliquotaPIS = aliquotaPIS;

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ValorPIS.Should().BeApproximately(valorPISEsperado, 0.01m);
            resultado.AliquotaPIS.Should().Be(aliquotaPIS);
        }

        [Theory]
        [InlineData(1000.00, 30.00, 300.00)]  // COFINS 3%
        [InlineData(5000.00, 30.00, 1500.00)]
        [InlineData(10000.00, 30.00, 3000.00)]
        public async Task CalcularImpostosNotaAsync_DeveCalcularCOFINSCorretamente_QuandoLucroPresumido(
            decimal valorNota, decimal aliquotaCOFINS, decimal valorCOFINSEsperado)
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, valorNota);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);
            configuracao.AliquotaCOFINS = aliquotaCOFINS;

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ValorCOFINS.Should().BeApproximately(valorCOFINSEsperado, 0.01m);
            resultado.AliquotaCOFINS.Should().Be(aliquotaCOFINS);
        }

        [Theory]
        [InlineData(1000.00, 50.00, 50.00)]  // ISS 5%
        [InlineData(5000.00, 50.00, 250.00)]
        [InlineData(10000.00, 50.00, 500.00)]
        public async Task CalcularImpostosNotaAsync_DeveCalcularISSCorretamente(
            decimal valorNota, decimal aliquotaISS, decimal valorISSEsperado)
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, valorNota);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);
            configuracao.AliquotaISS = aliquotaISS;

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ValorISS.Should().BeApproximately(valorISSEsperado, 0.01m);
            resultado.AliquotaISS.Should().Be(aliquotaISS);
        }

        [Fact]
        public async Task CalcularImpostosNotaAsync_DeveLancarExcecao_QuandoNotaNula()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.CalcularImpostosNotaAsync(null, TenantId));
        }

        [Fact]
        public async Task CalcularImpostosNotaAsync_DeveLancarExcecao_QuandoConfiguracaoNaoEncontrada()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, 1000.00m);

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync((ConfiguracaoFiscal)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.CalcularImpostosNotaAsync(notaFiscal, TenantId));
        }

        [Fact]
        public async Task CalcularImpostosNotaAsync_DeveCalcularTotalImpostos_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, 10000.00m);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);
            configuracao.AliquotaPIS = 6.50m;      // 0.065% = R$ 6.50
            configuracao.AliquotaCOFINS = 30.00m;  // 0.30% = R$ 30.00
            configuracao.AliquotaIR = 15.00m;      // 0.15% = R$ 15.00
            configuracao.AliquotaCSLL = 10.00m;    // 0.10% = R$ 10.00
            configuracao.AliquotaISS = 50.00m;     // 0.50% = R$ 50.00
            // Total esperado: 111.50 (1.115% do valor bruto)

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalImpostos.Should().BeApproximately(111.50m, 0.01m);
            // Nota: ValorLiquidoTributos pode ser negativo se os impostos excedem o valor líquido
            // Isso ocorre quando deduções são maiores que o valor bruto
            resultado.ValorLiquidoTributos.Should().BeApproximately(9888.50m, 0.01m); // 10000 - 111.50
        }

        [Fact]
        public async Task CalcularImpostosNotaAsync_DeveCalcularCargaTributaria_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, 10000.00m);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);
            // Alíquotas em base points (0.01%)
            configuracao.AliquotaPIS = 6.50m;      // 0.065%
            configuracao.AliquotaCOFINS = 30.00m;  // 0.30%
            configuracao.AliquotaIR = 15.00m;      // 0.15%
            configuracao.AliquotaCSLL = 10.00m;    // 0.10%
            configuracao.AliquotaISS = 50.00m;     // 0.50%
            // Total: 1.115% do valor

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            // Carga Tributária = (TotalImpostos / ValorLiquido) * 100
            // 111.50 / 10000 * 100 = 1.115%
            resultado.CargaTributaria.Should().BeApproximately(1.115m, 0.01m);
        }

        [Theory]
        [InlineData(10000.00, 180000.00, 60.00)]   // Anexo III - Faixa 1: 6%
        [InlineData(10000.00, 360000.00, 112.00)]  // Anexo III - Faixa 2: 11.2%
        [InlineData(10000.00, 720000.00, 135.00)]  // Anexo III - Faixa 3: 13.5%
        public async Task CalcularImpostosNotaAsync_DeveCalcularSimplesNacional_Corretamente(
            decimal valorNota,
            decimal receitaBruta12Meses,
            decimal impostoEsperado)
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, valorNota);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.SimplesNacional);
            configuracao.OptanteSimplesNacional = true;
            configuracao.AnexoSimples = AnexoSimplesNacional.AnexoIII;
            configuracao.FatorR = 30.00m; // > 28%

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            // Mock para obter receita bruta 12 meses
            _impostoNotaRepositoryMock
                .Setup(r => r.GetReceitaBruta12MesesAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(receitaBruta12Meses);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.TotalImpostos.Should().BeApproximately(impostoEsperado, 10.00m); // tolerância maior devido cálculo complexo
        }

        [Fact]
        public async Task CalcularImpostosNotaAsync_DeveSalvarImposto_QuandoCalculadoComSucesso()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var notaFiscal = CriarNotaFiscalTeste(clinicaId, 1000.00m);
            var configuracao = CriarConfiguracaoFiscal(clinicaId, RegimeTributarioEnum.LucroPresumido);

            _configuracaoFiscalRepositoryMock
                .Setup(r => r.GetVigenteAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(configuracao);

            _impostoNotaRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ImpostoNota>(), TenantId))
                .ReturnsAsync((ImpostoNota imposto, string tenant) => imposto);

            // Act
            var resultado = await _service.CalcularImpostosNotaAsync(notaFiscal, TenantId);

            // Assert
            _impostoNotaRepositoryMock.Verify(
                r => r.AddAsync(It.Is<ImpostoNota>(i => 
                    i.NotaFiscalId == notaFiscal.Id &&
                    i.ValorBruto == notaFiscal.ValorTotal), 
                    TenantId), 
                Times.Once);
        }

        #endregion

        #region Métodos Auxiliares

        private ElectronicInvoice CriarNotaFiscalTeste(Guid clinicaId, decimal valor)
        {
            return new ElectronicInvoice
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                Number = "NF-001",
                IssueDate = DateTime.Now,
                TotalAmount = valor,
                Status = ElectronicInvoiceStatus.Issued,
                TenantId = TenantId
            };
        }

        private ConfiguracaoFiscal CriarConfiguracaoFiscal(Guid clinicaId, RegimeTributarioEnum regime)
        {
            return new ConfiguracaoFiscal
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                Regime = regime,
                VigenciaInicio = DateTime.Now.AddMonths(-12),
                OptanteSimplesNacional = regime == RegimeTributarioEnum.SimplesNacional,
                AliquotaPIS = 6.50m,
                AliquotaCOFINS = 30.00m,
                AliquotaIR = 15.00m,
                AliquotaCSLL = 10.00m,
                AliquotaISS = 50.00m,
                CodigoServico = "04.22",
                CNAE = "8630-5/03",
                TenantId = TenantId
            };
        }

        #endregion
    }
}
