using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um fornecedor de produtos/serviços
    /// </summary>
    public class Supplier : BaseEntity
    {
        public string Name { get; private set; }
        public string? TradeName { get; private set; }
        public string? DocumentNumber { get; private set; } // CNPJ/CPF
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public string? Address { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? ZipCode { get; private set; }
        public string? BankName { get; private set; }
        public string? BankAccount { get; private set; }
        public string? PixKey { get; private set; }
        public string? Notes { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Supplier()
        {
            // EF Constructor
            Name = null!;
        }

        public Supplier(
            string name,
            string tenantId,
            string? tradeName = null,
            string? documentNumber = null,
            string? email = null,
            string? phone = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            TradeName = tradeName?.Trim();
            DocumentNumber = documentNumber?.Trim();
            Email = email?.Trim();
            Phone = phone?.Trim();
        }

        public void UpdateInfo(
            string name,
            string? tradeName = null,
            string? documentNumber = null,
            string? email = null,
            string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            TradeName = tradeName?.Trim();
            DocumentNumber = documentNumber?.Trim();
            Email = email?.Trim();
            Phone = phone?.Trim();
            UpdateTimestamp();
        }

        public void UpdateAddress(
            string? address = null,
            string? city = null,
            string? state = null,
            string? zipCode = null)
        {
            Address = address?.Trim();
            City = city?.Trim();
            State = state?.Trim();
            ZipCode = zipCode?.Trim();
            UpdateTimestamp();
        }

        public void SetBankingInfo(string? bankName = null, string? bankAccount = null, string? pixKey = null)
        {
            BankName = bankName?.Trim();
            BankAccount = bankAccount?.Trim();
            PixKey = pixKey?.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }
    }
}
