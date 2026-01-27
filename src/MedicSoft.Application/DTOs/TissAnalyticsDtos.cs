using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for glosas (rejections) summary
    /// </summary>
    public class GlosasSummaryDto
    {
        public decimal TotalBilled { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalBatches { get; set; }
        public int TotalGuides { get; set; }
        public int GlosedGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosas by operator
    /// </summary>
    public class GlosasByOperatorDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalGuides { get; set; }
        public int GlosedGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosas trend over time
    /// </summary>
    public class GlosasTrendDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalGuides { get; set; }
    }

    /// <summary>
    /// DTO for procedure glosas (top procedures with rejections)
    /// </summary>
    public class ProcedureGlosasDto
    {
        public Guid ProcedureId { get; set; }
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalGlosed { get; set; }
        public decimal GlosaPercentage { get; set; }
        public int TotalOccurrences { get; set; }
        public int GlosedOccurrences { get; set; }
    }

    /// <summary>
    /// DTO for authorization rate
    /// </summary>
    public class AuthorizationRateDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int PendingRequests { get; set; }
        public decimal ApprovalRate { get; set; }
    }

    /// <summary>
    /// DTO for approval time by operator
    /// </summary>
    public class ApprovalTimeDto
    {
        public Guid OperatorId { get; set; }
        public string OperatorName { get; set; } = string.Empty;
        public double AverageApprovalDays { get; set; }
        public int TotalProcessed { get; set; }
        public double MinApprovalDays { get; set; }
        public double MaxApprovalDays { get; set; }
    }

    /// <summary>
    /// DTO for monthly performance comparison
    /// </summary>
    public class MonthlyPerformanceDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TotalBilled { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal GlosaPercentage { get; set; }
        public double AverageApprovalDays { get; set; }
        public int TotalGuides { get; set; }
    }

    /// <summary>
    /// DTO for glosa alerts
    /// </summary>
    public class GlosaAlertDto
    {
        public string AlertType { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty; // "high", "medium", "low"
        public decimal Value { get; set; }
        public decimal Threshold { get; set; }
    }

    // ====== TISS PHASE 2 - New Analytics DTOs ======

    /// <summary>
    /// DTO for detailed glosa analysis with recurso tracking
    /// </summary>
    public class GlosaDetailedAnalyticsDto
    {
        public int TotalGlosas { get; set; }
        public decimal ValorTotalGlosado { get; set; }
        public int GlosasAdministrativas { get; set; }
        public int GlosasTecnicas { get; set; }
        public int GlosasFinanceiras { get; set; }
        public int RecursosEnviados { get; set; }
        public int RecursosDeferidos { get; set; }
        public int RecursosIndeferidos { get; set; }
        public decimal TaxaSucessoRecursos { get; set; }
        public decimal ValorRecuperado { get; set; }
    }

    /// <summary>
    /// DTO for operator performance with glosa details
    /// </summary>
    public class OperadoraPerformanceDto
    {
        public Guid OperatorId { get; set; }
        public string NomeOperadora { get; set; } = string.Empty;
        public int GuiasEnviadas { get; set; }
        public int GuiasAprovadas { get; set; }
        public decimal TaxaAprovacao { get; set; }
        public decimal ValorFaturado { get; set; }
        public decimal ValorGlosado { get; set; }
        public decimal ValorRecebido { get; set; }
        public decimal TaxaGlosa { get; set; }
        public double TempoMedioRetornoDias { get; set; }
        public DateTime? UltimoEnvio { get; set; }
        public int TotalGlosas { get; set; }
        public int RecursosDeferidos { get; set; }
        public decimal TaxaSucessoRecursos { get; set; }
    }

    /// <summary>
    /// DTO for glosa trends with recurso data
    /// </summary>
    public class GlosaTendenciaDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal TaxaGlosa { get; set; }
        public decimal ValorGlosado { get; set; }
        public int TotalGlosas { get; set; }
        public int RecursosEnviados { get; set; }
        public decimal TaxaSucessoRecursos { get; set; }
    }

    /// <summary>
    /// DTO for most common glosa codes
    /// </summary>
    public class GlosaCodigoFrequenteDto
    {
        public string CodigoGlosa { get; set; } = string.Empty;
        public string DescricaoGlosa { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public int Ocorrencias { get; set; }
        public decimal ValorTotal { get; set; }
        public int RecursosDeferidos { get; set; }
        public decimal TaxaSucessoRecursos { get; set; }
    }

    /// <summary>
    /// DTO for procedures most commonly rejected
    /// </summary>
    public class ProcedimentoMaisGlosadoDto
    {
        public string CodigoProcedimento { get; set; } = string.Empty;
        public string NomeProcedimento { get; set; } = string.Empty;
        public int TotalGlosas { get; set; }
        public decimal ValorTotalGlosado { get; set; }
        public List<string> MotivosFrequentes { get; set; } = new();
    }

    /// <summary>
    /// DTO for dashboard main data
    /// </summary>
    public class DashboardTissDto
    {
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }
        public int TotalGuiasEnviadas { get; set; }
        public int TotalGuiasAprovadas { get; set; }
        public int TotalGuiasGlosadas { get; set; }
        public decimal TaxaGlosa { get; set; }
        public decimal ValorTotalFaturado { get; set; }
        public decimal ValorTotalGlosado { get; set; }
        public decimal ValorTotalRecebido { get; set; }
        public List<OperadoraPerformanceDto> PerformancePorOperadora { get; set; } = new();
        public List<GlosaCodigoFrequenteDto> GlosasMaisFrequentes { get; set; } = new();
        public List<ProcedimentoMaisGlosadoDto> ProcedimentosMaisGlosados { get; set; } = new();
        public GlosaDetailedAnalyticsDto AnaliseGlosas { get; set; } = new();
        public List<GlosaTendenciaDto> TendenciaGlosas { get; set; } = new();
    }
}
