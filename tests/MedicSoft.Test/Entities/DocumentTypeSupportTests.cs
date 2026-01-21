using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Test.Entities
{
    /// <summary>
    /// Tests for CPF/CNPJ document type support in Clinic and Owner entities
    /// </summary>
    public class DocumentTypeSupportTests
    {
        private readonly string _tenantId = "test-tenant";

        #region Clinic Tests

        [Fact]
        public void Clinic_Constructor_WithValidCPF_CreatesClinic()
        {
            // Arrange
            var name = "Dr. João Silva - Consultório";
            var cpf = "12345678909"; // Valid CPF format (11 digits)
            var phone = "+55 11 98765-4321";
            var email = "joao@example.com";
            var address = "Rua Principal, 456";
            var openingTime = new TimeSpan(9, 0, 0);
            var closingTime = new TimeSpan(17, 0, 0);

            // Act
            var clinic = new Clinic(name, name, cpf, phone, email, address, 
                openingTime, closingTime, _tenantId, 30, DocumentType.CPF);

            // Assert
            Assert.NotEqual(Guid.Empty, clinic.Id);
            Assert.Equal(cpf, clinic.Document);
            Assert.Equal(DocumentType.CPF, clinic.DocumentType);
        }

        [Fact]
        public void Clinic_Constructor_WithValidCNPJ_CreatesClinic()
        {
            // Arrange
            var name = "Clínica Médica XYZ";
            var cnpj = "11222333000181"; // Valid CNPJ format (14 digits)
            var phone = "+55 11 3333-4444";
            var email = "clinica@example.com";
            var address = "Av. Comercial, 789";
            var openingTime = new TimeSpan(8, 0, 0);
            var closingTime = new TimeSpan(18, 0, 0);

            // Act
            var clinic = new Clinic(name, name, cnpj, phone, email, address, 
                openingTime, closingTime, _tenantId, 30, DocumentType.CNPJ);

            // Assert
            Assert.NotEqual(Guid.Empty, clinic.Id);
            Assert.Equal(cnpj, clinic.Document);
            Assert.Equal(DocumentType.CNPJ, clinic.DocumentType);
        }

        [Fact]
        public void Clinic_Constructor_WithFormattedCPF_CreatesClinic()
        {
            // Arrange
            var cpf = "123.456.789-09"; // CPF with formatting
            
            // Act
            var clinic = new Clinic("Test", "Test", cpf, "+55 11 98765-4321", 
                "test@example.com", "Address", new TimeSpan(9, 0, 0), 
                new TimeSpan(17, 0, 0), _tenantId, 30, DocumentType.CPF);

            // Assert
            Assert.Equal(cpf, clinic.Document);
            Assert.Equal(DocumentType.CPF, clinic.DocumentType);
        }

        [Fact]
        public void Clinic_Constructor_WithFormattedCNPJ_CreatesClinic()
        {
            // Arrange
            var cnpj = "11.222.333/0001-81"; // CNPJ with formatting
            
            // Act
            var clinic = new Clinic("Test", "Test", cnpj, "+55 11 3333-4444", 
                "test@example.com", "Address", new TimeSpan(8, 0, 0), 
                new TimeSpan(18, 0, 0), _tenantId, 30, DocumentType.CNPJ);

            // Assert
            Assert.Equal(cnpj, clinic.Document);
            Assert.Equal(DocumentType.CNPJ, clinic.DocumentType);
        }

        [Fact]
        public void Clinic_UpdateDocument_FromCPFToCNPJ_UpdatesDocument()
        {
            // Arrange
            var clinic = new Clinic("Test Clinic", "Test", "12345678909", 
                "+55 11 98765-4321", "test@example.com", "Address", 
                new TimeSpan(9, 0, 0), new TimeSpan(17, 0, 0), _tenantId, 30, DocumentType.CPF);
            
            var newCnpj = "11222333000181";

            // Act
            clinic.UpdateDocument(newCnpj, DocumentType.CNPJ);

            // Assert
            Assert.Equal(newCnpj, clinic.Document);
            Assert.Equal(DocumentType.CNPJ, clinic.DocumentType);
        }

        [Fact]
        public void Clinic_Constructor_WithInvalidCPFLength_ThrowsArgumentException()
        {
            // Arrange
            var invalidCpf = "123456789"; // Only 9 digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Test", "Test", invalidCpf, "+55 11 98765-4321", 
                    "test@example.com", "Address", new TimeSpan(9, 0, 0), 
                    new TimeSpan(17, 0, 0), _tenantId, 30, DocumentType.CPF));

            Assert.Contains("CPF must have 11 digits", exception.Message);
        }

        [Fact]
        public void Clinic_Constructor_WithInvalidCNPJLength_ThrowsArgumentException()
        {
            // Arrange
            var invalidCnpj = "1122233300"; // Only 10 digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Clinic("Test", "Test", invalidCnpj, "+55 11 3333-4444", 
                    "test@example.com", "Address", new TimeSpan(8, 0, 0), 
                    new TimeSpan(18, 0, 0), _tenantId, 30, DocumentType.CNPJ));

            Assert.Contains("CNPJ must have 14 digits", exception.Message);
        }

        #endregion

        #region Owner Tests

        [Fact]
        public void Owner_Constructor_WithValidCPF_CreatesOwner()
        {
            // Arrange
            var username = "joaosilva";
            var email = "joao@example.com";
            var passwordHash = "hashed_password_123";
            var fullName = "João Silva";
            var phone = "+55 11 98765-4321";
            var cpf = "12345678909";
            var clinicId = Guid.NewGuid();

            // Act
            var owner = new Owner(username, email, passwordHash, fullName, phone, 
                _tenantId, clinicId, null, null, cpf, DocumentType.CPF);

            // Assert
            Assert.NotEqual(Guid.Empty, owner.Id);
            Assert.Equal(cpf, owner.Document);
            Assert.Equal(DocumentType.CPF, owner.DocumentType);
        }

        [Fact]
        public void Owner_Constructor_WithValidCNPJ_CreatesOwner()
        {
            // Arrange
            var username = "empresaxyz";
            var email = "contato@empresaxyz.com";
            var passwordHash = "hashed_password_456";
            var fullName = "Empresa XYZ Ltda";
            var phone = "+55 11 3333-4444";
            var cnpj = "11222333000181";
            var clinicId = Guid.NewGuid();

            // Act
            var owner = new Owner(username, email, passwordHash, fullName, phone, 
                _tenantId, clinicId, null, null, cnpj, DocumentType.CNPJ);

            // Assert
            Assert.NotEqual(Guid.Empty, owner.Id);
            Assert.Equal(cnpj, owner.Document);
            Assert.Equal(DocumentType.CNPJ, owner.DocumentType);
        }

        [Fact]
        public void Owner_Constructor_WithFormattedCPF_CreatesOwner()
        {
            // Arrange
            var cpf = "123.456.789-09";
            var clinicId = Guid.NewGuid();

            // Act
            var owner = new Owner("test", "test@example.com", "hash", "Test User", 
                "+55 11 98765-4321", _tenantId, clinicId, null, null, cpf, DocumentType.CPF);

            // Assert
            Assert.Equal(cpf, owner.Document);
            Assert.Equal(DocumentType.CPF, owner.DocumentType);
        }

        [Fact]
        public void Owner_Constructor_WithNullDocument_CreatesOwnerWithoutDocument()
        {
            // Arrange
            var clinicId = Guid.NewGuid();

            // Act
            var owner = new Owner("test", "test@example.com", "hash", "Test User", 
                "+55 11 98765-4321", _tenantId, clinicId);

            // Assert
            Assert.Null(owner.Document);
            Assert.Null(owner.DocumentType);
        }

        [Fact]
        public void Owner_Constructor_WithDocumentButNoType_ThrowsArgumentException()
        {
            // Arrange
            var cpf = "12345678909";
            var clinicId = Guid.NewGuid();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Owner("test", "test@example.com", "hash", "Test User", 
                    "+55 11 98765-4321", _tenantId, clinicId, null, null, cpf, null));

            Assert.Contains("Document type must be specified", exception.Message);
        }

        [Fact]
        public void Owner_UpdateDocument_FromCPFToCNPJ_UpdatesDocument()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var owner = new Owner("test", "test@example.com", "hash", "Test User", 
                "+55 11 98765-4321", _tenantId, clinicId, null, null, "12345678909", DocumentType.CPF);
            
            var newCnpj = "11222333000181";

            // Act
            owner.UpdateDocument(newCnpj, DocumentType.CNPJ);

            // Assert
            Assert.Equal(newCnpj, owner.Document);
            Assert.Equal(DocumentType.CNPJ, owner.DocumentType);
        }

        [Fact]
        public void Owner_UpdateDocument_WithInvalidCPF_ThrowsArgumentException()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var owner = new Owner("test", "test@example.com", "hash", "Test User", 
                "+55 11 98765-4321", _tenantId, clinicId);
            
            var invalidCpf = "123456789"; // Only 9 digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                owner.UpdateDocument(invalidCpf, DocumentType.CPF));

            Assert.Contains("CPF must have 11 digits", exception.Message);
        }

        [Fact]
        public void Owner_UpdateDocument_WithInvalidCNPJ_ThrowsArgumentException()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var owner = new Owner("test", "test@example.com", "hash", "Test User", 
                "+55 11 3333-4444", _tenantId, clinicId);
            
            var invalidCnpj = "1122233300"; // Only 10 digits

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                owner.UpdateDocument(invalidCnpj, DocumentType.CNPJ));

            Assert.Contains("CNPJ must have 14 digits", exception.Message);
        }

        #endregion
    }
}
