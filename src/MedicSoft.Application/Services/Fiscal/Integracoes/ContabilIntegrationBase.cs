using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces.Integracoes;

namespace MedicSoft.Application.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Classe base para implementações de integração contábil
    /// </summary>
    public abstract class ContabilIntegrationBase : IContabilIntegration
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger _logger;
        protected readonly ConfiguracaoIntegracao _configuracao;

        public abstract string NomeProvedor { get; }

        protected ContabilIntegrationBase(
            HttpClient httpClient,
            ILogger logger,
            ConfiguracaoIntegracao configuracao)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuracao = configuracao ?? throw new ArgumentNullException(nameof(configuracao));
        }

        public abstract Task<bool> TestarConexaoAsync();
        public abstract Task<string> EnviarLancamentoAsync(LancamentoContabil lancamento);
        public abstract Task<bool> EnviarPlanoContasAsync(IEnumerable<PlanoContas> contas);
        public abstract Task<ArquivoExportacao> ExportarArquivoAsync(DateTime inicio, DateTime fim, FormatoExportacao formato);
        public abstract Task<bool> ValidarCredenciaisAsync();

        /// <summary>
        /// Envia múltiplos lançamentos em lote (implementação padrão)
        /// </summary>
        public virtual async Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(IEnumerable<LancamentoContabil> lancamentos)
        {
            var resultado = new ResultadoEnvioLote();
            var lista = lancamentos.ToList();
            resultado.TotalEnviados = lista.Count;

            foreach (var lancamento in lista)
            {
                try
                {
                    await EnviarLancamentoAsync(lancamento);
                    resultado.TotalSucesso++;
                }
                catch (Exception ex)
                {
                    resultado.TotalErros++;
                    resultado.Erros.Add(new ErroEnvio
                    {
                        LancamentoId = lancamento.Id,
                        Mensagem = ex.Message,
                        Detalhes = ex.ToString()
                    });
                    
                    _logger.LogError(ex, "Erro ao enviar lançamento {LancamentoId} para {Provedor}", 
                        lancamento.Id, NomeProvedor);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Exporta lançamentos como CSV (implementação padrão)
        /// </summary>
        protected virtual string GerarCSV(IEnumerable<LancamentoContabil> lancamentos)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Data;Conta;Historico;Debito;Credito;Documento");

            foreach (var lanc in lancamentos)
            {
                var debito = lanc.Tipo == TipoLancamentoContabil.Debito ? lanc.Valor.ToString("F2") : "0.00";
                var credito = lanc.Tipo == TipoLancamentoContabil.Credito ? lanc.Valor.ToString("F2") : "0.00";
                
                sb.AppendLine($"{lanc.DataLancamento:dd/MM/yyyy};{lanc.Conta?.Codigo};{lanc.Historico};{debito};{credito};{lanc.NumeroDocumento}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Valida se a configuração está completa
        /// </summary>
        protected virtual bool ValidarConfiguracao()
        {
            if (_configuracao == null)
            {
                _logger.LogWarning("Configuração de integração não encontrada");
                return false;
            }

            if (!_configuracao.Ativa)
            {
                _logger.LogWarning("Integração está inativa");
                return false;
            }

            return true;
        }
    }
}
