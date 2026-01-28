using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Regime tributário da clínica
    /// </summary>
    public enum RegimeTributarioEnum
    {
        SimplesNacional = 1,
        LucroPresumido = 2,
        LucroReal = 3,
        MEI = 4
    }

    /// <summary>
    /// Anexo do Simples Nacional para serviços de saúde
    /// </summary>
    public enum AnexoSimplesNacional
    {
        AnexoIII = 3,  // Serviços (FatorR >= 28%)
        AnexoV = 5     // Serviços (FatorR < 28%)
    }

    /// <summary>
    /// Configuração fiscal e tributária da clínica
    /// </summary>
    public class ConfiguracaoFiscal : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        
        // Regime tributário
        public RegimeTributarioEnum Regime { get; set; }
        public DateTime VigenciaInicio { get; set; }
        public DateTime? VigenciaFim { get; set; }
        
        // Simples Nacional
        public bool OptanteSimplesNacional { get; set; }
        public AnexoSimplesNacional? AnexoSimples { get; set; }
        public decimal? FatorR { get; set; } // Para Anexo III/V
        
        // Alíquotas (quando não Simples)
        public decimal AliquotaISS { get; set; } // %
        public decimal AliquotaPIS { get; set; }
        public decimal AliquotaCOFINS { get; set; }
        public decimal AliquotaIR { get; set; }
        public decimal AliquotaCSLL { get; set; }
        
        // INSS
        public bool RetemINSS { get; set; }
        public decimal AliquotaINSS { get; set; }
        
        // Configurações específicas
        public string CodigoServico { get; set; } = null!; // LC 116/2003
        public string CNAE { get; set; } = null!;
        public string? InscricaoMunicipal { get; set; }
        public bool ISS_Retido { get; set; }

        // Navigation
        public virtual Clinic? Clinica { get; set; }
    }
}
