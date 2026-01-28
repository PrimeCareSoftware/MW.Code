using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Application.Services.Fiscal
{
    /// <summary>
    /// Serviço para apuração mensal de impostos
    /// </summary>
    public class ApuracaoImpostosService : IApuracaoImpostosService
    {
        private readonly IApuracaoImpostosRepository _apuracaoRepository;
        private readonly IElectronicInvoiceRepository _invoiceRepository;
        private readonly IImpostoNotaRepository _impostoNotaRepository;
        private readonly IConfiguracaoFiscalRepository _configuracaoFiscalRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<ApuracaoImpostosService> _logger;

        public ApuracaoImpostosService(
            IApuracaoImpostosRepository apuracaoRepository,
            IElectronicInvoiceRepository invoiceRepository,
            IImpostoNotaRepository impostoNotaRepository,
            IConfiguracaoFiscalRepository configuracaoFiscalRepository,
            IClinicRepository clinicRepository,
            ILogger<ApuracaoImpostosService> logger)
        {
            _apuracaoRepository = apuracaoRepository;
            _invoiceRepository = invoiceRepository;
            _impostoNotaRepository = impostoNotaRepository;
            _configuracaoFiscalRepository = configuracaoFiscalRepository;
            _clinicRepository = clinicRepository;
            _logger = logger;
        }

        /// <summary>
        /// Gera apuração mensal de impostos
        /// </summary>
        public async Task<ApuracaoImpostos> GerarApuracaoMensalAsync(Guid clinicaId, int mes, int ano, string tenantId)
        {
            _logger.LogInformation(
                "Iniciando apuração de impostos para clínica {ClinicaId}, período {Mes}/{Ano}",
                clinicaId, mes, ano);

            // Validar parâmetros
            if (mes < 1 || mes > 12)
                throw new ArgumentException("Mês deve estar entre 1 e 12.", nameof(mes));

            if (ano < 2000 || ano > 2100)
                throw new ArgumentException("Ano inválido.", nameof(ano));

            // Verificar se já existe apuração para o período
            var apuracaoExistente = await _apuracaoRepository.GetByClinicaAndMesAnoAsync(clinicaId, mes, ano, tenantId);
            if (apuracaoExistente != null)
            {
                _logger.LogWarning(
                    "Já existe apuração para clínica {ClinicaId}, período {Mes}/{Ano}. Status: {Status}",
                    clinicaId, mes, ano, apuracaoExistente.Status);
                return apuracaoExistente;
            }

            // Buscar configuração fiscal vigente
            var dataReferencia = new DateTime(ano, mes, 1);
            var configuracao = await _configuracaoFiscalRepository.GetConfiguracaoVigenteAsync(
                clinicaId, 
                dataReferencia, 
                tenantId);

            if (configuracao == null)
            {
                throw new InvalidOperationException(
                    $"Configuração fiscal não encontrada para clínica {clinicaId} no período {mes}/{ano}.");
            }

            // Definir período de apuração
            var dataInicio = new DateTime(ano, mes, 1);
            var dataFim = dataInicio.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);

            // Buscar notas autorizadas no período
            var notasPeriodo = await _invoiceRepository.GetAuthorizedInPeriodAsync(dataInicio, dataFim, tenantId);
            
            // Buscar clínica para filtrar notas pelo CNPJ
            var clinic = await _clinicRepository.GetByIdAsync(clinicaId, tenantId);
            if (clinic == null)
                throw new InvalidOperationException($"Clínica {clinicaId} não encontrada.");
            
            var notasClinica = notasPeriodo.Where(n => n.ProviderCnpj == clinic.Document).ToList();

            _logger.LogInformation(
                "Encontradas {QtdNotas} notas autorizadas no período {Mes}/{Ano}",
                notasClinica.Count, mes, ano);

            // Calcular faturamento
            var faturamentoBruto = notasClinica.Sum(n => n.ServiceAmount);
            var deducoes = 0m; // Poderia incluir cancelamentos, descontos, etc.

            // Criar apuração
            var apuracao = new ApuracaoImpostos(tenantId)
            {
                ClinicaId = clinicaId,
                Mes = mes,
                Ano = ano,
                DataApuracao = DateTime.UtcNow,
                FaturamentoBruto = faturamentoBruto,
                Deducoes = deducoes,
                Status = StatusApuracao.EmAberto
            };

            // Buscar impostos das notas do período
            var impostosPeriodo = await _impostoNotaRepository.GetByClinicaAndPeriodoAsync(
                clinicaId, 
                dataInicio, 
                dataFim, 
                tenantId);

            // Somar impostos
            apuracao.TotalPIS = impostosPeriodo.Sum(i => i.ValorPIS);
            apuracao.TotalCOFINS = impostosPeriodo.Sum(i => i.ValorCOFINS);
            apuracao.TotalIR = impostosPeriodo.Sum(i => i.ValorIR);
            apuracao.TotalCSLL = impostosPeriodo.Sum(i => i.ValorCSLL);
            apuracao.TotalISS = impostosPeriodo.Sum(i => i.ValorISS);
            apuracao.TotalINSS = impostosPeriodo.Sum(i => i.ValorINSS);

            // Para Simples Nacional, calcular DAS
            if (configuracao.Regime == RegimeTributarioEnum.SimplesNacional)
            {
                await CalcularDASSimples(apuracao, configuracao, tenantId);
            }

            // Atualizar status para apurado
            apuracao.Status = StatusApuracao.Apurado;

            // Salvar apuração
            var resultado = await _apuracaoRepository.AddAsync(apuracao);

            _logger.LogInformation(
                "Apuração concluída para clínica {ClinicaId}, período {Mes}/{Ano}. " +
                "Faturamento: {Faturamento:C}, Total Impostos: {TotalImpostos:C}",
                clinicaId, mes, ano, 
                resultado.FaturamentoBruto,
                resultado.TotalPIS + resultado.TotalCOFINS + resultado.TotalIR + 
                resultado.TotalCSLL + resultado.TotalISS + resultado.TotalINSS);

            return resultado;
        }

        /// <summary>
        /// Atualiza status da apuração
        /// </summary>
        public async Task<ApuracaoImpostos> AtualizarStatusAsync(Guid apuracaoId, StatusApuracao novoStatus, string tenantId)
        {
            _logger.LogInformation(
                "Atualizando status da apuração {ApuracaoId} para {NovoStatus}",
                apuracaoId, novoStatus);

            var apuracao = await _apuracaoRepository.GetByIdAsync(apuracaoId, tenantId);
            if (apuracao == null)
                throw new InvalidOperationException($"Apuração {apuracaoId} não encontrada.");

            // Validar transição de status
            ValidarTransicaoStatus(apuracao.Status, novoStatus);

            apuracao.Status = novoStatus;
            await _apuracaoRepository.UpdateAsync(apuracao);

            _logger.LogInformation(
                "Status da apuração {ApuracaoId} atualizado para {NovoStatus}",
                apuracaoId, novoStatus);

            return apuracao;
        }

        /// <summary>
        /// Registra pagamento de apuração
        /// </summary>
        public async Task<ApuracaoImpostos> RegistrarPagamentoAsync(
            Guid apuracaoId, 
            DateTime dataPagamento, 
            string comprovante, 
            string tenantId)
        {
            _logger.LogInformation(
                "Registrando pagamento da apuração {ApuracaoId} em {DataPagamento}",
                apuracaoId, dataPagamento);

            var apuracao = await _apuracaoRepository.GetByIdAsync(apuracaoId, tenantId);
            if (apuracao == null)
                throw new InvalidOperationException($"Apuração {apuracaoId} não encontrada.");

            if (apuracao.Status != StatusApuracao.Apurado && apuracao.Status != StatusApuracao.EmAberto)
            {
                throw new InvalidOperationException(
                    $"Não é possível registrar pagamento para apuração com status {apuracao.Status}.");
            }

            apuracao.Status = StatusApuracao.Pago;
            apuracao.DataPagamento = dataPagamento;
            apuracao.ComprovantesPagamento = comprovante;

            await _apuracaoRepository.UpdateAsync(apuracao);

            _logger.LogInformation(
                "Pagamento registrado para apuração {ApuracaoId}. Data: {DataPagamento}",
                apuracaoId, dataPagamento);

            return apuracao;
        }

        /// <summary>
        /// Calcula o valor do DAS para Simples Nacional
        /// </summary>
        private async Task CalcularDASSimples(
            ApuracaoImpostos apuracao, 
            ConfiguracaoFiscal configuracao, 
            string tenantId)
        {
            if (!configuracao.AnexoSimples.HasValue)
            {
                _logger.LogWarning("Anexo do Simples Nacional não configurado, não será calculado o DAS");
                return;
            }

            // Calcular receita bruta dos últimos 12 meses
            var dataFim = new DateTime(apuracao.Ano, apuracao.Mes, 1).AddDays(-1);
            var dataInicio = dataFim.AddMonths(-11).AddDays(1);

            var receitaBruta12Meses = await _invoiceRepository.GetTotalIssuedInPeriodAsync(
                dataInicio, 
                dataFim, 
                tenantId);

            // Adicionar faturamento do mês atual
            receitaBruta12Meses += apuracao.FaturamentoBruto;

            apuracao.ReceitaBruta12Meses = receitaBruta12Meses;

            // Calcular alíquota efetiva
            apuracao.AliquotaEfetiva = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                receitaBruta12Meses, 
                configuracao.AnexoSimples.Value);

            // Calcular valor do DAS
            apuracao.ValorDAS = SimplesNacionalHelper.CalcularValorDAS(
                receitaBruta12Meses,
                apuracao.FaturamentoBruto,
                configuracao.AnexoSimples.Value);

            _logger.LogDebug(
                "DAS Simples Nacional calculado. Anexo: {Anexo}, Receita 12m: {Receita12m:C}, " +
                "Alíquota Efetiva: {AliquotaEfetiva:N2}%, Valor DAS: {ValorDAS:C}",
                configuracao.AnexoSimples.Value,
                receitaBruta12Meses,
                apuracao.AliquotaEfetiva,
                apuracao.ValorDAS);
        }

        /// <summary>
        /// Valida a transição de status
        /// </summary>
        private void ValidarTransicaoStatus(StatusApuracao statusAtual, StatusApuracao novoStatus)
        {
            // Regras de transição permitidas
            var transicoesPermitidas = new[]
            {
                (StatusApuracao.EmAberto, StatusApuracao.Apurado),
                (StatusApuracao.Apurado, StatusApuracao.Pago),
                (StatusApuracao.Apurado, StatusApuracao.Parcelado),
                (StatusApuracao.Parcelado, StatusApuracao.Pago),
                (StatusApuracao.EmAberto, StatusApuracao.Atrasado),
                (StatusApuracao.Apurado, StatusApuracao.Atrasado),
                (StatusApuracao.Atrasado, StatusApuracao.Pago),
                (StatusApuracao.Atrasado, StatusApuracao.Parcelado)
            };

            if (!transicoesPermitidas.Contains((statusAtual, novoStatus)))
            {
                throw new InvalidOperationException(
                    $"Transição de status de {statusAtual} para {novoStatus} não é permitida.");
            }
        }
    }
}
