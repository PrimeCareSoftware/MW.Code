using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Application.Services.Fiscal
{
    /// <summary>
    /// Serviço para geração de DRE (Demonstração do Resultado do Exercício)
    /// </summary>
    public class DREService : IDREService
    {
        private readonly IDRERepository _dreRepository;
        private readonly ILancamentoContabilRepository _lancamentoRepository;
        private readonly IPlanoContasRepository _planoContasRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<DREService> _logger;

        public DREService(
            IDRERepository dreRepository,
            ILancamentoContabilRepository lancamentoRepository,
            IPlanoContasRepository planoContasRepository,
            IClinicRepository clinicRepository,
            ILogger<DREService> logger)
        {
            _dreRepository = dreRepository;
            _lancamentoRepository = lancamentoRepository;
            _planoContasRepository = planoContasRepository;
            _clinicRepository = clinicRepository;
            _logger = logger;
        }

        public async Task<DRE> GerarDREAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            _logger.LogInformation(
                "Gerando DRE para clínica {ClinicaId}, período {Inicio} a {Fim}",
                clinicaId, dataInicio, dataFim);

            // Valida clínica
            var clinica = await _clinicRepository.GetByIdAsync(clinicaId, tenantId);
            if (clinica == null)
            {
                throw new InvalidOperationException($"Clínica {clinicaId} não encontrada");
            }

            // Verifica se já existe DRE para o período
            var dreExistente = await _dreRepository.GetByPeriodoAsync(clinicaId, dataInicio, dataFim, tenantId);
            if (dreExistente != null)
            {
                _logger.LogInformation(
                    "DRE já existe para o período. Recalculando...");
                
                await RecalcularDREAsync(dreExistente, dataInicio, dataFim, tenantId);
                await _dreRepository.UpdateAsync(dreExistente);
                return dreExistente;
            }

            // Cria nova DRE
            var dre = new DRE(tenantId)
            {
                ClinicaId = clinicaId,
                PeriodoInicio = dataInicio.Date,
                PeriodoFim = dataFim.Date
            };

            await RecalcularDREAsync(dre, dataInicio, dataFim, tenantId);

            // Salva DRE
            var resultado = await _dreRepository.AddAsync(dre);

            _logger.LogInformation(
                "DRE gerada com sucesso. ID: {DreId}, Lucro Líquido: {LucroLiquido:C}",
                resultado.Id, resultado.LucroLiquido);

            return resultado;
        }

        public async Task<DRE?> ObterDREAsync(Guid id, string tenantId)
        {
            return await _dreRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<DRE?> ObterDREPorPeriodoAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            return await _dreRepository.GetByPeriodoAsync(clinicaId, dataInicio, dataFim, tenantId);
        }

        /// <summary>
        /// Recalcula todos os valores da DRE com base nos lançamentos contábeis
        /// </summary>
        private async Task RecalcularDREAsync(DRE dre, DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            // 1. Receita Bruta (Contas de Receita)
            dre.ReceitaBruta = await CalcularSaldoPorTipoAsync(
                dre.ClinicaId, TipoConta.Receita, dataInicio, dataFim, tenantId);

            // 2. Deduções (estimativa baseada em impostos - pode ser refinado)
            // Para simplificar, vamos considerar as deduções como zero por enquanto
            // Em uma implementação completa, deveria haver contas específicas para deduções
            dre.Deducoes = 0;

            // 3. Receita Líquida
            dre.ReceitaLiquida = dre.ReceitaBruta - dre.Deducoes;

            // 4. Custo dos Serviços (Contas de Custo)
            dre.CustoServicos = await CalcularSaldoPorTipoAsync(
                dre.ClinicaId, TipoConta.Custos, dataInicio, dataFim, tenantId);

            // 5. Lucro Bruto
            dre.LucroBruto = dre.ReceitaLiquida - dre.CustoServicos;
            dre.MargemBruta = dre.ReceitaLiquida > 0 
                ? (dre.LucroBruto / dre.ReceitaLiquida * 100) 
                : 0;

            // 6. Despesas Operacionais (Contas de Despesa)
            var despesasTotal = await CalcularSaldoPorTipoAsync(
                dre.ClinicaId, TipoConta.Despesa, dataInicio, dataFim, tenantId);
            
            // Para simplificar, vamos distribuir as despesas em categorias proporcionalmente
            // Em uma implementação completa, cada despesa deveria ter classificação específica
            dre.DespesasOperacionais = despesasTotal;
            dre.DespesasAdministrativas = despesasTotal * 0.6m; // 60% administrativas
            dre.DespesasComerciais = despesasTotal * 0.4m; // 40% comerciais

            // 7. EBITDA
            dre.EBITDA = dre.LucroBruto - dre.DespesasOperacionais;
            dre.MargemEBITDA = dre.ReceitaLiquida > 0 
                ? (dre.EBITDA / dre.ReceitaLiquida * 100) 
                : 0;

            // 8. Depreciação e Amortização (estimativa)
            // Em uma implementação completa, deveria haver contas específicas
            dre.DepreciacaoAmortizacao = 0;

            // 9. EBIT
            dre.EBIT = dre.EBITDA - dre.DepreciacaoAmortizacao;

            // 10. Resultado Financeiro
            // Em uma implementação completa, deveria haver contas específicas de receitas/despesas financeiras
            dre.ReceitasFinanceiras = 0;
            dre.DespesasFinanceiras = 0;
            dre.ResultadoFinanceiro = dre.ReceitasFinanceiras - dre.DespesasFinanceiras;

            // 11. Lucro Antes do IR
            dre.LucroAntesIR = dre.EBIT + dre.ResultadoFinanceiro;

            // 12. IR e CSLL (estimativa)
            // Em uma implementação completa, deveria buscar dos impostos calculados
            dre.ImpostoRenda = 0;
            dre.CSLL = 0;

            // 13. Lucro Líquido
            dre.LucroLiquido = dre.LucroAntesIR - dre.ImpostoRenda - dre.CSLL;
            dre.MargemLiquida = dre.ReceitaLiquida > 0 
                ? (dre.LucroLiquido / dre.ReceitaLiquida * 100) 
                : 0;

            dre.DataGeracao = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcula o saldo total de um tipo de conta (Receita, Despesa, Custo) em um período
        /// </summary>
        private async Task<decimal> CalcularSaldoPorTipoAsync(
            Guid clinicaId,
            TipoConta tipo,
            DateTime dataInicio,
            DateTime dataFim,
            string tenantId)
        {
            // Busca todas as contas do tipo
            var contas = await _planoContasRepository.GetByTipoAsync(clinicaId, tipo, tenantId);
            
            decimal saldoTotal = 0;

            // Soma o saldo de cada conta
            foreach (var conta in contas.Where(c => c.Analitica)) // Apenas contas analíticas têm lançamentos
            {
                var lancamentos = await _lancamentoRepository.GetByContaAndPeriodoAsync(
                    conta.Id, dataInicio, dataFim, tenantId);

                // Calcula saldo baseado na natureza da conta
                foreach (var lancamento in lancamentos)
                {
                    if (conta.Natureza == NaturezaSaldo.Credora)
                    {
                        // Para contas credoras (Receita), crédito aumenta e débito diminui
                        saldoTotal += lancamento.Tipo == TipoLancamentoContabil.Credito 
                            ? lancamento.Valor 
                            : -lancamento.Valor;
                    }
                    else
                    {
                        // Para contas devedoras (Despesa, Custo), débito aumenta e crédito diminui
                        saldoTotal += lancamento.Tipo == TipoLancamentoContabil.Debito 
                            ? lancamento.Valor 
                            : -lancamento.Valor;
                    }
                }
            }

            return Math.Abs(saldoTotal);
        }
    }
}
