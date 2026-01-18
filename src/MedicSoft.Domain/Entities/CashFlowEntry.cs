using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Tipo de movimentação no fluxo de caixa
    /// </summary>
    public enum CashFlowType
    {
        Income = 1,   // Entrada
        Expense = 2   // Saída
    }

    /// <summary>
    /// Categoria de movimentação no fluxo de caixa
    /// </summary>
    public enum CashFlowCategory
    {
        // Entradas
        ConsultationPayment = 1,      // Pagamento de consulta
        ProcedurePayment = 2,         // Pagamento de procedimento
        ExamPayment = 3,              // Pagamento de exame
        HealthInsurancePayment = 4,   // Recebimento de convênio
        OtherIncome = 5,              // Outras receitas
        
        // Saídas
        Rent = 101,                   // Aluguel
        Salaries = 102,               // Salários
        Supplies = 103,               // Materiais e suprimentos
        Equipment = 104,              // Equipamentos
        Maintenance = 105,            // Manutenção
        Utilities = 106,              // Utilidades (água, luz, etc.)
        Marketing = 107,              // Marketing
        Insurance = 108,              // Seguros
        Taxes = 109,                  // Impostos
        ProfessionalServices = 110,   // Serviços profissionais
        Laboratory = 111,             // Laboratório
        Pharmacy = 112,               // Farmácia
        OtherExpense = 199            // Outras despesas
    }

    /// <summary>
    /// Representa uma entrada no fluxo de caixa
    /// </summary>
    public class CashFlowEntry : BaseEntity
    {
        public CashFlowType Type { get; private set; }
        public CashFlowCategory Category { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Description { get; private set; }
        public string? Reference { get; private set; }  // Número de documento, referência externa
        public string? Notes { get; private set; }
        
        // Referências opcionais a outras entidades
        public Guid? PaymentId { get; private set; }
        public Guid? ReceivableId { get; private set; }
        public Guid? PayableId { get; private set; }
        public Guid? AppointmentId { get; private set; }
        
        // Informações bancárias
        public string? BankAccount { get; private set; }
        public string? PaymentMethod { get; private set; }
        
        // Navigation properties
        public Payment? Payment { get; private set; }
        public AccountsReceivable? Receivable { get; private set; }
        public AccountsPayable? Payable { get; private set; }
        public Appointment? Appointment { get; private set; }

        private CashFlowEntry()
        {
            // EF Constructor
            Description = null!;
        }

        public CashFlowEntry(
            CashFlowType type,
            CashFlowCategory category,
            DateTime transactionDate,
            decimal amount,
            string description,
            string tenantId,
            string? reference = null,
            Guid? paymentId = null,
            Guid? receivableId = null,
            Guid? payableId = null,
            Guid? appointmentId = null) : base(tenantId)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            // Validar que o tipo e categoria são compatíveis
            if (type == CashFlowType.Income && (int)category >= 100)
                throw new ArgumentException("Income type must use income categories (< 100)", nameof(category));

            if (type == CashFlowType.Expense && (int)category < 100)
                throw new ArgumentException("Expense type must use expense categories (>= 100)", nameof(category));

            Type = type;
            Category = category;
            TransactionDate = transactionDate;
            Amount = amount;
            Description = description.Trim();
            Reference = reference?.Trim();
            PaymentId = paymentId;
            ReceivableId = receivableId;
            PayableId = payableId;
            AppointmentId = appointmentId;
        }

        public void UpdateBankingInfo(string? bankAccount = null, string? paymentMethod = null)
        {
            BankAccount = bankAccount?.Trim();
            PaymentMethod = paymentMethod?.Trim();
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            Description = description.Trim();
            UpdateTimestamp();
        }
    }
}
