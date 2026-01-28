using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Impostos calculados para uma nota fiscal
    /// </summary>
    public class ImpostoNota : BaseEntity
    {
        public Guid NotaFiscalId { get; set; }
        
        // Valores base
        public decimal ValorBruto { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorLiquido => ValorBruto - ValorDesconto;
        
        // Tributos Federais
        public decimal AliquotaPIS { get; set; }
        public decimal ValorPIS { get; set; }
        
        public decimal AliquotaCOFINS { get; set; }
        public decimal ValorCOFINS { get; set; }
        
        public decimal AliquotaIR { get; set; }
        public decimal ValorIR { get; set; }
        
        public decimal AliquotaCSLL { get; set; }
        public decimal ValorCSLL { get; set; }
        
        // Tributo Municipal
        public decimal AliquotaISS { get; set; }
        public decimal ValorISS { get; set; }
        public bool ISSRetido { get; set; }
        public string? CodigoServicoMunicipal { get; set; }
        
        // INSS
        public decimal AliquotaINSS { get; set; }
        public decimal ValorINSS { get; set; }
        public bool INSSRetido { get; set; }
        
        // Totalizadores
        public decimal TotalImpostos => ValorPIS + ValorCOFINS + ValorIR + ValorCSLL + ValorISS + ValorINSS;
        public decimal ValorLiquidoTributos => ValorLiquido - TotalImpostos;
        public decimal CargaTributaria => ValorLiquido > 0 ? (TotalImpostos / ValorLiquido * 100) : 0;
        
        // Metadados
        public DateTime DataCalculo { get; set; }
        public string RegimeTributario { get; set; } = null!;

        // Navigation
        public virtual ElectronicInvoice? NotaFiscal { get; set; }
    }
}
