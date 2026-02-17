using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/SNGPCReports")]
    public class SNGPCReportsController : BaseController
    {
        private readonly ISNGPCReportRepository _reportRepository;
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly ISNGPCXmlGeneratorService _xmlGeneratorService;
        private readonly ISngpcTransmissionService _transmissionService;
        private readonly ISngpcAlertService _alertService;
        private readonly IMapper _mapper;

        public SNGPCReportsController(
            ISNGPCReportRepository reportRepository,
            IDigitalPrescriptionRepository prescriptionRepository,
            ISNGPCXmlGeneratorService xmlGeneratorService,
            ISngpcTransmissionService transmissionService,
            ISngpcAlertService alertService,
            IMapper mapper,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _reportRepository = reportRepository;
            _prescriptionRepository = prescriptionRepository;
            _xmlGeneratorService = xmlGeneratorService;
            _transmissionService = transmissionService;
            _alertService = alertService;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new SNGPC report for a specific month/year
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<SNGPCReportDto>> Create([FromBody] CreateSNGPCReportDto dto)
        {
            try
            {
                // Check if report already exists
                if (await _reportRepository.ReportExistsAsync(dto.Month, dto.Year, GetTenantId()))
                    return BadRequest($"Report for {dto.Month}/{dto.Year} already exists");

                // Create report
                var report = new SNGPCReport(dto.Month, dto.Year, GetTenantId());

                // Get unreported prescriptions for the period
                var prescriptions = await _prescriptionRepository.GetUnreportedToSNGPCAsync(
                    report.ReportPeriodStart,
                    report.ReportPeriodEnd,
                    GetTenantId()
                );

                // Add prescriptions to report
                foreach (var prescription in prescriptions)
                {
                    report.AddPrescription(prescription.Id);
                }

                var created = await _reportRepository.AddAsync(report);
                var result = _mapper.Map<SNGPCReportDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get most recent report
        /// </summary>
        [HttpGet("latest")]
        public async Task<ActionResult<SNGPCReportDto>> GetLatest()
        {
            var report = await _reportRepository.GetMostRecentReportAsync(GetTenantId());

            if (report == null)
                return NotFound("No reports found");

            return Ok(_mapper.Map<SNGPCReportDto>(report));
        }

        /// <summary>
        /// Get overdue reports (not transmitted past deadline)
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<SNGPCReportDto>>> GetOverdue()
        {
            var reports = await _reportRepository.GetOverdueReportsAsync(GetTenantId());
            return Ok(_mapper.Map<IEnumerable<SNGPCReportDto>>(reports));
        }

        /// <summary>
        /// Get transmission history (last N reports)
        /// </summary>
        [HttpGet("history")]
        public async Task<ActionResult<IEnumerable<SNGPCReportDto>>> GetHistory([FromQuery] int count = 12)
        {
            if (count <= 0 || count > 100)
                return BadRequest("Count must be between 1 and 100");

            var reports = await _reportRepository.GetTransmissionHistoryAsync(count, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<SNGPCReportDto>>(reports));
        }

        /// <summary>
        /// Get all reports for a specific year
        /// </summary>
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<SNGPCReportDto>>> GetByYear(int year)
        {
            var reports = await _reportRepository.GetByYearAsync(year, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<SNGPCReportDto>>(reports));
        }

        /// <summary>
        /// Get reports by status
        /// </summary>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<SNGPCReportDto>>> GetByStatus(string status)
        {
            if (!Enum.TryParse<SNGPCReportStatus>(status, true, out var reportStatus))
                return BadRequest($"Invalid status: {status}");

            var reports = await _reportRepository.GetByStatusAsync(reportStatus, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<SNGPCReportDto>>(reports));
        }

        /// <summary>
        /// Get report by month and year
        /// </summary>
        [HttpGet("{year}/{month}")]
        public async Task<ActionResult<SNGPCReportDto>> GetByMonthYear(int year, int month)
        {
            var report = await _reportRepository.GetByMonthYearAsync(month, year, GetTenantId());

            if (report == null)
                return NotFound($"Report for {month}/{year} not found");

            return Ok(_mapper.Map<SNGPCReportDto>(report));
        }

        /// <summary>
        /// Get report by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<SNGPCReportDto>> GetById(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

            if (report == null)
                return NotFound($"Report {id} not found");

            return Ok(_mapper.Map<SNGPCReportDto>(report));
        }

        /// <summary>
        /// Generate XML for a report
        /// </summary>
        [HttpPost("{id}/generate-xml")]
        public async Task<ActionResult> GenerateXML(Guid id)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

                if (report == null)
                    return NotFound($"Report {id} not found");

                // Get all prescriptions for this report
                var prescriptions = new List<DigitalPrescription>();
                foreach (var prescriptionId in report.PrescriptionIds)
                {
                    var prescription = await _prescriptionRepository.GetByIdWithItemsAsync(prescriptionId, GetTenantId());
                    if (prescription != null)
                    {
                        prescriptions.Add(prescription);
                    }
                }

                if (!prescriptions.Any())
                    return BadRequest("No prescriptions found for this report");

                // Generate XML using ANVISA schema v2.1
                var xmlContent = await _xmlGeneratorService.GenerateXmlAsync(report, prescriptions);
                
                // Count total items from prescriptions
                var totalItems = prescriptions.Sum(p => p.Items.Count(i => i.IsControlledSubstance));

                report.GenerateXML(xmlContent, totalItems);
                await _reportRepository.UpdateAsync(report);

                return Ok(new { message = "XML generated successfully using ANVISA schema v2.1" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Transmit report to ANVISA
        /// </summary>
        [HttpPost("{id}/transmit")]
        public async Task<ActionResult<SngpcTransmission>> TransmitReport(Guid id)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

                if (report == null)
                    return NotFound($"Report {id} not found");

                if (string.IsNullOrEmpty(report.XmlContent))
                    return BadRequest("Report XML must be generated before transmission");

                var transmission = await _transmissionService.TransmitReportAsync(
                    id,
                    GetTenantId(),
                    GetUserId());

                return Ok(transmission);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Mark report transmission as failed
        /// </summary>
        [HttpPost("{id}/transmission-failed")]
        public async Task<ActionResult> MarkTransmissionFailed(Guid id, [FromBody] TransmissionFailedDto dto)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

                if (report == null)
                    return NotFound($"Report {id} not found");

                report.MarkAsTransmissionFailed(dto.ErrorMessage);
                await _reportRepository.UpdateAsync(report);

                return Ok(new { message = "Report marked as failed" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Download XML for a report
        /// </summary>
        [HttpGet("{id}/download-xml")]
        public async Task<ActionResult> DownloadXML(Guid id)
        {
            var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

            if (report == null)
                return NotFound($"Report {id} not found");

            if (string.IsNullOrEmpty(report.XmlContent))
                return BadRequest("Report XML has not been generated yet");

            var fileName = $"SNGPC_{report.Year}_{report.Month:D2}_{report.Id}.xml";
            var bytes = System.Text.Encoding.UTF8.GetBytes(report.XmlContent);

            return File(bytes, "application/xml", fileName);
        }

        /// <summary>
        /// Get transmission history for a report
        /// </summary>
        [HttpGet("{id}/transmissions")]
        public async Task<ActionResult<IEnumerable<SngpcTransmission>>> GetTransmissions(Guid id)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

                if (report == null)
                    return NotFound($"Report {id} not found");

                var transmissions = await _transmissionService.GetTransmissionHistoryAsync(
                    id,
                    GetTenantId());

                return Ok(transmissions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Retry a failed transmission
        /// </summary>
        [HttpPost("transmissions/{transmissionId}/retry")]
        public async Task<ActionResult<SngpcTransmission>> RetryTransmission(Guid transmissionId)
        {
            try
            {
                var transmission = await _transmissionService.RetryTransmissionAsync(
                    transmissionId,
                    GetTenantId());

                return Ok(transmission);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get transmission statistics for a date range
        /// </summary>
        [HttpGet("transmissions/statistics")]
        public async Task<ActionResult<TransmissionStatistics>> GetTransmissionStatistics(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (endDate < startDate)
                    return BadRequest("End date must be after start date");

                var statistics = await _transmissionService.GetStatisticsAsync(
                    startDate,
                    endDate,
                    GetTenantId());

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get SNGPC alerts for approaching deadlines
        /// </summary>
        [HttpGet("alerts/deadlines")]
        public async Task<ActionResult<IEnumerable<SngpcAlert>>> GetApproachingDeadlines([FromQuery] int daysBeforeDeadline = 5)
        {
            try
            {
                var alerts = await _alertService.CheckApproachingDeadlinesAsync(GetTenantId(), daysBeforeDeadline);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get overdue SNGPC reports
        /// </summary>
        [HttpGet("alerts/overdue")]
        public async Task<ActionResult<IEnumerable<SngpcAlert>>> GetOverdueReports()
        {
            try
            {
                var alerts = await _alertService.CheckOverdueReportsAsync(GetTenantId());
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Validate SNGPC compliance
        /// </summary>
        [HttpGet("alerts/compliance")]
        public async Task<ActionResult<IEnumerable<SngpcAlert>>> ValidateCompliance()
        {
            try
            {
                var alerts = await _alertService.ValidateComplianceAsync(GetTenantId());
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Detect anomalies in controlled medication movements
        /// </summary>
        [HttpGet("alerts/anomalies")]
        public async Task<ActionResult<IEnumerable<SngpcAlert>>> DetectAnomalies(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var alerts = await _alertService.DetectAnomaliesAsync(GetTenantId(), startDate, endDate);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all active alerts
        /// </summary>
        [HttpGet("alerts")]
        public async Task<ActionResult<IEnumerable<SngpcAlert>>> GetActiveAlerts([FromQuery] AlertSeverity? severity = null)
        {
            try
            {
                var alerts = await _alertService.GetActiveAlertsAsync(GetTenantId(), severity);
                return Ok(alerts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    // Additional DTOs
    public class TransmitReportDto
    {
        public string TransmissionProtocol { get; set; } = null!;
    }

    public class TransmissionFailedDto
    {
        public string ErrorMessage { get; set; } = null!;
    }
}
