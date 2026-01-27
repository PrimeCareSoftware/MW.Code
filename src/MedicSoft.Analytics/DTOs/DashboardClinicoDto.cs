using System;
using System.Collections.Generic;

namespace MedicSoft.Analytics.DTOs
{
    public class DashboardClinicoDto
    {
        public PeriodoDto Periodo { get; set; }
        
        // KPIs principais
        public int TotalConsultas { get; set; }
        public decimal TaxaOcupacao { get; set; }
        public int TempoMedioConsulta { get; set; }
        public decimal TaxaNoShow { get; set; }
        
        // Distribuição
        public List<ConsultasPorEspecialidadeDto> ConsultasPorEspecialidade { get; set; }
        public List<ConsultasPorMedicoDto> ConsultasPorMedico { get; set; }
        public List<ConsultasPorDiaDto> ConsultasPorDiaSemana { get; set; }
        public List<ConsultasPorHorarioDto> ConsultasPorHorario { get; set; }
        
        // Top diagnósticos
        public List<DiagnosticoFrequenciaDto> DiagnosticosMaisFrequentes { get; set; }
        
        // Novos vs Retorno
        public int PacientesNovos { get; set; }
        public int PacientesRetorno { get; set; }
        
        // Tendências
        public List<TendenciaMensalDto> TendenciaConsultas { get; set; }
    }

    public class PeriodoDto
    {
        public DateTime Inicio { get; set; }
        public DateTime Fim { get; set; }
    }

    public class ConsultasPorEspecialidadeDto
    {
        public string Especialidade { get; set; }
        public int Total { get; set; }
        public decimal Percentual { get; set; }
    }

    public class ConsultasPorMedicoDto
    {
        public string NomeMedico { get; set; }
        public string CRM { get; set; }
        public int Total { get; set; }
        public decimal TaxaOcupacao { get; set; }
    }

    public class ConsultasPorDiaDto
    {
        public string DiaSemana { get; set; }
        public int Total { get; set; }
        public decimal MediaPorDia { get; set; }
    }

    public class ConsultasPorHorarioDto
    {
        public int Hora { get; set; }
        public int Total { get; set; }
    }

    public class DiagnosticoFrequenciaDto
    {
        public string CodigoCid { get; set; }
        public string Descricao { get; set; }
        public int Frequencia { get; set; }
        public decimal Percentual { get; set; }
    }

    public class TendenciaMensalDto
    {
        public string Mes { get; set; }
        public int Agendadas { get; set; }
        public int Realizadas { get; set; }
        public int Canceladas { get; set; }
        public int NoShow { get; set; }
    }
}
