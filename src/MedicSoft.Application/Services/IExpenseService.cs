using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    public interface IExpenseService
    {
        Task<List<ExpenseDto>> GetAllExpensesAsync(string tenantId, Guid? clinicId = null, string? status = null, string? category = null);
        Task<ExpenseDto?> GetExpenseByIdAsync(Guid id, string tenantId);
        Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, string tenantId);
        Task UpdateExpenseAsync(Guid id, UpdateExpenseDto dto, string tenantId);
        Task MarkExpenseAsPaidAsync(Guid id, PayExpenseDto dto, string tenantId);
        Task CancelExpenseAsync(Guid id, CancelExpenseDto dto, string tenantId);
        Task DeleteExpenseAsync(Guid id, string tenantId);
    }
}
