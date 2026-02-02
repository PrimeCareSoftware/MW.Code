using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Test.Entities
{
    public class RecurringAppointmentPatternTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();
        private readonly Guid _professionalId = Guid.NewGuid();
        private readonly Guid _patientId = Guid.NewGuid();

        [Fact]
        public void Constructor_ForWeeklyRecurringAppointments_CreatesPattern()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(1);
            var startTime = new TimeSpan(10, 0, 0);
            var endTime = new TimeSpan(11, 0, 0);

            // Act
            var pattern = new RecurringAppointmentPattern(
                _clinicId,
                RecurrenceFrequency.Weekly,
                startDate,
                startTime,
                endTime,
                _tenantId,
                professionalId: _professionalId,
                patientId: _patientId,
                daysOfWeek: RecurrenceDays.Monday | RecurrenceDays.Wednesday,
                durationMinutes: 60,
                appointmentType: AppointmentType.Regular);

            // Assert
            Assert.NotEqual(Guid.Empty, pattern.Id);
            Assert.Equal(_clinicId, pattern.ClinicId);
            Assert.Equal(RecurrenceFrequency.Weekly, pattern.Frequency);
            Assert.Equal(startDate, pattern.StartDate);
            Assert.Equal(startTime, pattern.StartTime);
            Assert.Equal(endTime, pattern.EndTime);
            Assert.True(pattern.IsActive);
            Assert.True(pattern.IsForAppointments());
            Assert.False(pattern.IsForBlockedSlots());
        }

        [Fact]
        public void Constructor_ForMonthlyRecurringBlocks_CreatesPattern()
        {
            // Arrange
            var startDate = DateTime.Today.AddDays(1);

            // Act
            var pattern = new RecurringAppointmentPattern(
                _clinicId,
                RecurrenceFrequency.Monthly,
                startDate,
                new TimeSpan(12, 0, 0),
                new TimeSpan(13, 0, 0),
                _tenantId,
                dayOfMonth: 15,
                blockedSlotType: BlockedTimeSlotType.Meeting);

            // Assert
            Assert.Equal(15, pattern.DayOfMonth);
            Assert.True(pattern.IsForBlockedSlots());
            Assert.False(pattern.IsForAppointments());
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    Guid.Empty,
                    RecurrenceFrequency.Daily,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId));

            Assert.Contains("Clinic ID cannot be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithStartTimeAfterEndTime_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Daily,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(14, 0, 0),
                    new TimeSpan(13, 0, 0),
                    _tenantId));

            Assert.Contains("Start time must be before end time", exception.Message);
        }

        [Fact]
        public void Constructor_WithWeeklyFrequencyWithoutDaysOfWeek_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Weekly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId));

            Assert.Contains("Days of week must be specified", exception.Message);
        }

        [Fact]
        public void Constructor_WithMonthlyFrequencyWithoutDayOfMonth_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Monthly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId));

            Assert.Contains("Day of month must be specified", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidDayOfMonth_ThrowsArgumentException()
        {
            // Act & Assert - day of month = 0
            var exception1 = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Monthly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId,
                    dayOfMonth: 0));

            Assert.Contains("Day of month must be between 1 and 31", exception1.Message);

            // Act & Assert - day of month = 32
            var exception2 = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Monthly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId,
                    dayOfMonth: 32));

            Assert.Contains("Day of month must be between 1 and 31", exception2.Message);
        }

        [Fact]
        public void Constructor_WithAppointmentTypeAndBlockedSlotType_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Daily,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId,
                    appointmentType: AppointmentType.Regular,
                    blockedSlotType: BlockedTimeSlotType.Break));

            Assert.Contains("Pattern cannot be both appointment and blocked slot", exception.Message);
        }

        [Fact]
        public void Constructor_WithAppointmentTypeWithoutPatient_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Weekly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId,
                    daysOfWeek: RecurrenceDays.Monday,
                    appointmentType: AppointmentType.Regular));

            Assert.Contains("Patient ID is required for recurring appointments", exception.Message);
        }

        [Fact]
        public void Constructor_WithAppointmentTypeWithoutDuration_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new RecurringAppointmentPattern(
                    _clinicId,
                    RecurrenceFrequency.Weekly,
                    DateTime.Today.AddDays(1),
                    new TimeSpan(10, 0, 0),
                    new TimeSpan(11, 0, 0),
                    _tenantId,
                    patientId: _patientId,
                    daysOfWeek: RecurrenceDays.Monday,
                    appointmentType: AppointmentType.Regular));

            Assert.Contains("Duration is required for recurring appointments", exception.Message);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var pattern = CreateValidAppointmentPattern();
            Assert.True(pattern.IsActive);

            // Act
            pattern.Deactivate();

            // Assert
            Assert.False(pattern.IsActive);
            Assert.NotNull(pattern.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var pattern = CreateValidAppointmentPattern();
            pattern.Deactivate();
            Assert.False(pattern.IsActive);

            // Act
            pattern.Activate();

            // Assert
            Assert.True(pattern.IsActive);
        }

        [Fact]
        public void UpdateEndDate_UpdatesEndDateAndClearsOccurrencesCount()
        {
            // Arrange
            var pattern = CreateValidAppointmentPattern(occurrencesCount: 10);
            var newEndDate = DateTime.Today.AddMonths(1);

            // Act
            pattern.UpdateEndDate(newEndDate);

            // Assert
            Assert.Equal(newEndDate, pattern.EndDate);
            Assert.Null(pattern.OccurrencesCount);
        }

        [Fact]
        public void UpdateOccurrencesCount_UpdatesCountAndClearsEndDate()
        {
            // Arrange
            var pattern = CreateValidAppointmentPattern(endDate: DateTime.Today.AddMonths(1));
            var newCount = 20;

            // Act
            pattern.UpdateOccurrencesCount(newCount);

            // Assert
            Assert.Equal(newCount, pattern.OccurrencesCount);
            Assert.Null(pattern.EndDate);
        }

        [Fact]
        public void UpdateNotes_UpdatesNotesSuccessfully()
        {
            // Arrange
            var pattern = CreateValidAppointmentPattern();
            var newNotes = "Updated notes";

            // Act
            pattern.UpdateNotes(newNotes);

            // Assert
            Assert.Equal(newNotes, pattern.Notes);
        }

        private RecurringAppointmentPattern CreateValidAppointmentPattern(
            DateTime? endDate = null,
            int? occurrencesCount = null)
        {
            return new RecurringAppointmentPattern(
                _clinicId,
                RecurrenceFrequency.Weekly,
                DateTime.Today.AddDays(1),
                new TimeSpan(10, 0, 0),
                new TimeSpan(11, 0, 0),
                _tenantId,
                professionalId: _professionalId,
                patientId: _patientId,
                daysOfWeek: RecurrenceDays.Monday,
                endDate: endDate,
                occurrencesCount: occurrencesCount,
                durationMinutes: 60,
                appointmentType: AppointmentType.Regular);
        }
    }
}
