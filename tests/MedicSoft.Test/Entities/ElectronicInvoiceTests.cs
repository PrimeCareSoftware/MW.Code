using System;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class ElectronicInvoiceTests
    {
        private const string TestTenantId = "test-tenant";
        private const string TestCnpj = "12.345.678/0001-90";
        private const string TestCpf = "123.456.789-00";

        [Fact]
        public void Constructor_WithValidData_CreatesElectronicInvoice()
        {
            // Arrange & Act
            var invoice = CreateValidInvoice();

            // Assert
            Assert.Equal(ElectronicInvoiceType.NFSe, invoice.Type);
            Assert.Equal("1", invoice.Series);
            Assert.Equal(TestCnpj, invoice.ProviderCnpj);
            Assert.Equal("Test Clinic", invoice.ProviderName);
            Assert.Equal(TestCpf, invoice.ClientCpfCnpj);
            Assert.Equal("John Doe", invoice.ClientName);
            Assert.Equal("Medical consultation", invoice.ServiceDescription);
            Assert.Equal(100.00m, invoice.ServiceAmount);
            Assert.Equal(ElectronicInvoiceStatus.Draft, invoice.Status);
        }

        [Fact]
        public void Constructor_WithEmptyProviderCnpj_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ElectronicInvoice(
                    ElectronicInvoiceType.NFSe,
                    "1",
                    "",
                    "Test Clinic",
                    TestCpf,
                    "John Doe",
                    "Medical consultation",
                    100.00m,
                    TestTenantId
                ));
        }

        [Fact]
        public void Constructor_WithEmptyProviderName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ElectronicInvoice(
                    ElectronicInvoiceType.NFSe,
                    "1",
                    TestCnpj,
                    "",
                    TestCpf,
                    "John Doe",
                    "Medical consultation",
                    100.00m,
                    TestTenantId
                ));
        }

        [Fact]
        public void Constructor_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ElectronicInvoice(
                    ElectronicInvoiceType.NFSe,
                    "1",
                    TestCnpj,
                    "Test Clinic",
                    TestCpf,
                    "John Doe",
                    "Medical consultation",
                    0m,
                    TestTenantId
                ));
        }

        [Fact]
        public void CalculateTaxes_WithValidData_CalculatesCorrectly()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            var issRate = 5.00m;

            // Act
            invoice.CalculateTaxes(issRate);

            // Assert
            Assert.Equal(5.00m, invoice.IssRate);
            Assert.Equal(5.00m, invoice.IssAmount);  // 5% of 100
            Assert.Equal(0.65m, invoice.PisAmount);   // 0.65% of 100
            Assert.Equal(3.00m, invoice.CofinsAmount); // 3% of 100
            Assert.Equal(1.00m, invoice.CsllAmount);   // 1% of 100
            Assert.Equal(9.65m, invoice.TotalTaxes);
            Assert.Equal(90.35m, invoice.NetAmount);  // 100 - 9.65
        }

        [Fact]
        public void CalculateTaxes_OnNonDraftInvoice_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.SetInvoiceNumber("123");
            invoice.MarkAsPending();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                invoice.CalculateTaxes(5.00m));
        }

        [Fact]
        public void SetInvoiceNumber_WithValidNumber_SetsNumber()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            invoice.SetInvoiceNumber("123");

            // Assert
            Assert.Equal("123", invoice.Number);
        }

        [Fact]
        public void SetInvoiceNumber_WithEmptyNumber_ThrowsArgumentException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                invoice.SetInvoiceNumber(""));
        }

        [Fact]
        public void MarkAsPending_OnDraftInvoice_ChangesStatus()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            invoice.MarkAsPending();

            // Assert
            Assert.Equal(ElectronicInvoiceStatus.Pending, invoice.Status);
        }

        [Fact]
        public void MarkAsPending_OnNonDraftInvoice_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.SetInvoiceNumber("123");
            invoice.MarkAsPending();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                invoice.MarkAsPending());
        }

        [Fact]
        public void Authorize_WithValidData_AuthorizesInvoice()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.SetInvoiceNumber("123");
            invoice.MarkAsPending();
            invoice.MarkAsPendingAuthorization();

            var authCode = "AUTH123";
            var accessKey = "12345678901234567890123456789012345678901234";
            var protocol = "PROT123";
            var authDate = DateTime.UtcNow;

            // Act
            invoice.Authorize(authCode, accessKey, protocol, authDate);

            // Assert
            Assert.Equal(ElectronicInvoiceStatus.Authorized, invoice.Status);
            Assert.Equal(authCode, invoice.AuthorizationCode);
            Assert.Equal(accessKey, invoice.AccessKey);
            Assert.Equal(protocol, invoice.Protocol);
            Assert.Equal(authDate, invoice.AuthorizationDate);
        }

        [Fact]
        public void Cancel_OnAuthorizedInvoice_CancelsInvoice()
        {
            // Arrange
            var invoice = CreateAuthorizedInvoice();
            var reason = "Client requested cancellation";

            // Act
            invoice.Cancel(reason);

            // Assert
            Assert.Equal(ElectronicInvoiceStatus.Cancelled, invoice.Status);
            Assert.Equal(reason, invoice.CancellationReason);
            Assert.NotNull(invoice.CancellationDate);
        }

        [Fact]
        public void Cancel_OnNonAuthorizedInvoice_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                invoice.Cancel("Test reason"));
        }

        [Fact]
        public void Cancel_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var invoice = CreateAuthorizedInvoice();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                invoice.Cancel(""));
        }

        [Fact]
        public void CanBeCancelled_OnAuthorizedInvoice_ReturnsTrue()
        {
            // Arrange
            var invoice = CreateAuthorizedInvoice();

            // Act & Assert
            Assert.True(invoice.CanBeCancelled());
        }

        [Fact]
        public void CanBeCancelled_OnDraftInvoice_ReturnsFalse()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.False(invoice.CanBeCancelled());
        }

        [Fact]
        public void CanBeIssued_OnDraftInvoice_ReturnsTrue()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.True(invoice.CanBeIssued());
        }

        [Fact]
        public void CanBeIssued_OnAuthorizedInvoice_ReturnsFalse()
        {
            // Arrange
            var invoice = CreateAuthorizedInvoice();

            // Act & Assert
            Assert.False(invoice.CanBeIssued());
        }

        [Fact]
        public void SetClientDetails_UpdatesClientInformation()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            var email = "john@example.com";
            var phone = "(11) 98765-4321";
            var address = "123 Main St";

            // Act
            invoice.SetClientDetails(email, phone, address);

            // Assert
            Assert.Equal(email, invoice.ClientEmail);
            Assert.Equal(phone, invoice.ClientPhone);
            Assert.Equal(address, invoice.ClientAddress);
        }

        [Fact]
        public void SetProviderDetails_UpdatesProviderInformation()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            var municipalReg = "123456";
            var stateReg = "789012";
            var address = "456 Provider Ave";

            // Act
            invoice.SetProviderDetails(municipalReg, stateReg, address);

            // Assert
            Assert.Equal(municipalReg, invoice.ProviderMunicipalRegistration);
            Assert.Equal(stateReg, invoice.ProviderStateRegistration);
            Assert.Equal(address, invoice.ProviderAddress);
        }

        private ElectronicInvoice CreateValidInvoice()
        {
            return new ElectronicInvoice(
                ElectronicInvoiceType.NFSe,
                "1",
                TestCnpj,
                "Test Clinic",
                TestCpf,
                "John Doe",
                "Medical consultation",
                100.00m,
                TestTenantId,
                "12345",
                "john@example.com",
                "8501"
            );
        }

        private ElectronicInvoice CreateAuthorizedInvoice()
        {
            var invoice = CreateValidInvoice();
            invoice.SetInvoiceNumber("123");
            invoice.MarkAsPending();
            invoice.MarkAsPendingAuthorization();
            invoice.Authorize(
                "AUTH123",
                "12345678901234567890123456789012345678901234",
                "PROT123",
                DateTime.UtcNow
            );
            return invoice;
        }
    }
}
