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
    /// Integração com Domínio Sistemas (software contábil)
    /// </summary>
    public class DominioIntegration : ContabilIntegrationBase
    {
        public override string NomeProvedor => "Domínio Sistemas";

        public DominioIntegration(
            HttpClient httpClient,
            ILogger<DominioIntegration> logger,
            ConfiguracaoIntegracao configuracao)
            : base(httpClient, logger, configuracao)
        {
            if (!string.IsNullOrEmpty(configuracao?.ApiUrl))
            {
                _httpClient.BaseAddress = new Uri(configuracao.ApiUrl);
            }
        }

        public override async Task<bool> TestarConexaoAsync()
        {
            try
            {
                if (!ValidarConfiguracao())
                    return false;

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.ApiKey}");

                var response = await _httpClient.GetAsync("/api/v1/ping");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com Domínio");
                return false;
            }
        }

        public override async Task<bool> ValidarCredenciaisAsync()
        {
            if (string.IsNullOrEmpty(_configuracao?.ApiKey))
            {
                _logger.LogWarning("API Key não configurada para Domínio");
                return false;
            }

            if (string.IsNullOrEmpty(_configuracao?.ApiUrl))
            {
                _logger.LogWarning("URL da API não configurada para Domínio");
                return false;
            }

            return await TestarConexaoAsync();
        }

        public override async Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Domínio");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.ApiKey}");

            var payload = new
            {
                empresa_id = _configuracao.CodigoEmpresa,
                data = lancamento.DataLancamento.ToString("yyyy-MM-dd"),
                historico = lancamento.Historico,
                documento = lancamento.NumeroDocumento,
                lote_id = lancamento.LoteId?.ToString(),
                lancamentos = new[]
                {
                    new
                    {
                        conta = lancamento.Conta?.Codigo ?? "0",
                        tipo = lancamento.Tipo == TipoLancamentoContabil.Debito ? "D" : "C",
                        valor = lancamento.Valor
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/api/v1/lancamentos", payload);
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<DominioLancamentoResponse>();
            return resultado?.id ?? string.Empty;
        }

        public override async Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Domínio");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_configuracao.ApiKey}");

            var contasList = contas.Where(c => c.Analitica).Select(c => new
            {
                empresa_id = _configuracao.CodigoEmpresa,
                codigo = c.Codigo,
                nome = c.Nome,
                tipo = MapearTipoConta(c.Tipo),
                natureza = c.Natureza == NaturezaSaldo.Devedora ? "D" : "C",
                nivel = c.Nivel,
                analitica = c.Analitica
            }).ToList();

            var response = await _httpClient.PostAsJsonAsync("/api/v1/plano-contas/lote", new { contas = contasList });
            response.EnsureSuccessStatusCode();

            return true;
        }

        public override async Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Domínio");

            // Nota: Em produção, isso deveria buscar os lançamentos do banco
            // Para esta implementação, retornamos um arquivo de exemplo
            var nomeArquivo = $"dominio_export_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.{formato.ToString().ToLower()}";
            var conteudo = Encoding.UTF8.GetBytes($"Exportação Domínio - {inicio:dd/MM/yyyy} a {fim:dd/MM/yyyy}");

            return new ArquivoExportacao
            {
                NomeArquivo = nomeArquivo,
                Conteudo = conteudo,
                ContentType = formato == FormatoExportacao.CSV ? "text/csv" : "text/plain",
                Formato = formato,
                DataGeracao = DateTime.UtcNow
            };
        }

        private string MapearTipoConta(TipoConta tipo)
        {
            return tipo switch
            {
                TipoConta.Ativo => "ATIVO",
                TipoConta.Passivo => "PASSIVO",
                TipoConta.PatrimonioLiquido => "PATRIMONIO_LIQUIDO",
                TipoConta.Receita => "RECEITA",
                TipoConta.Despesa => "DESPESA",
                TipoConta.Custos => "CUSTO",
                _ => "OUTROS"
            };
        }

        private class DominioLancamentoResponse
        {
            public string? id { get; set; }
            public string? status { get; set; }
        }
    }
}
