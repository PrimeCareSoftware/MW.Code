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
    /// Serviço para cálculo automático de impostos em notas fiscais
    /// </summary>
    public class CalculoImpostosService : ICalculoImpostosService
    {
        private readonly IElectronicInvoiceRepository _invoiceRepository;
        private readonly IConfiguracaoFiscalRepository _configuracaoFiscalRepository;
        private readonly IImpostoNotaRepository _impostoNotaRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<CalculoImpostosService> _logger;

        public CalculoImpostosService(
            IElectronicInvoiceRepository invoiceRepository,
            IConfiguracaoFiscalRepository configuracaoFiscalRepository,
            IImpostoNotaRepository impostoNotaRepository,
            IClinicRepository clinicRepository,
            ILogger<CalculoImpostosService> logger)
        {
            _invoiceRepository = invoiceRepository;
            _configuracaoFiscalRepository = configuracaoFiscalRepository;
            _impostoNotaRepository = impostoNotaRepository;
            _clinicRepository = clinicRepository;
            _logger = logger;
        }

        /// <summary>
        /// Calcula impostos automaticamente para uma nota fiscal
        /// </summary>
        public async Task<ImpostoNota> CalcularImpostosAsync(Guid notaFiscalId, string tenantId)
        {
            _logger.LogInformation("Iniciando cálculo de impostos para nota fiscal {NotaFiscalId}", notaFiscalId);

            // Buscar a nota fiscal
            var nota = await _invoiceRepository.GetByIdAsync(notaFiscalId, tenantId);
            if (nota == null)
                throw new InvalidOperationException($"Nota fiscal {notaFiscalId} não encontrada.");

            // Buscar clínica através do CNPJ do prestador
            var clinic = await _clinicRepository.GetByDocumentAsync(nota.ProviderCnpj, tenantId);
            if (clinic == null)
                throw new InvalidOperationException($"Clínica não encontrada para CNPJ {nota.ProviderCnpj}.");

            // Buscar configuração fiscal vigente
            var configuracao = await _configuracaoFiscalRepository.GetConfiguracaoVigenteAsync(
                clinic.Id, 
                nota.IssueDate, 
                tenantId);

            if (configuracao == null)
                throw new InvalidOperationException("Configuração fiscal não encontrada para a clínica. Configure os dados fiscais antes de calcular impostos.");

            // Criar entidade de impostos
            var impostoNota = new ImpostoNota(tenantId)
            {
                NotaFiscalId = notaFiscalId,
                ValorBruto = nota.ServiceAmount,
                ValorDesconto = 0,
                DataCalculo = DateTime.UtcNow,
                RegimeTributario = configuracao.Regime.ToString()
            };

            // Calcular impostos de acordo com o regime tributário
            switch (configuracao.Regime)
            {
                case RegimeTributarioEnum.SimplesNacional:
                    await CalcularImpostosSimples(impostoNota, configuracao, tenantId);
                    break;

                case RegimeTributarioEnum.LucroPresumido:
                    CalcularImpostosLucroPresumido(impostoNota, configuracao);
                    break;

                case RegimeTributarioEnum.LucroReal:
                    CalcularImpostosLucroReal(impostoNota, configuracao);
                    break;

                case RegimeTributarioEnum.MEI:
                    CalcularImpostosMEI(impostoNota, configuracao);
                    break;

                default:
                    throw new InvalidOperationException($"Regime tributário {configuracao.Regime} não suportado.");
            }

            // Salvar impostos calculados
            var result = await _impostoNotaRepository.AddAsync(impostoNota);

            _logger.LogInformation(
                "Impostos calculados com sucesso para nota {NotaFiscalId}. Total de impostos: {TotalImpostos:C}",
                notaFiscalId, 
                result.TotalImpostos);

            return result;
        }

        /// <summary>
        /// Recalcula impostos de uma nota existente
        /// </summary>
        public async Task<ImpostoNota> RecalcularImpostosAsync(Guid notaFiscalId, string tenantId)
        {
            _logger.LogInformation("Recalculando impostos para nota fiscal {NotaFiscalId}", notaFiscalId);

            // Buscar impostos existentes
            var impostoExistente = await _impostoNotaRepository.GetByNotaFiscalIdAsync(notaFiscalId, tenantId);
            if (impostoExistente != null)
            {
                // Remover impostos antigos
                await _impostoNotaRepository.DeleteAsync(impostoExistente.Id, tenantId);
            }

            // Calcular novamente
            return await CalcularImpostosAsync(notaFiscalId, tenantId);
        }

        /// <summary>
        /// Calcula impostos para Simples Nacional
        /// </summary>
        private async Task CalcularImpostosSimples(ImpostoNota impostoNota, ConfiguracaoFiscal configuracao, string tenantId)
        {
            if (!configuracao.AnexoSimples.HasValue)
                throw new InvalidOperationException("Anexo do Simples Nacional não configurado.");

            // Calcular receita dos últimos 12 meses
            var dataInicio = DateTime.UtcNow.AddMonths(-12);
            var dataFim = DateTime.UtcNow;
            var receitaBruta12Meses = await _invoiceRepository.GetTotalIssuedInPeriodAsync(dataInicio, dataFim, tenantId);

            // Verificar limite do Simples Nacional
            if (!SimplesNacionalHelper.VerificarLimiteSimples(receitaBruta12Meses))
            {
                _logger.LogWarning(
                    "Receita bruta de 12 meses ({ReceitaBruta12Meses:C}) ultrapassou o limite do Simples Nacional",
                    receitaBruta12Meses);
            }

            // Calcular alíquota efetiva
            var aliquotaEfetiva = SimplesNacionalHelper.CalcularAliquotaEfetiva(
                receitaBruta12Meses, 
                configuracao.AnexoSimples.Value);

            // Calcular valor do DAS
            var valorDAS = impostoNota.ValorLiquido * aliquotaEfetiva / 100;

            // Distribuir impostos
            var distribuicao = SimplesNacionalHelper.DistribuirImpostos(
                valorDAS, 
                configuracao.AnexoSimples.Value);

            // Atribuir valores aos impostos
            impostoNota.AliquotaPIS = aliquotaEfetiva * (distribuicao.ContainsKey("PIS") ? distribuicao["PIS"] : 0) / 100;
            impostoNota.ValorPIS = distribuicao.ContainsKey("PIS") ? distribuicao["PIS"] : 0;

            impostoNota.AliquotaCOFINS = aliquotaEfetiva * (distribuicao.ContainsKey("COFINS") ? distribuicao["COFINS"] : 0) / 100;
            impostoNota.ValorCOFINS = distribuicao.ContainsKey("COFINS") ? distribuicao["COFINS"] : 0;

            impostoNota.AliquotaIR = aliquotaEfetiva * (distribuicao.ContainsKey("IRPJ") ? distribuicao["IRPJ"] : 0) / 100;
            impostoNota.ValorIR = distribuicao.ContainsKey("IRPJ") ? distribuicao["IRPJ"] : 0;

            impostoNota.AliquotaCSLL = aliquotaEfetiva * (distribuicao.ContainsKey("CSLL") ? distribuicao["CSLL"] : 0) / 100;
            impostoNota.ValorCSLL = distribuicao.ContainsKey("CSLL") ? distribuicao["CSLL"] : 0;

            impostoNota.AliquotaISS = aliquotaEfetiva * (distribuicao.ContainsKey("ISS") ? distribuicao["ISS"] : 0) / 100;
            impostoNota.ValorISS = distribuicao.ContainsKey("ISS") ? distribuicao["ISS"] : 0;
            impostoNota.ISSRetido = configuracao.ISS_Retido;

            // CPP (INSS Patronal) no Anexo III
            if (configuracao.AnexoSimples == AnexoSimplesNacional.AnexoIII && distribuicao.ContainsKey("CPP"))
            {
                impostoNota.AliquotaINSS = aliquotaEfetiva * distribuicao["CPP"] / 100;
                impostoNota.ValorINSS = distribuicao["CPP"];
                impostoNota.INSSRetido = false;
            }

            impostoNota.CodigoServicoMunicipal = configuracao.CodigoServico;

            _logger.LogDebug(
                "Simples Nacional - Anexo {Anexo}, Receita 12m: {Receita12m:C}, Alíquota Efetiva: {AliquotaEfetiva:N2}%",
                configuracao.AnexoSimples.Value,
                receitaBruta12Meses,
                aliquotaEfetiva);
        }

        /// <summary>
        /// Calcula impostos para Lucro Presumido
        /// </summary>
        private void CalcularImpostosLucroPresumido(ImpostoNota impostoNota, ConfiguracaoFiscal configuracao)
        {
            var valorLiquido = impostoNota.ValorLiquido;

            // PIS (0,65% sobre faturamento)
            impostoNota.AliquotaPIS = configuracao.AliquotaPIS > 0 ? configuracao.AliquotaPIS : 0.65m;
            impostoNota.ValorPIS = valorLiquido * impostoNota.AliquotaPIS / 100;

            // COFINS (3% sobre faturamento)
            impostoNota.AliquotaCOFINS = configuracao.AliquotaCOFINS > 0 ? configuracao.AliquotaCOFINS : 3.00m;
            impostoNota.ValorCOFINS = valorLiquido * impostoNota.AliquotaCOFINS / 100;

            // ISS (2% a 5% conforme município)
            impostoNota.AliquotaISS = configuracao.AliquotaISS > 0 ? configuracao.AliquotaISS : 2.00m;
            impostoNota.ValorISS = valorLiquido * impostoNota.AliquotaISS / 100;
            impostoNota.ISSRetido = configuracao.ISS_Retido;

            // IR (4,8% sobre 32% da receita = 1,536% sobre faturamento)
            impostoNota.AliquotaIR = configuracao.AliquotaIR > 0 ? configuracao.AliquotaIR : 1.536m;
            impostoNota.ValorIR = valorLiquido * impostoNota.AliquotaIR / 100;

            // CSLL (2,88% sobre 32% da receita = 0,9216% sobre faturamento)
            impostoNota.AliquotaCSLL = configuracao.AliquotaCSLL > 0 ? configuracao.AliquotaCSLL : 0.9216m;
            impostoNota.ValorCSLL = valorLiquido * impostoNota.AliquotaCSLL / 100;

            // INSS (se houver retenção)
            if (configuracao.RetemINSS)
            {
                impostoNota.AliquotaINSS = configuracao.AliquotaINSS > 0 ? configuracao.AliquotaINSS : 11.00m;
                impostoNota.ValorINSS = valorLiquido * impostoNota.AliquotaINSS / 100;
                impostoNota.INSSRetido = true;
            }

            impostoNota.CodigoServicoMunicipal = configuracao.CodigoServico;

            _logger.LogDebug("Lucro Presumido - Total de impostos: {TotalImpostos:C}", impostoNota.TotalImpostos);
        }

        /// <summary>
        /// Calcula impostos para Lucro Real
        /// </summary>
        private void CalcularImpostosLucroReal(ImpostoNota impostoNota, ConfiguracaoFiscal configuracao)
        {
            var valorLiquido = impostoNota.ValorLiquido;

            // PIS (1,65% sobre faturamento)
            impostoNota.AliquotaPIS = configuracao.AliquotaPIS > 0 ? configuracao.AliquotaPIS : 1.65m;
            impostoNota.ValorPIS = valorLiquido * impostoNota.AliquotaPIS / 100;

            // COFINS (7,6% sobre faturamento)
            impostoNota.AliquotaCOFINS = configuracao.AliquotaCOFINS > 0 ? configuracao.AliquotaCOFINS : 7.60m;
            impostoNota.ValorCOFINS = valorLiquido * impostoNota.AliquotaCOFINS / 100;

            // ISS (2% a 5% conforme município)
            impostoNota.AliquotaISS = configuracao.AliquotaISS > 0 ? configuracao.AliquotaISS : 2.00m;
            impostoNota.ValorISS = valorLiquido * impostoNota.AliquotaISS / 100;
            impostoNota.ISSRetido = configuracao.ISS_Retido;

            // IR (15% sobre lucro real + 10% adicional sobre o que exceder R$ 20.000/mês)
            impostoNota.AliquotaIR = configuracao.AliquotaIR > 0 ? configuracao.AliquotaIR : 15.00m;
            impostoNota.ValorIR = valorLiquido * impostoNota.AliquotaIR / 100;

            // CSLL (9% sobre lucro real)
            impostoNota.AliquotaCSLL = configuracao.AliquotaCSLL > 0 ? configuracao.AliquotaCSLL : 9.00m;
            impostoNota.ValorCSLL = valorLiquido * impostoNota.AliquotaCSLL / 100;

            // INSS (se houver retenção)
            if (configuracao.RetemINSS)
            {
                impostoNota.AliquotaINSS = configuracao.AliquotaINSS > 0 ? configuracao.AliquotaINSS : 11.00m;
                impostoNota.ValorINSS = valorLiquido * impostoNota.AliquotaINSS / 100;
                impostoNota.INSSRetido = true;
            }

            impostoNota.CodigoServicoMunicipal = configuracao.CodigoServico;

            _logger.LogDebug("Lucro Real - Total de impostos: {TotalImpostos:C}", impostoNota.TotalImpostos);
        }

        /// <summary>
        /// Calcula impostos para MEI (Microempreendedor Individual)
        /// </summary>
        private void CalcularImpostosMEI(ImpostoNota impostoNota, ConfiguracaoFiscal configuracao)
        {
            // MEI paga valor fixo mensal, não sobre cada nota
            // Impostos são zerados na nota, pois são pagos via DAS-MEI fixo

            impostoNota.AliquotaPIS = 0;
            impostoNota.ValorPIS = 0;

            impostoNota.AliquotaCOFINS = 0;
            impostoNota.ValorCOFINS = 0;

            impostoNota.AliquotaIR = 0;
            impostoNota.ValorIR = 0;

            impostoNota.AliquotaCSLL = 0;
            impostoNota.ValorCSLL = 0;

            // ISS fixo mensal (R$ 5,00)
            impostoNota.AliquotaISS = 0;
            impostoNota.ValorISS = 0;
            impostoNota.ISSRetido = false;

            impostoNota.AliquotaINSS = 0;
            impostoNota.ValorINSS = 0;
            impostoNota.INSSRetido = false;

            impostoNota.CodigoServicoMunicipal = configuracao.CodigoServico;

            _logger.LogDebug("MEI - Impostos fixos mensais, nota sem cálculo de impostos");
        }
    }
}
