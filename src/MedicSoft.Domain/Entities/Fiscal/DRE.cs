using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Demonstração do Resultado do Exercício (DRE)
    /// Relatório contábil que mostra o resultado econômico de um período
    /// </summary>
    public class DRE : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public DateTime DataGeracao { get; set; }
        
        // Receitas
        public decimal ReceitaBruta { get; set; }
        public decimal Deducoes { get; set; }
        public decimal ReceitaLiquida { get; set; }
        
        // Custos
        public decimal CustoServicos { get; set; }
        
        // Lucro Bruto
        public decimal LucroBruto { get; set; }
        public decimal MargemBruta { get; set; }
        
        // Despesas
        public decimal DespesasOperacionais { get; set; }
        public decimal DespesasAdministrativas { get; set; }
        public decimal DespesasComerciais { get; set; }
        
        // EBITDA (Earnings Before Interest, Taxes, Depreciation and Amortization)
        public decimal EBITDA { get; set; }
        public decimal MargemEBITDA { get; set; }
        
        // Depreciação
        public decimal DepreciacaoAmortizacao { get; set; }
        
        // EBIT (Earnings Before Interest and Taxes)
        public decimal EBIT { get; set; }
        
        // Resultado Financeiro
        public decimal ReceitasFinanceiras { get; set; }
        public decimal DespesasFinanceiras { get; set; }
        public decimal ResultadoFinanceiro { get; set; }
        
        // Lucro
        public decimal LucroAntesIR { get; set; }
        public decimal ImpostoRenda { get; set; }
        public decimal CSLL { get; set; }
        public decimal LucroLiquido { get; set; }
        public decimal MargemLiquida { get; set; }
        
        // Navigation
        public virtual Clinic? Clinica { get; set; }

        /// <summary>
        /// Construtor protegido para Entity Framework
        /// </summary>
        protected DRE() : base()
        {
        }

        /// <summary>
        /// Construtor para uso em serviços
        /// </summary>
        public DRE(string tenantId) : base(tenantId)
        {
            DataGeracao = DateTime.UtcNow;
        }
    }
}
