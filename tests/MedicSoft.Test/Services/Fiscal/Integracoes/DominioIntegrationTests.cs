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
    /// Testes para integração com Domínio Sistemas
    /// </summary>
    public class DominioIntegrationTests
    {
        private readonly Mock<ILogger<DominioIntegration>> _loggerMock;
        private readonly ConfiguracaoIntegracao _configuracao;

        public DominioIntegrationTests()
        {
            _loggerMock = new Mock<ILogger<DominioIntegration>>();
            
            _configuracao = new ConfiguracaoIntegracao
            {
                Id = Guid.NewGuid(),
                ClinicaId = Guid.NewGuid(),
                Provedor = ProvedorIntegracao.Dominio,
                Ativa = true,
                ApiUrl = "https://api.dominio.com.br",
                ApiKey = "test-api-key",
                CodigoEmpresa = "123"
            };
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarTrue_QuandoConexaoEhSucesso()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"status\": \"ok\" }");
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.TestarConexaoAsync();

            // Assert
            Assert.True(resultado);
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarFalse_QuandoConfiguracaoInativa()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"status\": \"ok\" }");
            _configuracao.Ativa = false;
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.TestarConexaoAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoApiKeyNaoConfigurada()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.ApiKey = null;
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);

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
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => integration.EnviarLancamentoAsync(lancamento));
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveRetornarId_QuandoEnvioEhSucesso()
        {
            // Arrange
            var responseJson = "{ \"id\": \"12345\", \"status\": \"success\" }";
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, responseJson);
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act
            var resultado = await integration.EnviarLancamentoAsync(lancamento);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("12345", resultado);
        }

        [Fact]
        public void NomeProvedor_DeveRetornarDominioSistemas()
        {
            // Arrange
            var httpClient = new HttpClient();
            var integration = new DominioIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var nome = integration.NomeProvedor;

            // Assert
            Assert.Equal("Domínio Sistemas", nome);
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
                BaseAddress = new Uri("https://api.dominio.com.br")
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
                }
            };
        }
    }
}
