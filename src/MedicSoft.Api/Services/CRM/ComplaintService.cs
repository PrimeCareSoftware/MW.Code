using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
{
    public class ComplaintService : IComplaintService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ComplaintService> _logger;

        public ComplaintService(
            MedicSoftDbContext context,
            ILogger<ComplaintService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ComplaintDto> CreateAsync(CreateComplaintDto dto, string tenantId)
        {
            var protocolNumber = await GenerateProtocolNumberAsync(tenantId);
            
            var priority = dto.Priority ?? CalculatePriority(dto.Category);
            
            var complaint = new Complaint(
                protocolNumber,
                dto.PatientId,
                dto.Subject,
                dto.Description,
                dto.Category,
                tenantId);
            
            complaint.SetPriority(priority);
            
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Created complaint {ProtocolNumber} for patient {PatientId} in tenant {TenantId}",
                protocolNumber, dto.PatientId, tenantId);
            
            return await MapToDtoAsync(complaint);
        }

        public async Task<ComplaintDto> UpdateAsync(Guid id, UpdateComplaintDto dto, string tenantId)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
            
            if (complaint == null)
                throw new KeyNotFoundException($"Complaint {id} not found");
            
            if (dto.Priority.HasValue)
            {
                complaint.SetPriority(dto.Priority.Value);
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated complaint {ComplaintId}", id);
            
            return await MapToDtoAsync(complaint);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
            
            if (complaint == null)
                return false;
            
            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Deleted complaint {ComplaintId}", id);
            
            return true;
        }

        public async Task<ComplaintDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
            
            return complaint == null ? null : await MapToDtoAsync(complaint);
        }

        public async Task<IEnumerable<ComplaintDto>> GetAllAsync(string tenantId)
        {
            var complaints = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.ReceivedAt)
                .ToListAsync();
            
            return await Task.WhenAll(complaints.Select(MapToDtoAsync));
        }

        public async Task<ComplaintInteractionDto> AddInteractionAsync(
            Guid complaintId, 
            AddComplaintInteractionDto dto, 
            Guid userId, 
            string userName, 
            string tenantId)
        {
            var complaint = await _context.Complaints
                .FirstOrDefaultAsync(c => c.Id == complaintId && c.TenantId == tenantId);
            
            if (complaint == null)
                throw new KeyNotFoundException($"Complaint {complaintId} not found");
            
            var interaction = new ComplaintInteraction(
                complaintId,
                userId,
                userName,
                dto.Message,
                dto.IsInternal,
                tenantId);
            
            complaint.AddInteraction(interaction);
            _context.ComplaintInteractions.Add(interaction);
            
            if (complaint.Status == ComplaintStatus.Received && !dto.IsInternal)
            {
                complaint.UpdateStatus(ComplaintStatus.InProgress);
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Added interaction to complaint {ComplaintId} by user {UserId}",
                complaintId, userId);
            
            return MapInteractionToDto(interaction);
        }

        public async Task<ComplaintDto> UpdateStatusAsync(Guid complaintId, ComplaintStatus status, string tenantId)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.Id == complaintId && c.TenantId == tenantId);
            
            if (complaint == null)
                throw new KeyNotFoundException($"Complaint {complaintId} not found");
            
            complaint.UpdateStatus(status);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Updated status of complaint {ComplaintId} to {Status}",
                complaintId, status);
            
            return await MapToDtoAsync(complaint);
        }

        public async Task<ComplaintDto> AssignToUserAsync(Guid complaintId, Guid userId, string userName, string tenantId)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.Id == complaintId && c.TenantId == tenantId);
            
            if (complaint == null)
                throw new KeyNotFoundException($"Complaint {complaintId} not found");
            
            complaint.AssignTo(userId, userName);
            
            if (complaint.Status == ComplaintStatus.Received)
            {
                complaint.UpdateStatus(ComplaintStatus.InAnalysis);
            }
            
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Assigned complaint {ComplaintId} to user {UserId}",
                complaintId, userId);
            
            return await MapToDtoAsync(complaint);
        }

        public async Task<ComplaintDto?> GetByProtocolNumberAsync(string protocolNumber, string tenantId)
        {
            var complaint = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .FirstOrDefaultAsync(c => c.ProtocolNumber == protocolNumber && c.TenantId == tenantId);
            
            return complaint == null ? null : await MapToDtoAsync(complaint);
        }

        public async Task<IEnumerable<ComplaintDto>> GetByCategoryAsync(ComplaintCategory category, string tenantId)
        {
            var complaints = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .Where(c => c.Category == category && c.TenantId == tenantId)
                .OrderByDescending(c => c.ReceivedAt)
                .ToListAsync();
            
            return await Task.WhenAll(complaints.Select(MapToDtoAsync));
        }

        public async Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status, string tenantId)
        {
            var complaints = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .Where(c => c.Status == status && c.TenantId == tenantId)
                .OrderByDescending(c => c.ReceivedAt)
                .ToListAsync();
            
            return await Task.WhenAll(complaints.Select(MapToDtoAsync));
        }

        public async Task<IEnumerable<ComplaintDto>> GetByPriorityAsync(ComplaintPriority priority, string tenantId)
        {
            var complaints = await _context.Complaints
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .Where(c => c.Priority == priority && c.TenantId == tenantId)
                .OrderByDescending(c => c.ReceivedAt)
                .ToListAsync();
            
            return await Task.WhenAll(complaints.Select(MapToDtoAsync));
        }

        public async Task<ComplaintDashboardDto> GetDashboardMetricsAsync(string tenantId)
        {
            var complaints = await _context.Complaints
                .Where(c => c.TenantId == tenantId)
                .Include(c => c.Patient)
                .Include(c => c.Interactions)
                .ToListAsync();
            
            var dashboard = new ComplaintDashboardDto
            {
                TotalComplaints = complaints.Count,
                OpenComplaints = complaints.Count(c => c.Status == ComplaintStatus.Received),
                InProgressComplaints = complaints.Count(c => c.Status == ComplaintStatus.InProgress || c.Status == ComplaintStatus.InAnalysis),
                ResolvedComplaints = complaints.Count(c => c.Status == ComplaintStatus.Resolved),
                ClosedComplaints = complaints.Count(c => c.Status == ComplaintStatus.Closed)
            };
            
            foreach (ComplaintCategory category in Enum.GetValues(typeof(ComplaintCategory)))
            {
                dashboard.ComplaintsByCategory[category] = complaints.Count(c => c.Category == category);
            }
            
            foreach (ComplaintPriority priority in Enum.GetValues(typeof(ComplaintPriority)))
            {
                dashboard.ComplaintsByPriority[priority] = complaints.Count(c => c.Priority == priority);
            }
            
            foreach (ComplaintStatus status in Enum.GetValues(typeof(ComplaintStatus)))
            {
                dashboard.ComplaintsByStatus[status] = complaints.Count(c => c.Status == status);
            }
            
            var complaintsWithResponseTime = complaints.Where(c => c.ResponseTimeMinutes.HasValue).ToList();
            if (complaintsWithResponseTime.Any())
            {
                dashboard.AverageResponseTimeMinutes = complaintsWithResponseTime.Average(c => c.ResponseTimeMinutes!.Value);
            }
            
            var complaintsWithResolutionTime = complaints.Where(c => c.ResolutionTimeMinutes.HasValue).ToList();
            if (complaintsWithResolutionTime.Any())
            {
                dashboard.AverageResolutionTimeMinutes = complaintsWithResolutionTime.Average(c => c.ResolutionTimeMinutes!.Value);
            }
            
            var slaResponseTimeMinutes = 240;
            var slaResolutionTimeMinutes = 2880;
            
            dashboard.ComplaintsWithinSLA = complaints.Count(c =>
                (!c.ResponseTimeMinutes.HasValue || c.ResponseTimeMinutes.Value <= slaResponseTimeMinutes) &&
                (!c.ResolutionTimeMinutes.HasValue || c.ResolutionTimeMinutes.Value <= slaResolutionTimeMinutes));
            
            dashboard.ComplaintsOutsideSLA = complaints.Count(c =>
                (c.ResponseTimeMinutes.HasValue && c.ResponseTimeMinutes.Value > slaResponseTimeMinutes) ||
                (c.ResolutionTimeMinutes.HasValue && c.ResolutionTimeMinutes.Value > slaResolutionTimeMinutes));
            
            if (complaints.Any())
            {
                dashboard.SLAComplianceRate = (double)dashboard.ComplaintsWithinSLA / complaints.Count * 100;
            }
            
            var complaintsWithRating = complaints.Where(c => c.SatisfactionRating.HasValue).ToList();
            if (complaintsWithRating.Any())
            {
                dashboard.AverageSatisfactionRating = complaintsWithRating.Average(c => c.SatisfactionRating!.Value);
                dashboard.TotalRatings = complaintsWithRating.Count;
            }
            
            var recentComplaints = complaints
                .OrderByDescending(c => c.ReceivedAt)
                .Take(10)
                .ToList();
            dashboard.RecentComplaints = (await Task.WhenAll(recentComplaints.Select(MapToDtoAsync))).ToList();
            
            var urgentComplaints = complaints
                .Where(c => c.Priority == ComplaintPriority.Urgent && 
                           c.Status != ComplaintStatus.Resolved && 
                           c.Status != ComplaintStatus.Closed)
                .OrderByDescending(c => c.ReceivedAt)
                .Take(10)
                .ToList();
            dashboard.UrgentComplaints = (await Task.WhenAll(urgentComplaints.Select(MapToDtoAsync))).ToList();
            
            return dashboard;
        }

        private async Task<string> GenerateProtocolNumberAsync(string tenantId)
        {
            var year = DateTime.UtcNow.Year;
            var prefix = $"CMP-{year}-";
            
            var lastComplaint = await _context.Complaints
                .Where(c => c.TenantId == tenantId && c.ProtocolNumber.StartsWith(prefix))
                .OrderByDescending(c => c.ProtocolNumber)
                .FirstOrDefaultAsync();
            
            int nextNumber = 1;
            if (lastComplaint != null)
            {
                var lastNumberStr = lastComplaint.ProtocolNumber.Substring(prefix.Length);
                if (int.TryParse(lastNumberStr, out var lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            
            return $"{prefix}{nextNumber:D6}";
        }

        private ComplaintPriority CalculatePriority(ComplaintCategory category)
        {
            return category switch
            {
                ComplaintCategory.MedicalTreatment => ComplaintPriority.Urgent,
                ComplaintCategory.HealthcareProfessional => ComplaintPriority.High,
                ComplaintCategory.Service => ComplaintPriority.High,
                ComplaintCategory.Billing => ComplaintPriority.Medium,
                ComplaintCategory.Scheduling => ComplaintPriority.Medium,
                ComplaintCategory.WaitTime => ComplaintPriority.Medium,
                ComplaintCategory.Facilities => ComplaintPriority.Low,
                _ => ComplaintPriority.Medium
            };
        }

        private async Task<ComplaintDto> MapToDtoAsync(Complaint complaint)
        {
            var patient = complaint.Patient ?? await _context.Patients
                .FirstOrDefaultAsync(p => p.Id == complaint.PatientId && p.TenantId == complaint.TenantId);
            
            var interactions = complaint.Interactions
                .OrderBy(i => i.InteractionDate)
                .ToList();
            
            return new ComplaintDto
            {
                Id = complaint.Id,
                ProtocolNumber = complaint.ProtocolNumber,
                PatientId = complaint.PatientId,
                PatientName = patient?.Name ?? "Unknown",
                Subject = complaint.Subject,
                Description = complaint.Description,
                Category = complaint.Category,
                CategoryName = complaint.Category.ToString(),
                Priority = complaint.Priority,
                PriorityName = complaint.Priority.ToString(),
                Status = complaint.Status,
                StatusName = complaint.Status.ToString(),
                AssignedToUserId = complaint.AssignedToUserId,
                AssignedToUserName = complaint.AssignedToUserName,
                ReceivedAt = complaint.ReceivedAt,
                FirstResponseAt = complaint.FirstResponseAt,
                ResolvedAt = complaint.ResolvedAt,
                ClosedAt = complaint.ClosedAt,
                ResponseTimeMinutes = complaint.ResponseTimeMinutes,
                ResolutionTimeMinutes = complaint.ResolutionTimeMinutes,
                SatisfactionRating = complaint.SatisfactionRating,
                SatisfactionFeedback = complaint.SatisfactionFeedback,
                Interactions = interactions.Select(MapInteractionToDto).ToList(),
                CreatedAt = complaint.CreatedAt,
                UpdatedAt = complaint.UpdatedAt ?? complaint.CreatedAt
            };
        }

        private ComplaintInteractionDto MapInteractionToDto(ComplaintInteraction interaction)
        {
            return new ComplaintInteractionDto
            {
                Id = interaction.Id,
                ComplaintId = interaction.ComplaintId,
                UserId = interaction.UserId,
                UserName = interaction.UserName,
                Message = interaction.Message,
                IsInternal = interaction.IsInternal,
                InteractionDate = interaction.InteractionDate
            };
        }
    }
}
