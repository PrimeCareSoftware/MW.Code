using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Analytics.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Analytics.Services
{
    public class DashboardFinanceiroService : IDashboardFinanceiroService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<DashboardFinanceiroService> _logger;

        public DashboardFinanceiroService(
            MedicSoftDbContext context,
            ILogger<DashboardFinanceiroService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardFinanceiroDto> GetDashboardAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId)
        {
            try
            {
                _logger.LogInformation("Gerando dashboard financeiro para período {Inicio} até {Fim}", inicio, fim);

                // Buscar receitas (pagamentos)
                var receitas = await _context.Payments
                    .Where(p => p.TenantId == tenantId 
                        && p.PaymentDate >= inicio 
                        && p.PaymentDate <= fim)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.HealthInsurancePlan)
                    .Include(p => p.Appointment)
                        .ThenInclude(a => a.Professional)
                    .ToListAsync();

                // Buscar despesas
                var despesas = await _context.Expenses
                    .Where(e => e.TenantId == tenantId 
                        && e.DueDate >= inicio 
                        && e.DueDate <= fim)
                    .ToListAsync();

                var receitaTotal = receitas.Sum(r => r.Amount);
                var receitaRecebida = receitas.Where(r => r.Status == PaymentStatus.Paid).Sum(r => r.Amount);
                var receitaPendente = receitas.Where(r => r.Status == PaymentStatus.Pending).Sum(r => r.Amount);
                var receitaAtrasada = receitas
                    .Where(r => r.PaymentDate < DateTime.Now && r.Status == PaymentStatus.Pending)
                    .Sum(r => r.Amount);

                var despesaTotal = despesas.Sum(d => d.Amount);
                var despesaPaga = despesas.Where(d => d.Status == ExpenseStatus.Paid).Sum(d => d.Amount);
                var despesaPendente = despesas.Where(d => d.Status == ExpenseStatus.Pending).Sum(d => d.Amount);

                var lucroBruto = receitaRecebida - despesaPaga;
                var margemLucro = receitaRecebida > 0 ? (lucroBruto / receitaRecebida) * 100 : 0;

                var dashboard = new DashboardFinanceiroDto
                {
                    Periodo = new PeriodoDto { Inicio = inicio, Fim = fim },
                    
                    ReceitaTotal = receitaTotal,
                    ReceitaRecebida = receitaRecebida,
                    ReceitaPendente = receitaPendente,
                    ReceitaAtrasada = receitaAtrasada,
                    
                    DespesaTotal = despesaTotal,
                    DespesaPaga = despesaPaga,
                    DespesaPendente = despesaPendente,
                    
                    LucroBruto = lucroBruto,
                    MargemLucro = margemLucro,
                    
                    ReceitaPorConvenio = GetReceitaPorConvenio(receitas),
                    ReceitaPorMedico = GetReceitaPorMedico(receitas),
                    ReceitaPorFormaPagamento = GetReceitaPorFormaPagamento(receitas),
                    DespesaPorCategoria = GetDespesaPorCategoria(despesas),
                    
                    TicketMedio = receitas.Any() ? receitas.Average(r => r.Amount) : 0,
                    
                    ProjecaoMesAtual = await ProjetarReceitaMesAsync(DateTime.Now, tenantId),
                    
                    FluxoCaixaDiario = GetFluxoCaixaDiario(receitas, despesas, inicio, fim)
                };

                _logger.LogInformation("Dashboard financeiro gerado com sucesso");
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar dashboard financeiro");
                throw;
            }
        }

        public async Task<decimal> ProjetarReceitaMesAsync(DateTime mes, string tenantId)
        {
            try
            {
                var diaAtual = DateTime.Now.Day;
                var diasNoMes = DateTime.DaysInMonth(mes.Year, mes.Month);
                
                var inicioMes = new DateTime(mes.Year, mes.Month, 1);
                var hoje = DateTime.Now;

                var receitaAteAgora = await _context.Payments
                    .Where(p => p.TenantId == tenantId
                        && p.PaymentDate >= inicioMes
                        && p.PaymentDate <= hoje
                        && p.Status == PaymentStatus.Paid)
                    .SumAsync(p => p.Amount);

                if (diaAtual == 0)
                    return 0;

                // Projeção linear simples
                var mediaDiaria = receitaAteAgora / diaAtual;
                var projecao = mediaDiaria * diasNoMes;

                return projecao;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao projetar receita do mês");
                return 0;
            }
        }

        private List<ReceitaPorConvenioDto> GetReceitaPorConvenio(List<Payment> receitas)
        {
            var receitasComConvenio = receitas
                .Where(r => r.Appointment?.HealthInsurancePlan != null)
                .ToList();

            var grouped = receitasComConvenio
                .GroupBy(r => new 
                { 
                    ConvenioId = r.Appointment!.HealthInsurancePlan!.Id,
                    Nome = r.Appointment.HealthInsurancePlan.PlanName 
                })
                .Select(g => new ReceitaPorConvenioDto
                {
                    NomeConvenio = g.Key.Nome,
                    Total = g.Sum(r => r.Amount),
                    QuantidadeConsultas = g.Count(),
                    TicketMedio = g.Average(r => r.Amount)
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            // Adicionar particular
            var receitasParticular = receitas
                .Where(r => r.Appointment?.HealthInsurancePlan == null)
                .ToList();

            if (receitasParticular.Any())
            {
                grouped.Insert(0, new ReceitaPorConvenioDto
                {
                    NomeConvenio = "Particular",
                    Total = receitasParticular.Sum(r => r.Amount),
                    QuantidadeConsultas = receitasParticular.Count,
                    TicketMedio = receitasParticular.Average(r => r.Amount)
                });
            }

            return grouped;
        }

        private List<ReceitaPorMedicoDto> GetReceitaPorMedico(List<Payment> receitas)
        {
            var receitasComMedico = receitas
                .Where(r => r.Appointment?.Professional != null)
                .ToList();

            var grouped = receitasComMedico
                .GroupBy(r => new 
                { 
                    MedicoId = r.Appointment!.ProfessionalId!.Value,
                    Nome = r.Appointment.Professional!.FullName 
                })
                .Select(g => new ReceitaPorMedicoDto
                {
                    NomeMedico = g.Key.Nome,
                    Total = g.Sum(r => r.Amount),
                    QuantidadeConsultas = g.Count()
                })
                .OrderByDescending(m => m.Total)
                .ToList();

            return grouped;
        }

        private List<ReceitaPorFormaPagamentoDto> GetReceitaPorFormaPagamento(List<Payment> receitas)
        {
            var total = receitas.Sum(r => r.Amount);

            var grouped = receitas
                .GroupBy(r => r.Method)
                .Select(g => new ReceitaPorFormaPagamentoDto
                {
                    FormaPagamento = GetFormaPagamentoNome(g.Key),
                    Total = g.Sum(r => r.Amount),
                    Percentual = total > 0 ? (g.Sum(r => r.Amount) / total) * 100 : 0
                })
                .OrderByDescending(f => f.Total)
                .ToList();

            return grouped;
        }

        private List<DespesaPorCategoriaDto> GetDespesaPorCategoria(List<Expense> despesas)
        {
            var total = despesas.Sum(d => d.Amount);

            var grouped = despesas
                .GroupBy(d => d.Category)
                .Select(g => new DespesaPorCategoriaDto
                {
                    Categoria = GetCategoriaDespesaNome(g.Key),
                    Total = g.Sum(d => d.Amount),
                    Percentual = total > 0 ? (g.Sum(d => d.Amount) / total) * 100 : 0
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            return grouped;
        }

        private List<FluxoCaixaDiarioDto> GetFluxoCaixaDiario(
            List<Payment> receitas, 
            List<Expense> despesas, 
            DateTime inicio, 
            DateTime fim)
        {
            var fluxo = new List<FluxoCaixaDiarioDto>();
            var saldoAcumulado = 0m;

            var dataAtual = inicio.Date;
            while (dataAtual <= fim.Date)
            {
                var entradas = receitas
                    .Where(r => r.PaymentDate.Date == dataAtual && r.Status == PaymentStatus.Paid)
                    .Sum(r => r.Amount);

                var saidas = despesas
                    .Where(d => d.PaidDate.HasValue && d.PaidDate.Value.Date == dataAtual)
                    .Sum(d => d.Amount);

                saldoAcumulado += entradas - saidas;

                fluxo.Add(new FluxoCaixaDiarioDto
                {
                    Data = dataAtual,
                    Entradas = entradas,
                    Saidas = saidas,
                    Saldo = saldoAcumulado
                });

                dataAtual = dataAtual.AddDays(1);
            }

            return fluxo;
        }

        private string GetFormaPagamentoNome(PaymentMethod method)
        {
            return method switch
            {
                PaymentMethod.Cash => "Dinheiro",
                PaymentMethod.CreditCard => "Cartão de Crédito",
                PaymentMethod.DebitCard => "Cartão de Débito",
                PaymentMethod.Pix => "PIX",
                PaymentMethod.BankTransfer => "Transferência Bancária",
                PaymentMethod.Check => "Cheque",
                _ => "Outro"
            };
        }

        private string GetCategoriaDespesaNome(ExpenseCategory category)
        {
            return category switch
            {
                ExpenseCategory.Rent => "Aluguel",
                ExpenseCategory.Utilities => "Utilidades",
                ExpenseCategory.Supplies => "Materiais",
                ExpenseCategory.Equipment => "Equipamentos",
                ExpenseCategory.Maintenance => "Manutenção",
                ExpenseCategory.Marketing => "Marketing",
                ExpenseCategory.Software => "Software",
                ExpenseCategory.Salary => "Salários",
                ExpenseCategory.Taxes => "Impostos",
                ExpenseCategory.Insurance => "Seguros",
                ExpenseCategory.ProfessionalServices => "Serviços Profissionais",
                ExpenseCategory.Transportation => "Transporte",
                ExpenseCategory.Training => "Treinamento",
                ExpenseCategory.Other => "Outros",
                _ => "Outros"
            };
        }
    }
}
