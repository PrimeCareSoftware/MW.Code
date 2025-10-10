using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum ExpenseCategory
    {
        Rent = 1,               // Aluguel
        Utilities = 2,          // Utilidades (água, luz, internet)
        Supplies = 3,           // Materiais e suprimentos
        Equipment = 4,          // Equipamentos
        Maintenance = 5,        // Manutenção
        Marketing = 6,          // Marketing e publicidade
        Software = 7,           // Software e assinaturas
        Salary = 8,             // Salários e folha de pagamento
        Taxes = 9,              // Impostos e taxas
        Insurance = 10,         // Seguros
        ProfessionalServices = 11, // Serviços profissionais
        Transportation = 12,    // Transporte
        Training = 13,          // Treinamento e educação
        Other = 14              // Outros
    }

    public enum ExpenseStatus
    {
        Pending = 1,            // Pendente
        Paid = 2,               // Pago
        Overdue = 3,            // Vencido
        Cancelled = 4           // Cancelado
    }

    /// <summary>
    /// Represents an expense (accounts payable) for the clinic.
    /// Tracks all outgoing payments and financial obligations.
    /// </summary>
    public class Expense : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public string Description { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime DueDate { get; private set; }
        public DateTime? PaidDate { get; private set; }
        public ExpenseStatus Status { get; private set; }
        public PaymentMethod? PaymentMethod { get; private set; }
        public string? PaymentReference { get; private set; }
        public string? SupplierName { get; private set; }
        public string? SupplierDocument { get; private set; }
        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }

        // Navigation properties
        public Clinic Clinic { get; private set; } = null!;

        private Expense()
        {
            // EF Constructor
            Description = null!;
        }

        public Expense(Guid clinicId, string description, ExpenseCategory category,
            decimal amount, DateTime dueDate, string tenantId,
            string? supplierName = null, string? supplierDocument = null, string? notes = null)
            : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            ClinicId = clinicId;
            Description = description.Trim();
            Category = category;
            Amount = amount;
            DueDate = dueDate.Date; // Store only the date part
            Status = ExpenseStatus.Pending;
            SupplierName = supplierName?.Trim();
            SupplierDocument = supplierDocument?.Trim();
            Notes = notes?.Trim();
        }

        public void Update(string description, ExpenseCategory category, decimal amount,
            DateTime dueDate, string? supplierName = null, string? supplierDocument = null, string? notes = null)
        {
            if (Status == ExpenseStatus.Paid)
                throw new InvalidOperationException("Cannot update a paid expense");

            if (Status == ExpenseStatus.Cancelled)
                throw new InvalidOperationException("Cannot update a cancelled expense");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            Description = description.Trim();
            Category = category;
            Amount = amount;
            DueDate = dueDate.Date;
            SupplierName = supplierName?.Trim();
            SupplierDocument = supplierDocument?.Trim();
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public void MarkAsPaid(PaymentMethod paymentMethod, string? paymentReference = null)
        {
            if (Status == ExpenseStatus.Paid)
                throw new InvalidOperationException("Expense is already marked as paid");

            if (Status == ExpenseStatus.Cancelled)
                throw new InvalidOperationException("Cannot pay a cancelled expense");

            Status = ExpenseStatus.Paid;
            PaidDate = DateTime.UtcNow;
            PaymentMethod = paymentMethod;
            PaymentReference = paymentReference?.Trim();
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == ExpenseStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a paid expense");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = ExpenseStatus.Cancelled;
            CancellationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void CheckOverdue()
        {
            if (Status == ExpenseStatus.Pending && DueDate < DateTime.UtcNow.Date)
            {
                Status = ExpenseStatus.Overdue;
                UpdateTimestamp();
            }
        }

        public bool IsOverdue()
        {
            return Status == ExpenseStatus.Pending && DueDate < DateTime.UtcNow.Date;
        }

        public int DaysOverdue()
        {
            if (!IsOverdue())
                return 0;

            return (DateTime.UtcNow.Date - DueDate).Days;
        }

        public bool IsPending()
        {
            return Status == ExpenseStatus.Pending;
        }

        public bool IsPaid()
        {
            return Status == ExpenseStatus.Paid;
        }

        public bool CanBePaid()
        {
            return Status == ExpenseStatus.Pending || Status == ExpenseStatus.Overdue;
        }

        public bool CanBeCancelled()
        {
            return Status != ExpenseStatus.Paid;
        }
    }
}
