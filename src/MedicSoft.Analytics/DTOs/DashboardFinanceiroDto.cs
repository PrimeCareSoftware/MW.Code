using System;
using System.Collections.Generic;

namespace MedicSoft.Analytics.DTOs
{
    public class DashboardFinanceiroDto
    {
        public PeriodoDto Periodo { get; set; }
        
        // Receitas
        public decimal ReceitaTotal { get; set; }
        public decimal ReceitaRecebida { get; set; }
        public decimal ReceitaPendente { get; set; }
        public decimal ReceitaAtrasada { get; set; }
        
        // Despesas
        public decimal DespesaTotal { get; set; }
        public decimal DespesaPaga { get; set; }
        public decimal DespesaPendente { get; set; }
        
        // Resultado
        public decimal LucroBruto { get; set; }
        public decimal MargemLucro { get; set; }
        
        // Análises
        public List<ReceitaPorConvenioDto> ReceitaPorConvenio { get; set; }
        public List<ReceitaPorMedicoDto> ReceitaPorMedico { get; set; }
        public List<ReceitaPorFormaPagamentoDto> ReceitaPorFormaPagamento { get; set; }
        public List<DespesaPorCategoriaDto> DespesaPorCategoria { get; set; }
        
        // Ticket médio
        public decimal TicketMedio { get; set; }
        
        // Projeções
        public decimal ProjecaoMesAtual { get; set; }
        
        // Fluxo de caixa
        public List<FluxoCaixaDiarioDto> FluxoCaixaDiario { get; set; }
    }

    public class ReceitaPorConvenioDto
    {
        public string NomeConvenio { get; set; }
        public decimal Total { get; set; }
        public int QuantidadeConsultas { get; set; }
        public decimal TicketMedio { get; set; }
    }

    public class ReceitaPorMedicoDto
    {
        public string NomeMedico { get; set; }
        public decimal Total { get; set; }
        public int QuantidadeConsultas { get; set; }
    }

    public class ReceitaPorFormaPagamentoDto
    {
        public string FormaPagamento { get; set; }
        public decimal Total { get; set; }
        public decimal Percentual { get; set; }
    }

    public class DespesaPorCategoriaDto
    {
        public string Categoria { get; set; }
        public decimal Total { get; set; }
        public decimal Percentual { get; set; }
    }

    public class FluxoCaixaDiarioDto
    {
        public DateTime Data { get; set; }
        public decimal Entradas { get; set; }
        public decimal Saidas { get; set; }
        public decimal Saldo { get; set; }
    }
}
