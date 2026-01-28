using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using MedicSoft.Application.Services.Fiscal.Integracoes;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Test.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Testes para integração com ContaAzul
    /// </summary>
    public class ContaAzulIntegrationTests
    {
        private readonly Mock<ILogger<ContaAzulIntegration>> _loggerMock;
        private readonly ConfiguracaoIntegracao _configuracao;

        public ContaAzulIntegrationTests()
        {
            _loggerMock = new Mock<ILogger<ContaAzulIntegration>>();
            
            _configuracao = new ConfiguracaoIntegracao
            {
                Id = Guid.NewGuid(),
                ClinicaId = Guid.NewGuid(),
                Provedor = ProvedorIntegracao.ContaAzul,
                Ativa = true,
                ApiUrl = "https://api.contaazul.com",
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                AccessToken = "test-access-token",
                RefreshToken = "test-refresh-token",
                TokenExpiraEm = DateTime.UtcNow.AddHours(1)
            };
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarTrue_QuandoConexaoEhSucesso()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"id\": \"123\", \"name\": \"Empresa Teste\" }");
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.TestarConexaoAsync();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarFalse_QuandoConfiguracaoInativa()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.Ativa = false;
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.TestarConexaoAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoClientIdNaoConfigurado()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.ClientId = null;
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.ValidarCredenciaisAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoClientSecretNaoConfigurado()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.ClientSecret = null;
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.ValidarCredenciaisAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoAccessTokenNaoConfigurado()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.AccessToken = null;
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.ValidarCredenciaisAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveLancarExcecao_QuandoConfiguracaoInvalida()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.Ativa = false;
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => integration.EnviarLancamentoAsync(lancamento));
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveRetornarId_QuandoEnvioEhSucesso()
        {
            // Arrange
            var responseJson = "{ \"id\": \"ca-67890\" }";
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, responseJson);
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act
            var resultado = await integration.EnviarLancamentoAsync(lancamento);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("ca-67890", resultado);
        }

        [Fact]
        public void NomeProvedor_DeveRetornarContaAzul()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var nome = integration.NomeProvedor;

            // Assert
            Assert.Equal("ContaAzul", nome);
        }

        [Fact]
        public async Task EnviarPlanoContasAsync_DeveRetornarTrue_QuandoEnvioEhSucesso()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"id\": \"123\" }");
            var integration = new ContaAzulIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var contas = new[]
            {
                new PlanoContas
                {
                    Id = Guid.NewGuid(),
                    Codigo = "1.1.01.001",
                    Nome = "Caixa",
                    Tipo = TipoConta.Ativo,
                    Natureza = NaturezaSaldo.Devedora,
                    Analitica = true,
                    Ativa = true,
                    Nivel = 4
                }
            };

            // Act
            var resultado = await integration.EnviarPlanoContasAsync(contas);

            // Assert
            Assert.True(resultado);
        }

        private HttpClient CriarHttpClientComResposta(HttpStatusCode statusCode, string conteudo)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(conteudo)
                });

            return new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri("https://api.contaazul.com")
            };
        }

        private LancamentoContabil CriarLancamentoTeste()
        {
            return new LancamentoContabil
            {
                Id = Guid.NewGuid(),
                ClinicaId = _configuracao.ClinicaId,
                DataLancamento = DateTime.Now,
                Tipo = TipoLancamentoContabil.Debito,
                Valor = 1500.00m,
                Historico = "Teste de lançamento ContaAzul",
                NumeroDocumento = "DOC-CA-001",
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
                }
            };
        }
    }
}
