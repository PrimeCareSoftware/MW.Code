using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Status da apuração de impostos
    /// </summary>
    public enum StatusApuracao
    {
        EmAberto = 1,
        Apurado = 2,
        Pago = 3,
        Parcelado = 4,
        Atrasado = 5
    }

    /// <summary>
    /// Apuração mensal de impostos da clínica
    /// </summary>
    public class ApuracaoImpostos : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public int Mes { get; set; }
        public int Ano { get; set; }
        public DateTime DataApuracao { get; set; }
        
        // Faturamento
        public decimal FaturamentoBruto { get; set; }
        public decimal Deducoes { get; set; }
        public decimal FaturamentoLiquido => FaturamentoBruto - Deducoes;
        
        // Impostos apurados
        public decimal TotalPIS { get; set; }
        public decimal TotalCOFINS { get; set; }
        public decimal TotalIR { get; set; }
        public decimal TotalCSLL { get; set; }
        public decimal TotalISS { get; set; }
        public decimal TotalINSS { get; set; }
        
        // Simples Nacional
        public decimal? ReceitaBruta12Meses { get; set; }
        public decimal? AliquotaEfetiva { get; set; }
        public decimal? ValorDAS { get; set; }
        
        // Status
        public StatusApuracao Status { get; set; }
        public DateTime? DataPagamento { get; set; }
        public string? ComprovantesPagamento { get; set; }
        
        // Navigation
        public virtual Clinic? Clinica { get; set; }
        public virtual ICollection<ElectronicInvoice> NotasIncluidas { get; set; } = new List<ElectronicInvoice>();
        
        // Constructors
        protected ApuracaoImpostos() : base() { }
        
        public ApuracaoImpostos(string tenantId) : base(tenantId) { }
    }
}
