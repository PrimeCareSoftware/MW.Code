using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using MedicSoft.Application.Services.Fiscal.Integracoes;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Test.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Testes unitários para IntegracaoContabilService
    /// </summary>
    public class IntegracaoContabilServiceTests
    {
        private readonly Mock<IConfiguracaoIntegracaoRepository> _configuracaoRepositoryMock;
        private readonly Mock<ILogger<IntegracaoContabilService>> _loggerMock;
        private readonly IntegracaoContabilService _service;
        private const string TenantId = "test-tenant";

        public IntegracaoContabilServiceTests()
        {
            _configuracaoRepositoryMock = new Mock<IConfiguracaoIntegracaoRepository>();
            _loggerMock = new Mock<ILogger<IntegracaoContabilService>>();

            _service = new IntegracaoContabilService(
                _configuracaoRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        #region GetConfiguracaoAsync Tests

        [Fact]
        public async Task GetConfiguracaoAsync_DeveRetornarConfiguracao_QuandoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var configuracao = CriarConfiguracaoTeste(clinicaId, ProvedorIntegracao.Dominio);

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.GetConfiguracaoAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.ClinicaId.Should().Be(clinicaId);
            resultado.Provedor.Should().Be(ProvedorIntegracao.Dominio);
        }

        [Fact]
        public async Task GetConfiguracaoAsync_DeveRetornarNull_QuandoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync((ConfiguracaoIntegracao)null);

            // Act
            var resultado = await _service.GetConfiguracaoAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().BeNull();
        }

        #endregion

        #region SalvarConfiguracaoAsync Tests

        [Fact]
        public async Task SalvarConfiguracaoAsync_DeveCriarNovaConfiguracao_QuandoNaoExiste()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.ContaAzul);

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(configuracao.ClinicaId, TenantId))
                .ReturnsAsync((ConfiguracaoIntegracao)null);

            _configuracaoRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<ConfiguracaoIntegracao>(), TenantId))
                .ReturnsAsync((ConfiguracaoIntegracao c, string t) => c);

            // Act
            var resultado = await _service.SalvarConfiguracaoAsync(configuracao, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            _configuracaoRepositoryMock.Verify(
                r => r.AddAsync(It.IsAny<ConfiguracaoIntegracao>(), TenantId),
                Times.Once);
        }

        [Fact]
        public async Task SalvarConfiguracaoAsync_DeveAtualizarConfiguracao_QuandoJaExiste()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.Omie);
            var configuracaoExistente = CriarConfiguracaoTeste(configuracao.ClinicaId, ProvedorIntegracao.Dominio);

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(configuracao.ClinicaId, TenantId))
                .ReturnsAsync(configuracaoExistente);

            _configuracaoRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<ConfiguracaoIntegracao>(), TenantId))
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _service.SalvarConfiguracaoAsync(configuracao, TenantId);

            // Assert
            resultado.Should().NotBeNull();
            _configuracaoRepositoryMock.Verify(
                r => r.UpdateAsync(It.IsAny<ConfiguracaoIntegracao>(), TenantId),
                Times.Once);
        }

        [Fact]
        public async Task SalvarConfiguracaoAsync_DeveLancarExcecao_QuandoConfiguracaoNula()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.SalvarConfiguracaoAsync(null, TenantId));
        }

        #endregion

        #region ValidarConfiguracaoAsync Tests

        [Fact]
        public async Task ValidarConfiguracaoAsync_DeveRetornarTrue_QuandoConfiguracaoValida()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.Dominio);
            configuracao.Ativa = true;
            configuracao.ApiKey = "valid-key";
            configuracao.ApiUrl = "https://api.dominio.com.br";

            // Act
            var resultado = await _service.ValidarConfiguracaoAsync(configuracao);

            // Assert
            resultado.Should().BeTrue();
        }

        [Fact]
        public async Task ValidarConfiguracaoAsync_DeveRetornarFalse_QuandoConfiguracaoInativa()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.Dominio);
            configuracao.Ativa = false;

            // Act
            var resultado = await _service.ValidarConfiguracaoAsync(configuracao);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task ValidarConfiguracaoAsync_DeveRetornarFalse_QuandoApiKeySemValor()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.ContaAzul);
            configuracao.Ativa = true;
            configuracao.ApiKey = null;

            // Act
            var resultado = await _service.ValidarConfiguracaoAsync(configuracao);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task ValidarConfiguracaoAsync_DeveRetornarFalse_QuandoApiUrlSemValor()
        {
            // Arrange
            var configuracao = CriarConfiguracaoTeste(Guid.NewGuid(), ProvedorIntegracao.Omie);
            configuracao.Ativa = true;
            configuracao.ApiUrl = "";

            // Act
            var resultado = await _service.ValidarConfiguracaoAsync(configuracao);

            // Assert
            resultado.Should().BeFalse();
        }

        #endregion

        #region EnviarLancamentoAsync Tests

        [Fact]
        public async Task EnviarLancamentoAsync_DeveLancarExcecao_QuandoConfiguracaoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var lancamento = CriarLancamentoTeste(clinicaId);

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync((ConfiguracaoIntegracao)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.EnviarLancamentoAsync(lancamento, TenantId));
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveLancarExcecao_QuandoConfiguracaoInativa()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var lancamento = CriarLancamentoTeste(clinicaId);
            var configuracao = CriarConfiguracaoTeste(clinicaId, ProvedorIntegracao.Dominio);
            configuracao.Ativa = false;

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(configuracao);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.EnviarLancamentoAsync(lancamento, TenantId));
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveLancarExcecao_QuandoLancamentoNulo()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.EnviarLancamentoAsync(null, TenantId));
        }

        #endregion

        #region TestarConexaoAsync Tests

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarFalse_QuandoConfiguracaoNaoExiste()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync((ConfiguracaoIntegracao)null);

            // Act
            var resultado = await _service.TestarConexaoAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().BeFalse();
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarFalse_QuandoConfiguracaoInativa()
        {
            // Arrange
            var clinicaId = Guid.NewGuid();
            var configuracao = CriarConfiguracaoTeste(clinicaId, ProvedorIntegracao.Dominio);
            configuracao.Ativa = false;

            _configuracaoRepositoryMock
                .Setup(r => r.GetByClinicaIdAsync(clinicaId, TenantId))
                .ReturnsAsync(configuracao);

            // Act
            var resultado = await _service.TestarConexaoAsync(clinicaId, TenantId);

            // Assert
            resultado.Should().BeFalse();
        }

        #endregion

        #region ObterProvedoresDisponiveis Tests

        [Fact]
        public void ObterProvedoresDisponiveis_DeveRetornarTodosProvedores()
        {
            // Act
            var provedores = _service.ObterProvedoresDisponiveis();

            // Assert
            provedores.Should().NotBeNull();
            provedores.Should().Contain(ProvedorIntegracao.Dominio);
            provedores.Should().Contain(ProvedorIntegracao.ContaAzul);
            provedores.Should().Contain(ProvedorIntegracao.Omie);
            provedores.Should().HaveCount(3);
        }

        #endregion

        #region Métodos Auxiliares

        private ConfiguracaoIntegracao CriarConfiguracaoTeste(Guid clinicaId, ProvedorIntegracao provedor)
        {
            return new ConfiguracaoIntegracao
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                Provedor = provedor,
                Ativa = true,
                ApiUrl = GetApiUrlPorProvedor(provedor),
                ApiKey = "test-api-key",
                CodigoEmpresa = "123",
                TenantId = TenantId
            };
        }

        private string GetApiUrlPorProvedor(ProvedorIntegracao provedor)
        {
            return provedor switch
            {
                ProvedorIntegracao.Dominio => "https://api.dominio.com.br",
                ProvedorIntegracao.ContaAzul => "https://api.contaazul.com",
                ProvedorIntegracao.Omie => "https://api.omie.com.br",
                _ => "https://api.example.com"
            };
        }

        private LancamentoContabil CriarLancamentoTeste(Guid clinicaId)
        {
            return new LancamentoContabil
            {
                Id = Guid.NewGuid(),
                ClinicaId = clinicaId,
                DataLancamento = DateTime.Now,
                Tipo = TipoLancamentoContabil.Debito,
                Valor = 1000.00m,
                Historico = "Teste de lançamento",
                NumeroDocumento = "DOC-001",
                Conta = new PlanoContas
                {
                    Id = Guid.NewGuid(),
                    Codigo = "1.1.01.001",
                    Nome = "Caixa",
                    Tipo = TipoConta.Ativo,
                    Natureza = NaturezaSaldo.Devedora,
                    Analitica = true,
                    Ativa = true,
                    Nivel = 4
                },
                TenantId = TenantId
            };
        }

        #endregion
    }
}
