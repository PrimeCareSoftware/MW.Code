using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class TissGuideTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _tissBatchId = Guid.NewGuid();
        private readonly Guid _appointmentId = Guid.NewGuid();
        private readonly Guid _patientHealthInsuranceId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesTissGuide()
        {
            // Arrange
            var guideNumber = "GUIDE-001";
            var serviceDate = DateTime.UtcNow;

            // Act
            var guide = new TissGuide(
                _tissBatchId, _appointmentId, _patientHealthInsuranceId,
                guideNumber, TissGuideType.Consultation, serviceDate, TenantId);

            // Assert
            guide.Id.Should().NotBeEmpty();
            guide.TissBatchId.Should().Be(_tissBatchId);
            guide.AppointmentId.Should().Be(_appointmentId);
            guide.PatientHealthInsuranceId.Should().Be(_patientHealthInsuranceId);
            guide.GuideNumber.Should().Be(guideNumber);
            guide.GuideType.Should().Be(TissGuideType.Consultation);
            guide.ServiceDate.Should().Be(serviceDate);
            guide.Status.Should().Be(GuideStatus.Draft);
            guide.TotalAmount.Should().Be(0);
            guide.Procedures.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithAuthorizationNumber_CreatesTissGuide()
        {
            // Arrange
            var authNumber = "AUTH-12345";

            // Act
            var guide = new TissGuide(
                _tissBatchId, _appointmentId, _patientHealthInsuranceId,
                "GUIDE-001", TissGuideType.SPSADT, DateTime.UtcNow, TenantId, authNumber);

            // Assert
            guide.AuthorizationNumber.Should().Be(authNumber);
        }

        [Fact]
        public void Constructor_WithEmptyTissBatchId_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissGuide(
                Guid.Empty, _appointmentId, _patientHealthInsuranceId,
                "GUIDE-001", TissGuideType.Consultation, DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*TISS Batch ID*");
        }

        [Fact]
        public void Constructor_WithEmptyAppointmentId_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissGuide(
                _tissBatchId, Guid.Empty, _patientHealthInsuranceId,
                "GUIDE-001", TissGuideType.Consultation, DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Appointment ID*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidGuideNumber_ThrowsArgumentException(string? guideNumber)
        {
            // Act
            var act = () => new TissGuide(
                _tissBatchId, _appointmentId, _patientHealthInsuranceId,
                guideNumber!, TissGuideType.Consultation, DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Guide number*");
        }

        [Fact]
        public void AddProcedure_WithValidProcedure_AddsProcedureAndRecalculatesTotal()
        {
            // Arrange
            var guide = CreateValidGuide();
            var procedure = CreateValidProcedure(guide.Id);

            // Act
            guide.AddProcedure(procedure);

            // Assert
            guide.Procedures.Should().ContainSingle()
                .Which.Should().Be(procedure);
            guide.TotalAmount.Should().Be(procedure.TotalPrice);
            guide.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void AddProcedure_WithMultipleProcedures_RecalculatesTotal()
        {
            // Arrange
            var guide = CreateValidGuide();
            var procedure1 = CreateValidProcedure(guide.Id);
            var procedure2 = CreateValidProcedure(guide.Id);

            // Act
            guide.AddProcedure(procedure1);
            guide.AddProcedure(procedure2);

            // Assert
            guide.Procedures.Should().HaveCount(2);
            guide.TotalAmount.Should().Be(procedure1.TotalPrice + procedure2.TotalPrice);
        }

        [Fact]
        public void AddProcedure_WithNullProcedure_ThrowsArgumentNullException()
        {
            // Arrange
            var guide = CreateValidGuide();

            // Act
            var act = () => guide.AddProcedure(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddProcedure_WithDuplicateProcedure_ThrowsInvalidOperationException()
        {
            // Arrange
            var guide = CreateValidGuide();
            var procedure = CreateValidProcedure(guide.Id);
            guide.AddProcedure(procedure);

            // Act
            var act = () => guide.AddProcedure(procedure);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*already in this guide*");
        }

        [Theory]
        [InlineData(GuideStatus.Sent)]
        [InlineData(GuideStatus.Approved)]
        [InlineData(GuideStatus.Rejected)]
        public void AddProcedure_WithInvalidStatus_ThrowsInvalidOperationException(GuideStatus status)
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            
            if (status == GuideStatus.Sent)
                guide.MarkAsSent();
            else if (status == GuideStatus.Approved)
            {
                guide.MarkAsSent();
                guide.Approve(100m);
            }
            else if (status == GuideStatus.Rejected)
            {
                guide.MarkAsSent();
                guide.Reject("Test rejection");
            }

            // Act
            var act = () => guide.AddProcedure(CreateValidProcedure(guide.Id));

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RemoveProcedure_WithExistingProcedure_RemovesProcedureAndRecalculatesTotal()
        {
            // Arrange
            var guide = CreateValidGuide();
            var procedure1 = CreateValidProcedure(guide.Id);
            var procedure2 = CreateValidProcedure(guide.Id);
            guide.AddProcedure(procedure1);
            guide.AddProcedure(procedure2);

            // Act
            guide.RemoveProcedure(procedure1.Id);

            // Assert
            guide.Procedures.Should().ContainSingle()
                .Which.Should().Be(procedure2);
            guide.TotalAmount.Should().Be(procedure2.TotalPrice);
        }

        [Fact]
        public void MarkAsSent_WithProcedures_ChangesStatus()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));

            // Act
            guide.MarkAsSent();

            // Assert
            guide.Status.Should().Be(GuideStatus.Sent);
            guide.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void MarkAsSent_WithoutProcedures_ThrowsInvalidOperationException()
        {
            // Arrange
            var guide = CreateValidGuide();

            // Act
            var act = () => guide.MarkAsSent();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*without procedures*");
        }

        [Fact]
        public void Approve_WithFullAmount_SetsStatusToApproved()
        {
            // Arrange
            var guide = CreateValidGuide();
            var procedure = CreateValidProcedure(guide.Id);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();
            var totalAmount = guide.TotalAmount;

            // Act
            guide.Approve(totalAmount);

            // Assert
            guide.Status.Should().Be(GuideStatus.Approved);
            guide.ApprovedAmount.Should().Be(totalAmount);
            guide.GlosedAmount.Should().Be(0);
            guide.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Approve_WithPartialAmount_SetsStatusToPartiallyApproved()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();
            var totalAmount = guide.TotalAmount;
            var approvedAmount = totalAmount * 0.8m;

            // Act
            guide.Approve(approvedAmount);

            // Assert
            guide.Status.Should().Be(GuideStatus.PartiallyApproved);
            guide.ApprovedAmount.Should().Be(approvedAmount);
            guide.GlosedAmount.Should().Be(totalAmount - approvedAmount);
        }

        [Fact]
        public void Approve_WithNegativeAmount_ThrowsArgumentException()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();

            // Act
            var act = () => guide.Approve(-100m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*cannot be negative*");
        }

        [Fact]
        public void Approve_WithInvalidStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));

            // Act
            var act = () => guide.Approve(100m);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void Reject_WithReason_RejectsGuide()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();
            var totalAmount = guide.TotalAmount;
            var reason = "Procedimento não autorizado";

            // Act
            guide.Reject(reason);

            // Assert
            guide.Status.Should().Be(GuideStatus.Rejected);
            guide.GlossReason.Should().Be(reason);
            guide.ApprovedAmount.Should().Be(0);
            guide.GlosedAmount.Should().Be(totalAmount);
            guide.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Reject_WithInvalidReason_ThrowsArgumentException(string? reason)
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();

            // Act
            var act = () => guide.Reject(reason!);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void MarkAsPaid_WithApprovedStatus_ChangesStatusToPaid()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();
            guide.Approve(guide.TotalAmount);

            // Act
            guide.MarkAsPaid();

            // Assert
            guide.Status.Should().Be(GuideStatus.Paid);
            guide.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void MarkAsPaid_WithPartiallyApprovedStatus_ChangesStatusToPaid()
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            guide.MarkAsSent();
            guide.Approve(guide.TotalAmount * 0.8m);

            // Act
            guide.MarkAsPaid();

            // Assert
            guide.Status.Should().Be(GuideStatus.Paid);
        }

        [Theory]
        [InlineData(GuideStatus.Draft)]
        [InlineData(GuideStatus.Sent)]
        [InlineData(GuideStatus.Rejected)]
        public void MarkAsPaid_WithInvalidStatus_ThrowsInvalidOperationException(GuideStatus status)
        {
            // Arrange
            var guide = CreateValidGuide();
            guide.AddProcedure(CreateValidProcedure(guide.Id));
            
            if (status == GuideStatus.Sent)
                guide.MarkAsSent();
            else if (status == GuideStatus.Rejected)
            {
                guide.MarkAsSent();
                guide.Reject("Test");
            }

            // Act
            var act = () => guide.MarkAsPaid();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        private TissGuide CreateValidGuide()
        {
            return new TissGuide(
                _tissBatchId,
                _appointmentId,
                _patientHealthInsuranceId,
                $"GUIDE-{Guid.NewGuid().ToString().Substring(0, 8)}",
                TissGuideType.Consultation,
                DateTime.UtcNow,
                TenantId
            );
        }

        private TissGuideProcedure CreateValidProcedure(Guid guideId)
        {
            return new TissGuideProcedure(
                guideId,
                "40101012",
                "Consulta médica",
                1,
                150.00m,
                TenantId
            );
        }
    }
}
