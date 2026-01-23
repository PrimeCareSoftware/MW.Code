using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class CompanyTests
    {
        private const string ValidCnpj = "11.222.333/0001-81"; // Valid CNPJ format
        private const string ValidCpf = "123.456.789-09"; // Valid CPF format
        private const string TenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidCnpjData_ShouldCreateCompany()
        {
            // Arrange
            var name = "Test Company Ltd";
            var tradeName = "Test Company";
            var phone = "(11) 98765-4321";
            var email = "test@company.com";

            // Act
            var company = new Company(name, tradeName, ValidCnpj, DocumentType.CNPJ, phone, email, TenantId);

            // Assert
            company.Should().NotBeNull();
            company.Name.Should().Be(name);
            company.TradeName.Should().Be(tradeName);
            company.Document.Should().Be(ValidCnpj);
            company.DocumentType.Should().Be(DocumentType.CNPJ);
            company.Phone.Should().Be(phone);
            company.Email.Should().Be(email);
            company.IsActive.Should().BeTrue();
            company.Subdomain.Should().BeNull();
            company.TenantId.Should().Be(TenantId);
        }

        [Fact]
        public void Constructor_WithValidCpfData_ShouldCreateCompany()
        {
            // Arrange
            var name = "John Doe";
            var tradeName = "John's Clinic";
            var phone = "(11) 98765-4321";
            var email = "john@clinic.com";

            // Act
            var company = new Company(name, tradeName, ValidCpf, DocumentType.CPF, phone, email, TenantId);

            // Assert
            company.Should().NotBeNull();
            company.DocumentType.Should().Be(DocumentType.CPF);
            company.Document.Should().Be(ValidCpf);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithEmptyName_ShouldThrowException(string invalidName)
        {
            // Act
            Action act = () => new Company(invalidName, "Trade Name", ValidCnpj, DocumentType.CNPJ, 
                "(11) 98765-4321", "test@company.com", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Name cannot be empty*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithEmptyTradeName_ShouldThrowException(string invalidTradeName)
        {
            // Act
            Action act = () => new Company("Company Name", invalidTradeName, ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Trade name cannot be empty*");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Constructor_WithEmptyDocument_ShouldThrowException(string invalidDocument)
        {
            // Act
            Action act = () => new Company("Company Name", "Trade Name", invalidDocument, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document cannot be empty*");
        }

        [Theory]
        [InlineData("12345678")] // Too short for CNPJ
        [InlineData("1234567890123456")] // Too long for CNPJ
        [InlineData("00.000.000/0000-00")] // Invalid CNPJ
        public void Constructor_WithInvalidCnpj_ShouldThrowException(string invalidCnpj)
        {
            // Act
            Action act = () => new Company("Company Name", "Trade Name", invalidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData("12345")] // Too short for CPF
        [InlineData("12345678901234")] // Too long for CPF
        [InlineData("000.000.000-00")] // Invalid CPF
        public void Constructor_WithInvalidCpf_ShouldThrowException(string invalidCpf)
        {
            // Act
            Action act = () => new Company("Company Name", "Trade Name", invalidCpf, DocumentType.CPF,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateInfo_WithValidData_ShouldUpdateCompanyInfo()
        {
            // Arrange
            var company = new Company("Old Name", "Old Trade", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "old@company.com", TenantId);
            var oldUpdatedAt = company.UpdatedAt;

            // Wait briefly to ensure timestamp resolution (testing timestamp update behavior)
            System.Threading.Thread.Sleep(10);

            // Act
            company.UpdateInfo("New Name", "New Trade", "(11) 11111-1111", "new@company.com");

            // Assert
            company.Name.Should().Be("New Name");
            company.TradeName.Should().Be("New Trade");
            company.Phone.Should().Be("(11) 11111-1111");
            company.Email.Should().Be("new@company.com");
            company.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
            // Document should remain unchanged
            company.Document.Should().Be(ValidCnpj);
        }

        [Fact]
        public void UpdateInfo_WithEmptyName_ShouldThrowException()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            Action act = () => company.UpdateInfo("", "New Trade", "(11) 11111-1111", "new@company.com");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Name cannot be empty*");
        }

        [Theory]
        [InlineData("test-company")]
        [InlineData("testcompany")]
        [InlineData("test123")]
        [InlineData("test-company-123")]
        public void SetSubdomain_WithValidSubdomain_ShouldSetSubdomain(string validSubdomain)
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            company.SetSubdomain(validSubdomain);

            // Assert
            company.Subdomain.Should().Be(validSubdomain.ToLowerInvariant());
        }

        [Theory]
        [InlineData("ab")] // Too short
        [InlineData("a")] // Too short
        [InlineData("Test-Company")] // Contains uppercase
        [InlineData("test_company")] // Contains underscore
        [InlineData("test company")] // Contains space
        [InlineData("test@company")] // Contains special character
        [InlineData("-testcompany")] // Starts with hyphen
        [InlineData("testcompany-")] // Ends with hyphen
        public void SetSubdomain_WithInvalidSubdomain_ShouldThrowException(string invalidSubdomain)
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            Action act = () => company.SetSubdomain(invalidSubdomain);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void SetSubdomain_WithNull_ShouldClearSubdomain()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);
            company.SetSubdomain("test-subdomain");

            // Act
            company.SetSubdomain(null);

            // Assert
            company.Subdomain.Should().BeNull();
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);
            var oldUpdatedAt = company.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            company.Deactivate();

            // Assert
            company.IsActive.Should().BeFalse();
            company.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);
            company.Deactivate();
            var oldUpdatedAt = company.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            company.Activate();

            // Assert
            company.IsActive.Should().BeTrue();
            company.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void UpdateDocument_WithValidCnpj_ShouldUpdateDocument()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCpf, DocumentType.CPF,
                "(11) 98765-4321", "test@company.com", TenantId);
            var oldUpdatedAt = company.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            company.UpdateDocument(ValidCnpj, DocumentType.CNPJ);

            // Assert
            company.Document.Should().Be(ValidCnpj);
            company.DocumentType.Should().Be(DocumentType.CNPJ);
            company.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void UpdateDocument_WithValidCpf_ShouldUpdateDocument()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            company.UpdateDocument(ValidCpf, DocumentType.CPF);

            // Assert
            company.Document.Should().Be(ValidCpf);
            company.DocumentType.Should().Be(DocumentType.CPF);
        }

        [Fact]
        public void UpdateDocument_WithInvalidDocument_ShouldThrowException()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            Action act = () => company.UpdateDocument("00.000.000/0000-00", DocumentType.CNPJ);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateDocument_WithEmptyDocument_ShouldThrowException()
        {
            // Arrange
            var company = new Company("Company Name", "Trade Name", ValidCnpj, DocumentType.CNPJ,
                "(11) 98765-4321", "test@company.com", TenantId);

            // Act
            Action act = () => company.UpdateDocument("", DocumentType.CNPJ);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Document cannot be empty*");
        }
    }
}
