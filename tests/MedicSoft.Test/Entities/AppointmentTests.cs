using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class AppointmentTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _clinicId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesAppointment()
        {
            // Arrange
            var scheduledDate = DateTime.Today.AddDays(1);
            var scheduledTime = new TimeSpan(10, 0, 0);
            var duration = 30;
            var type = AppointmentType.Regular;

            // Act
            var appointment = new Appointment(_patientId, _clinicId, scheduledDate, 
                scheduledTime, duration, type, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, appointment.Id);
            Assert.Equal(_patientId, appointment.PatientId);
            Assert.Equal(_clinicId, appointment.ClinicId);
            Assert.Equal(scheduledDate, appointment.ScheduledDate);
            Assert.Equal(scheduledTime, appointment.ScheduledTime);
            Assert.Equal(duration, appointment.DurationMinutes);
            Assert.Equal(type, appointment.Type);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
            Assert.Null(appointment.Notes);
            Assert.Null(appointment.CancellationReason);
        }

        [Fact]
        public void Constructor_WithNotes_CreatesAppointmentWithNotes()
        {
            // Arrange
            var notes = "Patient has specific requirements";

            // Act
            var appointment = CreateValidAppointment(notes);

            // Assert
            Assert.Equal(notes, appointment.Notes);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Appointment(Guid.Empty, _clinicId, DateTime.Today.AddDays(1), 
                    new TimeSpan(10, 0, 0), 30, AppointmentType.Regular, _tenantId));

            Assert.Equal("O ID do paciente não pode estar vazio (Parameter 'patientId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Appointment(_patientId, Guid.Empty, DateTime.Today.AddDays(1), 
                    new TimeSpan(10, 0, 0), 30, AppointmentType.Regular, _tenantId));

            Assert.Equal("O ID da clínica não pode estar vazio (Parameter 'clinicId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithPastDate_ThrowsArgumentException()
        {
            // Arrange
            var pastDate = DateTime.Today.AddDays(-1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Appointment(_patientId, _clinicId, pastDate, 
                    new TimeSpan(10, 0, 0), 30, AppointmentType.Regular, _tenantId));

            Assert.Equal("A data agendada não pode estar no passado (Parameter 'scheduledDate')", exception.Message);
        }

        [Fact]
        public void Constructor_WithPastDateAndAllowHistoricalData_CreatesAppointment()
        {
            // Arrange
            var pastDate = DateTime.Today.AddDays(-7);
            var scheduledTime = new TimeSpan(10, 0, 0);
            var duration = 30;
            var type = AppointmentType.Regular;

            // Act
            var appointment = new Appointment(_patientId, _clinicId, pastDate, 
                scheduledTime, duration, type, _tenantId, null, allowHistoricalData: true);

            // Assert
            Assert.NotEqual(Guid.Empty, appointment.Id);
            Assert.Equal(_patientId, appointment.PatientId);
            Assert.Equal(_clinicId, appointment.ClinicId);
            Assert.Equal(pastDate, appointment.ScheduledDate);
            Assert.Equal(scheduledTime, appointment.ScheduledTime);
            Assert.Equal(duration, appointment.DurationMinutes);
            Assert.Equal(type, appointment.Type);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
        }

        [Fact]
        public void Constructor_WithInvalidDuration_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Appointment(_patientId, _clinicId, DateTime.Today.AddDays(1), 
                    new TimeSpan(10, 0, 0), 0, AppointmentType.Regular, _tenantId));

            Assert.Equal("A duração deve ser positiva (Parameter 'durationMinutes')", exception.Message);
        }

        [Fact]
        public void Confirm_WithScheduledAppointment_ConfirmsAppointment()
        {
            // Arrange
            var appointment = CreateValidAppointment();

            // Act
            appointment.Confirm();

            // Assert
            Assert.Equal(AppointmentStatus.Confirmed, appointment.Status);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void Confirm_WithNonScheduledAppointment_ThrowsInvalidOperationException()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            appointment.Confirm();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => appointment.Confirm());
            Assert.Equal("Apenas agendamentos marcados podem ser confirmados", exception.Message);
        }

        [Fact]
        public void Cancel_WithValidReason_CancelsAppointment()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            var reason = "Patient requested cancellation";

            // Act
            appointment.Cancel(reason);

            // Assert
            Assert.Equal(AppointmentStatus.Cancelled, appointment.Status);
            Assert.Equal(reason, appointment.CancellationReason);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void Cancel_CompletedAppointment_ThrowsInvalidOperationException()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            appointment.CheckIn();
            appointment.CheckOut();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => appointment.Cancel("reason"));
            Assert.Equal("Não é possível cancelar agendamentos concluídos ou já cancelados", exception.Message);
        }

        [Fact]
        public void MarkAsNoShow_WithScheduledAppointment_MarksAsNoShow()
        {
            // Arrange
            var appointment = CreateValidAppointment();

            // Act
            appointment.MarkAsNoShow();

            // Assert
            Assert.Equal(AppointmentStatus.NoShow, appointment.Status);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void CheckIn_WithConfirmedAppointment_ChecksInAppointment()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            appointment.Confirm();

            // Act
            appointment.CheckIn();

            // Assert
            Assert.Equal(AppointmentStatus.InProgress, appointment.Status);
            Assert.NotNull(appointment.CheckInTime);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void CheckOut_WithInProgressAppointment_ChecksOutAppointment()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            appointment.CheckIn();
            var notes = "Consultation completed successfully";

            // Act
            appointment.CheckOut(notes);

            // Assert
            Assert.Equal(AppointmentStatus.Completed, appointment.Status);
            Assert.NotNull(appointment.CheckOutTime);
            Assert.Equal(notes, appointment.Notes);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void CheckOut_WithNonInProgressAppointment_ThrowsInvalidOperationException()
        {
            // Arrange
            var appointment = CreateValidAppointment();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => appointment.CheckOut());
            Assert.Equal("Apenas agendamentos em andamento podem fazer check-out", exception.Message);
        }

        [Fact]
        public void Reschedule_WithNewDateTime_ReschedulesAppointment()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            var newDate = DateTime.Today.AddDays(5);
            var newTime = new TimeSpan(14, 0, 0);

            // Act
            appointment.Reschedule(newDate, newTime);

            // Assert
            Assert.Equal(newDate, appointment.ScheduledDate);
            Assert.Equal(newTime, appointment.ScheduledTime);
            Assert.Equal(AppointmentStatus.Scheduled, appointment.Status);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void Reschedule_CompletedAppointment_ThrowsInvalidOperationException()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            appointment.CheckIn();
            appointment.CheckOut();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                appointment.Reschedule(DateTime.Today.AddDays(5), new TimeSpan(14, 0, 0)));
            Assert.Equal("Não é possível reagendar consultas concluídas ou canceladas", exception.Message);
        }

        [Fact]
        public void UpdateNotes_UpdatesAppointmentNotes()
        {
            // Arrange
            var appointment = CreateValidAppointment();
            var newNotes = "Updated notes";

            // Act
            appointment.UpdateNotes(newNotes);

            // Assert
            Assert.Equal(newNotes, appointment.Notes);
            Assert.NotNull(appointment.UpdatedAt);
        }

        [Fact]
        public void GetScheduledDateTime_ReturnsCombinedDateTime()
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var time = new TimeSpan(10, 30, 0);
            var appointment = new Appointment(_patientId, _clinicId, date, time, 30, 
                AppointmentType.Regular, _tenantId);

            // Act
            var scheduledDateTime = appointment.GetScheduledDateTime();

            // Assert
            Assert.Equal(date.Add(time), scheduledDateTime);
        }

        [Fact]
        public void GetEndDateTime_ReturnsEndDateTime()
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var time = new TimeSpan(10, 0, 0);
            var duration = 30;
            var appointment = new Appointment(_patientId, _clinicId, date, time, duration, 
                AppointmentType.Regular, _tenantId);

            // Act
            var endDateTime = appointment.GetEndDateTime();

            // Assert
            Assert.Equal(date.Add(time).AddMinutes(duration), endDateTime);
        }

        [Theory]
        [InlineData(10, 0, 30, 10, 15, 10, 45, true)]  // Overlaps
        [InlineData(10, 0, 30, 10, 30, 11, 0, false)]  // Adjacent, no overlap
        [InlineData(10, 0, 30, 9, 0, 9, 30, false)]    // Before, no overlap
        [InlineData(10, 0, 30, 11, 0, 11, 30, false)]  // After, no overlap
        public void IsOverlapping_ReturnsCorrectValue(
            int appointmentHour, int appointmentMinute, int appointmentDuration,
            int checkStartHour, int checkStartMinute, int checkEndHour, int checkEndMinute,
            bool expectedResult)
        {
            // Arrange
            var date = DateTime.Today.AddDays(1);
            var time = new TimeSpan(appointmentHour, appointmentMinute, 0);
            var appointment = new Appointment(_patientId, _clinicId, date, time, 
                appointmentDuration, AppointmentType.Regular, _tenantId);

            var checkStart = date.Add(new TimeSpan(checkStartHour, checkStartMinute, 0));
            var checkEnd = date.Add(new TimeSpan(checkEndHour, checkEndMinute, 0));

            // Act
            var result = appointment.IsOverlapping(checkStart, checkEnd);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        private Appointment CreateValidAppointment(string? notes = null)
        {
            return new Appointment(
                _patientId,
                _clinicId,
                DateTime.Today.AddDays(1),
                new TimeSpan(10, 0, 0),
                30,
                AppointmentType.Regular,
                _tenantId,
                AppointmentMode.InPerson,
                PaymentType.Private,
                null,
                null,
                notes
            );
        }
    }
}
