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
    /// Testes para integração com Omie ERP
    /// </summary>
    public class OmieIntegrationTests
    {
        private readonly Mock<ILogger<OmieIntegration>> _loggerMock;
        private readonly ConfiguracaoIntegracao _configuracao;

        public OmieIntegrationTests()
        {
            _loggerMock = new Mock<ILogger<OmieIntegration>>();
            
            _configuracao = new ConfiguracaoIntegracao
            {
                Id = Guid.NewGuid(),
                ClinicaId = Guid.NewGuid(),
                Provedor = ProvedorIntegracao.Omie,
                Ativa = true,
                ApiUrl = "https://app.omie.com.br/api/v1",
                ApiKey = "test-app-key",
                ClientSecret = "test-app-secret"
            };
        }

        [Fact]
        public async Task TestarConexaoAsync_DeveRetornarTrue_QuandoConexaoEhSucesso()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"empresas\": [] }");
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);

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
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.TestarConexaoAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoAppKeyNaoConfigurada()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.ApiKey = null;
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var resultado = await integration.ValidarCredenciaisAsync();

            // Assert
            Assert.False(resultado);
        }

        [Fact]
        public async Task ValidarCredenciaisAsync_DeveRetornarFalse_QuandoAppSecretNaoConfigurado()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{}");
            _configuracao.ClientSecret = null;
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);

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
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => integration.EnviarLancamentoAsync(lancamento));
        }

        [Fact]
        public async Task EnviarLancamentoAsync_DeveRetornarId_QuandoEnvioEhSucesso()
        {
            // Arrange
            var responseJson = "{ \"cCodLanc\": 54321 }";
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, responseJson);
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var lancamento = CriarLancamentoTeste();

            // Act
            var resultado = await integration.EnviarLancamentoAsync(lancamento);

            // Assert
            Assert.NotNull(resultado);
            Assert.Equal("54321", resultado);
        }

        [Fact]
        public void NomeProvedor_DeveRetornarOmieERP()
        {
            // Arrange
            using var httpClient = new HttpClient();
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);

            // Act
            var nome = integration.NomeProvedor;

            // Assert
            Assert.Equal("Omie ERP", nome);
        }

        [Fact]
        public async Task EnviarPlanoContasAsync_DeveRetornarTrue_QuandoEnvioEhSucesso()
        {
            // Arrange
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, "{ \"cCodConta\": \"1.1.01.001\" }");
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);
            
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

        [Fact]
        public async Task ExportarArquivoAsync_DeveRetornarArquivo_QuandoExportacaoEhSucesso()
        {
            // Arrange
            var responseJson = "{ \"lancamentos\": [] }";
            var httpClient = CriarHttpClientComResposta(HttpStatusCode.OK, responseJson);
            var integration = new OmieIntegration(httpClient, _loggerMock.Object, _configuracao);
            
            var inicio = new DateTime(2024, 1, 1);
            var fim = new DateTime(2024, 1, 31);

            // Act
            var resultado = await integration.ExportarArquivoAsync(inicio, fim, FormatoExportacao.JSON);

            // Assert
            Assert.NotNull(resultado);
            Assert.NotNull(resultado.NomeArquivo);
            Assert.Contains("omie_export", resultado.NomeArquivo);
            Assert.Equal(FormatoExportacao.JSON, resultado.Formato);
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
                BaseAddress = new Uri("https://app.omie.com.br/api/v1")
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
                Valor = 2000.00m,
                Historico = "Teste de lançamento Omie",
                NumeroDocumento = "DOC-OMIE-001",
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
