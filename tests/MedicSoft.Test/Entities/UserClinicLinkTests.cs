using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class UserClinicLinkTests
    {
        private const string TenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_ShouldCreateLink()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            // Act
            var link = new UserClinicLink(userId, clinicId, TenantId, isPreferredClinic: false);

            // Assert
            link.Should().NotBeNull();
            link.UserId.Should().Be(userId);
            link.ClinicId.Should().Be(clinicId);
            link.TenantId.Should().Be(TenantId);
            link.IsActive.Should().BeTrue();
            link.IsPreferredClinic.Should().BeFalse();
            link.LinkedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            link.InactivatedDate.Should().BeNull();
            link.InactivationReason.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithPreferredFlag_ShouldCreatePreferredLink()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            // Act
            var link = new UserClinicLink(userId, clinicId, TenantId, isPreferredClinic: true);

            // Assert
            link.IsPreferredClinic.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithEmptyUserId_ShouldThrowException()
        {
            // Arrange
            var clinicId = Guid.NewGuid();

            // Act
            Action act = () => new UserClinicLink(Guid.Empty, clinicId, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*User ID cannot be empty*");
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ShouldThrowException()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            Action act = () => new UserClinicLink(userId, Guid.Empty, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Clinic ID cannot be empty*");
        }

        [Fact]
        public void SetAsPreferred_ShouldSetPreferredFlagToTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId, isPreferredClinic: false);
            var oldUpdatedAt = link.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            link.SetAsPreferred();

            // Assert
            link.IsPreferredClinic.Should().BeTrue();
            link.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void RemoveAsPreferred_ShouldSetPreferredFlagToFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId, isPreferredClinic: true);
            var oldUpdatedAt = link.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            link.RemoveAsPreferred();

            // Assert
            link.IsPreferredClinic.Should().BeFalse();
            link.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void Deactivate_WithReason_ShouldDeactivateLink()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);
            var reason = "User transferred to another clinic";
            var oldUpdatedAt = link.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            link.Deactivate(reason);

            // Assert
            link.IsActive.Should().BeFalse();
            link.InactivationReason.Should().Be(reason);
            link.InactivatedDate.Should().NotBeNull();
            link.InactivatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            link.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Deactivate_WithEmptyReason_ShouldThrowException(string invalidReason)
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);

            // Act
            Action act = () => link.Deactivate(invalidReason);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Inactivation reason is required*");
        }

        [Fact]
        public void Reactivate_ShouldReactivateLink()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);
            link.Deactivate("Testing deactivation");
            var oldUpdatedAt = link.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act
            link.Reactivate();

            // Assert
            link.IsActive.Should().BeTrue();
            link.InactivationReason.Should().BeNull();
            link.InactivatedDate.Should().BeNull();
            link.UpdatedAt.Should().BeAfter(oldUpdatedAt.Value);
        }

        [Fact]
        public void Deactivate_ThenReactivate_ShouldClearInactivationData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);

            // Act
            link.Deactivate("Initial deactivation");
            link.Reactivate();

            // Assert
            link.IsActive.Should().BeTrue();
            link.InactivationReason.Should().BeNull();
            link.InactivatedDate.Should().BeNull();
        }

        [Fact]
        public void SetAsPreferred_ThenDeactivate_ShouldMaintainPreferredStatus()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);
            link.SetAsPreferred();

            // Act
            link.Deactivate("User no longer works here");

            // Assert
            link.IsActive.Should().BeFalse();
            link.IsPreferredClinic.Should().BeTrue(); // Preferred status should be maintained
        }

        [Fact]
        public void MultipleOperations_ShouldUpdateTimestampEachTime()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var link = new UserClinicLink(userId, clinicId, TenantId);
            var timestamp1 = link.UpdatedAt;

            System.Threading.Thread.Sleep(10);

            // Act & Assert
            link.SetAsPreferred();
            var timestamp2 = link.UpdatedAt.Value;
            timestamp2.Should().BeAfter(timestamp1.Value);

            System.Threading.Thread.Sleep(10);

            link.RemoveAsPreferred();
            var timestamp3 = link.UpdatedAt.Value;
            timestamp3.Should().BeAfter(timestamp2.Value);

            System.Threading.Thread.Sleep(10);

            link.Deactivate("Test reason");
            var timestamp4 = link.UpdatedAt.Value;
            timestamp4.Should().BeAfter(timestamp3.Value);

            System.Threading.Thread.Sleep(10);

            link.Reactivate();
            var timestamp5 = link.UpdatedAt.Value;
            timestamp5.Should().BeAfter(timestamp4.Value);
        }
    }
}
