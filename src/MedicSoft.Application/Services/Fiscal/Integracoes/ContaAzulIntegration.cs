using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces.Integracoes;

namespace MedicSoft.Application.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Integração com ContaAzul (software de gestão empresarial)
    /// Utiliza OAuth2 para autenticação
    /// </summary>
    public class ContaAzulIntegration : ContabilIntegrationBase
    {
        public override string NomeProvedor => "ContaAzul";

        public ContaAzulIntegration(
            HttpClient httpClient,
            ILogger<ContaAzulIntegration> logger,
            ConfiguracaoIntegracao configuracao)
            : base(httpClient, logger, configuracao)
        {
            if (!string.IsNullOrEmpty(configuracao?.ApiUrl))
            {
                _httpClient.BaseAddress = new Uri(configuracao.ApiUrl);
            }
            else
            {
                _httpClient.BaseAddress = new Uri("https://api.contaazul.com");
            }
        }

        public override async Task<bool> TestarConexaoAsync()
        {
            try
            {
                if (!ValidarConfiguracao())
                    return false;

                await RefreshTokenIfNeededAsync();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.AccessToken}");

                var response = await _httpClient.GetAsync("/v1/me");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com ContaAzul");
                return false;
            }
        }

        public override async Task<bool> ValidarCredenciaisAsync()
        {
            if (string.IsNullOrEmpty(_configuracao?.ClientId))
            {
                _logger.LogWarning("Client ID não configurado para ContaAzul");
                return false;
            }

            if (string.IsNullOrEmpty(_configuracao?.ClientSecret))
            {
                _logger.LogWarning("Client Secret não configurado para ContaAzul");
                return false;
            }

            if (string.IsNullOrEmpty(_configuracao?.AccessToken))
            {
                _logger.LogWarning("Access Token não configurado para ContaAzul");
                return false;
            }

            return await TestarConexaoAsync();
        }

        public override async Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração ContaAzul");

            await RefreshTokenIfNeededAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.AccessToken}");

            var payload = new
            {
                date = lancamento.DataLancamento.ToString("yyyy-MM-dd"),
                description = lancamento.Historico,
                account_id = lancamento.Conta?.Codigo,
                type = lancamento.Tipo == TipoLancamentoContabil.Debito ? "DEBIT" : "CREDIT",
                value = lancamento.Valor,
                document_number = lancamento.NumeroDocumento
            };

            var response = await _httpClient.PostAsJsonAsync("/v1/financial-entries", payload);
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<ContaAzulResponse>();
            return resultado?.id ?? string.Empty;
        }

        public override async Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração ContaAzul");

            await RefreshTokenIfNeededAsync();

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.AccessToken}");

            foreach (var conta in contas.Where(c => c.Analitica))
            {
                var payload = new
                {
                    code = conta.Codigo,
                    name = conta.Nome,
                    type = MapearTipoConta(conta.Tipo),
                    nature = conta.Natureza == NaturezaSaldo.Devedora ? "DEBIT" : "CREDIT",
                    level = conta.Nivel,
                    active = conta.Ativa
                };

                var response = await _httpClient.PostAsJsonAsync("/v1/accounts", payload);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Erro ao enviar conta {Codigo} para ContaAzul", conta.Codigo);
                }
            }

            return true;
        }

        public override async Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração ContaAzul");

            await RefreshTokenIfNeededAsync();

            // ContaAzul suporta exportação direta via API
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.AccessToken}");

            var url = $"/v1/financial-entries/export?start_date={inicio:yyyy-MM-dd}&end_date={fim:yyyy-MM-dd}&format={formato.ToString().ToLower()}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var conteudo = await response.Content.ReadAsByteArrayAsync();
            var nomeArquivo = $"contaazul_export_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.{formato.ToString().ToLower()}";

            return new ArquivoExportacao
            {
                NomeArquivo = nomeArquivo,
                Conteudo = conteudo,
                ContentType = ObterContentType(formato),
                Formato = formato,
                DataGeracao = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Renova o token se estiver próximo de expirar
        /// </summary>
        private async Task RefreshTokenIfNeededAsync()
        {
            if (_configuracao.TokenExpiraEm.HasValue && 
                _configuracao.TokenExpiraEm.Value > DateTime.UtcNow.AddMinutes(5))
            {
                return; // Token ainda válido
            }

            if (string.IsNullOrEmpty(_configuracao.RefreshToken))
            {
                _logger.LogWarning("Refresh token não disponível para ContaAzul");
                return;
            }

            try
            {
                var payload = new Dictionary<string, string>
                {
                    { "grant_type", "refresh_token" },
                    { "refresh_token", _configuracao.RefreshToken },
                    { "client_id", _configuracao.ClientId! },
                    { "client_secret", _configuracao.ClientSecret! }
                };

                var response = await _httpClient.PostAsync("/oauth2/token", new FormUrlEncodedContent(payload));
                response.EnsureSuccessStatusCode();

                var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
                
                if (tokenResponse != null)
                {
                    _configuracao.AccessToken = tokenResponse.access_token;
                    _configuracao.RefreshToken = tokenResponse.refresh_token;
                    _configuracao.TokenExpiraEm = DateTime.UtcNow.AddSeconds(tokenResponse.expires_in);
                    
                    _logger.LogInformation("Token ContaAzul renovado com sucesso");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao renovar token ContaAzul");
                throw;
            }
        }

        private string MapearTipoConta(TipoConta tipo)
        {
            return tipo switch
            {
                TipoConta.Ativo => "ASSET",
                TipoConta.Passivo => "LIABILITY",
                TipoConta.PatrimonioLiquido => "EQUITY",
                TipoConta.Receita => "REVENUE",
                TipoConta.Despesa => "EXPENSE",
                TipoConta.Custos => "COST",
                _ => "OTHER"
            };
        }

        private string ObterContentType(FormatoExportacao formato)
        {
            return formato switch
            {
                FormatoExportacao.CSV => "text/csv",
                FormatoExportacao.JSON => "application/json",
                FormatoExportacao.XML => "application/xml",
                FormatoExportacao.TXT => "text/plain",
                _ => "application/octet-stream"
            };
        }

        private class ContaAzulResponse
        {
            public string? id { get; set; }
        }

        private class TokenResponse
        {
            public string access_token { get; set; } = null!;
            public string refresh_token { get; set; } = null!;
            public int expires_in { get; set; }
            public string token_type { get; set; } = null!;
        }
    }
}
