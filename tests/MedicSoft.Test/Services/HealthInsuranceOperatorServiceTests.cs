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
    public class HealthInsuranceOperatorServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<IHealthInsuranceOperatorRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly HealthInsuranceOperatorService _service;

        public HealthInsuranceOperatorServiceTests()
        {
            _repositoryMock = new Mock<IHealthInsuranceOperatorRepository>();
            _mapperMock = new Mock<IMapper>();
            _service = new HealthInsuranceOperatorService(_repositoryMock.Object, _mapperMock.Object);
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesOperator()
        {
            // Arrange
            var dto = new CreateHealthInsuranceOperatorDto
            {
                TradeName = "Test Operator",
                CompanyName = "Test Operator LTDA",
                RegisterNumber = "123456",
                Document = "12345678000190",
                Phone = "1199999999",
                Email = "contact@operator.com",
                ContactPerson = "John Doe",
                WebsiteUrl = "https://operator.com",
                RequiresPriorAuthorization = true
            };

            _repositoryMock.Setup(r => r.GetByRegisterNumberAsync(dto.RegisterNumber, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HealthInsuranceOperator>()))
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithDuplicateRegisterNumber_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateHealthInsuranceOperatorDto
            {
                TradeName = "Test Operator",
                CompanyName = "Test Operator LTDA",
                RegisterNumber = "123456",
                Document = "12345678000190"
            };

            var existingOperator = new HealthInsuranceOperator(
                "Existing Operator",
                "Existing LTDA",
                "123456",
                "12345678000191",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByRegisterNumberAsync(dto.RegisterNumber, TenantId))
                .ReturnsAsync(existingOperator);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*already exists*");
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HealthInsuranceOperator>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WithoutWebsiteUrl_CreatesOperatorWithManualIntegration()
        {
            // Arrange
            var dto = new CreateHealthInsuranceOperatorDto
            {
                TradeName = "Test Operator",
                CompanyName = "Test Operator LTDA",
                RegisterNumber = "123456",
                Document = "12345678000190",
                RequiresPriorAuthorization = false
            };

            _repositoryMock.Setup(r => r.GetByRegisterNumberAsync(dto.RegisterNumber, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<HealthInsuranceOperator>()))
                .Returns(Task.CompletedTask);

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_WithValidData_UpdatesOperator()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new UpdateHealthInsuranceOperatorDto
            {
                TradeName = "Updated Name",
                CompanyName = "Updated Company",
                Phone = "1188888888",
                Email = "updated@operator.com",
                ContactPerson = "Jane Doe",
                WebsiteUrl = "https://updated.com",
                RequiresPriorAuthorization = true
            };

            var existingOperator = new HealthInsuranceOperator(
                "Old Name",
                "Old Company",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(existingOperator);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.UpdateAsync(operatorId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(operatorId);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new UpdateHealthInsuranceOperatorDto
            {
                TradeName = "Updated Name",
                CompanyName = "Updated Company"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.UpdateAsync(operatorId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Never);
        }

        #endregion

        #region ConfigureIntegrationAsync Tests

        [Fact]
        public async Task ConfigureIntegrationAsync_WithValidData_ConfiguresIntegration()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new ConfigureOperatorIntegrationDto
            {
                IntegrationType = "API",
                WebsiteUrl = "https://api.operator.com",
                ApiEndpoint = "https://api.operator.com/v1",
                ApiKey = "secret-key-123"
            };

            var existingOperator = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(existingOperator);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ConfigureIntegrationAsync(operatorId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfigureIntegrationAsync_WithInvalidIntegrationType_ThrowsArgumentException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new ConfigureOperatorIntegrationDto
            {
                IntegrationType = "InvalidType",
                WebsiteUrl = "https://operator.com"
            };

            var existingOperator = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(existingOperator);

            // Act
            var act = async () => await _service.ConfigureIntegrationAsync(operatorId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Invalid integration type*");
        }

        [Fact]
        public async Task ConfigureIntegrationAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new ConfigureOperatorIntegrationDto
            {
                IntegrationType = "API",
                WebsiteUrl = "https://operator.com"
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.ConfigureIntegrationAsync(operatorId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region ConfigureTissAsync Tests

        [Fact]
        public async Task ConfigureTissAsync_WithValidData_ConfiguresTiss()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new ConfigureOperatorTissDto
            {
                TissVersion = "4.02.00",
                SupportsTissXml = true,
                BatchSubmissionEmail = "tiss@operator.com"
            };

            var existingOperator = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(existingOperator);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ConfigureTissAsync(operatorId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfigureTissAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var dto = new ConfigureOperatorTissDto
            {
                TissVersion = "4.02.00",
                SupportsTissXml = true
            };

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.ConfigureTissAsync(operatorId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region GetAllAsync Tests

        [Fact]
        public async Task GetAllAsync_WithActiveOnly_ReturnsOnlyActiveOperators()
        {
            // Arrange
            var operators = new List<HealthInsuranceOperator>
            {
                new HealthInsuranceOperator("Active 1", "Active 1 LTDA", "111111", "11111111000111", TenantId),
                new HealthInsuranceOperator("Active 2", "Active 2 LTDA", "222222", "22222222000122", TenantId)
            };

            var inactiveOperator = new HealthInsuranceOperator("Inactive", "Inactive LTDA", "333333", "33333333000133", TenantId);
            inactiveOperator.Deactivate();
            operators.Add(inactiveOperator);

            _repositoryMock.Setup(r => r.GetAllAsync(TenantId))
                .ReturnsAsync(operators);

            var expectedDtos = new List<HealthInsuranceOperatorDto>
            {
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() },
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<HealthInsuranceOperatorDto>>(It.IsAny<IEnumerable<HealthInsuranceOperator>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetAllAsync(TenantId, includeInactive: false);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetAllAsync(TenantId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithIncludeInactive_ReturnsAllOperators()
        {
            // Arrange
            var operators = new List<HealthInsuranceOperator>
            {
                new HealthInsuranceOperator("Active", "Active LTDA", "111111", "11111111000111", TenantId),
                new HealthInsuranceOperator("Inactive", "Inactive LTDA", "222222", "22222222000122", TenantId)
            };
            operators[1].Deactivate();

            _repositoryMock.Setup(r => r.GetAllAsync(TenantId))
                .ReturnsAsync(operators);

            var expectedDtos = new List<HealthInsuranceOperatorDto>
            {
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() },
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<HealthInsuranceOperatorDto>>(It.IsAny<IEnumerable<HealthInsuranceOperator>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetAllAsync(TenantId, includeInactive: true);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.GetAllAsync(TenantId), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_FiltersCorrectlyByTenantId()
        {
            // Arrange
            var operators = new List<HealthInsuranceOperator>
            {
                new HealthInsuranceOperator("Operator 1", "Company 1", "111111", "11111111000111", TenantId)
            };

            _repositoryMock.Setup(r => r.GetAllAsync(TenantId))
                .ReturnsAsync(operators);

            _mapperMock.Setup(m => m.Map<IEnumerable<HealthInsuranceOperatorDto>>(It.IsAny<IEnumerable<HealthInsuranceOperator>>()))
                .Returns(new List<HealthInsuranceOperatorDto> { new HealthInsuranceOperatorDto() });

            // Act
            var result = await _service.GetAllAsync(TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.GetAllAsync(TenantId), Times.Once);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsOperator()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(operatorEntity);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(operatorEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(operatorId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(operatorId);
            _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var result = await _service.GetByIdAsync(operatorId, TenantId);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.GetByIdAsync(operatorId, TenantId), Times.Once);
        }

        #endregion

        #region GetByRegisterNumberAsync Tests

        [Fact]
        public async Task GetByRegisterNumberAsync_WithExistingRegister_ReturnsOperator()
        {
            // Arrange
            var registerNumber = "123456";
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                registerNumber,
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByRegisterNumberAsync(registerNumber, TenantId))
                .ReturnsAsync(operatorEntity);

            var expectedDto = new HealthInsuranceOperatorDto { RegisterNumber = registerNumber };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(operatorEntity))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByRegisterNumberAsync(registerNumber, TenantId);

            // Assert
            result.Should().NotBeNull();
            result!.RegisterNumber.Should().Be(registerNumber);
            _repositoryMock.Verify(r => r.GetByRegisterNumberAsync(registerNumber, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByRegisterNumberAsync_WithNonExistentRegister_ReturnsNull()
        {
            // Arrange
            var registerNumber = "999999";

            _repositoryMock.Setup(r => r.GetByRegisterNumberAsync(registerNumber, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var result = await _service.GetByRegisterNumberAsync(registerNumber, TenantId);

            // Assert
            result.Should().BeNull();
            _repositoryMock.Verify(r => r.GetByRegisterNumberAsync(registerNumber, TenantId), Times.Once);
        }

        #endregion

        #region SearchByNameAsync Tests

        [Fact]
        public async Task SearchByNameAsync_WithMatchingName_ReturnsOperators()
        {
            // Arrange
            var searchName = "Test";
            var operators = new List<HealthInsuranceOperator>
            {
                new HealthInsuranceOperator("Test Operator 1", "Test LTDA 1", "111111", "11111111000111", TenantId),
                new HealthInsuranceOperator("Test Operator 2", "Test LTDA 2", "222222", "22222222000122", TenantId)
            };

            _repositoryMock.Setup(r => r.SearchByNameAsync(searchName, TenantId))
                .ReturnsAsync(operators);

            var expectedDtos = new List<HealthInsuranceOperatorDto>
            {
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() },
                new HealthInsuranceOperatorDto { Id = Guid.NewGuid() }
            };

            _mapperMock.Setup(m => m.Map<IEnumerable<HealthInsuranceOperatorDto>>(operators))
                .Returns(expectedDtos);

            // Act
            var result = await _service.SearchByNameAsync(searchName, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _repositoryMock.Verify(r => r.SearchByNameAsync(searchName, TenantId), Times.Once);
        }

        [Fact]
        public async Task SearchByNameAsync_WithNoMatches_ReturnsEmptyList()
        {
            // Arrange
            var searchName = "NonExistent";

            _repositoryMock.Setup(r => r.SearchByNameAsync(searchName, TenantId))
                .ReturnsAsync(new List<HealthInsuranceOperator>());

            _mapperMock.Setup(m => m.Map<IEnumerable<HealthInsuranceOperatorDto>>(It.IsAny<IEnumerable<HealthInsuranceOperator>>()))
                .Returns(new List<HealthInsuranceOperatorDto>());

            // Act
            var result = await _service.SearchByNameAsync(searchName, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _repositoryMock.Verify(r => r.SearchByNameAsync(searchName, TenantId), Times.Once);
        }

        #endregion

        #region ActivateAsync Tests

        [Fact]
        public async Task ActivateAsync_WithValidOperator_ActivatesOperator()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );
            operatorEntity.Deactivate();

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(operatorEntity);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ActivateAsync(operatorId, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.ActivateAsync(operatorId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region DeactivateAsync Tests

        [Fact]
        public async Task DeactivateAsync_WithValidOperator_DeactivatesOperator()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(operatorEntity);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            var expectedDto = new HealthInsuranceOperatorDto { Id = operatorId };
            _mapperMock.Setup(m => m.Map<HealthInsuranceOperatorDto>(It.IsAny<HealthInsuranceOperator>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.DeactivateAsync(operatorId, TenantId);

            // Assert
            result.Should().NotBeNull();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.DeactivateAsync(operatorId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not found*");
        }

        #endregion

        #region DeleteAsync Tests

        [Fact]
        public async Task DeleteAsync_WithExistingOperator_DeactivatesAndReturnsTrue()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test LTDA",
                "123456",
                "12345678000190",
                TenantId
            );

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(operatorEntity);

            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()));

            _repositoryMock.Setup(r => r.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _service.DeleteAsync(operatorId, TenantId);

            // Assert
            result.Should().BeTrue();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentOperator_ReturnsFalse()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            _repositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var result = await _service.DeleteAsync(operatorId, TenantId);

            // Assert
            result.Should().BeFalse();
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<HealthInsuranceOperator>()), Times.Never);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        #endregion
    }
}
