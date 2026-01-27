using System;

namespace MedicSoft.Analytics.Models
{
    /// <summary>
    /// Medical professional dimension for analytics
    /// </summary>
    public class DimensaoMedico
    {
        public Guid MedicoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string CRM { get; set; } = string.Empty;
        public Guid? EspecialidadeId { get; set; }
        public string EspecialidadeNome { get; set; } = string.Empty;
        public bool Ativo { get; set; }
        public DateTime DataCadastro { get; set; }
    }
}
