using System;
using System.Collections.Generic;
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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<IntegracaoContabilService> _logger;

        public IntegracaoContabilService(
            IUnitOfWork unitOfWork,
            IHttpClientFactory httpClientFactory,
            ILogger<IntegracaoContabilService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IContabilIntegration?> ObterIntegracaoAsync(Guid clinicaId)
        {
            // Buscar configuração de integração
            var configuracao = await BuscarConfiguracaoAsync(clinicaId);
            
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
                await AtualizarUltimaSincronizacaoAsync(clinicaId);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lançamento {LancamentoId}", lancamento.Id);
                await RegistrarErroIntegracaoAsync(clinicaId, ex.Message);
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
                    await AtualizarUltimaSincronizacaoAsync(clinicaId);
                }
                else
                {
                    var mensagemErro = $"{resultado.TotalErros} erro(s) no envio do lote";
                    await RegistrarErroIntegracaoAsync(clinicaId, mensagemErro);
                }
                
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar lote de lançamentos para clínica {ClinicaId}", clinicaId);
                await RegistrarErroIntegracaoAsync(clinicaId, ex.Message);
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
                await AtualizarUltimaSincronizacaoAsync(clinicaId);
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar plano de contas para clínica {ClinicaId}", clinicaId);
                await RegistrarErroIntegracaoAsync(clinicaId, ex.Message);
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

                await AtualizarUltimaSincronizacaoAsync(clinicaId);
                _logger.LogInformation("Sincronização concluída com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro na sincronização da clínica {ClinicaId}", clinicaId);
                await RegistrarErroIntegracaoAsync(clinicaId, ex.Message);
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
                    _logger as ILogger<DominioIntegration> ?? throw new InvalidOperationException(), 
                    configuracao),
                    
                ProvedorIntegracao.ContaAzul => new ContaAzulIntegration(
                    httpClient, 
                    _logger as ILogger<ContaAzulIntegration> ?? throw new InvalidOperationException(), 
                    configuracao),
                    
                ProvedorIntegracao.Omie => new OmieIntegration(
                    httpClient, 
                    _logger as ILogger<OmieIntegration> ?? throw new InvalidOperationException(), 
                    configuracao),
                    
                _ => throw new NotSupportedException($"Provedor {configuracao.Provedor} não suportado")
            };
        }

        private async Task<ConfiguracaoIntegracao?> BuscarConfiguracaoAsync(Guid clinicaId)
        {
            // Placeholder - deve ser implementado com repositório real
            await Task.CompletedTask;
            return null;
        }

        private async Task<List<PlanoContas>> BuscarPlanoContasAsync(Guid clinicaId)
        {
            // Placeholder - deve ser implementado com repositório real
            await Task.CompletedTask;
            return new List<PlanoContas>();
        }

        private async Task<List<LancamentoContabil>> BuscarLancamentosAsync(Guid clinicaId, DateTime inicio, DateTime fim)
        {
            // Placeholder - deve ser implementado com repositório real
            await Task.CompletedTask;
            return new List<LancamentoContabil>();
        }

        private async Task AtualizarUltimaSincronizacaoAsync(Guid clinicaId)
        {
            // Placeholder - deve atualizar a data de última sincronização
            await Task.CompletedTask;
            _logger.LogDebug("Última sincronização atualizada para clínica {ClinicaId}", clinicaId);
        }

        private async Task RegistrarErroIntegracaoAsync(Guid clinicaId, string mensagem)
        {
            // Placeholder - deve registrar erro no banco
            await Task.CompletedTask;
            _logger.LogWarning("Erro registrado para clínica {ClinicaId}: {Mensagem}", clinicaId, mensagem);
        }
    }
}
