using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Controllers
{
    public class BlockedTimeSlotsControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly Mock<ITenantContext> _mockTenantContext;
        private readonly BlockedTimeSlotsController _controller;
        private readonly string _testTenantId = "test-tenant";
        private readonly Guid _testClinicId = Guid.NewGuid();

        public BlockedTimeSlotsControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _mockTenantContext = new Mock<ITenantContext>();

            _mockTenantContext.Setup(t => t.TenantId).Returns(_testTenantId);

            _controller = new BlockedTimeSlotsController(
                _mockMediator.Object,
                _mockTenantContext.Object
            );

            // Setup controller user context with required permissions
            var claims = new List<Claim>
            {
                new Claim("clinic_id", _testClinicId.ToString()),
                new Claim("sub", "test-user"),
                new Claim("permission", "appointments.create"),
                new Claim("permission", "appointments.delete"),
                new Claim("permission", "appointments.view")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        #region Delete Tests with Scope

        [Fact]
        public async Task Delete_WithThisOccurrenceScope_SendsCorrectCommand()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var scope = RecurringDeleteScope.ThisOccurrence;
            var reason = "Test reason";

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId, scope, reason);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.BlockedSlotId == blockedSlotId &&
                    cmd.Scope == RecurringDeleteScope.ThisOccurrence &&
                    cmd.TenantId == _testTenantId &&
                    cmd.DeletionReason == reason
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_WithThisAndFutureScope_SendsCorrectCommand()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var scope = RecurringDeleteScope.ThisAndFuture;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId, scope, null);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.BlockedSlotId == blockedSlotId &&
                    cmd.Scope == RecurringDeleteScope.ThisAndFuture &&
                    cmd.TenantId == _testTenantId &&
                    cmd.DeletionReason == null
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_WithAllInSeriesScope_SendsCorrectCommand()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var scope = RecurringDeleteScope.AllInSeries;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId, scope);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.BlockedSlotId == blockedSlotId &&
                    cmd.Scope == RecurringDeleteScope.AllInSeries &&
                    cmd.TenantId == _testTenantId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_WithDefaultScope_UsesThisOccurrence()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act - not specifying scope parameter
            var result = await _controller.Delete(blockedSlotId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.Scope == RecurringDeleteScope.ThisOccurrence
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion

        #region Backward Compatibility Tests

        [Fact]
        public async Task Delete_WithDeleteSeriesTrue_MapsToAllInSeries()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var deleteSeries = true;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act - using old deleteSeries parameter
            var result = await _controller.Delete(
                blockedSlotId,
                RecurringDeleteScope.ThisOccurrence, // Default scope
                null,
                deleteSeries
            );

            // Assert - should override scope to AllInSeries
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.BlockedSlotId == blockedSlotId &&
                    cmd.Scope == RecurringDeleteScope.AllInSeries // Should be overridden
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_WithDeleteSeriesFalse_KeepsOriginalScope()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var deleteSeries = false;
            var scope = RecurringDeleteScope.ThisAndFuture;

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act - using old deleteSeries parameter with false
            var result = await _controller.Delete(
                blockedSlotId,
                scope,
                null,
                deleteSeries
            );

            // Assert - should keep the original scope
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.Scope == scope // Should not be overridden
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion

        #region Reason Parameter Tests

        [Fact]
        public async Task Delete_WithReason_PassesReasonToCommand()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var reason = "Doctor unavailable due to emergency";

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId, RecurringDeleteScope.ThisOccurrence, reason);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.DeletionReason == reason
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        [Fact]
        public async Task Delete_WithoutReason_PassesNullToCommand()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.DeletionReason == null
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion

        #region Error Scenarios

        [Fact]
        public async Task Delete_WithNonExistentSlot_ReturnsNotFound()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false); // Indicates slot not found

            // Act
            var result = await _controller.Delete(blockedSlotId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task Delete_WhenHandlerReturnsTrue_ReturnsNoContent()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        #endregion

        #region All Scope Values Tests

        [Theory]
        [InlineData(RecurringDeleteScope.ThisOccurrence)]
        [InlineData(RecurringDeleteScope.ThisAndFuture)]
        [InlineData(RecurringDeleteScope.AllInSeries)]
        public async Task Delete_WithAllValidScopes_SendsCorrectCommands(RecurringDeleteScope scope)
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId, scope);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd => cmd.Scope == scope),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion

        #region Tenant Context Tests

        [Fact]
        public async Task Delete_AlwaysUsesTenantIdFromContext()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var expectedTenantId = "specific-tenant-123";

            // Update tenant context for this test
            _mockTenantContext.Setup(t => t.TenantId).Returns(expectedTenantId);

            _mockMediator
                .Setup(m => m.Send(It.IsAny<DeleteRecurringScopeCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(blockedSlotId);

            // Assert
            _mockMediator.Verify(m => m.Send(
                It.Is<DeleteRecurringScopeCommand>(cmd =>
                    cmd.TenantId == expectedTenantId
                ),
                It.IsAny<CancellationToken>()
            ), Times.Once);
        }

        #endregion
    }
}
