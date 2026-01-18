using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class AuthorizationRequestTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _patientHealthInsuranceId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesAuthorizationRequest()
        {
            // Arrange
            var requestNumber = "REQ-001";
            var procedureCode = "40101012";
            var procedureDescription = "Consulta médica";
            var quantity = 1;

            // Act
            var auth = new AuthorizationRequest(
                _patientId, _patientHealthInsuranceId, requestNumber,
                procedureCode, procedureDescription, quantity, TenantId);

            // Assert
            auth.Id.Should().NotBeEmpty();
            auth.PatientId.Should().Be(_patientId);
            auth.PatientHealthInsuranceId.Should().Be(_patientHealthInsuranceId);
            auth.RequestNumber.Should().Be(requestNumber);
            auth.ProcedureCode.Should().Be(procedureCode);
            auth.ProcedureDescription.Should().Be(procedureDescription);
            auth.Quantity.Should().Be(quantity);
            auth.Status.Should().Be(AuthorizationStatus.Pending);
            auth.RequestDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            auth.AuthorizationNumber.Should().BeNull();
            auth.DenialReason.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithOptionalData_CreatesAuthorizationRequest()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var clinicalIndication = "Dor abdominal há 3 dias";
            var diagnosis = "K29.7";

            // Act
            var auth = new AuthorizationRequest(
                _patientId, _patientHealthInsuranceId, "REQ-001",
                "40101012", "Consulta", 1, TenantId,
                appointmentId, clinicalIndication, diagnosis);

            // Assert
            auth.AppointmentId.Should().Be(appointmentId);
            auth.ClinicalIndication.Should().Be(clinicalIndication);
            auth.Diagnosis.Should().Be(diagnosis);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act
            var act = () => new AuthorizationRequest(
                Guid.Empty, _patientHealthInsuranceId, "REQ-001",
                "40101012", "Consulta", 1, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Patient ID*");
        }

        [Fact]
        public void Constructor_WithEmptyPatientHealthInsuranceId_ThrowsArgumentException()
        {
            // Act
            var act = () => new AuthorizationRequest(
                _patientId, Guid.Empty, "REQ-001",
                "40101012", "Consulta", 1, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Patient Health Insurance ID*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidRequestNumber_ThrowsArgumentException(string? requestNumber)
        {
            // Act
            var act = () => new AuthorizationRequest(
                _patientId, _patientHealthInsuranceId, requestNumber!,
                "40101012", "Consulta", 1, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Request number*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidProcedureCode_ThrowsArgumentException(string? procedureCode)
        {
            // Act
            var act = () => new AuthorizationRequest(
                _patientId, _patientHealthInsuranceId, "REQ-001",
                procedureCode!, "Consulta", 1, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Procedure code*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Constructor_WithInvalidQuantity_ThrowsArgumentException(int quantity)
        {
            // Act
            var act = () => new AuthorizationRequest(
                _patientId, _patientHealthInsuranceId, "REQ-001",
                "40101012", "Consulta", quantity, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Quantity*");
        }

        [Fact]
        public void Approve_WithValidData_ApprovesRequest()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            var authNumber = "AUTH-12345";
            var expirationDate = DateTime.UtcNow.AddMonths(3);

            // Act
            auth.Approve(authNumber, expirationDate);

            // Assert
            auth.Status.Should().Be(AuthorizationStatus.Approved);
            auth.AuthorizationNumber.Should().Be(authNumber);
            auth.AuthorizationDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            auth.ExpirationDate.Should().Be(expirationDate);
            auth.DenialReason.Should().BeNull();
            auth.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Approve_WithoutExpirationDate_ApprovesRequest()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();

            // Act
            auth.Approve("AUTH-12345");

            // Assert
            auth.Status.Should().Be(AuthorizationStatus.Approved);
            auth.ExpirationDate.Should().BeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Approve_WithInvalidAuthorizationNumber_ThrowsArgumentException(string? authNumber)
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();

            // Act
            var act = () => auth.Approve(authNumber!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Authorization number*");
        }

        [Theory]
        [InlineData(AuthorizationStatus.Approved)]
        [InlineData(AuthorizationStatus.Denied)]
        [InlineData(AuthorizationStatus.Cancelled)]
        [InlineData(AuthorizationStatus.Expired)]
        public void Approve_WithInvalidStatus_ThrowsInvalidOperationException(AuthorizationStatus status)
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            if (status == AuthorizationStatus.Approved)
                auth.Approve("AUTH-123");
            else if (status == AuthorizationStatus.Denied)
                auth.Deny("Denied");
            else if (status == AuthorizationStatus.Cancelled)
                auth.Cancel();
            else if (status == AuthorizationStatus.Expired)
            {
                auth.Approve("AUTH-123", DateTime.UtcNow.AddDays(-1));
                auth.MarkAsExpired();
            }

            // Act
            var act = () => auth.Approve("AUTH-NEW");

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"*{status}*");
        }

        [Fact]
        public void Deny_WithValidReason_DeniesRequest()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            var denialReason = "Procedimento não coberto pelo plano";

            // Act
            auth.Deny(denialReason);

            // Assert
            auth.Status.Should().Be(AuthorizationStatus.Denied);
            auth.DenialReason.Should().Be(denialReason);
            auth.AuthorizationNumber.Should().BeNull();
            auth.AuthorizationDate.Should().BeNull();
            auth.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Deny_WithInvalidReason_ThrowsArgumentException(string? denialReason)
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();

            // Act
            var act = () => auth.Deny(denialReason!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Denial reason*");
        }

        [Theory]
        [InlineData(AuthorizationStatus.Approved)]
        [InlineData(AuthorizationStatus.Denied)]
        [InlineData(AuthorizationStatus.Cancelled)]
        public void Deny_WithInvalidStatus_ThrowsInvalidOperationException(AuthorizationStatus status)
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            if (status == AuthorizationStatus.Approved)
                auth.Approve("AUTH-123");
            else if (status == AuthorizationStatus.Denied)
                auth.Deny("Already denied");
            else if (status == AuthorizationStatus.Cancelled)
                auth.Cancel();

            // Act
            var act = () => auth.Deny("New reason");

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Cancel_CancelsRequest()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();

            // Act
            auth.Cancel();

            // Assert
            auth.Status.Should().Be(AuthorizationStatus.Cancelled);
            auth.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Cancel_WhenAlreadyCancelled_ThrowsInvalidOperationException()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Cancel();

            // Act
            var act = () => auth.Cancel();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*already cancelled*");
        }

        [Fact]
        public void MarkAsExpired_WithApprovedStatus_MarksAsExpired()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123");

            // Act
            auth.MarkAsExpired();

            // Assert
            auth.Status.Should().Be(AuthorizationStatus.Expired);
            auth.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(AuthorizationStatus.Pending)]
        [InlineData(AuthorizationStatus.Denied)]
        [InlineData(AuthorizationStatus.Cancelled)]
        public void MarkAsExpired_WithInvalidStatus_ThrowsInvalidOperationException(AuthorizationStatus status)
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            if (status == AuthorizationStatus.Denied)
                auth.Deny("Test");
            else if (status == AuthorizationStatus.Cancelled)
                auth.Cancel();

            // Act
            var act = () => auth.MarkAsExpired();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*Only approved*");
        }

        [Fact]
        public void IsExpired_WithExpiredDate_ReturnsTrue()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123", DateTime.UtcNow.AddDays(-1));

            // Act
            var isExpired = auth.IsExpired();

            // Assert
            isExpired.Should().BeTrue();
        }

        [Fact]
        public void IsExpired_WithFutureDate_ReturnsFalse()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123", DateTime.UtcNow.AddDays(30));

            // Act
            var isExpired = auth.IsExpired();

            // Assert
            isExpired.Should().BeFalse();
        }

        [Fact]
        public void IsExpired_WithoutExpirationDate_ReturnsFalse()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123");

            // Act
            var isExpired = auth.IsExpired();

            // Assert
            isExpired.Should().BeFalse();
        }

        [Fact]
        public void IsValidForUse_WithApprovedAndNotExpired_ReturnsTrue()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123", DateTime.UtcNow.AddMonths(3));

            // Act
            var isValid = auth.IsValidForUse();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValidForUse_WithPendingStatus_ReturnsFalse()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();

            // Act
            var isValid = auth.IsValidForUse();

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void IsValidForUse_WithExpiredAuthorization_ReturnsFalse()
        {
            // Arrange
            var auth = CreateValidAuthorizationRequest();
            auth.Approve("AUTH-123", DateTime.UtcNow.AddDays(-1));

            // Act
            var isValid = auth.IsValidForUse();

            // Assert
            isValid.Should().BeFalse();
        }

        private AuthorizationRequest CreateValidAuthorizationRequest()
        {
            return new AuthorizationRequest(
                _patientId,
                _patientHealthInsuranceId,
                $"REQ-{Guid.NewGuid().ToString().Substring(0, 8)}",
                "40101012",
                "Consulta médica",
                1,
                TenantId
            );
        }
    }
}
