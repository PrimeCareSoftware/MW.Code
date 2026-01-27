using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for TISS analytics and reporting
    /// </summary>
    public class TissAnalyticsService : ITissAnalyticsService
    {
        private readonly ITissBatchRepository _tissBatchRepository;
        private readonly ITissGuideRepository _tissGuideRepository;
        private readonly IAuthorizationRequestRepository _authorizationRequestRepository;
        private readonly IHealthInsuranceOperatorRepository _operatorRepository;
        private readonly ITissGlosaRepository _glosaRepository;
        private readonly ITissRecursoGlosaRepository _recursoRepository;

        public TissAnalyticsService(
            ITissBatchRepository tissBatchRepository,
            ITissGuideRepository tissGuideRepository,
            IAuthorizationRequestRepository authorizationRequestRepository,
            IHealthInsuranceOperatorRepository operatorRepository,
            ITissGlosaRepository glosaRepository,
            ITissRecursoGlosaRepository recursoRepository)
        {
            _tissBatchRepository = tissBatchRepository;
            _tissGuideRepository = tissGuideRepository;
            _authorizationRequestRepository = authorizationRequestRepository;
            _operatorRepository = operatorRepository;
            _glosaRepository = glosaRepository;
            _recursoRepository = recursoRepository;
        }

        public async Task<GlosasSummaryDto> GetGlosasSummaryAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.ProcessedDate.HasValue && 
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate).ToList();

            var guides = new List<TissGuide>();
            foreach (var batch in filteredBatches)
            {
                var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                guides.AddRange(batchGuides);
            }

            var totalBilled = guides.Sum(g => g.TotalAmount);
            var totalApproved = guides.Sum(g => g.ApprovedAmount ?? 0);
            var totalGlosed = guides.Sum(g => g.GlosedAmount ?? 0);
            var glosedGuides = guides.Count(g => g.GlosedAmount.HasValue && g.GlosedAmount.Value > 0);

            return new GlosasSummaryDto
            {
                TotalBilled = totalBilled,
                TotalApproved = totalApproved,
                TotalGlosed = totalGlosed,
                GlosaPercentage = totalBilled > 0 ? (totalGlosed / totalBilled) * 100 : 0,
                TotalBatches = filteredBatches.Count,
                TotalGuides = guides.Count,
                GlosedGuides = glosedGuides
            };
        }

        public async Task<IEnumerable<GlosasByOperatorDto>> GetGlosasByOperatorAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.ProcessedDate.HasValue && 
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate).ToList();

            var operatorGroups = filteredBatches.GroupBy(b => b.OperatorId);
            var results = new List<GlosasByOperatorDto>();

            foreach (var operatorGroup in operatorGroups)
            {
                var operatorId = operatorGroup.Key;
                var operatorBatches = operatorGroup.ToList();
                
                var guides = new List<TissGuide>();
                foreach (var batch in operatorBatches)
                {
                    var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                    guides.AddRange(batchGuides);
                }

                var totalBilled = guides.Sum(g => g.TotalAmount);
                var totalGlosed = guides.Sum(g => g.GlosedAmount ?? 0);
                var glosedGuides = guides.Count(g => g.GlosedAmount.HasValue && g.GlosedAmount.Value > 0);

                var operatorEntity = await _operatorRepository.GetByIdAsync(operatorId, tenantId);
                var operatorName = operatorEntity?.TradeName ?? "Desconhecida";

                results.Add(new GlosasByOperatorDto
                {
                    OperatorId = operatorId,
                    OperatorName = operatorName,
                    TotalBilled = totalBilled,
                    TotalGlosed = totalGlosed,
                    GlosaPercentage = totalBilled > 0 ? (totalGlosed / totalBilled) * 100 : 0,
                    TotalGuides = guides.Count,
                    GlosedGuides = glosedGuides
                });
            }

            return results.OrderByDescending(r => r.GlosaPercentage);
        }

        public async Task<IEnumerable<GlosasTrendDto>> GetGlosasTrendAsync(Guid clinicId, int months, string tenantId)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);

            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.ProcessedDate.HasValue && 
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate).ToList();

            var monthGroups = filteredBatches.GroupBy(b => new 
            { 
                Year = b.ProcessedDate!.Value.Year, 
                Month = b.ProcessedDate!.Value.Month 
            });

            var results = new List<GlosasTrendDto>();

            foreach (var monthGroup in monthGroups)
            {
                var monthBatches = monthGroup.ToList();
                var guides = new List<TissGuide>();
                
                foreach (var batch in monthBatches)
                {
                    var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                    guides.AddRange(batchGuides);
                }

                var totalBilled = guides.Sum(g => g.TotalAmount);
                var totalGlosed = guides.Sum(g => g.GlosedAmount ?? 0);

                var monthName = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1)
                    .ToString("MMMM yyyy", new CultureInfo("pt-BR"));

                results.Add(new GlosasTrendDto
                {
                    Year = monthGroup.Key.Year,
                    Month = monthGroup.Key.Month,
                    MonthName = monthName,
                    TotalBilled = totalBilled,
                    TotalGlosed = totalGlosed,
                    GlosaPercentage = totalBilled > 0 ? (totalGlosed / totalBilled) * 100 : 0,
                    TotalGuides = guides.Count
                });
            }

            return results.OrderBy(r => r.Year).ThenBy(r => r.Month);
        }

        public async Task<IEnumerable<ProcedureGlosasDto>> GetProcedureGlosasAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.ProcessedDate.HasValue && 
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate).ToList();

            var allProcedures = new List<TissGuideProcedure>();
            
            foreach (var batch in filteredBatches)
            {
                var guides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                foreach (var guide in guides)
                {
                    // Load procedures for each guide
                    var procedures = guide.Procedures;
                    if (procedures != null && procedures.Any())
                    {
                        allProcedures.AddRange(procedures);
                    }
                }
            }

            var procedureGroups = allProcedures.GroupBy(p => new 
            { 
                p.ProcedureCode, 
                p.ProcedureDescription 
            });

            var results = new List<ProcedureGlosasDto>();

            foreach (var procedureGroup in procedureGroups)
            {
                var procedures = procedureGroup.ToList();
                var totalBilled = procedures.Sum(p => p.TotalPrice);
                var totalGlosed = procedures.Sum(p => p.GlosedAmount ?? 0);
                var glosedCount = procedures.Count(p => p.GlosedAmount.HasValue && p.GlosedAmount.Value > 0);

                results.Add(new ProcedureGlosasDto
                {
                    ProcedureId = Guid.Empty, // Group by code/name, not specific ID
                    ProcedureCode = procedureGroup.Key.ProcedureCode,
                    ProcedureName = procedureGroup.Key.ProcedureDescription,
                    TotalBilled = totalBilled,
                    TotalGlosed = totalGlosed,
                    GlosaPercentage = totalBilled > 0 ? (totalGlosed / totalBilled) * 100 : 0,
                    TotalOccurrences = procedures.Count,
                    GlosedOccurrences = glosedCount
                });
            }

            return results
                .OrderByDescending(r => r.TotalGlosed)
                .Take(10);
        }

        public async Task<IEnumerable<AuthorizationRateDto>> GetAuthorizationRateAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            // Get all authorizations for the tenant with details (including PatientHealthInsurance)
            var allAuthorizations = await _authorizationRequestRepository.GetAllWithDetailsAsync(tenantId);
            
            // Filter by date range
            var filteredAuths = allAuthorizations.Where(a => 
                a.RequestDate >= startDate && 
                a.RequestDate <= endDate &&
                a.PatientHealthInsurance != null).ToList();

            // Group by operator through PatientHealthInsurance -> HealthInsurancePlan -> OperatorId
            var operatorGroups = filteredAuths
                .Where(a => a.PatientHealthInsurance?.HealthInsurancePlan?.OperatorId != null)
                .GroupBy(a => a.PatientHealthInsurance!.HealthInsurancePlan!.OperatorId);

            var results = new List<AuthorizationRateDto>();

            foreach (var operatorGroup in operatorGroups)
            {
                var operatorId = operatorGroup.Key;
                var auths = operatorGroup.ToList();

                var total = auths.Count;
                var approved = auths.Count(a => a.Status == AuthorizationStatus.Approved);
                var rejected = auths.Count(a => a.Status == AuthorizationStatus.Denied);
                var pending = auths.Count(a => a.Status == AuthorizationStatus.Pending);

                var operatorEntity = await _operatorRepository.GetByIdAsync(operatorId, tenantId);
                var operatorName = operatorEntity?.TradeName ?? "Desconhecida";

                results.Add(new AuthorizationRateDto
                {
                    OperatorId = operatorId,
                    OperatorName = operatorName,
                    TotalRequests = total,
                    ApprovedRequests = approved,
                    RejectedRequests = rejected,
                    PendingRequests = pending,
                    ApprovalRate = total > 0 ? ((decimal)approved / total) * 100 : 0
                });
            }

            return results.OrderByDescending(r => r.ApprovalRate);
        }

        public async Task<IEnumerable<ApprovalTimeDto>> GetApprovalTimeAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.SubmittedDate.HasValue && 
                b.ProcessedDate.HasValue &&
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate).ToList();

            var operatorGroups = filteredBatches.GroupBy(b => b.OperatorId);
            var results = new List<ApprovalTimeDto>();

            foreach (var operatorGroup in operatorGroups)
            {
                var operatorId = operatorGroup.Key;
                var operatorBatches = operatorGroup.ToList();

                var approvalTimes = operatorBatches
                    .Select(b => (b.ProcessedDate!.Value - b.SubmittedDate!.Value).TotalDays)
                    .ToList();

                var operatorEntity = await _operatorRepository.GetByIdAsync(operatorId, tenantId);
                var operatorName = operatorEntity?.TradeName ?? "Desconhecida";

                results.Add(new ApprovalTimeDto
                {
                    OperatorId = operatorId,
                    OperatorName = operatorName,
                    AverageApprovalDays = approvalTimes.Average(),
                    TotalProcessed = operatorBatches.Count,
                    MinApprovalDays = approvalTimes.Min(),
                    MaxApprovalDays = approvalTimes.Max()
                });
            }

            return results.OrderBy(r => r.AverageApprovalDays);
        }

        public async Task<IEnumerable<MonthlyPerformanceDto>> GetMonthlyPerformanceAsync(Guid clinicId, int months, string tenantId)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);

            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.ProcessedDate.HasValue && 
                b.ProcessedDate.Value >= startDate && 
                b.ProcessedDate.Value <= endDate &&
                b.SubmittedDate.HasValue).ToList();

            var monthGroups = filteredBatches.GroupBy(b => new 
            { 
                Year = b.ProcessedDate!.Value.Year, 
                Month = b.ProcessedDate!.Value.Month 
            });

            var results = new List<MonthlyPerformanceDto>();

            foreach (var monthGroup in monthGroups)
            {
                var monthBatches = monthGroup.ToList();
                var guides = new List<TissGuide>();
                
                foreach (var batch in monthBatches)
                {
                    var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                    guides.AddRange(batchGuides);
                }

                var totalBilled = guides.Sum(g => g.TotalAmount);
                var totalApproved = guides.Sum(g => g.ApprovedAmount ?? 0);
                var totalGlosed = guides.Sum(g => g.GlosedAmount ?? 0);

                var approvalTimes = monthBatches
                    .Where(b => b.SubmittedDate.HasValue && b.ProcessedDate.HasValue)
                    .Select(b => (b.ProcessedDate!.Value - b.SubmittedDate!.Value).TotalDays)
                    .ToList();

                var avgApprovalDays = approvalTimes.Any() ? approvalTimes.Average() : 0;

                var monthName = new DateTime(monthGroup.Key.Year, monthGroup.Key.Month, 1)
                    .ToString("MMMM yyyy", new CultureInfo("pt-BR"));

                results.Add(new MonthlyPerformanceDto
                {
                    Year = monthGroup.Key.Year,
                    Month = monthGroup.Key.Month,
                    MonthName = monthName,
                    TotalBilled = totalBilled,
                    TotalApproved = totalApproved,
                    GlosaPercentage = totalBilled > 0 ? (totalGlosed / totalBilled) * 100 : 0,
                    AverageApprovalDays = avgApprovalDays,
                    TotalGuides = guides.Count
                });
            }

            return results.OrderBy(r => r.Year).ThenBy(r => r.Month);
        }

        public async Task<IEnumerable<GlosaAlertDto>> GetGlosaAlertsAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var alerts = new List<GlosaAlertDto>();

            // Get overall glosa statistics
            var summary = await GetGlosasSummaryAsync(clinicId, startDate, endDate, tenantId);
            var operatorStats = await GetGlosasByOperatorAsync(clinicId, startDate, endDate, tenantId);

            // Alert threshold: 15% glosa rate
            const decimal glosaThreshold = 15.0m;

            // Check overall glosa rate
            if (summary.GlosaPercentage > glosaThreshold)
            {
                alerts.Add(new GlosaAlertDto
                {
                    AlertType = "Taxa de Glosa Geral",
                    Message = $"Taxa de glosa geral está em {summary.GlosaPercentage:F2}%, acima do limite de {glosaThreshold}%",
                    Severity = summary.GlosaPercentage > 25 ? "high" : "medium",
                    Value = summary.GlosaPercentage,
                    Threshold = glosaThreshold
                });
            }

            // Check operators with high glosa rate
            foreach (var operatorStat in operatorStats.Where(o => o.GlosaPercentage > glosaThreshold))
            {
                alerts.Add(new GlosaAlertDto
                {
                    AlertType = "Taxa de Glosa por Operadora",
                    Message = $"Operadora {operatorStat.OperatorName} tem taxa de glosa de {operatorStat.GlosaPercentage:F2}%, acima da média",
                    Severity = operatorStat.GlosaPercentage > 30 ? "high" : "medium",
                    Value = operatorStat.GlosaPercentage,
                    Threshold = glosaThreshold
                });
            }

            // Check procedures with high glosa rate
            var procedureStats = await GetProcedureGlosasAsync(clinicId, startDate, endDate, tenantId);
            foreach (var procedureStat in procedureStats.Where(p => p.GlosaPercentage > glosaThreshold).Take(5))
            {
                alerts.Add(new GlosaAlertDto
                {
                    AlertType = "Procedimento com Alta Glosa",
                    Message = $"Procedimento {procedureStat.ProcedureCode} - {procedureStat.ProcedureName} tem {procedureStat.GlosaPercentage:F2}% de glosa",
                    Severity = procedureStat.GlosaPercentage > 50 ? "high" : "medium",
                    Value = procedureStat.GlosaPercentage,
                    Threshold = glosaThreshold
                });
            }

            return alerts.OrderByDescending(a => a.Severity == "high" ? 3 : a.Severity == "medium" ? 2 : 1)
                        .ThenByDescending(a => a.Value);
        }

        // ====== TISS PHASE 2 - New Analytics Methods Implementation ======

        public async Task<DashboardTissDto> GetDashboardDataAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var dashboard = new DashboardTissDto
            {
                PeriodoInicio = startDate,
                PeriodoFim = endDate
            };

            // Get guides data
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.SubmittedDate.HasValue && 
                b.SubmittedDate.Value >= startDate && 
                b.SubmittedDate.Value <= endDate).ToList();

            var guides = new List<TissGuide>();
            foreach (var batch in filteredBatches)
            {
                var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                guides.AddRange(batchGuides);
            }

            dashboard.TotalGuiasEnviadas = guides.Count;
            dashboard.TotalGuiasAprovadas = guides.Count(g => g.Status == GuideStatus.Approved || g.Status == GuideStatus.PartiallyApproved);
            dashboard.TotalGuiasGlosadas = guides.Count(g => g.GlosedAmount.HasValue && g.GlosedAmount.Value > 0);
            dashboard.ValorTotalFaturado = guides.Sum(g => g.TotalAmount);
            dashboard.ValorTotalGlosado = guides.Sum(g => g.GlosedAmount ?? 0);
            dashboard.ValorTotalRecebido = guides.Sum(g => g.ApprovedAmount ?? 0);
            dashboard.TaxaGlosa = dashboard.ValorTotalFaturado > 0 
                ? (dashboard.ValorTotalGlosado / dashboard.ValorTotalFaturado) * 100 
                : 0;

            // Get detailed analytics
            dashboard.AnaliseGlosas = await GetGlosaDetailedAnalyticsAsync(clinicId, startDate, endDate, tenantId);
            dashboard.PerformancePorOperadora = (await GetOperadoraPerformanceAsync(clinicId, startDate, endDate, tenantId)).ToList();
            dashboard.GlosasMaisFrequentes = (await GetGlosaCodigosFrequentesAsync(clinicId, startDate, endDate, 10, tenantId)).ToList();
            dashboard.ProcedimentosMaisGlosados = (await GetProcedimentosMaisGlosadosAsync(clinicId, startDate, endDate, 10, tenantId)).ToList();
            dashboard.TendenciaGlosas = (await GetGlosaTendenciasAsync(clinicId, 12, tenantId)).ToList();

            return dashboard;
        }

        public async Task<GlosaDetailedAnalyticsDto> GetGlosaDetailedAnalyticsAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var glosas = (await _glosaRepository.GetByDateRangeAsync(startDate, endDate, tenantId)).ToList();

            var analytics = new GlosaDetailedAnalyticsDto
            {
                TotalGlosas = glosas.Count,
                ValorTotalGlosado = glosas.Sum(g => g.ValorGlosado),
                GlosasAdministrativas = glosas.Count(g => g.Tipo == TipoGlosa.Administrativa),
                GlosasTecnicas = glosas.Count(g => g.Tipo == TipoGlosa.Tecnica),
                GlosasFinanceiras = glosas.Count(g => g.Tipo == TipoGlosa.Financeira)
            };

            // Get recursos data
            var recursos = (await _recursoRepository.GetByDateRangeAsync(startDate, endDate, tenantId)).ToList();
            analytics.RecursosEnviados = recursos.Count;
            analytics.RecursosDeferidos = recursos.Count(r => r.Resultado == ResultadoRecurso.Deferido || r.Resultado == ResultadoRecurso.Parcial);
            analytics.RecursosIndeferidos = recursos.Count(r => r.Resultado == ResultadoRecurso.Indeferido);
            analytics.TaxaSucessoRecursos = analytics.RecursosEnviados > 0 
                ? ((decimal)analytics.RecursosDeferidos / analytics.RecursosEnviados) * 100 
                : 0;
            analytics.ValorRecuperado = recursos.Where(r => r.ValorDeferido.HasValue).Sum(r => r.ValorDeferido!.Value);

            return analytics;
        }

        public async Task<IEnumerable<OperadoraPerformanceDto>> GetOperadoraPerformanceAsync(Guid clinicId, DateTime startDate, DateTime endDate, string tenantId)
        {
            var batches = await _tissBatchRepository.GetByClinicIdAsync(clinicId, tenantId);
            var filteredBatches = batches.Where(b => 
                b.SubmittedDate.HasValue && 
                b.SubmittedDate.Value >= startDate && 
                b.SubmittedDate.Value <= endDate).ToList();

            var operatorGroups = filteredBatches.GroupBy(b => b.OperatorId);
            var results = new List<OperadoraPerformanceDto>();

            foreach (var operatorGroup in operatorGroups)
            {
                var operatorId = operatorGroup.Key;
                var operatorBatches = operatorGroup.ToList();
                
                var guides = new List<TissGuide>();
                foreach (var batch in operatorBatches)
                {
                    var batchGuides = await _tissGuideRepository.GetByBatchIdAsync(batch.Id, tenantId);
                    guides.AddRange(batchGuides);
                }

                var valorFaturado = guides.Sum(g => g.TotalAmount);
                var valorGlosado = guides.Sum(g => g.GlosedAmount ?? 0);
                var valorRecebido = guides.Sum(g => g.ApprovedAmount ?? 0);

                // Get glosas for this operator
                var guideIds = guides.Select(g => g.Id).ToHashSet();
                var allGlosas = await _glosaRepository.GetByDateRangeAsync(startDate, endDate, tenantId);
                var operatorGlosas = allGlosas.Where(g => guideIds.Contains(g.GuideId)).ToList();

                // Get recursos success rate
                var recursos = new List<TissRecursoGlosa>();
                foreach (var glosa in operatorGlosas)
                {
                    var glosaRecursos = await _recursoRepository.GetByGlosaIdAsync(glosa.Id, tenantId);
                    recursos.AddRange(glosaRecursos);
                }

                var recursosDeferidos = recursos.Count(r => r.Resultado == ResultadoRecurso.Deferido || r.Resultado == ResultadoRecurso.Parcial);

                var operatorEntity = await _operatorRepository.GetByIdAsync(operatorId, tenantId);
                var operatorName = operatorEntity?.TradeName ?? "Desconhecida";

                // Calculate average return time
                var processedBatches = operatorBatches.Where(b => b.SubmittedDate.HasValue && b.ProcessedDate.HasValue).ToList();
                var avgReturnTime = processedBatches.Any() 
                    ? processedBatches.Average(b => (b.ProcessedDate!.Value - b.SubmittedDate!.Value).TotalDays) 
                    : 0;

                results.Add(new OperadoraPerformanceDto
                {
                    OperatorId = operatorId,
                    NomeOperadora = operatorName,
                    GuiasEnviadas = guides.Count,
                    GuiasAprovadas = guides.Count(g => g.Status == GuideStatus.Approved || g.Status == GuideStatus.PartiallyApproved),
                    TaxaAprovacao = guides.Count > 0 
                        ? ((decimal)guides.Count(g => g.Status == GuideStatus.Approved || g.Status == GuideStatus.PartiallyApproved) / guides.Count) * 100 
                        : 0,
                    ValorFaturado = valorFaturado,
                    ValorGlosado = valorGlosado,
                    ValorRecebido = valorRecebido,
                    TaxaGlosa = valorFaturado > 0 ? (valorGlosado / valorFaturado) * 100 : 0,
                    TempoMedioRetornoDias = avgReturnTime,
                    UltimoEnvio = operatorBatches.Max(b => b.SubmittedDate),
                    TotalGlosas = operatorGlosas.Count,
                    RecursosDeferidos = recursosDeferidos,
                    TaxaSucessoRecursos = recursos.Count > 0 ? ((decimal)recursosDeferidos / recursos.Count) * 100 : 0
                });
            }

            return results.OrderByDescending(r => r.ValorFaturado);
        }

        public async Task<IEnumerable<GlosaTendenciaDto>> GetGlosaTendenciasAsync(Guid clinicId, int months, string tenantId)
        {
            var results = new List<GlosaTendenciaDto>();
            var today = DateTime.UtcNow;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = today.AddMonths(-i);
                var startDate = new DateTime(targetDate.Year, targetDate.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var glosas = (await _glosaRepository.GetByDateRangeAsync(startDate, endDate, tenantId)).ToList();
                var recursos = (await _recursoRepository.GetByDateRangeAsync(startDate, endDate, tenantId)).ToList();

                var valorGlosado = glosas.Sum(g => g.ValorGlosado);
                var recursosDeferidos = recursos.Count(r => r.Resultado == ResultadoRecurso.Deferido || r.Resultado == ResultadoRecurso.Parcial);

                results.Add(new GlosaTendenciaDto
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    MonthName = CultureInfo.GetCultureInfo("pt-BR").DateTimeFormat.GetMonthName(targetDate.Month),
                    TaxaGlosa = valorGlosado,
                    ValorGlosado = valorGlosado,
                    TotalGlosas = glosas.Count,
                    RecursosEnviados = recursos.Count,
                    TaxaSucessoRecursos = recursos.Count > 0 ? ((decimal)recursosDeferidos / recursos.Count) * 100 : 0
                });
            }

            return results;
        }

        public async Task<IEnumerable<GlosaCodigoFrequenteDto>> GetGlosaCodigosFrequentesAsync(Guid clinicId, DateTime startDate, DateTime endDate, int top, string tenantId)
        {
            var glosas = (await _glosaRepository.GetByDateRangeAsync(startDate, endDate, tenantId)).ToList();
            
            var codigoGroups = glosas.GroupBy(g => new { g.CodigoGlosa, g.DescricaoGlosa, g.Tipo });
            var results = new List<GlosaCodigoFrequenteDto>();

            foreach (var group in codigoGroups)
            {
                var groupGlosas = group.ToList();
                var recursos = new List<TissRecursoGlosa>();
                
                foreach (var glosa in groupGlosas)
                {
                    var glosaRecursos = await _recursoRepository.GetByGlosaIdAsync(glosa.Id, tenantId);
                    recursos.AddRange(glosaRecursos);
                }

                var recursosDeferidos = recursos.Count(r => r.Resultado == ResultadoRecurso.Deferido || r.Resultado == ResultadoRecurso.Parcial);

                results.Add(new GlosaCodigoFrequenteDto
                {
                    CodigoGlosa = group.Key.CodigoGlosa,
                    DescricaoGlosa = group.Key.DescricaoGlosa,
                    Tipo = group.Key.Tipo.ToString(),
                    Ocorrencias = groupGlosas.Count,
                    ValorTotal = groupGlosas.Sum(g => g.ValorGlosado),
                    RecursosDeferidos = recursosDeferidos,
                    TaxaSucessoRecursos = recursos.Count > 0 ? ((decimal)recursosDeferidos / recursos.Count) * 100 : 0
                });
            }

            return results.OrderByDescending(r => r.Ocorrencias).Take(top);
        }

        public async Task<IEnumerable<ProcedimentoMaisGlosadoDto>> GetProcedimentosMaisGlosadosAsync(Guid clinicId, DateTime startDate, DateTime endDate, int top, string tenantId)
        {
            var glosas = (await _glosaRepository.GetByDateRangeAsync(startDate, endDate, tenantId))
                .Where(g => !string.IsNullOrEmpty(g.CodigoProcedimento))
                .ToList();
            
            var procedureGroups = glosas.GroupBy(g => new { g.CodigoProcedimento, g.NomeProcedimento });
            var results = new List<ProcedimentoMaisGlosadoDto>();

            foreach (var group in procedureGroups)
            {
                var groupGlosas = group.ToList();
                var motivos = groupGlosas.Select(g => g.DescricaoGlosa).Distinct().Take(5).ToList();

                results.Add(new ProcedimentoMaisGlosadoDto
                {
                    CodigoProcedimento = group.Key.CodigoProcedimento ?? string.Empty,
                    NomeProcedimento = group.Key.NomeProcedimento ?? string.Empty,
                    TotalGlosas = groupGlosas.Count,
                    ValorTotalGlosado = groupGlosas.Sum(g => g.ValorGlosado),
                    MotivosFrequentes = motivos
                });
            }

            return results.OrderByDescending(r => r.TotalGlosas).Take(top);
        }
    }
}
