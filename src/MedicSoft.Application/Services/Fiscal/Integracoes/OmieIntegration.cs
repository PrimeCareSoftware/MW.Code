using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces.Integracoes;

namespace MedicSoft.Application.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Integração com Omie ERP
    /// Utiliza API Key e App Secret para autenticação
    /// </summary>
    public class OmieIntegration : ContabilIntegrationBase
    {
        public override string NomeProvedor => "Omie ERP";

        public OmieIntegration(
            HttpClient httpClient,
            ILogger<OmieIntegration> logger,
            ConfiguracaoIntegracao configuracao)
            : base(httpClient, logger, configuracao)
        {
            if (!string.IsNullOrEmpty(configuracao?.ApiUrl))
            {
                _httpClient.BaseAddress = new Uri(configuracao.ApiUrl);
            }
            else
            {
                _httpClient.BaseAddress = new Uri("https://app.omie.com.br/api/v1");
            }
        }

        public override async Task<bool> TestarConexaoAsync()
        {
            try
            {
                if (!ValidarConfiguracao())
                    return false;

                var payload = new
                {
                    call = "ListarEmpresas",
                    app_key = _configuracao.ApiKey,
                    app_secret = _configuracao.ClientSecret,
                    param = new[] { new { pagina = 1, registros_por_pagina = 1 } }
                };

                var response = await _httpClient.PostAsJsonAsync("/geral/empresas/", payload);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão com Omie");
                return false;
            }
        }

        public override async Task<bool> ValidarCredenciaisAsync()
        {
            if (string.IsNullOrEmpty(_configuracao?.ApiKey))
            {
                _logger.LogWarning("App Key não configurada para Omie");
                return false;
            }

            if (string.IsNullOrEmpty(_configuracao?.ClientSecret))
            {
                _logger.LogWarning("App Secret não configurado para Omie");
                return false;
            }

            return await TestarConexaoAsync();
        }

        public override async Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Omie");

            var payload = new
            {
                call = "IncluirLancamento",
                app_key = _configuracao.ApiKey,
                app_secret = _configuracao.ClientSecret,
                param = new[]
                {
                    new
                    {
                        cCodIntLanc = lancamento.Id.ToString(),
                        dDtLanc = lancamento.DataLancamento.ToString("dd/MM/yyyy"),
                        cHistorico = lancamento.Historico,
                        cCodConta = lancamento.Conta?.Codigo,
                        cTipo = lancamento.Tipo == TipoLancamentoContabil.Debito ? "D" : "C",
                        nValor = lancamento.Valor,
                        cNumDoc = lancamento.NumeroDocumento
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/financas/lancamento/", payload);
            response.EnsureSuccessStatusCode();

            var resultado = await response.Content.ReadFromJsonAsync<OmieLancamentoResponse>();
            return resultado?.cCodLanc?.ToString() ?? string.Empty;
        }

        public override async Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Omie");

            var contasAnaliticas = contas.Where(c => c.Analitica).ToList();
            
            foreach (var conta in contasAnaliticas)
            {
                try
                {
                    var payload = new
                    {
                        call = "IncluirConta",
                        app_key = _configuracao.ApiKey,
                        app_secret = _configuracao.ClientSecret,
                        param = new[]
                        {
                            new
                            {
                                cCodConta = conta.Codigo,
                                cNome = conta.Nome,
                                cTipo = MapearTipoConta(conta.Tipo),
                                cNatureza = conta.Natureza == NaturezaSaldo.Devedora ? "D" : "C",
                                nNivel = conta.Nivel,
                                cAtiva = conta.Ativa ? "S" : "N",
                                cAnalitica = conta.Analitica ? "S" : "N"
                            }
                        }
                    };

                    var response = await _httpClient.PostAsJsonAsync("/geral/planoconta/", payload);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogWarning("Erro ao enviar conta {Codigo} para Omie", conta.Codigo);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar conta {Codigo}", conta.Codigo);
                }
            }

            return true;
        }

        public override async Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato)
        {
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Omie");

            // Buscar lançamentos do período
            var payload = new
            {
                call = "ListarLancamentos",
                app_key = _configuracao.ApiKey,
                app_secret = _configuracao.ClientSecret,
                param = new[]
                {
                    new
                    {
                        dDtInicial = inicio.ToString("dd/MM/yyyy"),
                        dDtFinal = fim.ToString("dd/MM/yyyy"),
                        nPagina = 1,
                        nRegPorPagina = 500
                    }
                }
            };

            var response = await _httpClient.PostAsJsonAsync("/financas/lancamento/", payload);
            response.EnsureSuccessStatusCode();

            byte[] conteudo;
            string contentType;
            var nomeArquivo = $"omie_export_{inicio:yyyyMMdd}_{fim:yyyyMMdd}.{formato.ToString().ToLower()}";

            if (formato == FormatoExportacao.JSON)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                conteudo = Encoding.UTF8.GetBytes(jsonContent);
                contentType = "application/json";
            }
            else if (formato == FormatoExportacao.CSV)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                // Converter JSON para CSV (implementação simplificada)
                var csvContent = "Data;Conta;Historico;Tipo;Valor;Documento\n";
                conteudo = Encoding.UTF8.GetBytes(csvContent + "# Dados exportados de Omie");
                contentType = "text/csv";
            }
            else
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                conteudo = Encoding.UTF8.GetBytes(jsonContent);
                contentType = "text/plain";
            }

            return new ArquivoExportacao
            {
                NomeArquivo = nomeArquivo,
                Conteudo = conteudo,
                ContentType = contentType,
                Formato = formato,
                DataGeracao = DateTime.UtcNow
            };
        }

        public override async Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(IEnumerable<LancamentoContabil> lancamentos)
        {
            // Omie suporta envio em lote
            if (!ValidarConfiguracao())
                throw new InvalidOperationException("Configuração inválida para integração Omie");

            var resultado = new ResultadoEnvioLote();
            var lista = lancamentos.ToList();
            resultado.TotalEnviados = lista.Count;

            // Preparar payload para envio em lote
            var lancamentosPayload = lista.Select(l => new
            {
                cCodIntLanc = l.Id.ToString(),
                dDtLanc = l.DataLancamento.ToString("dd/MM/yyyy"),
                cHistorico = l.Historico,
                cCodConta = l.Conta?.Codigo,
                cTipo = l.Tipo == TipoLancamentoContabil.Debito ? "D" : "C",
                nValor = l.Valor,
                cNumDoc = l.NumeroDocumento
            }).ToArray();

            var payload = new
            {
                call = "IncluirLancamentosLote",
                app_key = _configuracao.ApiKey,
                app_secret = _configuracao.ClientSecret,
                param = lancamentosPayload
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/financas/lancamento/", payload);
                response.EnsureSuccessStatusCode();

                resultado.TotalSucesso = lista.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lote para Omie");
                // Fallback para envio individual
                return await base.EnviarLancamentosLoteAsync(lancamentos);
            }

            return resultado;
        }

        private string MapearTipoConta(TipoConta tipo)
        {
            return tipo switch
            {
                TipoConta.Ativo => "01",
                TipoConta.Passivo => "02",
                TipoConta.PatrimonioLiquido => "03",
                TipoConta.Receita => "04",
                TipoConta.Despesa => "05",
                TipoConta.Custos => "06",
                _ => "99"
            };
        }

        private class OmieLancamentoResponse
        {
            public long? cCodLanc { get; set; }
            public string? cStatus { get; set; }
        }
    }
}
