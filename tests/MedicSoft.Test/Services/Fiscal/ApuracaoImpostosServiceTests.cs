using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MedicSoft.Application.Services.Fiscal;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Test.Services.Fiscal
{
    /// <summary>
    /// Testes unitários para ApuracaoImpostosService
    /// </summary>
    public class ApuracaoImpostosServiceTests
    {
        private readonly Mock<IApuracaoImpostosRepository> _apuracaoRepositoryMock;
        private readonly Mock<IImpostoNotaRepository> _impostoNotaRepositoryMock;
        private readonly Mock<ICalculoImpostosService> _calculoImpostosServiceMock;
        private readonly Mock<ILogger<ApuracaoImpostosService>> _loggerMock;
        private readonly ApuracaoImpostosService _service;
        private const string TenantId = "test-tenant";

        public ApuracaoImpostosServiceTests()
        {
            _apuracaoRepositoryMock = new Mock<IApuracaoImpostosRepository>();
            _impostoNotaRepositoryMock = new Mock<IImpostoNotaRepository>();
            _calculoImpostosServiceMock = new Mock<ICalculoImpostosService>();
            _loggerMock = new Mock<ILogger<ApuracaoImpostosService>>();

            _service = new ApuracaoImpostosService(
                _apuracaoRepositoryMock.Object,
                _impostoNotaRepositoryMock.Object,
                _calculoImpostosServiceMock.Object,
                _loggerMock.Object
            );
        }

        #region GerarApuracaoMensalAsync Tests

        [Fact]
        public async Task GerarApuracaoMensalAsync_DeveCriarNovaApuracao_QuandoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var impostos = CriarListaImpostos(clinicaId, mes, ano, 3);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(impostos);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetReceitaBruta12MesesAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(1800000m);

            _apuracaoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ApuracaoImpostos>(), TenantId))
                .ReturnsAsync((ApuracaoImpostos a, string t) => a);

            // Act
            var resultado = await _service.GerarApuracaoMensalAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Mes.Should().Be(mes);
            resultado.Ano.Should().Be(ano);
            resultado.ClinicaId.Should().Be(clinicaId);
            resultado.Status.Should().Be(StatusApuracao.Apurado);
        }

        [Fact]
        public async Task GerarApuracaoMensalAsync_DeveRetornarApuracaoExistente_QuandoJaFoiApurado()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracaoExistente = CriarApuracaoTeste(clinicaId, mes, ano);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracaoExistente);

            // Act
            var resultado = await _service.GerarApuracaoMensalAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().Be(apuracaoExistente);
            _apuracaoRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ApuracaoImpostos>(), TenantId),
                Times.Never);
        }

        [Fact]
        public async Task GerarApuracaoMensalAsync_DeveSomarImpostosCorretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var impostos = new List<ImpostoNota>
            {
                CriarImpostoNota(clinicaId, 100, 50, 30, 20, 10, 5),
                CriarImpostoNota(clinicaId, 200, 100, 60, 40, 20, 10),
                CriarImpostoNota(clinicaId, 150, 75, 45, 30, 15, 7.5m)
            };

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(impostos);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetReceitaBruta12MesesAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(1800000m);

            _apuracaoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ApuracaoImpostos>(), TenantId))
                .ReturnsAsync((ApuracaoImpostos a, string t) => a);

            // Act
            var resultado = await _service.GerarApuracaoMensalAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.FaturamentoBruto.Should().Be(450); // 100 + 200 + 150
            resultado.TotalPIS.Should().Be(225);         // 100 + 100 + 75
            resultado.TotalCOFINS.Should().Be(135);      // 50 + 60 + 45
            resultado.TotalIR.Should().Be(90);           // 30 + 40 + 30
            resultado.TotalCSLL.Should().Be(45);         // 20 + 20 + 15
            resultado.TotalISS.Should().Be(22.5m);       // 10 + 10 + 7.5
        }

        [Fact]
        public async Task GerarApuracaoMensalAsync_DeveCalcularReceitaBruta12Meses()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 6;
            var ano = 2026;
            var impostos = CriarListaImpostos(clinicaId, mes, ano, 2);
            var receitaBruta12Meses = 2400000m;

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(impostos);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetReceitaBruta12MesesAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(receitaBruta12Meses);

            _apuracaoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ApuracaoImpostos>(), TenantId))
                .ReturnsAsync((ApuracaoImpostos a, string t) => a);

            // Act
            var resultado = await _service.GerarApuracaoMensalAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ReceitaBruta12Meses.Should().Be(receitaBruta12Meses);
        }

        [Fact]
        public async Task GerarApuracaoMensalAsync_DeveRetornarApuracaoVazia_QuandoSemImpostos()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            _impostoNotaRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(new List<ImpostoNota>());

            _impostoNotaRepositoryMock
                .Setup(r => r.GetReceitaBruta12MesesAsync(clinicaId, It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(0m);

            _apuracaoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ApuracaoImpostos>(), TenantId))
                .ReturnsAsync((ApuracaoImpostos a, string t) => a);

            // Act
            var resultado = await _service.GerarApuracaoMensalAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.FaturamentoBruto.Should().Be(0);
            resultado.TotalPIS.Should().Be(0);
            resultado.TotalCOFINS.Should().Be(0);
            resultado.TotalIR.Should().Be(0);
            resultado.TotalCSLL.Should().Be(0);
            resultado.TotalISS.Should().Be(0);
        }

        #endregion

        #region GetApuracoesAsync Tests

        [Fact]
        public async Task GetApuracoesAsync_DeveRetornarListaDeApuracoes()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var apuracoes = new List<ApuracaoImpostos>
            {
                CriarApuracaoTeste(clinicaId, 1, 2026),
                CriarApuracaoTeste(clinicaId, 2, 2026),
                CriarApuracaoTeste(clinicaId, 3, 2026)
            };

            _apuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(apuracoes);

            // Act
            var resultado = await _service.GetApuracoesAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(apuracoes);
        }

        [Fact]
        public async Task GetApuracoesAsync_DeveRetornarListaVazia_QuandoSemApuracoes()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();

            _apuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(new List<ApuracaoImpostos>());

            // Act
            var resultado = await _service.GetApuracoesAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        #endregion

        #region GetApuracaoByIdAsync Tests

        [Fact]
        public async Task GetApuracaoByIdAsync_DeveRetornarApuracao_QuandoExiste()
        {
            // Arrange
            var apuracaoId = Guid.NewGuid();
            var apuracao = CriarApuracaoTeste(Guid.NewGuid(), 1, 2026);
            apuracao.Id = apuracaoId;

            _apuracaoRepositoryMock
                .Setup(r => r.GetByIdAsync(apuracaoId, TenantId))
                .ReturnsAsync(apuracao);

            // Act
            var resultado = await _service.GetApuracaoByIdAsync(apuracaoId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(apuracaoId);
        }

        [Fact]
        public async Task GetApuracaoByIdAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            // Arrange
            var apuracaoId = Guid.NewGuid();

            _apuracaoRepositoryMock
                .Setup(r => r.GetByIdAsync(apuracaoId, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            // Act
            var resultado = await _service.GetApuracaoByIdAsync(apuracaoId, TenantId);

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region MarcarComoPagoAsync Tests

        [Fact]
        public async Task MarcarComoPagoAsync_DeveAtualizarStatus_ParaPago()
        {
            // Arrange
            var apuracaoId = Guid.NewGuid();
            var apuracao = CriarApuracaoTeste(Guid.NewGuid(), 1, 2026);
            apuracao.Id = apuracaoId;
            apuracao.Status = StatusApuracao.Apurado;

            _apuracaoRepositoryMock
                .Setup(r => r.GetByIdAsync(apuracaoId, TenantId))
                .ReturnsAsync(apuracao);

            _apuracaoRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<ApuracaoImpostos>(), TenantId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.MarcarComoPagoAsync(apuracaoId, DateTime.Now, TenantId);

            // Assert
            _apuracaoRepositoryMock.Verify(
                r => r.UpdateAsync(
                    It.Is<ApuracaoImpostos>(a => 
                        a.Status == StatusApuracao.Pago &&
                        a.DataPagamento.HasValue),
                    TenantId),
                Times.Once);
        }

        [Fact]
        public async Task MarcarComoPagoAsync_DeveLancarExcecao_QuandoApuracaoNaoExiste()
        {
            // Arrange
            var apuracaoId = Guid.NewGuid();

            _apuracaoRepositoryMock
                .Setup(r => r.GetByIdAsync(apuracaoId, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.MarcarComoPagoAsync(apuracaoId, DateTime.Now, TenantId));
        }

        #endregion

        #region GetEvolucaoMensalAsync Tests

        [Fact]
        public async Task GetEvolucaoMensalAsync_DeveRetornarUltimosNMeses()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var meses = 6;
            var apuracoes = new List<ApuracaoImpostos>();
            
            for (int i = 0; i < meses; i++)
            {
                var data = DateTime.Now.AddMonths(-i);
                apuracoes.Add(CriarApuracaoTeste(clinicaId, data.Month, data.Year));
            }

            _apuracaoRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(apuracoes);

            // Act
            var resultado = await _service.GetEvolucaoMensalAsync(clinicaId, meses, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(meses);
        }

        [Fact]
        public async Task GetEvolucaoMensalAsync_DeveOrdenarPorDataCrescente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var apuracoes = new List<ApuracaoImpostos>
            {
                CriarApuracaoTeste(clinicaId, 3, 2026),
                CriarApuracaoTeste(clinicaId, 1, 2026),
                CriarApuracaoTeste(clinicaId, 2, 2026)
            };

            _apuracaoRepositoryMock
                .Setup(r => r.GetByPeriodoAsync(clinicaId, It.IsAny<DateTime>(), It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(apuracoes);

            // Act
            var resultado = await _service.GetEvolucaoMensalAsync(clinicaId, 12, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeInAscendingOrder(a => new DateTime(a.Ano, a.Mes, 1));
        }

        #endregion

        #region Métodos Auxiliares

        private List<ImpostoNota> CriarListaImpostos(Guid clinicaId, int mes, int ano, int quantidade)
        {
            var lista = new List<ImpostoNota>();
            for (int i = 0; i < quantidade; i++)
            {
                lista.Add(CriarImpostoNota(clinicaId, 1000, 65, 300, 150, 100, 50));
            }
            return lista;
        }

        private ImpostoNota CriarImpostoNota(
            Guid clinicaId,
            decimal valorBruto,
            decimal valorPIS,
            decimal valorCOFINS,
            decimal valorIR,
            decimal valorCSLL,
            decimal valorISS)
        {
            return new ImpostoNota
            {
                Id = Guid.NewGuid(),
                NotaFiscalId = Guid.NewGuid(),
                ValorBruto = valorBruto,
                ValorDesconto = 0,
                ValorPIS = valorPIS,
                ValorCOFINS = valorCOFINS,
                ValorIR = valorIR,
                ValorCSLL = valorCSLL,
                ValorISS = valorISS,
                ValorINSS = 0,
                DataCalculo = DateTime.Now,
                TenantId = TenantId
            };
        }

        private ApuracaoImpostos CriarApuracaoTeste(Guid clinicaId, int mes, int ano)
        {
            return new ApuracaoImpostos
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                Mes = mes,
                Ano = ano,
                DataApuracao = new DateTime(ano, mes, 1),
                FaturamentoBruto = 150000m,
                Deducoes = 0,
                TotalPIS = 975m,
                TotalCOFINS = 4500m,
                TotalIR = 1500m,
                TotalCSLL = 1350m,
                TotalISS = 7500m,
                TotalINSS = 0,
                ReceitaBruta12Meses = 1800000m,
                AliquotaEfetiva = 11.20m,
                ValorDAS = 16800m,
                Status = StatusApuracao.Apurado,
                TenantId = TenantId
            };
        }

        #endregion
    }
}
