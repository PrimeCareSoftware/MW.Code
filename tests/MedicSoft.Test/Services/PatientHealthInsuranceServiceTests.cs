using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class PatientHealthInsuranceServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<IPatientHealthInsuranceRepository> _repositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IHealthInsurancePlanRepository> _planRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly PatientHealthInsuranceService _service;

        public PatientHealthInsuranceServiceTests()
        {
            _repositoryMock = new Mock<IPatientHealthInsuranceRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _planRepositoryMock = new Mock<IHealthInsurancePlanRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new PatientHealthInsuranceService(
                _repositoryMock.Object,
                _patientRepositoryMock.Object,
                _planRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesInsurance()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new CreatePatientHealthInsuranceDto
            {
                PatientId = patientId,
                HealthInsurancePlanId = planId,
                CardNumber = "1234567890",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true,
                CardValidationCode = "ABC123"
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);
            var plan = new HealthInsurancePlan("Test Plan", Guid.NewGuid(), "123456", TenantId);

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _planRepositoryMock.Setup(r => r.GetByIdAsync(planId, TenantId))
                .ReturnsAsync(plan);

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(dto.CardNumber, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PatientHealthInsurance>()))
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var insurance = new PatientHealthInsurance(patientId, planId, dto.CardNumber, dto.ValidFrom, TenantId, true);
            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(insurance);

            var expectedDto = new PatientHealthInsuranceDto { Id = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(It.IsAny<PatientHealthInsurance>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PatientHealthInsurance>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentPatient_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreatePatientHealthInsuranceDto
            {
                PatientId = Guid.NewGuid(),
                HealthInsurancePlanId = Guid.NewGuid(),
                CardNumber = "1234567890",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientId, TenantId))
                .ReturnsAsync((Patient?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Patient*not found*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PatientHealthInsurance>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentPlan_ThrowsInvalidOperationException()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new CreatePatientHealthInsuranceDto
            {
                PatientId = patientId,
                HealthInsurancePlanId = Guid.NewGuid(),
                CardNumber = "1234567890",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _planRepositoryMock.Setup(r => r.GetByIdAsync(dto.HealthInsurancePlanId, TenantId))
                .ReturnsAsync((HealthInsurancePlan?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*plan*not found*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PatientHealthInsurance>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateCardNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new CreatePatientHealthInsuranceDto
            {
                PatientId = patientId,
                HealthInsurancePlanId = planId,
                CardNumber = "1234567890",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);
            var plan = new HealthInsurancePlan("Test Plan", Guid.NewGuid(), "123456", TenantId);
            var existingInsurance = new PatientHealthInsurance(Guid.NewGuid(), planId, dto.CardNumber, DateTime.UtcNow, TenantId, true);

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _planRepositoryMock.Setup(r => r.GetByIdAsync(planId, TenantId))
                .ReturnsAsync(plan);

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(dto.CardNumber, TenantId))
                .ReturnsAsync(existingInsurance);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already registered*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<PatientHealthInsurance>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesInsurance()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new UpdatePatientHealthInsuranceDto
            {
                CardNumber = "9876543210",
                CardValidationCode = "XYZ789",
                ValidFrom = DateTime.UtcNow.AddDays(-30),
                ValidUntil = DateTime.UtcNow.AddDays(365),
                IsHolder = true
            };

            var existingInsurance = new PatientHealthInsurance(patientId, planId, "1234567890", DateTime.UtcNow, TenantId, true);

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(existingInsurance);

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(dto.CardNumber, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(insuranceId, TenantId))
                .ReturnsAsync(existingInsurance);

            var expectedDto = new PatientHealthInsuranceDto { Id = insuranceId };
            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(It.IsAny<PatientHealthInsurance>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.UpdateAsync(insuranceId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(insuranceId);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();
            var dto = new UpdatePatientHealthInsuranceDto
            {
                CardNumber = "9876543210",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var act = async () => await _service.UpdateAsync(insuranceId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithDuplicateCardNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new UpdatePatientHealthInsuranceDto
            {
                CardNumber = "9876543210",
                ValidFrom = DateTime.UtcNow,
                IsHolder = true
            };

            var existingInsurance = new PatientHealthInsurance(patientId, planId, "1234567890", DateTime.UtcNow, TenantId, true);
            var duplicateInsurance = new PatientHealthInsurance(Guid.NewGuid(), planId, dto.CardNumber, DateTime.UtcNow, TenantId, true);

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(existingInsurance);

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(dto.CardNumber, TenantId))
                .ReturnsAsync(duplicateInsurance);

            // Act
            var act = async () => await _service.UpdateAsync(insuranceId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already registered*");
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()), Times.Never);
        }

        #endregion

        #region GetByPatientIdAsync Tests

        [Fact]
        public async Task GetByPatientIdAsync_WithExistingPatient_ReturnsInsurances()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurances = new List<PatientHealthInsurance>
            {
                new PatientHealthInsurance(patientId, planId, "1234567890", DateTime.UtcNow, TenantId, true),
                new PatientHealthInsurance(patientId, planId, "0987654321", DateTime.UtcNow, TenantId, false)
            };

            _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, TenantId))
                .ReturnsAsync(insurances);

            var expectedDtos = new List<PatientHealthInsuranceDto>
            {
                new PatientHealthInsuranceDto { Id = Guid.NewGuid() },
                new PatientHealthInsuranceDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<PatientHealthInsuranceDto>>(insurances))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetByPatientIdAsync(patientId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetByPatientIdAsync(patientId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByPatientIdAsync_WithNoInsurances_ReturnsEmptyList()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, TenantId))
                .ReturnsAsync(new List<PatientHealthInsurance>());

            _mapperMock.Setup(m => m.Map<IEnumerable<PatientHealthInsuranceDto>>(It.IsAny<IEnumerable<PatientHealthInsurance>>()))
                .Returns(new List<PatientHealthInsuranceDto>());

            // Act
            var result = await _service.GetByPatientIdAsync(patientId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetActiveByPatientIdAsync Tests

        [Fact]
        public async Task GetActiveByPatientIdAsync_ReturnsOnlyActiveAndValidInsurances()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            
            var activeInsurance = new PatientHealthInsurance(
                patientId, planId, "1234567890", 
                DateTime.UtcNow.AddDays(-30), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(30)
            );

            var inactiveInsurance = new PatientHealthInsurance(
                patientId, planId, "0987654321", 
                DateTime.UtcNow.AddDays(-30), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(30)
            );
            inactiveInsurance.Deactivate();

            var expiredInsurance = new PatientHealthInsurance(
                patientId, planId, "1111111111", 
                DateTime.UtcNow.AddDays(-365), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(-1)
            );

            var insurances = new List<PatientHealthInsurance>
            {
                activeInsurance,
                inactiveInsurance,
                expiredInsurance
            };

            _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, TenantId))
                .ReturnsAsync(insurances);

            _mapperMock.Setup(m => m.Map<IEnumerable<PatientHealthInsuranceDto>>(It.IsAny<IEnumerable<PatientHealthInsurance>>()))
                .Returns((IEnumerable<PatientHealthInsurance> src) => 
                    src.Select(i => new PatientHealthInsuranceDto { Id = i.Id }).ToList());

            // Act
            var result = await _service.GetActiveByPatientIdAsync(patientId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
        }

        #endregion

        #region ValidateCardAsync Tests

        [Fact]
        public async Task ValidateCardAsync_WithValidCard_ReturnsValidResult()
        {
            // Arrange
            var cardNumber = "1234567890";
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(
                patientId, planId, cardNumber, 
                DateTime.UtcNow.AddDays(-30), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(30)
            );

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(cardNumber, TenantId))
                .ReturnsAsync(insurance);

            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(insurance))
                .Returns(new PatientHealthInsuranceDto { Id = insurance.Id });

            // Act
            var result = await _service.ValidateCardAsync(cardNumber, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Message.Should().Contain("valid");
            result.Insurance.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateCardAsync_WithNonExistentCard_ReturnsInvalidResult()
        {
            // Arrange
            var cardNumber = "9999999999";

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(cardNumber, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var result = await _service.ValidateCardAsync(cardNumber, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("not found");
            result.Insurance.Should().BeNull();
        }

        [Fact]
        public async Task ValidateCardAsync_WithInactiveCard_ReturnsInvalidResult()
        {
            // Arrange
            var cardNumber = "1234567890";
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(
                patientId, planId, cardNumber, 
                DateTime.UtcNow.AddDays(-30), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(30)
            );
            insurance.Deactivate();

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(cardNumber, TenantId))
                .ReturnsAsync(insurance);

            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(insurance))
                .Returns(new PatientHealthInsuranceDto { Id = insurance.Id });

            // Act
            var result = await _service.ValidateCardAsync(cardNumber, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("inactive");
            result.Insurance.Should().NotBeNull();
        }

        [Fact]
        public async Task ValidateCardAsync_WithExpiredCard_ReturnsInvalidResult()
        {
            // Arrange
            var cardNumber = "1234567890";
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(
                patientId, planId, cardNumber, 
                DateTime.UtcNow.AddDays(-365), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(-1)
            );

            _repositoryMock.Setup(r => r.GetByCardNumberAsync(cardNumber, TenantId))
                .ReturnsAsync(insurance);

            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(insurance))
                .Returns(new PatientHealthInsuranceDto { Id = insurance.Id });

            // Act
            var result = await _service.ValidateCardAsync(cardNumber, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("expired");
            result.Insurance.Should().NotBeNull();
        }

        #endregion

        #region ActivateAsync Tests

        [Fact]
        public async Task ActivateAsync_WithValidInsurance_ActivatesInsurance()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(patientId, planId, "1234567890", DateTime.UtcNow, TenantId, true);
            insurance.Deactivate();

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            var expectedDto = new PatientHealthInsuranceDto { Id = insuranceId };
            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(It.IsAny<PatientHealthInsurance>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ActivateAsync(insuranceId, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_WithNonExistentInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var act = async () => await _service.ActivateAsync(insuranceId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region DeactivateAsync Tests

        [Fact]
        public async Task DeactivateAsync_WithValidInsurance_DeactivatesInsurance()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(patientId, planId, "1234567890", DateTime.UtcNow, TenantId, true);

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            var expectedDto = new PatientHealthInsuranceDto { Id = insuranceId };
            _mapperMock.Setup(m => m.Map<PatientHealthInsuranceDto>(It.IsAny<PatientHealthInsurance>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.DeactivateAsync(insuranceId, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<PatientHealthInsurance>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_WithNonExistentInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var insuranceId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var act = async () => await _service.DeactivateAsync(insuranceId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion
    }
}
