using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Tipo de lançamento contábil
    /// </summary>
    public enum TipoLancamentoContabil
    {
        Debito = 1,
        Credito = 2
    }

    /// <summary>
    /// Origem do lançamento contábil
    /// </summary>
    public enum OrigemLancamento
    {
        Manual = 1,
        NotaFiscal = 2,
        Pagamento = 3,
        Recebimento = 4,
        FechamentoMensal = 5,
        Ajuste = 6
    }

    /// <summary>
    /// Lançamento contábil individual
    /// </summary>
    public class LancamentoContabil : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public Guid PlanoContasId { get; set; }
        public DateTime DataLancamento { get; set; }
        public TipoLancamentoContabil Tipo { get; set; }
        public decimal Valor { get; set; }
        public string Historico { get; set; } = null!;
        public OrigemLancamento Origem { get; set; }
        public Guid? DocumentoOrigemId { get; set; } // ID da nota, pagamento, etc
        public string? NumeroDocumento { get; set; }
        public Guid? LoteId { get; set; } // Para agrupar débitos e créditos de uma operação
        
        // Navigation
        public virtual Clinic? Clinica { get; set; }
        public virtual PlanoContas? Conta { get; set; }
    }
}
