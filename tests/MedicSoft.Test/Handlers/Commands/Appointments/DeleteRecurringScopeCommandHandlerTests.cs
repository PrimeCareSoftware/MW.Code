using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.Handlers.Commands.Appointments;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Handlers.Commands.Appointments
{
    public class DeleteRecurringScopeCommandHandlerTests
    {
        private readonly Mock<IBlockedTimeSlotRepository> _mockBlockedTimeSlotRepository;
        private readonly Mock<IRecurrenceExceptionRepository> _mockExceptionRepository;
        private readonly Mock<IRecurringAppointmentPatternRepository> _mockPatternRepository;
        private readonly DeleteRecurringScopeCommandHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public DeleteRecurringScopeCommandHandlerTests()
        {
            _mockBlockedTimeSlotRepository = new Mock<IBlockedTimeSlotRepository>();
            _mockExceptionRepository = new Mock<IRecurrenceExceptionRepository>();
            _mockPatternRepository = new Mock<IRecurringAppointmentPatternRepository>();
            _handler = new DeleteRecurringScopeCommandHandler(
                _mockBlockedTimeSlotRepository.Object,
                _mockExceptionRepository.Object,
                _mockPatternRepository.Object);
        }

        #region ThisOccurrence Tests

        [Fact]
        public async Task Handle_ThisOccurrence_WithRecurringSlot_CreatesExceptionAndDeletesSlot()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var seriesId = Guid.NewGuid();
            var patternId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);
            var reason = "Doctor unavailable";

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(), // clinicId
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Original reason"
            );
            
            // Use reflection to set private properties for testing
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);
            typeof(BlockedTimeSlot).GetProperty("IsRecurring")?.SetValue(blockedSlot, true);
            typeof(BlockedTimeSlot).GetProperty("RecurringSeriesId")?.SetValue(blockedSlot, seriesId);
            typeof(BlockedTimeSlot).GetProperty("RecurringPatternId")?.SetValue(blockedSlot, patternId);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.ThisOccurrence,
                _tenantId,
                reason);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockExceptionRepository
                .Setup(r => r.AddAsync(It.IsAny<RecurrenceException>()))
                .ReturnsAsync((RecurrenceException ex) => ex);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteAsync(blockedSlotId, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockExceptionRepository.Verify(r => r.AddAsync(It.Is<RecurrenceException>(ex =>
                ex.RecurringPatternId == patternId &&
                ex.RecurringSeriesId == seriesId &&
                ex.OriginalDate == date &&
                ex.ExceptionType == RecurrenceExceptionType.Deleted &&
                ex.Reason == reason
            )), Times.Once);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteAsync(blockedSlotId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_ThisOccurrence_WithNonRecurringSlot_DeletesSlotDirectly()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.ThisOccurrence,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteAsync(blockedSlotId, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockExceptionRepository.Verify(r => r.AddAsync(It.IsAny<RecurrenceException>()), Times.Never);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteAsync(blockedSlotId, _tenantId), Times.Once);
        }

        #endregion

        #region ThisAndFuture Tests

        [Fact]
        public async Task Handle_ThisAndFuture_UpdatesPatternAndDeletesFutureOccurrences()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var seriesId = Guid.NewGuid();
            var patternId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);
            typeof(BlockedTimeSlot).GetProperty("IsRecurring")?.SetValue(blockedSlot, true);
            typeof(BlockedTimeSlot).GetProperty("RecurringSeriesId")?.SetValue(blockedSlot, seriesId);
            typeof(BlockedTimeSlot).GetProperty("RecurringPatternId")?.SetValue(blockedSlot, patternId);

            var pattern = new RecurringAppointmentPattern(
                RecurrenceFrequency.Weekly,
                1,
                new DateTime(2026, 3, 1),
                new DateTime(2026, 12, 31),
                null,
                _tenantId
            );
            typeof(RecurringAppointmentPattern).GetProperty("Id")?.SetValue(pattern, patternId);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.ThisAndFuture,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockPatternRepository
                .Setup(r => r.GetByIdAsync(patternId, _tenantId))
                .ReturnsAsync(pattern);

            _mockPatternRepository
                .Setup(r => r.UpdateAsync(It.IsAny<RecurringAppointmentPattern>()))
                .Returns(Task.CompletedTask);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteFutureOccurrencesAsync(seriesId, date, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockPatternRepository.Verify(r => r.GetByIdAsync(patternId, _tenantId), Times.Once);
            _mockPatternRepository.Verify(r => r.UpdateAsync(It.Is<RecurringAppointmentPattern>(p => 
                p.EffectiveEndDate == date.AddDays(-1)
            )), Times.Once);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteFutureOccurrencesAsync(seriesId, date, _tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_ThisAndFuture_WithNonRecurringSlot_DeletesSlotDirectly()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.ThisAndFuture,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteAsync(blockedSlotId, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockPatternRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteAsync(blockedSlotId, _tenantId), Times.Once);
        }

        #endregion

        #region AllInSeries Tests

        [Fact]
        public async Task Handle_AllInSeries_DeletesSeriesDeactivatesPatternAndCleansExceptions()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var seriesId = Guid.NewGuid();
            var patternId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);
            typeof(BlockedTimeSlot).GetProperty("IsRecurring")?.SetValue(blockedSlot, true);
            typeof(BlockedTimeSlot).GetProperty("RecurringSeriesId")?.SetValue(blockedSlot, seriesId);
            typeof(BlockedTimeSlot).GetProperty("RecurringPatternId")?.SetValue(blockedSlot, patternId);

            var pattern = new RecurringAppointmentPattern(
                RecurrenceFrequency.Weekly,
                1,
                new DateTime(2026, 3, 1),
                new DateTime(2026, 12, 31),
                null,
                _tenantId
            );
            typeof(RecurringAppointmentPattern).GetProperty("Id")?.SetValue(pattern, patternId);

            var exceptions = new List<RecurrenceException>
            {
                new RecurrenceException(patternId, seriesId, new DateTime(2026, 3, 8), RecurrenceExceptionType.Deleted, _tenantId, null)
            };
            typeof(RecurrenceException).GetProperty("Id")?.SetValue(exceptions[0], Guid.NewGuid());

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.AllInSeries,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteByRecurringSeriesIdAsync(seriesId, _tenantId))
                .Returns(Task.CompletedTask);

            _mockPatternRepository
                .Setup(r => r.GetByIdAsync(patternId, _tenantId))
                .ReturnsAsync(pattern);

            _mockPatternRepository
                .Setup(r => r.UpdateAsync(It.IsAny<RecurringAppointmentPattern>()))
                .Returns(Task.CompletedTask);

            _mockExceptionRepository
                .Setup(r => r.GetByRecurringSeriesIdAsync(seriesId, _tenantId))
                .ReturnsAsync(exceptions);

            _mockExceptionRepository
                .Setup(r => r.DeleteAsync(It.IsAny<Guid>(), _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteByRecurringSeriesIdAsync(seriesId, _tenantId), Times.Once);
            _mockPatternRepository.Verify(r => r.GetByIdAsync(patternId, _tenantId), Times.Once);
            _mockPatternRepository.Verify(r => r.UpdateAsync(It.Is<RecurringAppointmentPattern>(p => !p.IsActive)), Times.Once);
            _mockExceptionRepository.Verify(r => r.GetByRecurringSeriesIdAsync(seriesId, _tenantId), Times.Once);
            _mockExceptionRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), _tenantId), Times.Exactly(exceptions.Count));
        }

        [Fact]
        public async Task Handle_AllInSeries_WithoutPattern_DeletesSeriesOnly()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var seriesId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);
            typeof(BlockedTimeSlot).GetProperty("IsRecurring")?.SetValue(blockedSlot, true);
            typeof(BlockedTimeSlot).GetProperty("RecurringSeriesId")?.SetValue(blockedSlot, seriesId);
            typeof(BlockedTimeSlot).GetProperty("RecurringPatternId")?.SetValue(blockedSlot, null);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.AllInSeries,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteByRecurringSeriesIdAsync(seriesId, _tenantId))
                .Returns(Task.CompletedTask);

            _mockExceptionRepository
                .Setup(r => r.GetByRecurringSeriesIdAsync(seriesId, _tenantId))
                .ReturnsAsync(new List<RecurrenceException>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteByRecurringSeriesIdAsync(seriesId, _tenantId), Times.Once);
            _mockPatternRepository.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AllInSeries_WithNonRecurringSlot_DeletesSlotDirectly()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.AllInSeries,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.DeleteAsync(blockedSlotId, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteAsync(blockedSlotId, _tenantId), Times.Once);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteByRecurringSeriesIdAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region Error Scenarios

        [Fact]
        public async Task Handle_WithNonExistentSlot_ReturnsFalse()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                RecurringDeleteScope.ThisOccurrence,
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync((BlockedTimeSlot?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _mockExceptionRepository.Verify(r => r.AddAsync(It.IsAny<RecurrenceException>()), Times.Never);
            _mockBlockedTimeSlotRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithInvalidScope_ThrowsArgumentException()
        {
            // Arrange
            var blockedSlotId = Guid.NewGuid();
            var date = new DateTime(2026, 3, 15);

            var blockedSlot = new BlockedTimeSlot(
                Guid.NewGuid(),
                date,
                new TimeSpan(9, 0, 0),
                new TimeSpan(10, 0, 0),
                BlockedTimeSlotType.Holiday,
                _tenantId,
                null,
                "Reason"
            );
            
            typeof(BlockedTimeSlot).GetProperty("Id")?.SetValue(blockedSlot, blockedSlotId);
            typeof(BlockedTimeSlot).GetProperty("IsRecurring")?.SetValue(blockedSlot, true);
            typeof(BlockedTimeSlot).GetProperty("RecurringSeriesId")?.SetValue(blockedSlot, Guid.NewGuid());

            var command = new DeleteRecurringScopeCommand(
                blockedSlotId,
                (RecurringDeleteScope)999, // Invalid scope
                _tenantId,
                null);

            _mockBlockedTimeSlotRepository
                .Setup(r => r.GetByIdAsync(blockedSlotId, _tenantId))
                .ReturnsAsync(blockedSlot);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _handler.Handle(command, CancellationToken.None));
        }

        #endregion
    }
}
