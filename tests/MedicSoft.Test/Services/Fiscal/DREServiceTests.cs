using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MedicSoft.Application.Services.Fiscal;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Test.Services.Fiscal
{
    /// <summary>
    /// Testes unitários para DREService (Demonstração do Resultado do Exercício)
    /// </summary>
    public class DREServiceTests
    {
        private readonly Mock<IDRERepository> _dreRepositoryMock;
        private readonly Mock<IApuracaoImpostosRepository> _apuracaoRepositoryMock;
        private readonly Mock<ILogger<DREService>> _loggerMock;
        private readonly DREService _service;
        private const string TenantId = "test-tenant";

        public DREServiceTests()
        {
            _dreRepositoryMock = new Mock<IDRERepository>();
            _apuracaoRepositoryMock = new Mock<IApuracaoImpostosRepository>();
            _loggerMock = new Mock<ILogger<DREService>>();

            _service = new DREService(
                _dreRepositoryMock.Object,
                _apuracaoRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        #region GerarDREAsync Tests

        [Fact]
        public async Task GerarDREAsync_DeveCriarNovaDRE_QuandoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracao = CriarApuracaoTeste(clinicaId, mes, ano);

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracao);

            _dreRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DRE>(), TenantId))
                .ReturnsAsync((DRE d, string t) => d);

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Mes.Should().Be(mes);
            resultado.Ano.Should().Be(ano);
            resultado.ClinicaId.Should().Be(clinicaId);
        }

        [Fact]
        public async Task GerarDREAsync_DeveRetornarDREExistente_QuandoJaFoiGerado()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var dreExistente = CriarDRETeste(clinicaId, mes, ano);

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(dreExistente);

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().Be(dreExistente);
            _dreRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<DRE>(), TenantId),
                Times.Never);
        }

        [Fact]
        public async Task GerarDREAsync_DeveCalcularReceitaLiquida_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracao = CriarApuracaoTeste(clinicaId, mes, ano);
            apuracao.FaturamentoBruto = 150000m;
            apuracao.Deducoes = 10000m;
            apuracao.TotalImpostos = 15000m;

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracao);

            _dreRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DRE>(), TenantId))
                .ReturnsAsync((DRE d, string t) => d);

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ReceitaBruta.Should().Be(150000m);
            resultado.Deducoes.Should().Be(10000m);
            resultado.ReceitaLiquida.Should().Be(140000m); // 150000 - 10000
        }

        [Fact]
        public async Task GerarDREAsync_DeveCalcularLucroOperacional_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracao = CriarApuracaoTeste(clinicaId, mes, ano);

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracao);

            _dreRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DRE>(), TenantId))
                .ReturnsAsync((DRE d, string t) => 
                {
                    d.CustosMercadorias = 30000m;
                    d.DespesasOperacionais = 20000m;
                    return d;
                });

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            // ReceitaLiquida - CustosMercadorias - DespesasOperacionais
            var lucroOperacionalEsperado = resultado.ReceitaLiquida - 30000m - 20000m;
            resultado.LucroOperacional.Should().Be(lucroOperacionalEsperado);
        }

        [Fact]
        public async Task GerarDREAsync_DeveCalcularLucroLiquido_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracao = CriarApuracaoTeste(clinicaId, mes, ano);

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracao);

            _dreRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DRE>(), TenantId))
                .ReturnsAsync((DRE d, string t) => 
                {
                    d.CustosMercadorias = 30000m;
                    d.DespesasOperacionais = 20000m;
                    d.ReceitasFinanceiras = 5000m;
                    d.DespesasFinanceiras = 2000m;
                    return d;
                });

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            // LucroOperacional + ReceitasFinanceiras - DespesasFinanceiras - Impostos
            var impostos = apuracao.TotalImpostos;
            var lucroLiquidoEsperado = resultado.LucroOperacional + 5000m - 2000m - impostos;
            resultado.LucroLiquido.Should().Be(lucroLiquidoEsperado);
        }

        [Fact]
        public async Task GerarDREAsync_DeveCalcularMargens_Corretamente()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;
            var apuracao = CriarApuracaoTeste(clinicaId, mes, ano);
            apuracao.FaturamentoBruto = 100000m;
            apuracao.Deducoes = 0;

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync(apuracao);

            _dreRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<DRE>(), TenantId))
                .ReturnsAsync((DRE d, string t) => 
                {
                    d.CustosMercadorias = 40000m;
                    d.DespesasOperacionais = 30000m;
                    d.ReceitaBruta = 100000m;
                    d.ReceitaLiquida = 100000m;
                    return d;
                });

            // Act
            var resultado = await _service.GerarDREAsync(clinicaId, mes, ano, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            
            // Margem Bruta = ((ReceitaLiquida - Custos) / ReceitaLiquida) * 100
            var margemBrutaEsperada = ((100000m - 40000m) / 100000m) * 100m;
            resultado.MargemBruta.Should().BeApproximately(margemBrutaEsperada, 0.01m);
            
            // Margem Operacional = (LucroOperacional / ReceitaLiquida) * 100
            var lucroOperacional = 100000m - 40000m - 30000m;
            var margemOperacionalEsperada = (lucroOperacional / 100000m) * 100m;
            resultado.MargemOperacional.Should().BeApproximately(margemOperacionalEsperada, 0.01m);
        }

        [Fact]
        public async Task GerarDREAsync_DeveLancarExcecao_QuandoApuracaoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var mes = 1;
            var ano = 2026;

            _dreRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((DRE)null);

            _apuracaoRepositoryMock
                .Setup(r => r.GetByMesAnoAsync(clinicaId, mes, ano, TenantId))
                .ReturnsAsync((ApuracaoImpostos)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.GerarDREAsync(clinicaId, mes, ano, TenantId));
        }

        #endregion

        #region GetDREByIdAsync Tests

        [Fact]
        public async Task GetDREByIdAsync_DeveRetornarDRE_QuandoExiste()
        {
            // Arrange
            var dreId = Guid.NewGuid();
            var dre = CriarDRETeste(Guid.NewGuid(), 1, 2026);
            dre.Id = dreId;

            _dreRepositoryMock
                .Setup(r => r.GetByIdAsync(dreId, TenantId))
                .ReturnsAsync(dre);

            // Act
            var resultado = await _service.GetDREByIdAsync(dreId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(dreId);
        }

        [Fact]
        public async Task GetDREByIdAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            // Arrange
            var dreId = Guid.NewGuid();

            _dreRepositoryMock
                .Setup(r => r.GetByIdAsync(dreId, TenantId))
                .ReturnsAsync((DRE)null);

            // Act
            var resultado = await _service.GetDREByIdAsync(dreId, TenantId);

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region GetDREsAsync Tests

        [Fact]
        public async Task GetDREsAsync_DeveRetornarListaDeDREs()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var dres = new List<DRE>
            {
                CriarDRETeste(clinicaId, 1, 2026),
                CriarDRETeste(clinicaId, 2, 2026),
                CriarDRETeste(clinicaId, 3, 2026)
            };

            _dreRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(dres);

            // Act
            var resultado = await _service.GetDREsAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetDREsAsync_DeveRetornarListaVazia_QuandoSemDREs()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();

            _dreRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(new List<DRE>());

            // Act
            var resultado = await _service.GetDREsAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        #endregion

        #region Análise Horizontal e Vertical

        [Fact]
        public async Task CalcularAnaliseHorizontalAsync_DeveCalcularVariacaoPercentual()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var dre1 = CriarDRETeste(clinicaId, 1, 2026);
            dre1.ReceitaBruta = 100000m;
            dre1.LucroLiquido = 10000m;

            var dre2 = CriarDRETeste(clinicaId, 2, 2026);
            dre2.ReceitaBruta = 120000m;  // +20%
            dre2.LucroLiquido = 15000m;   // +50%

            // Act
            var variacao = _service.CalcularAnaliseHorizontal(dre1, dre2);

            // Assert
            variacao.Should().NotBeNull();
            variacao["ReceitaBruta"].Should().BeApproximately(20.0m, 0.01m);
            variacao["LucroLiquido"].Should().BeApproximately(50.0m, 0.01m);
        }

        [Fact]
        public async Task CalcularAnaliseVerticalAsync_DeveCalcularPercentualSobreReceitaBruta()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var dre = CriarDRETeste(clinicaId, 1, 2026);
            dre.ReceitaBruta = 100000m;
            dre.CustosMercadorias = 40000m;      // 40%
            dre.DespesasOperacionais = 30000m;   // 30%
            dre.LucroLiquido = 20000m;           // 20%

            // Act
            var percentuais = _service.CalcularAnaliseVertical(dre);

            // Assert
            percentuais.Should().NotBeNull();
            percentuais["CustosMercadorias"].Should().BeApproximately(40.0m, 0.01m);
            percentuais["DespesasOperacionais"].Should().BeApproximately(30.0m, 0.01m);
            percentuais["LucroLiquido"].Should().BeApproximately(20.0m, 0.01m);
        }

        #endregion

        #region Métodos Auxiliares

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
                Deducoes = 10000m,
                TotalPIS = 975m,
                TotalCOFINS = 4500m,
                TotalIR = 1500m,
                TotalCSLL = 1350m,
                TotalISS = 7500m,
                TotalINSS = 0m,
                Status = StatusApuracao.Apurado,
                TenantId = TenantId
            };
        }

        private DRE CriarDRETeste(Guid clinicaId, int mes, int ano)
        {
            return new DRE
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                Mes = mes,
                Ano = ano,
                DataGeracao = new DateTime(ano, mes, 1),
                ReceitaBruta = 150000m,
                Deducoes = 10000m,
                ReceitaLiquida = 140000m,
                CustosMercadorias = 40000m,
                LucroBruto = 100000m,
                DespesasOperacionais = 30000m,
                LucroOperacional = 70000m,
                ReceitasFinanceiras = 5000m,
                DespesasFinanceiras = 2000m,
                LucroAntesImpostos = 73000m,
                Impostos = 15825m,
                LucroLiquido = 57175m,
                MargemBruta = 71.43m,
                MargemOperacional = 50.00m,
                MargemLiquida = 40.84m,
                TenantId = TenantId
            };
        }

        #endregion
    }
}
