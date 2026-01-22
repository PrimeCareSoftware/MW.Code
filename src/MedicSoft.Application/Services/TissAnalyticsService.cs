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

        public TissAnalyticsService(
            ITissBatchRepository tissBatchRepository,
            ITissGuideRepository tissGuideRepository,
            IAuthorizationRequestRepository authorizationRequestRepository,
            IHealthInsuranceOperatorRepository operatorRepository)
        {
            _tissBatchRepository = tissBatchRepository;
            _tissGuideRepository = tissGuideRepository;
            _authorizationRequestRepository = authorizationRequestRepository;
            _operatorRepository = operatorRepository;
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
    }
}
