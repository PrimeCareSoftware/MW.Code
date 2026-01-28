using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Interfaces.Integracoes;

namespace MedicSoft.Application.Services.Fiscal.Integracoes
{
    /// <summary>
    /// Serviço de orquestração para integrações contábeis
    /// </summary>
    public interface IIntegracaoContabilService
    {
        Task<IContabilIntegration?> ObterIntegracaoAsync(Guid clinicaId);
        Task<bool> TestarConexaoAsync(Guid clinicaId);
        Task<string> EnviarLancamentoAsync(Guid clinicaId, LancamentoContabil lancamento);
        Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(Guid clinicaId, IEnumerable<LancamentoContabil> lancamentos);
        Task<bool> EnviarPlanoContasAsync(Guid clinicaId, IEnumerable<PlanoContas> contas);
        Task<ArquivoExportacao> ExportarArquivoAsync(Guid clinicaId, DateTime inicio, DateTime fim, FormatoExportacao formato);
        Task<bool> SincronizarDadosAsync(Guid clinicaId, DateTime inicio, DateTime fim);
    }

    /// <summary>
    /// Implementação do serviço de integração contábil
    /// </summary>
    public class IntegracaoContabilService : IIntegracaoContabilService
    {
        private readonly IConfiguracaoIntegracaoRepository _configuracaoRepository;
        private readonly IPlanoContasRepository _planoContasRepository;
        private readonly ILancamentoContabilRepository _lancamentoRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<IntegracaoContabilService> _logger;

