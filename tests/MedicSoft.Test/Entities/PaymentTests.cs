using System;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class PaymentTests
    {
        private const string TestTenantId = "test-tenant";
        private readonly Guid TestAppointmentId = Guid.NewGuid();
        private readonly Guid TestSubscriptionId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidAppointmentData_CreatesPayment()
        {
            // Arrange & Act
            var payment = new Payment(100.00m, PaymentMethod.Cash, TestTenantId,
                appointmentId: TestAppointmentId);

            // Assert
            Assert.Equal(100.00m, payment.Amount);
            Assert.Equal(PaymentMethod.Cash, payment.Method);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.Equal(TestAppointmentId, payment.AppointmentId);
            Assert.Null(payment.ClinicSubscriptionId);
        }

        [Fact]
        public void Constructor_WithValidSubscriptionData_CreatesPayment()
        {
            // Arrange & Act
            var payment = new Payment(99.90m, PaymentMethod.CreditCard, TestTenantId,
                clinicSubscriptionId: TestSubscriptionId);

            // Assert
            Assert.Equal(99.90m, payment.Amount);
            Assert.Equal(PaymentMethod.CreditCard, payment.Method);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
            Assert.Null(payment.AppointmentId);
            Assert.Equal(TestSubscriptionId, payment.ClinicSubscriptionId);
        }

        [Fact]
        public void Constructor_WithZeroAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Payment(0, PaymentMethod.Cash, TestTenantId, appointmentId: TestAppointmentId));
        }

        [Fact]
        public void Constructor_WithNegativeAmount_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Payment(-10.00m, PaymentMethod.Cash, TestTenantId, appointmentId: TestAppointmentId));
        }

        [Fact]
        public void Constructor_WithoutAppointmentOrSubscription_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Payment(100.00m, PaymentMethod.Cash, TestTenantId));
        }

        [Fact]
        public void Constructor_WithEmptyAppointmentId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Payment(100.00m, PaymentMethod.Cash, TestTenantId, appointmentId: Guid.Empty));
        }

        [Fact]
        public void MarkAsPaid_WithValidTransaction_UpdatesStatus()
        {
            // Arrange
            var payment = CreateValidPayment();
            var transactionId = "TXN123456";

            // Act
            payment.MarkAsPaid(transactionId);

            // Assert
            Assert.Equal(PaymentStatus.Paid, payment.Status);
            Assert.Equal(transactionId, payment.TransactionId);
            Assert.NotNull(payment.ProcessedDate);
        }

        [Fact]
        public void MarkAsPaid_WhenAlreadyPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.MarkAsPaid("TXN456"));
        }

        [Fact]
        public void MarkAsPaid_WhenRefunded_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");
            payment.Refund("Customer request");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.MarkAsPaid("TXN456"));
        }

        [Fact]
        public void MarkAsProcessing_FromPending_UpdatesStatus()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act
            payment.MarkAsProcessing();

            // Assert
            Assert.Equal(PaymentStatus.Processing, payment.Status);
        }

        [Fact]
        public void MarkAsProcessing_WhenNotPending_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.MarkAsProcessing());
        }

        [Fact]
        public void MarkAsFailed_WithReason_UpdatesStatusAndNotes()
        {
            // Arrange
            var payment = CreateValidPayment();
            var reason = "Insufficient funds";

            // Act
            payment.MarkAsFailed(reason);

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Contains(reason, payment.Notes);
        }

        [Fact]
        public void MarkAsFailed_WhenPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.MarkAsFailed("Some reason"));
        }

        [Fact]
        public void Refund_WhenPaid_UpdatesStatus()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");
            var reason = "Customer request";

            // Act
            payment.Refund(reason);

            // Assert
            Assert.Equal(PaymentStatus.Refunded, payment.Status);
            Assert.Equal(reason, payment.CancellationReason);
            Assert.NotNull(payment.CancellationDate);
        }

        [Fact]
        public void Refund_WhenNotPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.Refund("Some reason"));
        }

        [Fact]
        public void Refund_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.Throws<ArgumentException>(() => payment.Refund(""));
        }

        [Fact]
        public void Cancel_WithReason_UpdatesStatus()
        {
            // Arrange
            var payment = CreateValidPayment();
            var reason = "Customer cancelled appointment";

            // Act
            payment.Cancel(reason);

            // Assert
            Assert.Equal(PaymentStatus.Cancelled, payment.Status);
            Assert.Equal(reason, payment.CancellationReason);
            Assert.NotNull(payment.CancellationDate);
        }

        [Fact]
        public void Cancel_WhenPaid_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.Cancel("Some reason"));
        }

        [Fact]
        public void Cancel_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => payment.Cancel(""));
        }

        [Fact]
        public void SetCardDetails_ForCreditCard_SetsDetails()
        {
            // Arrange
            var payment = new Payment(100.00m, PaymentMethod.CreditCard, TestTenantId,
                appointmentId: TestAppointmentId);
            var lastFour = "1234";

            // Act
            payment.SetCardDetails(lastFour);

            // Assert
            Assert.Equal(lastFour, payment.CardLastFourDigits);
        }

        [Fact]
        public void SetCardDetails_ForDebitCard_SetsDetails()
        {
            // Arrange
            var payment = new Payment(100.00m, PaymentMethod.DebitCard, TestTenantId,
                appointmentId: TestAppointmentId);
            var lastFour = "5678";

            // Act
            payment.SetCardDetails(lastFour);

            // Assert
            Assert.Equal(lastFour, payment.CardLastFourDigits);
        }

        [Fact]
        public void SetCardDetails_ForNonCardPayment_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment(); // Cash payment

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => payment.SetCardDetails("1234"));
        }

        [Fact]
        public void SetCardDetails_WithInvalidLength_ThrowsArgumentException()
        {
            // Arrange
            var payment = new Payment(100.00m, PaymentMethod.CreditCard, TestTenantId,
                appointmentId: TestAppointmentId);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => payment.SetCardDetails("123"));
            Assert.Throws<ArgumentException>(() => payment.SetCardDetails("12345"));
        }

        [Fact]
        public void SetPixDetails_ForPixPayment_SetsDetails()
        {
            // Arrange
            var payment = new Payment(100.00m, PaymentMethod.Pix, TestTenantId,
                appointmentId: TestAppointmentId);
            var pixKey = "user@example.com";
            var pixTxId = "PIX123456";

            // Act
            payment.SetPixDetails(pixKey, pixTxId);

            // Assert
            Assert.Equal(pixKey, payment.PixKey);
            Assert.Equal(pixTxId, payment.PixTransactionId);
        }

        [Fact]
        public void SetPixDetails_ForNonPixPayment_ThrowsInvalidOperationException()
        {
            // Arrange
            var payment = CreateValidPayment(); // Cash payment

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                payment.SetPixDetails("key@example.com", "TXN123"));
        }

        [Fact]
        public void SetPixDetails_WithEmptyKey_ThrowsArgumentException()
        {
            // Arrange
            var payment = new Payment(100.00m, PaymentMethod.Pix, TestTenantId,
                appointmentId: TestAppointmentId);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => payment.SetPixDetails("", "TXN123"));
        }

        [Fact]
        public void IsPaid_WhenPaid_ReturnsTrue()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.True(payment.IsPaid());
        }

        [Fact]
        public void IsPaid_WhenNotPaid_ReturnsFalse()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert
            Assert.False(payment.IsPaid());
        }

        [Fact]
        public void CanBeRefunded_WhenPaid_ReturnsTrue()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.True(payment.CanBeRefunded());
        }

        [Fact]
        public void CanBeRefunded_WhenNotPaid_ReturnsFalse()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert
            Assert.False(payment.CanBeRefunded());
        }

        [Fact]
        public void CanBeCancelled_WhenPending_ReturnsTrue()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert
            Assert.True(payment.CanBeCancelled());
        }

        [Fact]
        public void CanBeCancelled_WhenFailed_ReturnsTrue()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsFailed("Some reason");

            // Act & Assert
            Assert.True(payment.CanBeCancelled());
        }

        [Fact]
        public void CanBeCancelled_WhenPaid_ReturnsFalse()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act & Assert
            Assert.False(payment.CanBeCancelled());
        }

        [Fact]
        public void PaymentLifecycle_CompleteFlow_WorksCorrectly()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act & Assert - Flow: Pending → Processing → Paid
            Assert.Equal(PaymentStatus.Pending, payment.Status);

            payment.MarkAsProcessing();
            Assert.Equal(PaymentStatus.Processing, payment.Status);

            payment.MarkAsPaid("TXN123456");
            Assert.Equal(PaymentStatus.Paid, payment.Status);
            Assert.NotNull(payment.ProcessedDate);
            Assert.Equal("TXN123456", payment.TransactionId);
        }

        [Fact]
        public void PaymentRefundFlow_WorksCorrectly()
        {
            // Arrange
            var payment = CreateValidPayment();
            payment.MarkAsPaid("TXN123");

            // Act
            payment.Refund("Customer requested refund");

            // Assert
            Assert.Equal(PaymentStatus.Refunded, payment.Status);
            Assert.NotNull(payment.CancellationDate);
            Assert.Equal("Customer requested refund", payment.CancellationReason);
        }

        [Fact]
        public void PaymentFailureFlow_WorksCorrectly()
        {
            // Arrange
            var payment = CreateValidPayment();

            // Act
            payment.MarkAsProcessing();
            payment.MarkAsFailed("Card declined");

            // Assert
            Assert.Equal(PaymentStatus.Failed, payment.Status);
            Assert.Contains("Card declined", payment.Notes);
        }

        [Theory]
        [InlineData(PaymentMethod.Cash)]
        [InlineData(PaymentMethod.CreditCard)]
        [InlineData(PaymentMethod.DebitCard)]
        [InlineData(PaymentMethod.Pix)]
        [InlineData(PaymentMethod.BankTransfer)]
        [InlineData(PaymentMethod.Check)]
        public void Constructor_WithAllPaymentMethods_CreatesPayment(PaymentMethod method)
        {
            // Act
            var payment = new Payment(100.00m, method, TestTenantId, appointmentId: TestAppointmentId);

            // Assert
            Assert.Equal(method, payment.Method);
            Assert.Equal(PaymentStatus.Pending, payment.Status);
        }

        private Payment CreateValidPayment()
        {
            return new Payment(100.00m, PaymentMethod.Cash, TestTenantId,
                appointmentId: TestAppointmentId, notes: "Test payment");
        }
    }
}
