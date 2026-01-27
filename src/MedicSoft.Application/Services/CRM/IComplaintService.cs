using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.Services.CRM
{
    public interface IComplaintService
    {
        // CRUD operations
        Task<ComplaintDto> CreateAsync(CreateComplaintDto dto, string tenantId);
        Task<ComplaintDto> UpdateAsync(Guid id, UpdateComplaintDto dto, string tenantId);
        Task<bool> DeleteAsync(Guid id, string tenantId);
        Task<ComplaintDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<ComplaintDto>> GetAllAsync(string tenantId);
        
        // Interaction management
        Task<ComplaintInteractionDto> AddInteractionAsync(Guid complaintId, AddComplaintInteractionDto dto, Guid userId, string userName, string tenantId);
        
        // Status management
        Task<ComplaintDto> UpdateStatusAsync(Guid complaintId, ComplaintStatus status, string tenantId);
        
        // Assignment
        Task<ComplaintDto> AssignToUserAsync(Guid complaintId, Guid userId, string userName, string tenantId);
        
        // Filtering and search
        Task<ComplaintDto?> GetByProtocolNumberAsync(string protocolNumber, string tenantId);
        Task<IEnumerable<ComplaintDto>> GetByCategoryAsync(ComplaintCategory category, string tenantId);
        Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status, string tenantId);
        Task<IEnumerable<ComplaintDto>> GetByPriorityAsync(ComplaintPriority priority, string tenantId);
        
        // Dashboard and metrics
        Task<ComplaintDashboardDto> GetDashboardMetricsAsync(string tenantId);
    }
}
