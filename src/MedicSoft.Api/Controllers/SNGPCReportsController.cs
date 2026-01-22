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
        private readonly IMapper _mapper;

        public SNGPCReportsController(
            ISNGPCReportRepository reportRepository,
            IDigitalPrescriptionRepository prescriptionRepository,
            ISNGPCXmlGeneratorService xmlGeneratorService,
            IMapper mapper,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _reportRepository = reportRepository;
            _prescriptionRepository = prescriptionRepository;
            _xmlGeneratorService = xmlGeneratorService;
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
        /// Get overdue reports (not transmitted past deadline)
        /// </summary>
        [HttpGet("overdue")]
        public async Task<ActionResult<IEnumerable<SNGPCReportDto>>> GetOverdue()
        {
            var reports = await _reportRepository.GetOverdueReportsAsync(GetTenantId());
            return Ok(_mapper.Map<IEnumerable<SNGPCReportDto>>(reports));
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
        /// Mark report as transmitted
        /// </summary>
        [HttpPost("{id}/transmit")]
        public async Task<ActionResult> MarkAsTransmitted(Guid id, [FromBody] TransmitReportDto dto)
        {
            try
            {
                var report = await _reportRepository.GetByIdAsync(id, GetTenantId());

                if (report == null)
                    return NotFound($"Report {id} not found");

                report.MarkAsTransmitted(dto.TransmissionProtocol);
                await _reportRepository.UpdateAsync(report);

                // Mark prescriptions as reported
                foreach (var prescriptionId in report.PrescriptionIds)
                {
                    var prescription = await _prescriptionRepository.GetByIdAsync(prescriptionId, GetTenantId());
                    if (prescription != null)
                    {
                        prescription.MarkAsReportedToSNGPC();
                        await _prescriptionRepository.UpdateAsync(prescription);
                    }
                }

                return Ok(new { message = "Report marked as transmitted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
