using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.Fiscal
{
    /// <summary>
    /// Provedor de integração contábil
    /// </summary>
    public enum ProvedorIntegracao
    {
        Nenhum = 0,
        Dominio = 1,
        ContaAzul = 2,
        Omie = 3
    }

    /// <summary>
    /// Configuração de integração com sistemas contábeis
    /// </summary>
    public class ConfiguracaoIntegracao : BaseEntity
    {
        public Guid ClinicaId { get; set; }
        public ProvedorIntegracao Provedor { get; set; }
        public bool Ativa { get; set; }
        
        // Credenciais (armazenadas de forma segura)
        public string? ApiUrl { get; set; }
        public string? ApiKey { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiraEm { get; set; }
        
        // Configurações específicas
        public string? CodigoEmpresa { get; set; }
        public string? CodigoFilial { get; set; }
        public string? ConfiguracoesAdicionais { get; set; } // JSON para configs específicas
        
        // Controle
        public DateTime? UltimaSincronizacao { get; set; }
        public string? UltimoErro { get; set; }
        public int TentativasErro { get; set; }
        
        // Navigation
        public virtual Clinic? Clinica { get; set; }
    }
}