        public IntegracaoContabilService(
            IConfiguracaoIntegracaoRepository configuracaoRepository,
            IPlanoContasRepository planoContasRepository,
            ILancamentoContabilRepository lancamentoRepository,
            IHttpClientFactory httpClientFactory,
            ILoggerFactory loggerFactory,
            ILogger<IntegracaoContabilService> logger)
        {
            _configuracaoRepository = configuracaoRepository ?? throw new ArgumentNullException(nameof(configuracaoRepository));
            _planoContasRepository = planoContasRepository ?? throw new ArgumentNullException(nameof(planoContasRepository));
            _lancamentoRepository = lancamentoRepository ?? throw new ArgumentNullException(nameof(lancamentoRepository));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IContabilIntegration?> ObterIntegracaoAsync(Guid clinicaId)
        {
            // Buscar configuração de integração
            var configuracao = await _configuracaoRepository.ObterConfiguracaoAtivaAsync(clinicaId);
            
            if (configuracao == null || !configuracao.Ativa)
            {
                _logger.LogWarning("Nenhuma integração ativa configurada para clínica {ClinicaId}", clinicaId);
                return null;
            }

            return CriarIntegracao(configuracao);
        }

        public async Task<bool> TestarConexaoAsync(Guid clinicaId)
        {
            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
                return false;

            try
            {
                return await integracao.TestarConexaoAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao testar conexão para clínica {ClinicaId}", clinicaId);
                return false;
            }
        }

        public async Task<string> EnviarLancamentoAsync(Guid clinicaId, LancamentoContabil lancamento)
        {
            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
                throw new InvalidOperationException("Nenhuma integração configurada");

            try
            {
                var resultado = await integracao.EnviarLancamentoAsync(lancamento);
                await _configuracaoRepository.AtualizarUltimaSincronizacaoAsync(clinicaId, DateTime.UtcNow);
                await _configuracaoRepository.LimparErrosAsync(clinicaId);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lançamento {LancamentoId}", lancamento.Id);
                await _configuracaoRepository.RegistrarErroAsync(clinicaId, ex.Message);
                throw;
            }
        }

        public async Task<ResultadoEnvioLote> EnviarLancamentosLoteAsync(Guid clinicaId, IEnumerable<LancamentoContabil> lancamentos)
        {
            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
                throw new InvalidOperationException("Nenhuma integração configurada");

            try
            {
                var resultado = await integracao.EnviarLancamentosLoteAsync(lancamentos);
                
                if (resultado.Sucesso)
                {
                    await _configuracaoRepository.AtualizarUltimaSincronizacaoAsync(clinicaId, DateTime.UtcNow);
                    await _configuracaoRepository.LimparErrosAsync(clinicaId);
                }
                else
                {
                    var mensagemErro = $"{resultado.TotalErros} erro(s) no envio do lote";
                    await _configuracaoRepository.RegistrarErroAsync(clinicaId, mensagemErro);
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lote de lançamentos para clínica {ClinicaId}", clinicaId);
                await _configuracaoRepository.RegistrarErroAsync(clinicaId, ex.Message);
                throw;
            }
        }

        public async Task<bool> EnviarPlanoContasAsync(Guid clinicaId, IEnumerable<PlanoContas> contas)
        {
            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
                throw new InvalidOperationException("Nenhuma integração configurada");

            try
            {
                var resultado = await integracao.EnviarPlanoContasAsync(contas);
                await _configuracaoRepository.AtualizarUltimaSincronizacaoAsync(clinicaId, DateTime.UtcNow);
                await _configuracaoRepository.LimparErrosAsync(clinicaId);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar plano de contas para clínica {ClinicaId}", clinicaId);
                await _configuracaoRepository.RegistrarErroAsync(clinicaId, ex.Message);
                throw;
            }
        }

        public async Task<ArquivoExportacao> ExportarArquivoAsync(Guid clinicaId, DateTime inicio, DateTime fim, FormatoExportacao formato)
        {
            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
                throw new InvalidOperationException("Nenhuma integração configurada");

            try
            {
                return await integracao.ExportarArquivoAsync(inicio, fim, formato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao exportar arquivo para clínica {ClinicaId}", clinicaId);
                throw;
            }
        }

        public async Task<bool> SincronizarDadosAsync(Guid clinicaId, DateTime inicio, DateTime fim)
        {
            _logger.LogInformation("Iniciando sincronização para clínica {ClinicaId} - Período: {Inicio} a {Fim}", 
                clinicaId, inicio, fim);

            var integracao = await ObterIntegracaoAsync(clinicaId);
            
            if (integracao == null)
            {
                _logger.LogWarning("Integração não configurada para clínica {ClinicaId}", clinicaId);
                return false;
            }

            try
            {
                // 1. Buscar plano de contas
                var planoContas = await BuscarPlanoContasAsync(clinicaId);
                if (planoContas.Count > 0)
                {
                    _logger.LogInformation("Enviando {Total} contas", planoContas.Count);
                    await integracao.EnviarPlanoContasAsync(planoContas);
                }

                // 2. Buscar lançamentos do período
                var lancamentos = await BuscarLancamentosAsync(clinicaId, inicio, fim);
                if (lancamentos.Count > 0)
                {
                    _logger.LogInformation("Enviando {Total} lançamentos", lancamentos.Count);
                    var resultado = await integracao.EnviarLancamentosLoteAsync(lancamentos);
                    
                    if (!resultado.Sucesso)
                    {
                        _logger.LogWarning("Sincronização com erros: {TotalErros} erro(s)", resultado.TotalErros);
                        return false;
                    }
                }

                await _configuracaoRepository.AtualizarUltimaSincronizacaoAsync(clinicaId, DateTime.UtcNow);
                await _configuracaoRepository.LimparErrosAsync(clinicaId);
                _logger.LogInformation("Sincronização concluída com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na sincronização da clínica {ClinicaId}", clinicaId);
                await _configuracaoRepository.RegistrarErroAsync(clinicaId, ex.Message);
                return false;
            }
        }

        private IContabilIntegration CriarIntegracao(ConfiguracaoIntegracao configuracao)
        {
            var httpClient = _httpClientFactory.CreateClient("ContabilIntegration");
            
            return configuracao.Provedor switch
            {
                ProvedorIntegracao.Dominio => new DominioIntegration(
                    httpClient, 
                    _loggerFactory.CreateLogger<DominioIntegration>(), 
                    configuracao),
                    
                ProvedorIntegracao.ContaAzul => new ContaAzulIntegration(
                    httpClient, 
                    _loggerFactory.CreateLogger<ContaAzulIntegration>(), 
                    configuracao),
                    
                ProvedorIntegracao.Omie => new OmieIntegration(
                    httpClient, 
                    _loggerFactory.CreateLogger<OmieIntegration>(), 
                    configuracao),
                    
                _ => throw new NotSupportedException($"Provedor {configuracao.Provedor} não suportado")
            };
        }

        private async Task<List<PlanoContas>> BuscarPlanoContasAsync(Guid clinicaId)
        {
            // Buscar todas as contas ativas da clínica usando a clínica como tenant
            var clinicaIdStr = clinicaId.ToString();
            var todasContas = await _planoContasRepository.GetAllAsync(clinicaIdStr);
            return todasContas
                .Where(p => p.ClinicaId == clinicaId && p.Ativa)
                .OrderBy(p => p.Codigo)
                .ToList();
        }

        private async Task<List<LancamentoContabil>> BuscarLancamentosAsync(Guid clinicaId, DateTime inicio, DateTime fim)
        {
            // Buscar lançamentos do período usando a clínica como tenant
            var clinicaIdStr = clinicaId.ToString();
            var todosLancamentos = await _lancamentoRepository.GetAllAsync(clinicaIdStr);
            return todosLancamentos
                .Where(l => l.ClinicaId == clinicaId 
                         && l.DataLancamento >= inicio 
                         && l.DataLancamento <= fim)
                .OrderBy(l => l.DataLancamento)
                .ToList();
        }
    }
}
