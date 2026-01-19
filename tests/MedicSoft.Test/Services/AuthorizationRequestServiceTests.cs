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
    public class AuthorizationRequestServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<IAuthorizationRequestRepository> _repositoryMock;
        private readonly Mock<IPatientRepository> _patientRepositoryMock;
        private readonly Mock<IPatientHealthInsuranceRepository> _insuranceRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthorizationRequestService _service;

        public AuthorizationRequestServiceTests()
        {
            _repositoryMock = new Mock<IAuthorizationRequestRepository>();
            _patientRepositoryMock = new Mock<IPatientRepository>();
            _insuranceRepositoryMock = new Mock<IPatientHealthInsuranceRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new AuthorizationRequestService(
                _repositoryMock.Object,
                _patientRepositoryMock.Object,
                _insuranceRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesRequest()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var insuranceId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new CreateAuthorizationRequestDto
            {
                PatientId = patientId,
                PatientHealthInsuranceId = insuranceId,
                ProcedureCode = "12345",
                ProcedureDescription = "Test Procedure",
                Quantity = 1,
                ClinicalIndication = "Test indication",
                Diagnosis = "Test diagnosis"
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);
            var insurance = new PatientHealthInsurance(
                patientId, planId, "1234567890", 
                DateTime.UtcNow.AddDays(-30), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(30)
            );

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<AuthorizationRequest>()))
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var request = new AuthorizationRequest(patientId, insuranceId, "AUTH-123", dto.ProcedureCode, dto.ProcedureDescription, dto.Quantity, TenantId);
            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(request);

            var expectedDto = new AuthorizationRequestDto { Id = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<AuthorizationRequestDto>(It.IsAny<AuthorizationRequest>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<AuthorizationRequest>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentPatient_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateAuthorizationRequestDto
            {
                PatientId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                ProcedureCode = "12345",
                ProcedureDescription = "Test",
                Quantity = 1
            };

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientId, TenantId))
                .ReturnsAsync((Patient?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Patient*not found*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<AuthorizationRequest>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var dto = new CreateAuthorizationRequestDto
            {
                PatientId = patientId,
                PatientHealthInsuranceId = Guid.NewGuid(),
                ProcedureCode = "12345",
                ProcedureDescription = "Test",
                Quantity = 1
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientHealthInsuranceId, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*insurance*not found*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<AuthorizationRequest>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithInvalidInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var insuranceId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var dto = new CreateAuthorizationRequestDto
            {
                PatientId = patientId,
                PatientHealthInsuranceId = insuranceId,
                ProcedureCode = "12345",
                ProcedureDescription = "Test",
                Quantity = 1
            };

            var patient = new Patient("John Doe", new DateTime(1990, 1, 1), "12345678900", TenantId);
            var insurance = new PatientHealthInsurance(
                patientId, planId, "1234567890", 
                DateTime.UtcNow.AddDays(-365), TenantId, true,
                validUntil: DateTime.UtcNow.AddDays(-1)
            );

            _patientRepositoryMock.Setup(r => r.GetByIdAsync(patientId, TenantId))
                .ReturnsAsync(patient);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(insuranceId, TenantId))
                .ReturnsAsync(insurance);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not valid*");
        }

        #endregion

        #region ApproveAsync Tests

        [Fact]
        public async Task ApproveAsync_WithValidRequest_ApprovesRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var dto = new ApproveAuthorizationDto
            {
                AuthorizationNumber = "AUTH-APPROVED-123",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            var request = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-123", 
                "12345", "Test Procedure", 1, TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync(request);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(requestId, TenantId))
                .ReturnsAsync(request);

            var expectedDto = new AuthorizationRequestDto { Id = requestId };
            _mapperMock.Setup(m => m.Map<AuthorizationRequestDto>(It.IsAny<AuthorizationRequest>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ApproveAsync(requestId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ApproveAsync_WithNonExistentRequest_ThrowsInvalidOperationException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var dto = new ApproveAuthorizationDto
            {
                AuthorizationNumber = "AUTH-123",
                ExpirationDate = DateTime.UtcNow.AddDays(30)
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync((AuthorizationRequest?)null);

            // Act
            var act = async () => await _service.ApproveAsync(requestId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region DenyAsync Tests

        [Fact]
        public async Task DenyAsync_WithValidRequest_DeniesRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var dto = new DenyAuthorizationDto
            {
                DenialReason = "Insurance plan does not cover this procedure"
            };

            var request = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-123", 
                "12345", "Test Procedure", 1, TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync(request);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(requestId, TenantId))
                .ReturnsAsync(request);

            var expectedDto = new AuthorizationRequestDto { Id = requestId };
            _mapperMock.Setup(m => m.Map<AuthorizationRequestDto>(It.IsAny<AuthorizationRequest>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.DenyAsync(requestId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DenyAsync_WithNonExistentRequest_ThrowsInvalidOperationException()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var dto = new DenyAuthorizationDto
            {
                DenialReason = "Test reason"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync((AuthorizationRequest?)null);

            // Act
            var act = async () => await _service.DenyAsync(requestId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region CancelAsync Tests

        [Fact]
        public async Task CancelAsync_WithValidRequest_CancelsRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-123", 
                "12345", "Test Procedure", 1, TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync(request);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(requestId, TenantId))
                .ReturnsAsync(request);

            var expectedDto = new AuthorizationRequestDto { Id = requestId };
            _mapperMock.Setup(m => m.Map<AuthorizationRequestDto>(It.IsAny<AuthorizationRequest>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CancelAsync(requestId, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelAsync_WithNonExistentRequest_ThrowsInvalidOperationException()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(requestId, TenantId))
                .ReturnsAsync((AuthorizationRequest?)null);

            // Act
            var act = async () => await _service.CancelAsync(requestId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsRequest()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var request = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-123", 
                "12345", "Test Procedure", 1, TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(requestId, TenantId))
                .ReturnsAsync(request);

            var expectedDto = new AuthorizationRequestDto { Id = requestId };
            _mapperMock.Setup(m => m.Map<AuthorizationRequestDto>(request))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(requestId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(requestId);
            _repositoryMock.Verify(r => r.GetByIdWithDetailsAsync(requestId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdWithDetailsAsync(requestId, TenantId))
                .ReturnsAsync((AuthorizationRequest?)null);

            // Act
            var result = await _service.GetByIdAsync(requestId, TenantId);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetByPatientIdAsync Tests

        [Fact]
        public async Task GetByPatientIdAsync_WithExistingPatient_ReturnsRequests()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var requests = new List<AuthorizationRequest>
            {
                new AuthorizationRequest(patientId, Guid.NewGuid(), "REQ-1", "12345", "Procedure 1", 1, TenantId),
                new AuthorizationRequest(patientId, Guid.NewGuid(), "REQ-2", "67890", "Procedure 2", 2, TenantId)
            };

            _repositoryMock.Setup(r => r.GetByPatientIdAsync(patientId, TenantId))
                .ReturnsAsync(requests);

            var expectedDtos = new List<AuthorizationRequestDto>
            {
                new AuthorizationRequestDto { Id = Guid.NewGuid() },
                new AuthorizationRequestDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<AuthorizationRequestDto>>(requests))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetByPatientIdAsync(patientId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetByPatientIdAsync(patientId, TenantId), Times.Once);
        }

        #endregion

        #region GetPendingAsync Tests

        [Fact]
        public async Task GetPendingAsync_ReturnsPendingRequests()
        {
            // Arrange
            var requests = new List<AuthorizationRequest>
            {
                new AuthorizationRequest(Guid.NewGuid(), Guid.NewGuid(), "REQ-1", "12345", "Procedure 1", 1, TenantId),
                new AuthorizationRequest(Guid.NewGuid(), Guid.NewGuid(), "REQ-2", "67890", "Procedure 2", 2, TenantId)
            };

            _repositoryMock.Setup(r => r.GetByStatusAsync(AuthorizationStatus.Pending, TenantId))
                .ReturnsAsync(requests);

            var expectedDtos = new List<AuthorizationRequestDto>
            {
                new AuthorizationRequestDto { Id = Guid.NewGuid() },
                new AuthorizationRequestDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<AuthorizationRequestDto>>(requests))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetPendingAsync(TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetByStatusAsync(AuthorizationStatus.Pending, TenantId), Times.Once);
        }

        #endregion

        #region MarkExpiredAuthorizationsAsync Tests

        [Fact]
        public async Task MarkExpiredAuthorizationsAsync_WithExpiredRequests_MarksThemAsExpired()
        {
            // Arrange
            var approvedRequest1 = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-1", 
                "12345", "Procedure 1", 1, TenantId
            );
            approvedRequest1.Approve("AUTH-1", DateTime.UtcNow.AddDays(-1)); // Expired

            var approvedRequest2 = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-2", 
                "67890", "Procedure 2", 1, TenantId
            );
            approvedRequest2.Approve("AUTH-2", DateTime.UtcNow.AddDays(30)); // Still valid

            var requests = new List<AuthorizationRequest> { approvedRequest1, approvedRequest2 };

            _repositoryMock.Setup(r => r.GetByStatusAsync(AuthorizationStatus.Approved, TenantId))
                .ReturnsAsync(requests);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.MarkExpiredAuthorizationsAsync(TenantId);

            // Assert
            result.Should().Be(1);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AuthorizationRequest>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task MarkExpiredAuthorizationsAsync_WithNoExpiredRequests_DoesNotSaveChanges()
        {
            // Arrange
            var approvedRequest = new AuthorizationRequest(
                Guid.NewGuid(), Guid.NewGuid(), "REQ-1", 
                "12345", "Procedure 1", 1, TenantId
            );
            approvedRequest.Approve("AUTH-1", DateTime.UtcNow.AddDays(30));

            var requests = new List<AuthorizationRequest> { approvedRequest };

            _repositoryMock.Setup(r => r.GetByStatusAsync(AuthorizationStatus.Approved, TenantId))
                .ReturnsAsync(requests);

            // Act
            var result = await _service.MarkExpiredAuthorizationsAsync(TenantId);

            // Assert
            result.Should().Be(0);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        #endregion
    }
}
