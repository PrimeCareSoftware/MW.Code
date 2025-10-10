using System;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class InvoiceTests
    {
        private const string TestTenantId = "test-tenant";
        private readonly Guid TestPaymentId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesInvoice()
        {
            // Arrange & Act
            var invoice = CreateValidInvoice();

            // Assert
            Assert.Equal("INV-2024-001", invoice.InvoiceNumber);
            Assert.Equal(TestPaymentId, invoice.PaymentId);
            Assert.Equal(InvoiceType.Appointment, invoice.Type);
            Assert.Equal(InvoiceStatus.Draft, invoice.Status);
            Assert.Equal(100.00m, invoice.Amount);
            Assert.Equal(10.00m, invoice.TaxAmount);
            Assert.Equal(110.00m, invoice.TotalAmount);
            Assert.Equal("John Doe", invoice.CustomerName);
        }

        [Fact]
        public void Constructor_WithEmptyInvoiceNumber_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("", TestPaymentId, InvoiceType.Appointment,
                    100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId));
        }

        [Fact]
        public void Constructor_WithEmptyPaymentId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("INV-001", Guid.Empty, InvoiceType.Appointment,
                    100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId));
        }

        [Fact]
        public void Constructor_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                    0, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId));
        }

        [Fact]
        public void Constructor_WithNegativeAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                    -100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId));
        }

        [Fact]
        public void Constructor_WithNegativeTax_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                    100.00m, -10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId));
        }

        [Fact]
        public void Constructor_WithEmptyCustomerName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                    100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "", TestTenantId));
        }

        [Fact]
        public void Constructor_CalculatesTotalAmount_Correctly()
        {
            // Arrange & Act
            var invoice = new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                200.00m, 25.00m, DateTime.UtcNow.AddDays(30), "Jane Smith", TestTenantId);

            // Assert
            Assert.Equal(225.00m, invoice.TotalAmount);
        }

        [Fact]
        public void Issue_FromDraft_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            invoice.Issue();

            // Assert
            Assert.Equal(InvoiceStatus.Issued, invoice.Status);
            Assert.NotEqual(default(DateTime), invoice.IssueDate);
        }

        [Fact]
        public void Issue_WhenNotDraft_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.Issue());
        }

        [Fact]
        public void MarkAsSent_FromIssued_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act
            invoice.MarkAsSent();

            // Assert
            Assert.Equal(InvoiceStatus.Sent, invoice.Status);
            Assert.NotNull(invoice.SentDate);
        }

        [Fact]
        public void MarkAsSent_FromOverdue_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateOverdueInvoice();

            // Act
            invoice.MarkAsSent();

            // Assert
            Assert.Equal(InvoiceStatus.Sent, invoice.Status);
            Assert.NotNull(invoice.SentDate);
        }

        [Fact]
        public void MarkAsSent_WhenDraft_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.MarkAsSent());
        }

        [Fact]
        public void MarkAsPaid_WithDate_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();
            var paidDate = DateTime.UtcNow;

            // Act
            invoice.MarkAsPaid(paidDate);

            // Assert
            Assert.Equal(InvoiceStatus.Paid, invoice.Status);
            Assert.Equal(paidDate, invoice.PaidDate);
        }

        [Fact]
        public void MarkAsPaid_WhenCancelled_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Cancel("No longer needed");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.MarkAsPaid(DateTime.UtcNow));
        }

        [Fact]
        public void MarkAsPaid_WhenDraft_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.MarkAsPaid(DateTime.UtcNow));
        }

        [Fact]
        public void MarkAsOverdue_AfterDueDate_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateOverdueInvoice();

            // Act
            invoice.MarkAsOverdue();

            // Assert
            Assert.Equal(InvoiceStatus.Overdue, invoice.Status);
        }

        [Fact]
        public void MarkAsOverdue_BeforeDueDate_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.MarkAsOverdue());
        }

        [Fact]
        public void MarkAsOverdue_WhenPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();
            invoice.MarkAsPaid(DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.MarkAsOverdue());
        }

        [Fact]
        public void Cancel_WithReason_UpdatesStatus()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            var reason = "Customer cancelled service";

            // Act
            invoice.Cancel(reason);

            // Assert
            Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
            Assert.Equal(reason, invoice.CancellationReason);
            Assert.NotNull(invoice.CancellationDate);
        }

        [Fact]
        public void Cancel_WhenPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();
            invoice.MarkAsPaid(DateTime.UtcNow);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.Cancel("Some reason"));
        }

        [Fact]
        public void Cancel_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => invoice.Cancel(""));
        }

        [Fact]
        public void UpdateAmount_OnDraft_UpdatesAmounts()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            invoice.UpdateAmount(150.00m, 15.00m);

            // Assert
            Assert.Equal(150.00m, invoice.Amount);
            Assert.Equal(15.00m, invoice.TaxAmount);
            Assert.Equal(165.00m, invoice.TotalAmount);
        }

        [Fact]
        public void UpdateAmount_WhenIssued_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.UpdateAmount(150.00m, 15.00m));
        }

        [Fact]
        public void UpdateAmount_WithZeroAmount_ThrowsArgumentException()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => invoice.UpdateAmount(0, 10.00m));
        }

        [Fact]
        public void UpdateDescription_OnDraft_UpdatesDescription()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            var newDescription = "Updated description";

            // Act
            invoice.UpdateDescription(newDescription);

            // Assert
            Assert.Equal(newDescription, invoice.Description);
        }

        [Fact]
        public void UpdateDescription_WhenIssued_ThrowsInvalidOperationException()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => invoice.UpdateDescription("New description"));
        }

        [Fact]
        public void IsOverdue_BeforeDueDate_ReturnsFalse()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act & Assert
            Assert.False(invoice.IsOverdue());
        }

        [Fact]
        public void IsOverdue_AfterDueDate_ReturnsTrue()
        {
            // Arrange
            var invoice = CreateOverdueInvoice();

            // Act & Assert
            Assert.True(invoice.IsOverdue());
        }

        [Fact]
        public void IsOverdue_WhenPaid_ReturnsFalse()
        {
            // Arrange
            var invoice = CreateOverdueInvoice();
            invoice.MarkAsPaid(DateTime.UtcNow);

            // Act & Assert
            Assert.False(invoice.IsOverdue());
        }

        [Fact]
        public void DaysUntilDue_BeforeDueDate_ReturnsPositive()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            var days = invoice.DaysUntilDue();

            // Assert
            Assert.True(days > 0);
        }

        [Fact]
        public void DaysUntilDue_WhenPaid_ReturnsZero()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();
            invoice.MarkAsPaid(DateTime.UtcNow);

            // Act
            var days = invoice.DaysUntilDue();

            // Assert
            Assert.Equal(0, days);
        }

        [Fact]
        public void DaysOverdue_WhenNotOverdue_ReturnsZero()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act
            var days = invoice.DaysOverdue();

            // Assert
            Assert.Equal(0, days);
        }

        [Fact]
        public void DaysOverdue_WhenOverdue_ReturnsPositive()
        {
            // Arrange
            var invoice = CreateOverdueInvoice();

            // Act
            var days = invoice.DaysOverdue();

            // Assert
            Assert.True(days > 0);
        }

        [Fact]
        public void InvoiceLifecycle_CompleteFlow_WorksCorrectly()
        {
            // Arrange
            var invoice = CreateValidInvoice();

            // Act & Assert - Flow: Draft → Issued → Sent → Paid
            Assert.Equal(InvoiceStatus.Draft, invoice.Status);

            invoice.Issue();
            Assert.Equal(InvoiceStatus.Issued, invoice.Status);

            invoice.MarkAsSent();
            Assert.Equal(InvoiceStatus.Sent, invoice.Status);

            invoice.MarkAsPaid(DateTime.UtcNow);
            Assert.Equal(InvoiceStatus.Paid, invoice.Status);
        }

        [Fact]
        public void InvoiceCancellationFlow_WorksCorrectly()
        {
            // Arrange
            var invoice = CreateValidInvoice();
            invoice.Issue();

            // Act
            invoice.Cancel("Service cancelled by customer");

            // Assert
            Assert.Equal(InvoiceStatus.Cancelled, invoice.Status);
            Assert.NotNull(invoice.CancellationDate);
            Assert.Equal("Service cancelled by customer", invoice.CancellationReason);
        }

        [Theory]
        [InlineData(InvoiceType.Appointment)]
        [InlineData(InvoiceType.Subscription)]
        [InlineData(InvoiceType.Service)]
        public void Constructor_WithAllInvoiceTypes_CreatesInvoice(InvoiceType type)
        {
            // Act
            var invoice = new Invoice("INV-001", TestPaymentId, type,
                100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "Customer Name", TestTenantId);

            // Assert
            Assert.Equal(type, invoice.Type);
            Assert.Equal(InvoiceStatus.Draft, invoice.Status);
        }

        [Fact]
        public void Constructor_WithOptionalFields_SetsAllFields()
        {
            // Arrange & Act
            var invoice = new Invoice("INV-001", TestPaymentId, InvoiceType.Appointment,
                100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId,
                description: "Medical consultation",
                customerDocument: "123.456.789-00",
                customerAddress: "123 Main St, City");

            // Assert
            Assert.Equal("Medical consultation", invoice.Description);
            Assert.Equal("123.456.789-00", invoice.CustomerDocument);
            Assert.Equal("123 Main St, City", invoice.CustomerAddress);
        }

        private Invoice CreateValidInvoice()
        {
            return new Invoice("INV-2024-001", TestPaymentId, InvoiceType.Appointment,
                100.00m, 10.00m, DateTime.UtcNow.AddDays(30), "John Doe", TestTenantId,
                description: "Medical consultation");
        }

        private Invoice CreateOverdueInvoice()
        {
            var invoice = new Invoice("INV-2024-999", TestPaymentId, InvoiceType.Appointment,
                100.00m, 10.00m, DateTime.UtcNow.AddDays(-5), "Overdue Customer", TestTenantId);
            invoice.Issue();
            return invoice;
        }
    }
}
