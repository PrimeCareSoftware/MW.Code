using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class PaymentFlowServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
        private readonly Mock<IInvoiceRepository> _invoiceRepositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IClinicRepository> _clinicRepositoryMock;
        private readonly PaymentFlowService _service;

        public PaymentFlowServiceTests()
        {
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _paymentRepositoryMock = new Mock<IPaymentRepository>();
            _invoiceRepositoryMock = new Mock<IInvoiceRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _clinicRepositoryMock = new Mock<IClinicRepository>();

            _service = new PaymentFlowService(
                _appointmentRepositoryMock.Object,
                _paymentRepositoryMock.Object,
                _invoiceRepositoryMock.Object,
                _patientRepositoryMock.Object,
                _clinicRepositoryMock.Object
            );
        }

        #region RegisterAppointmentPaymentAsync Tests

        [Fact]
        public async Task RegisterAppointmentPaymentAsync_WithValidData_CreatesPaymentAndInvoice()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var paidByUserId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var appointment = CreateValidAppointment(appointmentId, patientId, clinicId);
            var patient = CreateValidPatient(patientId);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync(appointment);
            
            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => p);

            _paymentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            _invoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .ReturnsAsync((Invoice i) => i);

            _invoiceRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterAppointmentPaymentAsync(
                appointmentId,
                paidByUserId,
                "Secretary",
                150.00m,
                "Cash",
                TenantId,
                "Payment received at reception"
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.AppointmentId.Should().Be(appointmentId);
            result.PaymentId.Should().NotBe(Guid.Empty);
            result.InvoiceId.Should().NotBeNull();
            result.ErrorMessage.Should().BeNull();

            // Verify appointment was marked as paid
            _appointmentRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Appointment>(a =>
                a.IsPaid == true &&
                a.PaymentAmount == 150.00m &&
                a.PaymentMethod == PaymentMethod.Cash
            )), Times.Once);

            // Verify payment was created
            _paymentRepositoryMock.Verify(r => r.AddAsync(It.Is<Payment>(p =>
                p.AppointmentId == appointmentId &&
                p.Amount == 150.00m &&
                p.Method == PaymentMethod.Cash
            )), Times.Once);

            // Verify payment was marked as paid
            _paymentRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Payment>()), Times.Once);

            // Verify invoice was created
            _invoiceRepositoryMock.Verify(r => r.AddAsync(It.Is<Invoice>(i =>
                i.Type == InvoiceType.Appointment &&
                i.Amount == 150.00m
            )), Times.Once);

            // Verify invoice was updated (issued and marked as paid)
            _invoiceRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Invoice>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAppointmentPaymentAsync_WithInvalidAppointmentId_ReturnsFailure()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var paidByUserId = Guid.NewGuid();

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync((Appointment?)null);

            // Act
            var result = await _service.RegisterAppointmentPaymentAsync(
                appointmentId,
                paidByUserId,
                "Secretary",
                150.00m,
                "Cash",
                TenantId
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Be("Appointment not found");
            result.PaymentId.Should().Be(Guid.Empty);
            result.InvoiceId.Should().BeNull();
        }

        [Fact]
        public async Task RegisterAppointmentPaymentAsync_WithInvalidPaymentReceiverType_ReturnsFailure()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var paidByUserId = Guid.NewGuid();
            var appointment = CreateValidAppointment(appointmentId, Guid.NewGuid(), Guid.NewGuid());

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.RegisterAppointmentPaymentAsync(
                appointmentId,
                paidByUserId,
                "InvalidType",
                150.00m,
                "Cash",
                TenantId
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Invalid PaymentReceiverType");
        }

        [Fact]
        public async Task RegisterAppointmentPaymentAsync_WithInvalidPaymentMethod_ReturnsFailure()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var paidByUserId = Guid.NewGuid();
            var appointment = CreateValidAppointment(appointmentId, Guid.NewGuid(), Guid.NewGuid());

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync(appointment);

            // Act
            var result = await _service.RegisterAppointmentPaymentAsync(
                appointmentId,
                paidByUserId,
                "Secretary",
                150.00m,
                "InvalidMethod",
                TenantId
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Invalid PaymentMethod");
        }

        [Fact]
        public async Task RegisterAppointmentPaymentAsync_WithDifferentPaymentMethods_CreatesCorrectPayments()
        {
            // Test all payment methods
            var paymentMethods = new[] { "Cash", "CreditCard", "DebitCard", "Pix", "BankTransfer", "Check" };

            foreach (var method in paymentMethods)
            {
                // Arrange
                var appointmentId = Guid.NewGuid();
                var paidByUserId = Guid.NewGuid();
                var patientId = Guid.NewGuid();
                var clinicId = Guid.NewGuid();

                var appointment = CreateValidAppointment(appointmentId, patientId, clinicId);
                var patient = CreateValidPatient(patientId);

                _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                    .ReturnsAsync(appointment);

                _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                    .ReturnsAsync(patient);

                _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                    .Returns(Task.CompletedTask);

                _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
                    .ReturnsAsync((Payment p) => p);

                _paymentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Payment>()))
                    .Returns(Task.CompletedTask);

                _invoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                    .ReturnsAsync((Invoice i) => i);

                _invoiceRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>()))
                    .Returns(Task.CompletedTask);

                // Act
                var result = await _service.RegisterAppointmentPaymentAsync(
                    appointmentId,
                    paidByUserId,
                    "Secretary",
                    100.00m,
                    method,
                    TenantId
                );

                // Assert
                result.Should().NotBeNull();
                result.Success.Should().BeTrue($"Payment method {method} should be valid");
            }
        }

        #endregion

        #region RegisterPaymentOnCompletionAsync Tests

        [Fact]
        public async Task RegisterPaymentOnCompletionAsync_WithValidData_CreatesPaymentAndInvoice()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var completedByUserId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var appointment = CreateValidAppointment(appointmentId, patientId, clinicId);
            var patient = CreateValidPatient(patientId);
            var clinic = CreateValidClinic(clinicId);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync(appointment);

            _clinicRepositoryMock.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => p);

            _paymentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            _invoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .ReturnsAsync((Invoice i) => i);

            _invoiceRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterPaymentOnCompletionAsync(
                appointmentId,
                completedByUserId,
                200.00m,
                "CreditCard",
                TenantId,
                "Payment received after consultation"
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.AppointmentId.Should().Be(appointmentId);
            result.PaymentId.Should().NotBe(Guid.Empty);
            result.InvoiceId.Should().NotBeNull();

            // Verify appointment was fetched
            _appointmentRepositoryMock.Verify(r => r.GetByIdAsync(appointmentId, TenantId), Times.Once);

            // Verify clinic default payment receiver was used
            _clinicRepositoryMock.Verify(r => r.GetByIdAsync(clinicId, TenantId), Times.Once);
        }

        [Fact]
        public async Task RegisterPaymentOnCompletionAsync_WithoutClinic_UsesDoctorAsDefaultReceiver()
        {
            // Arrange
            var appointmentId = Guid.NewGuid();
            var completedByUserId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var appointment = CreateValidAppointment(appointmentId, patientId, clinicId);
            var patient = CreateValidPatient(patientId);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(appointmentId, TenantId))
                .ReturnsAsync(appointment);

            _clinicRepositoryMock.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync((Clinic?)null); // Clinic not found

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _appointmentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Appointment>()))
                .Returns(Task.CompletedTask);

            _paymentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Payment>()))
                .ReturnsAsync((Payment p) => p);

            _paymentRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Payment>()))
                .Returns(Task.CompletedTask);

            _invoiceRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Invoice>()))
                .ReturnsAsync((Invoice i) => i);

            _invoiceRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Invoice>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.RegisterPaymentOnCompletionAsync(
                appointmentId,
                completedByUserId,
                200.00m,
                "Cash",
                TenantId
            );

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            
            // Verify Doctor is used as default when clinic is not found
            _appointmentRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Appointment>(a =>
                a.PaymentReceivedBy == PaymentReceiverType.Doctor
            )), Times.Once);
        }

        #endregion

        #region Helper Methods

        private Appointment CreateValidAppointment(Guid appointmentId, Guid patientId, Guid clinicId)
        {
            return new Appointment(
                patientId: patientId,
                clinicId: clinicId,
                scheduledDate: DateTime.Today.AddDays(1),
                scheduledTime: new TimeSpan(10, 0, 0),
                durationMinutes: 30,
                type: AppointmentType.Regular,
                tenantId: TenantId,
                mode: AppointmentMode.InPerson,
                paymentType: PaymentType.Private,
                professionalId: Guid.NewGuid()
            );
        }

        private Patient CreateValidPatient(Guid patientId)
        {
            return new Patient(
                name: "John Doe",
                document: "12345678901",
                dateOfBirth: DateTime.Now.AddYears(-30),
                gender: "Male",
                email: new MedicSoft.Domain.ValueObjects.Email("john@example.com"),
                phone: new MedicSoft.Domain.ValueObjects.Phone("+55", "11", "987654321"),
                address: new MedicSoft.Domain.ValueObjects.Address(
                    street: "Main St",
                    number: "123",
                    city: "Sao Paulo",
                    state: "SP",
                    postalCode: "01234567",
                    country: "Brazil"
                ),
                tenantId: TenantId
            );
        }

        private Clinic CreateValidClinic(Guid clinicId)
        {
            var clinic = new Clinic(
                name: "Test Clinic",
                tradeName: "Test Clinic Ltd",
                document: "12345678000190",
                phone: "1234567890",
                email: "test@clinic.com",
                address: "123 Main St, São Paulo, SP",
                openingTime: new TimeSpan(8, 0, 0),
                closingTime: new TimeSpan(18, 0, 0),
                tenantId: TenantId,
                appointmentDurationMinutes: 30
            );
            // Set default payment receiver to Secretary
            clinic.UpdatePaymentReceiverType(PaymentReceiverType.Secretary);
            return clinic;
        }

        #endregion
    }
}
