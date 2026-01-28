using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Balanço Patrimonial
    /// Relatório contábil que mostra a situação patrimonial e financeira em um momento específico
    /// </summary>
    public class BalancoPatrimonial : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public DateTime DataReferencia { get; set; }
        public DateTime DataGeracao { get; set; }
        
        // ATIVO CIRCULANTE
        public decimal AtivoCirculante { get; set; }
        public decimal DisponibilidadesCaixa { get; set; }
        public decimal ContasReceber { get; set; }
        public decimal Estoques { get; set; }
        public decimal OutrosAtivosCirculantes { get; set; }
        
        // ATIVO NÃO CIRCULANTE
        public decimal AtivoNaoCirculante { get; set; }
        public decimal AtivoRealizavelLongoPrazo { get; set; }
        public decimal Investimentos { get; set; }
        public decimal Imobilizado { get; set; }
        public decimal DepreciacaoAcumulada { get; set; }
        public decimal Intangivel { get; set; }
        public decimal AmortizacaoAcumulada { get; set; }
        
        // TOTAL DO ATIVO
        public decimal TotalAtivo { get; set; }
        
        // PASSIVO CIRCULANTE
        public decimal PassivoCirculante { get; set; }
        public decimal FornecedoresPagar { get; set; }
        public decimal ObrigacoesTrabalhistas { get; set; }
        public decimal ObrigacoesTributarias { get; set; }
        public decimal EmprestimosFinanciamentos { get; set; }
        public decimal OutrosPassivosCirculantes { get; set; }
        
        // PASSIVO NÃO CIRCULANTE
        public decimal PassivoNaoCirculante { get; set; }
        public decimal EmprestimosLongoPrazo { get; set; }
        public decimal OutrosPassivosNaoCirculantes { get; set; }
        
        // PATRIMÔNIO LÍQUIDO
        public decimal PatrimonioLiquido { get; set; }
        public decimal CapitalSocial { get; set; }
        public decimal ReservasCapital { get; set; }
        public decimal ReservasLucros { get; set; }
        public decimal LucrosAcumulados { get; set; }
        public decimal PrejuizosAcumulados { get; set; }
        
        // TOTAL DO PASSIVO
        public decimal TotalPassivo { get; set; }
        
        // Navigation
        public virtual Clinic? Clinica { get; set; }

        /// <summary>
        /// Construtor protegido para Entity Framework
        /// </summary>
        protected BalancoPatrimonial() : base()
        {
        }

        /// <summary>
        /// Construtor para uso em serviços
        /// </summary>
        public BalancoPatrimonial(string tenantId) : base(tenantId)
        {
            DataGeracao = DateTime.UtcNow;
        }
    }
}
