using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Tipo de conta no plano de contas
    /// </summary>
    public enum TipoConta
    {
        Ativo = 1,
        Passivo = 2,
        PatrimonioLiquido = 3,
        Receita = 4,
        Despesa = 5,
        Custos = 6
    }

    /// <summary>
    /// Natureza do saldo da conta
    /// </summary>
    public enum NaturezaSaldo
    {
        Devedora = 1,
        Credora = 2
    }

    /// <summary>
    /// Plano de contas contábil
    /// </summary>
    public class PlanoContas : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public string Codigo { get; set; } = null!; // Ex: 1.1.01.001
        public string Nome { get; set; } = null!;
        public TipoConta Tipo { get; set; }
        public NaturezaSaldo Natureza { get; set; }
        public Guid? ContaPaiId { get; set; }
        public bool Analitica { get; set; } // true = aceita lançamentos, false = apenas agrupadora
        public bool Ativa { get; set; }
        public int Nivel { get; set; }
        public string? Observacoes { get; set; }

        // Navigation
        public virtual Clinic? Clinica { get; set; }
        public virtual PlanoContas? ContaPai { get; set; }
        public virtual ICollection<PlanoContas> SubContas { get; set; } = new List<PlanoContas>();
        public virtual ICollection<LancamentoContabil> Lancamentos { get; set; } = new List<LancamentoContabil>();
    }
}
