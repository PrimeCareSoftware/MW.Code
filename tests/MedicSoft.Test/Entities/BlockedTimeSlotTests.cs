using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Test.Entities
{
    public class BlockedTimeSlotTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();
        private readonly Guid _professionalId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesBlockedTimeSlot()
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var startTime = new TimeSpan(12, 0, 0);
            var endTime = new TimeSpan(13, 0, 0);
            var type = BlockedTimeSlotType.Break;

            // Act
            var blocked = new BlockedTimeSlot(_clinicId, date, startTime, endTime, type, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, blocked.Id);
            Assert.Equal(_clinicId, blocked.ClinicId);
            Assert.Equal(date, blocked.Date);
            Assert.Equal(startTime, blocked.StartTime);
            Assert.Equal(endTime, blocked.EndTime);
            Assert.Equal(type, blocked.Type);
            Assert.Null(blocked.ProfessionalId);
            Assert.Null(blocked.Reason);
            Assert.False(blocked.IsRecurring);
        }

        [Fact]
        public void Constructor_WithProfessionalAndReason_CreatesBlockedTimeSlot()
        {
            // Arrange
            var reason = "Lunch break";

            // Act
            var blocked = CreateValidBlockedTimeSlot(reason: reason, professionalId: _professionalId);

            // Assert
            Assert.Equal(_professionalId, blocked.ProfessionalId);
            Assert.Equal(reason, blocked.Reason);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new BlockedTimeSlot(Guid.Empty, DateTime.Today.AddDays(1), 
                    new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), 
                    BlockedTimeSlotType.Break, _tenantId));

            Assert.Contains("Clinic ID cannot be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithStartTimeAfterEndTime_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new BlockedTimeSlot(_clinicId, DateTime.Today.AddDays(1), 
                    new TimeSpan(14, 0, 0), new TimeSpan(13, 0, 0), 
                    BlockedTimeSlotType.Break, _tenantId));

            Assert.Contains("Start time must be before end time", exception.Message);
        }

        [Fact]
        public void Constructor_WithPastDateNonRecurring_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new BlockedTimeSlot(_clinicId, DateTime.Today.AddDays(-1), 
                    new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), 
                    BlockedTimeSlotType.Break, _tenantId, isRecurring: false));

            Assert.Contains("Cannot block time slots in the past", exception.Message);
        }

        [Fact]
        public void Constructor_WithPastDateRecurring_DoesNotThrow()
        {
            // Act - should not throw
            var blocked = new BlockedTimeSlot(_clinicId, DateTime.Today.AddDays(-1), 
                new TimeSpan(10, 0, 0), new TimeSpan(11, 0, 0), 
                BlockedTimeSlotType.Break, _tenantId, isRecurring: true);

            // Assert
            Assert.NotNull(blocked);
            Assert.True(blocked.IsRecurring);
        }

        [Fact]
        public void UpdateTimeSlot_WithValidTimes_UpdatesSuccessfully()
        {
            // Arrange
            var blocked = CreateValidBlockedTimeSlot();
            var newStartTime = new TimeSpan(14, 0, 0);
            var newEndTime = new TimeSpan(15, 0, 0);

            // Act
            blocked.UpdateTimeSlot(newStartTime, newEndTime);

            // Assert
            Assert.Equal(newStartTime, blocked.StartTime);
            Assert.Equal(newEndTime, blocked.EndTime);
            Assert.NotNull(blocked.UpdatedAt);
        }

        [Fact]
        public void UpdateTimeSlot_WithInvalidTimes_ThrowsArgumentException()
        {
            // Arrange
            var blocked = CreateValidBlockedTimeSlot();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                blocked.UpdateTimeSlot(new TimeSpan(15, 0, 0), new TimeSpan(14, 0, 0)));
        }

        [Fact]
        public void UpdateReason_UpdatesReasonSuccessfully()
        {
            // Arrange
            var blocked = CreateValidBlockedTimeSlot();
            var newReason = "Equipment maintenance";

            // Act
            blocked.UpdateReason(newReason);

            // Assert
            Assert.Equal(newReason, blocked.Reason);
            Assert.NotNull(blocked.UpdatedAt);
        }

        [Fact]
        public void UpdateType_UpdatesTypeSuccessfully()
        {
            // Arrange
            var blocked = CreateValidBlockedTimeSlot(type: BlockedTimeSlotType.Break);
            var newType = BlockedTimeSlotType.Maintenance;

            // Act
            blocked.UpdateType(newType);

            // Assert
            Assert.Equal(newType, blocked.Type);
            Assert.NotNull(blocked.UpdatedAt);
        }

        [Fact]
        public void IsOverlapping_WithOverlappingTimes_ReturnsTrue()
        {
            // Arrange - blocked slot from 10:00 to 11:00
            var blocked = CreateValidBlockedTimeSlot(
                startTime: new TimeSpan(10, 0, 0), 
                endTime: new TimeSpan(11, 0, 0));

            // Act & Assert - overlaps at the start
            Assert.True(blocked.IsOverlapping(new TimeSpan(9, 30, 0), new TimeSpan(10, 30, 0)));
            
            // Act & Assert - overlaps at the end
            Assert.True(blocked.IsOverlapping(new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0)));
            
            // Act & Assert - completely contains
            Assert.True(blocked.IsOverlapping(new TimeSpan(9, 0, 0), new TimeSpan(12, 0, 0)));
            
            // Act & Assert - completely within
            Assert.True(blocked.IsOverlapping(new TimeSpan(10, 15, 0), new TimeSpan(10, 45, 0)));
        }

        [Fact]
        public void IsOverlapping_WithNonOverlappingTimes_ReturnsFalse()
        {
            // Arrange - blocked slot from 10:00 to 11:00
            var blocked = CreateValidBlockedTimeSlot(
                startTime: new TimeSpan(10, 0, 0), 
                endTime: new TimeSpan(11, 0, 0));

            // Act & Assert - before
            Assert.False(blocked.IsOverlapping(new TimeSpan(8, 0, 0), new TimeSpan(9, 0, 0)));
            
            // Act & Assert - after
            Assert.False(blocked.IsOverlapping(new TimeSpan(12, 0, 0), new TimeSpan(13, 0, 0)));
            
            // Act & Assert - adjacent before (ends when blocked starts)
            Assert.False(blocked.IsOverlapping(new TimeSpan(9, 0, 0), new TimeSpan(10, 0, 0)));
            
            // Act & Assert - adjacent after (starts when blocked ends)
            Assert.False(blocked.IsOverlapping(new TimeSpan(11, 0, 0), new TimeSpan(12, 0, 0)));
        }

        [Fact]
        public void IsOverlappingWithDate_WithSameDateAndOverlappingTimes_ReturnsTrue()
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var blocked = CreateValidBlockedTimeSlot(
                date: date,
                startTime: new TimeSpan(10, 0, 0), 
                endTime: new TimeSpan(11, 0, 0));

            // Act & Assert
            Assert.True(blocked.IsOverlappingWithDate(date, new TimeSpan(10, 30, 0), new TimeSpan(11, 30, 0)));
        }

        [Fact]
        public void IsOverlappingWithDate_WithDifferentDate_ReturnsFalse()
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var blocked = CreateValidBlockedTimeSlot(date: date);

            // Act & Assert
            Assert.False(blocked.IsOverlappingWithDate(
                DateTime.Today.AddDays(2), 
                new TimeSpan(10, 30, 0), 
                new TimeSpan(11, 30, 0)));
        }

        private BlockedTimeSlot CreateValidBlockedTimeSlot(
            DateTime? date = null,
            TimeSpan? startTime = null,
            TimeSpan? endTime = null,
            BlockedTimeSlotType? type = null,
            Guid? professionalId = null,
            string? reason = null)
        {
            return new BlockedTimeSlot(
                _clinicId,
                date ?? DateTime.Today.AddDays(1),
                startTime ?? new TimeSpan(12, 0, 0),
                endTime ?? new TimeSpan(13, 0, 0),
                type ?? BlockedTimeSlotType.Break,
                _tenantId,
                professionalId,
                reason);
        }
    }
}
