using System;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class AppointmentProcedureTests
    {
        private const string TestTenantId = "test-tenant";
        private readonly Guid TestAppointmentId = Guid.NewGuid();
        private readonly Guid TestProcedureId = Guid.NewGuid();
        private readonly Guid TestPatientId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesAppointmentProcedure()
        {
            // Arrange & Act
            var appointmentProcedure = new AppointmentProcedure(
                TestAppointmentId,
                TestProcedureId,
                TestPatientId,
                150.00m,
                DateTime.UtcNow,
                TestTenantId,
                "Test notes"
            );

            // Assert
            Assert.Equal(TestAppointmentId, appointmentProcedure.AppointmentId);
            Assert.Equal(TestProcedureId, appointmentProcedure.ProcedureId);
            Assert.Equal(TestPatientId, appointmentProcedure.PatientId);
            Assert.Equal(150.00m, appointmentProcedure.PriceCharged);
            Assert.Equal("Test notes", appointmentProcedure.Notes);
        }

        [Fact]
        public void Constructor_WithEmptyAppointmentId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AppointmentProcedure(
                    Guid.Empty,
                    TestProcedureId,
                    TestPatientId,
                    150.00m,
                    DateTime.UtcNow,
                    TestTenantId
                ));
        }

        [Fact]
        public void Constructor_WithEmptyProcedureId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AppointmentProcedure(
                    TestAppointmentId,
                    Guid.Empty,
                    TestPatientId,
                    150.00m,
                    DateTime.UtcNow,
                    TestTenantId
                ));
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AppointmentProcedure(
                    TestAppointmentId,
                    TestProcedureId,
                    Guid.Empty,
                    150.00m,
                    DateTime.UtcNow,
                    TestTenantId
                ));
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new AppointmentProcedure(
                    TestAppointmentId,
                    TestProcedureId,
                    TestPatientId,
                    -10.00m,
                    DateTime.UtcNow,
                    TestTenantId
                ));
        }

        [Fact]
        public void UpdateNotes_WithValidNotes_UpdatesNotes()
        {
            // Arrange
            var appointmentProcedure = CreateValidAppointmentProcedure();

            // Act
            appointmentProcedure.UpdateNotes("Updated notes");

            // Assert
            Assert.Equal("Updated notes", appointmentProcedure.Notes);
            Assert.NotNull(appointmentProcedure.UpdatedAt);
        }

        [Fact]
        public void UpdateNotes_WithNullNotes_SetsNotesToNull()
        {
            // Arrange
            var appointmentProcedure = CreateValidAppointmentProcedure();

            // Act
            appointmentProcedure.UpdateNotes(null!);

            // Assert
            Assert.Null(appointmentProcedure.Notes);
        }

        [Fact]
        public void UpdatePrice_WithValidPrice_UpdatesPrice()
        {
            // Arrange
            var appointmentProcedure = CreateValidAppointmentProcedure();

            // Act
            appointmentProcedure.UpdatePrice(200.00m);

            // Assert
            Assert.Equal(200.00m, appointmentProcedure.PriceCharged);
            Assert.NotNull(appointmentProcedure.UpdatedAt);
        }

        [Fact]
        public void UpdatePrice_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var appointmentProcedure = CreateValidAppointmentProcedure();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                appointmentProcedure.UpdatePrice(-10.00m));
        }

        [Fact]
        public void UpdatePrice_WithZeroPrice_UpdatesPrice()
        {
            // Arrange
            var appointmentProcedure = CreateValidAppointmentProcedure();

            // Act
            appointmentProcedure.UpdatePrice(0m);

            // Assert
            Assert.Equal(0m, appointmentProcedure.PriceCharged);
        }

        private AppointmentProcedure CreateValidAppointmentProcedure()
        {
            return new AppointmentProcedure(
                TestAppointmentId,
                TestProcedureId,
                TestPatientId,
                150.00m,
                DateTime.UtcNow,
                TestTenantId,
                "Initial notes"
            );
        }
    }
}
