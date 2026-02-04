using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.DTOs.Common;
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
        Task<PagedResult<ComplaintDto>> GetAllPagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25);
        
        // Interaction management
        Task<ComplaintInteractionDto> AddInteractionAsync(Guid complaintId, AddComplaintInteractionDto dto, Guid userId, string userName, string tenantId);
        
        // Status management
        Task<ComplaintDto> UpdateStatusAsync(Guid complaintId, ComplaintStatus status, string tenantId);
        
        // Assignment
        Task<ComplaintDto> AssignToUserAsync(Guid complaintId, Guid userId, string userName, string tenantId);
        
        // Filtering and search
        Task<ComplaintDto?> GetByProtocolNumberAsync(string protocolNumber, string tenantId);
        Task<IEnumerable<ComplaintDto>> GetByCategoryAsync(ComplaintCategory category, string tenantId);
        Task<PagedResult<ComplaintDto>> GetByCategoryPagedAsync(ComplaintCategory category, string tenantId, int pageNumber = 1, int pageSize = 25);
        Task<IEnumerable<ComplaintDto>> GetByStatusAsync(ComplaintStatus status, string tenantId);
        Task<PagedResult<ComplaintDto>> GetByStatusPagedAsync(ComplaintStatus status, string tenantId, int pageNumber = 1, int pageSize = 25);
        Task<IEnumerable<ComplaintDto>> GetByPriorityAsync(ComplaintPriority priority, string tenantId);
        Task<PagedResult<ComplaintDto>> GetByPriorityPagedAsync(ComplaintPriority priority, string tenantId, int pageNumber = 1, int pageSize = 25);
        
        // Dashboard and metrics
        Task<ComplaintDashboardDto> GetDashboardMetricsAsync(string tenantId);
    }
}
