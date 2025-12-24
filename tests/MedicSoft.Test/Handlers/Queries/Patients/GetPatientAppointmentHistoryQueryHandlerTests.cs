using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MedicSoft.Application.Handlers.Queries.Patients;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;
using MedicSoft.Repository.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace MedicSoft.Test.Handlers.Queries.Patients
{
    public class GetPatientAppointmentHistoryQueryHandlerTests
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly GetPatientAppointmentHistoryQueryHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public GetPatientAppointmentHistoryQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new MedicSoftDbContext(options);

            _patientRepositoryMock = new Mock<IPatientRepository>();
            _handler = new GetPatientAppointmentHistoryQueryHandler(_context, _patientRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidPatientAndNoAppointments_ReturnsEmptyHistory()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var patient = CreateTestPatient(patientId, "John Doe");

            _patientRepositoryMock
                .Setup(x => x.GetByIdAsync(patientId, _tenantId))
                .ReturnsAsync(patient);

            var query = new GetPatientAppointmentHistoryQuery(patientId, _tenantId, false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(patientId, result.PatientId);
            Assert.Equal("John Doe", result.PatientName);
            Assert.Empty(result.Appointments);
        }

        [Fact]
        public async Task Handle_WithNonExistentPatient_ThrowsInvalidOperationException()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _patientRepositoryMock
                .Setup(x => x.GetByIdAsync(patientId, _tenantId))
                .ReturnsAsync((Patient?)null);

            var query = new GetPatientAppointmentHistoryQuery(patientId, _tenantId, false);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(query, CancellationToken.None));
        }

        private Patient CreateTestPatient(Guid id, string name)
        {
            var patient = new Patient(
                name,
                "12345678901",
                DateTime.UtcNow.AddYears(-30),
                "M",
                new Email("test@example.com"),
                new Phone("+55", "11999999999"),
                new Address("Test St", "123", "Test City", "Test Neighborhood", "TS", "12345-678", "Brasil"),
                _tenantId
            );

            return patient;
        }
    }
}
