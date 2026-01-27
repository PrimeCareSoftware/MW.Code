namespace MedicSoft.Analytics.DTOs
{
    // Dashboard Operacional DTOs
    
    public class DashboardOperacionalDto
    {
        public PeriodoDto Periodo { get; set; } = new();
        
        // KPIs principais
        public int TempoMedioEsperaMinutos { get; set; }
        public int TamanhoFilaAtual { get; set; }
        public int PacientesEmAtendimento { get; set; }
        public decimal TaxaAtendimentoNoPrazo { get; set; } // %
        
        // Tempos por etapa
        public List<TempoPorEtapaDto> TemposPorEtapa { get; set; } = new();
        
        // Performance por médico
        public List<PerformanceMedicoDto> PerformanceMedicos { get; set; } = new();
        
        // Distribuição de horários
        public List<DistribuicaoHorarioDto> DistribuicaoHorarios { get; set; } = new();
        
        // Tempo médio por especialidade
        public List<TempoPorEspecialidadeDto> TemposPorEspecialidade { get; set; } = new();
        
        // Tendência de tempo de espera
        public List<TendenciaTempoEsperaDto> TendenciaTempoEspera { get; set; } = new();
    }
    
    public class TempoPorEtapaDto
    {
        public string Etapa { get; set; } = string.Empty;
        public int TempoMedioMinutos { get; set; }
        public int TempoMinimoMinutos { get; set; }
        public int TempoMaximoMinutos { get; set; }
    }
    
    public class PerformanceMedicoDto
    {
        public string NomeMedico { get; set; } = string.Empty;
        public string Crm { get; set; } = string.Empty;
        public int TotalConsultas { get; set; }
        public int TempoMedioConsultaMinutos { get; set; }
        public int TempoMedioEsperaMinutos { get; set; }
        public decimal TaxaPontualidade { get; set; } // %
    }
    
    public class DistribuicaoHorarioDto
    {
        public int Hora { get; set; }
        public int TotalConsultas { get; set; }
        public int TempoMedioEsperaMinutos { get; set; }
    }
    
    public class TempoPorEspecialidadeDto
    {
        public string Especialidade { get; set; } = string.Empty;
        public int TempoMedioConsultaMinutos { get; set; }
        public int TempoMedioEsperaMinutos { get; set; }
    }
    
    public class TendenciaTempoEsperaDto
    {
        public string Data { get; set; } = string.Empty;
        public int TempoMedioEsperaMinutos { get; set; }
        public int TotalConsultas { get; set; }
    }
}
