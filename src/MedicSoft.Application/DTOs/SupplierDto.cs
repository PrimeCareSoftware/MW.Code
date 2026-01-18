using System;

namespace MedicSoft.Application.DTOs
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public string? TradeName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
        public string? Notes { get; set; }
    }
}
