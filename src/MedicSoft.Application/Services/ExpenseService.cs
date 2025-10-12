using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;

        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }

        public async Task<List<ExpenseDto>> GetAllExpensesAsync(string tenantId, Guid? clinicId = null, string? status = null, string? category = null)
        {
            var expenses = await _expenseRepository.GetAllAsync(tenantId);

            // Apply filters
            if (clinicId.HasValue)
                expenses = expenses.Where(e => e.ClinicId == clinicId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ExpenseStatus>(status, out var expenseStatus))
                expenses = expenses.Where(e => e.Status == expenseStatus);

            if (!string.IsNullOrEmpty(category) && Enum.TryParse<ExpenseCategory>(category, out var expenseCategory))
                expenses = expenses.Where(e => e.Category == expenseCategory);

            return expenses
                .OrderByDescending(e => e.DueDate)
                .Select(e => MapToDto(e))
                .ToList();
        }

        public async Task<ExpenseDto?> GetExpenseByIdAsync(Guid id, string tenantId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, tenantId);
            return expense != null ? MapToDto(expense) : null;
        }

        public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, string tenantId)
        {
            if (!Enum.TryParse<ExpenseCategory>(dto.Category, out var category))
                throw new ArgumentException("Invalid expense category");

            var expense = new Expense(
                dto.ClinicId,
                dto.Description,
                category,
                dto.Amount,
                dto.DueDate,
                tenantId,
                dto.SupplierName,
                dto.SupplierDocument,
                dto.Notes
            );

            await _expenseRepository.AddAsync(expense);
            return MapToDto(expense);
        }

        public async Task UpdateExpenseAsync(Guid id, UpdateExpenseDto dto, string tenantId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, tenantId);
            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            if (!Enum.TryParse<ExpenseCategory>(dto.Category, out var category))
                throw new ArgumentException("Invalid expense category");

            expense.Update(
                dto.Description,
                category,
                dto.Amount,
                dto.DueDate,
                dto.SupplierName,
                dto.SupplierDocument,
                dto.Notes
            );

            await _expenseRepository.UpdateAsync(expense);
        }

        public async Task MarkExpenseAsPaidAsync(Guid id, PayExpenseDto dto, string tenantId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, tenantId);
            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, out var paymentMethod))
                throw new ArgumentException("Invalid payment method");

            expense.MarkAsPaid(paymentMethod, dto.PaymentReference);
            await _expenseRepository.UpdateAsync(expense);
        }

        public async Task CancelExpenseAsync(Guid id, CancelExpenseDto dto, string tenantId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, tenantId);
            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            expense.Cancel(dto.Reason);
            await _expenseRepository.UpdateAsync(expense);
        }

        public async Task DeleteExpenseAsync(Guid id, string tenantId)
        {
            var expense = await _expenseRepository.GetByIdAsync(id, tenantId);
            if (expense == null)
                throw new KeyNotFoundException("Expense not found");

            await _expenseRepository.DeleteAsync(id, tenantId);
        }

        private ExpenseDto MapToDto(Expense expense)
        {
            return new ExpenseDto
            {
                Id = expense.Id,
                ClinicId = expense.ClinicId,
                Description = expense.Description,
                Category = expense.Category.ToString(),
                Amount = expense.Amount,
                DueDate = expense.DueDate,
                PaidDate = expense.PaidDate,
                Status = expense.Status.ToString(),
                PaymentMethod = expense.PaymentMethod?.ToString(),
                PaymentReference = expense.PaymentReference,
                SupplierName = expense.SupplierName,
                SupplierDocument = expense.SupplierDocument,
                Notes = expense.Notes,
                CancellationReason = expense.CancellationReason,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt,
                DaysOverdue = expense.IsOverdue() ? expense.DaysOverdue() : null
            };
        }
    }
}
