namespace MedicSoft.Analytics.DTOs
{
    // Dashboard de Qualidade DTOs
    
    public class DashboardQualidadeDto
    {
        public PeriodoDto Periodo { get; set; } = new();
        
        // KPIs principais
        public decimal NpsMedio { get; set; }
        public int TotalAvaliacoes { get; set; }
        public decimal TaxaSatisfacao { get; set; } // %
        public decimal TaxaRecomendacao { get; set; } // %
        
        // Distribuição NPS
        public DistribuicaoNpsDto DistribuicaoNps { get; set; } = new();
        
        // Avaliações por médico
        public List<AvaliacaoMedicoDto> AvaliacoesMedicos { get; set; } = new();
        
        // Avaliações por especialidade
        public List<AvaliacaoEspecialidadeDto> AvaliacoesEspecialidades { get; set; } = new();
        
        // Comentários recentes (top positivos e negativos)
        public List<ComentarioAvaliacaoDto> ComentariosPositivos { get; set; } = new();
        public List<ComentarioAvaliacaoDto> ComentariosNegativos { get; set; } = new();
        
        // Tendência NPS
        public List<TendenciaNpsDto> TendenciaNps { get; set; } = new();
        
        // Principais elogios e críticas
        public List<PalavraChaveDto> PrincipaisElogios { get; set; } = new();
        public List<PalavraChaveDto> PrincipaisCriticas { get; set; } = new();
    }
    
    public class DistribuicaoNpsDto
    {
        public int Promotores { get; set; } // 9-10
        public int Neutros { get; set; } // 7-8
        public int Detratores { get; set; } // 0-6
        
        public decimal PercentualPromotores { get; set; }
        public decimal PercentualNeutros { get; set; }
        public decimal PercentualDetratores { get; set; }
    }
    
    public class AvaliacaoMedicoDto
    {
        public string NomeMedico { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public decimal NpsMedio { get; set; }
        public int TotalAvaliacoes { get; set; }
        public decimal TaxaSatisfacao { get; set; }
    }
    
    public class AvaliacaoEspecialidadeDto
    {
        public string Especialidade { get; set; } = string.Empty;
        public decimal NpsMedio { get; set; }
        public int TotalAvaliacoes { get; set; }
        public decimal TaxaSatisfacao { get; set; }
    }
    
    public class ComentarioAvaliacaoDto
    {
        public Guid Id { get; set; }
        public string PacienteNome { get; set; } = string.Empty;
        public string MedicoNome { get; set; } = string.Empty;
        public DateTime DataAvaliacao { get; set; }
        public int Nota { get; set; }
        public string Comentario { get; set; } = string.Empty;
    }
    
    public class TendenciaNpsDto
    {
        public string Mes { get; set; } = string.Empty;
        public decimal NpsMedio { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
    
    public class PalavraChaveDto
    {
        public string Palavra { get; set; } = string.Empty;
        public int Frequencia { get; set; }
    }
}
