using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    public class TissBatchServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<ITissBatchRepository> _batchRepositoryMock;
        private readonly Mock<ITissGuideRepository> _guideRepositoryMock;
        private readonly Mock<IClinicRepository> _clinicRepositoryMock;
        private readonly Mock<IHealthInsuranceOperatorRepository> _operatorRepositoryMock;
        private readonly Mock<ITissXmlGeneratorService> _xmlGeneratorMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TissBatchService _service;

        public TissBatchServiceTests()
        {
            _batchRepositoryMock = new Mock<ITissBatchRepository>();
            _guideRepositoryMock = new Mock<ITissGuideRepository>();
            _clinicRepositoryMock = new Mock<IClinicRepository>();
            _operatorRepositoryMock = new Mock<IHealthInsuranceOperatorRepository>();
            _xmlGeneratorMock = new Mock<ITissXmlGeneratorService>();
            _mapperMock = new Mock<IMapper>();

            _service = new TissBatchService(
                _batchRepositoryMock.Object,
                _guideRepositoryMock.Object,
                _clinicRepositoryMock.Object,
                _operatorRepositoryMock.Object,
                _xmlGeneratorMock.Object,
                _mapperMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesBatch()
        {
            // Arrange
            var dto = new CreateTissBatchDto
            {
                ClinicId = Guid.NewGuid(),
                OperatorId = Guid.NewGuid()
            };

            var clinic = CreateValidClinic(dto.ClinicId);
            var operatorEntity = CreateValidOperator(dto.OperatorId);

            _clinicRepositoryMock.Setup(r => r.GetByIdAsync(dto.ClinicId, TenantId))
                .ReturnsAsync(clinic);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(dto.OperatorId, TenantId))
                .ReturnsAsync(operatorEntity);

            _batchRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TissBatch>()))
                .ReturnsAsync((TissBatch b) => b);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var createdBatch = CreateValidBatch(dto.ClinicId, dto.OperatorId);
            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(createdBatch);

            var expectedDto = new TissBatchDto { Id = createdBatch.Id };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _batchRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.GetWithGuidesAsync(It.IsAny<Guid>(), TenantId), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentClinic_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissBatchDto
            {
                ClinicId = Guid.NewGuid(),
                OperatorId = Guid.NewGuid()
            };

            _clinicRepositoryMock.Setup(r => r.GetByIdAsync(dto.ClinicId, TenantId))
                .ReturnsAsync((Clinic?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{dto.ClinicId}*not found*");
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentOperator_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissBatchDto
            {
                ClinicId = Guid.NewGuid(),
                OperatorId = Guid.NewGuid()
            };

            var clinic = CreateValidClinic(dto.ClinicId);

            _clinicRepositoryMock.Setup(r => r.GetByIdAsync(dto.ClinicId, TenantId))
                .ReturnsAsync(clinic);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(dto.OperatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{dto.OperatorId}*not found*");
        }

        #endregion

        #region AddGuideAsync Tests

        [Fact]
        public async Task AddGuideAsync_WithValidData_AddsGuideToBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();

            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            typeof(TissGuide).GetProperty("Id")!.SetValue(guide, guideId);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.AddGuideAsync(batchId, guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddGuideAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.AddGuideAsync(batchId, guideId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        [Fact]
        public async Task AddGuideAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();

            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.AddGuideAsync(batchId, guideId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region RemoveGuideAsync Tests

        [Fact]
        public async Task RemoveGuideAsync_WithValidData_RemovesGuideFromBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();

            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            typeof(TissGuide).GetProperty("Id")!.SetValue(guide, guideId);
            batch.AddGuide(guide);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.RemoveGuideAsync(batchId, guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveGuideAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.RemoveGuideAsync(batchId, guideId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region GenerateXmlAsync Tests

        [Fact]
        public async Task GenerateXmlAsync_WithValidBatch_GeneratesXmlSuccessfully()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var xmlFilePath = "/path/to/xml/batch.xml";
            var xmlFileName = "batch.xml";

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _xmlGeneratorMock.Setup(x => x.GenerateBatchXmlAsync(It.IsAny<TissBatch>(), It.IsAny<string>()))
                .ReturnsAsync(xmlFilePath);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _service.GenerateXmlAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.XmlFilePath.Should().Be(xmlFilePath);
            result.XmlFileName.Should().Be(xmlFileName);
            result.ErrorMessage.Should().BeNull();
            _xmlGeneratorMock.Verify(x => x.GenerateBatchXmlAsync(It.IsAny<TissBatch>(), It.IsAny<string>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GenerateXmlAsync_WithNonExistentBatch_ReturnsFailureResult()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var result = await _service.GenerateXmlAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not found");
            result.XmlFilePath.Should().BeNull();
            result.XmlFileName.Should().BeNull();
        }

        [Fact]
        public async Task GenerateXmlAsync_WhenXmlGenerationFails_ReturnsFailureResult()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _xmlGeneratorMock.Setup(x => x.GenerateBatchXmlAsync(It.IsAny<TissBatch>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("XML generation error"));

            // Act
            var result = await _service.GenerateXmlAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Failed to generate XML");
            result.ErrorMessage.Should().Contain("XML generation error");
        }

        #endregion

        #region MarkAsReadyToSendAsync Tests

        [Fact]
        public async Task MarkAsReadyToSendAsync_WithValidBatch_MarksAsReadyToSend()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            batch.AddGuide(guide);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "ReadyToSend" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.MarkAsReadyToSendAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("ReadyToSend");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkAsReadyToSendAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.MarkAsReadyToSendAsync(batchId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region SubmitAsync Tests

        [Fact]
        public async Task SubmitAsync_WithValidBatch_SubmitsBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Sent" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.SubmitAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Sent");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SubmitAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.SubmitAsync(batchId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region ProcessResponseAsync Tests

        [Fact]
        public async Task ProcessResponseAsync_WithFullApproval_ProcessesBatchSuccessfully()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");

            var dto = new ProcessBatchResponseDto
            {
                ProtocolNumber = "PROT-123",
                ResponseXmlFileName = "response.xml",
                ApprovedAmount = 150.00m,
                GlosedAmount = 0m,
                GuideResponses = new List<GuideResponseDto>
                {
                    new GuideResponseDto
                    {
                        GuideNumber = guide.GuideNumber,
                        ApprovedAmount = 150.00m,
                        GlosedAmount = 0m,
                        ProcedureResponses = new List<ProcedureResponseDto>()
                    }
                }
            };

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Processed" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(batchId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Processed");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithPartialApproval_ProcessesBatchWithGloss()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");

            var dto = new ProcessBatchResponseDto
            {
                ProtocolNumber = "PROT-123",
                ResponseXmlFileName = "response.xml",
                ApprovedAmount = 100.00m,
                GlosedAmount = 50.00m,
                GuideResponses = new List<GuideResponseDto>
                {
                    new GuideResponseDto
                    {
                        GuideNumber = guide.GuideNumber,
                        ApprovedAmount = 100.00m,
                        GlosedAmount = 50.00m,
                        GlossReason = "Partial gloss",
                        ProcedureResponses = new List<ProcedureResponseDto>()
                    }
                }
            };

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "PartiallyPaid" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(batchId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("PartiallyPaid");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithRejection_RejectsBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");

            var dto = new ProcessBatchResponseDto
            {
                ProtocolNumber = "PROT-123",
                ResponseXmlFileName = "response.xml",
                ApprovedAmount = null,
                GlosedAmount = null,
                GuideResponses = new List<GuideResponseDto>
                {
                    new GuideResponseDto
                    {
                        GuideNumber = guide.GuideNumber,
                        ApprovedAmount = null,
                        GlosedAmount = null,
                        GlossReason = "Complete rejection",
                        ProcedureResponses = new List<ProcedureResponseDto>()
                    }
                }
            };

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Rejected" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(batchId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Rejected");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithProcedureResponses_UpdatesProcedures()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var procedureId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            typeof(TissGuideProcedure).GetProperty("Id")!.SetValue(procedure, procedureId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");

            var dto = new ProcessBatchResponseDto
            {
                ProtocolNumber = "PROT-123",
                ResponseXmlFileName = "response.xml",
                ApprovedAmount = 100.00m,
                GlosedAmount = 50.00m,
                GuideResponses = new List<GuideResponseDto>
                {
                    new GuideResponseDto
                    {
                        GuideNumber = guide.GuideNumber,
                        ApprovedAmount = 100.00m,
                        GlosedAmount = 50.00m,
                        GlossReason = "Partial gloss",
                        ProcedureResponses = new List<ProcedureResponseDto>
                        {
                            new ProcedureResponseDto
                            {
                                ProcedureId = procedureId,
                                ApprovedAmount = 100.00m,
                                GlosedAmount = 50.00m,
                                GlossReason = "Procedure gloss"
                            }
                        }
                    }
                }
            };

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(batchId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var dto = new ProcessBatchResponseDto
            {
                ProtocolNumber = "PROT-123",
                ApprovedAmount = 150.00m,
                GuideResponses = new List<GuideResponseDto>()
            };

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.ProcessResponseAsync(batchId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region GetByOperatorIdAsync Tests

        [Fact]
        public async Task GetByOperatorIdAsync_WithExistingOperator_ReturnsBatches()
        {
            // Arrange
            var operatorId = Guid.NewGuid();
            var batches = new List<TissBatch>
            {
                CreateValidBatch(Guid.NewGuid(), operatorId),
                CreateValidBatch(Guid.NewGuid(), operatorId)
            };

            _batchRepositoryMock.Setup(r => r.GetByOperatorIdAsync(operatorId, TenantId))
                .ReturnsAsync(batches);

            var expectedDtos = new List<TissBatchDto>
            {
                new TissBatchDto { Id = batches[0].Id },
                new TissBatchDto { Id = batches[1].Id }
            };
            _mapperMock.Setup(m => m.Map<IEnumerable<TissBatchDto>>(It.IsAny<IEnumerable<TissBatch>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetByOperatorIdAsync(operatorId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _batchRepositoryMock.Verify(r => r.GetByOperatorIdAsync(operatorId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByOperatorIdAsync_WithNoOperatorBatches_ReturnsEmptyList()
        {
            // Arrange
            var operatorId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetByOperatorIdAsync(operatorId, TenantId))
                .ReturnsAsync(new List<TissBatch>());

            _mapperMock.Setup(m => m.Map<IEnumerable<TissBatchDto>>(It.IsAny<IEnumerable<TissBatch>>()))
                .Returns(new List<TissBatchDto>());

            // Act
            var result = await _service.GetByOperatorIdAsync(operatorId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region MarkAsPaidAsync Tests

        [Fact]
        public async Task MarkAsPaidAsync_WithProcessedBatch_MarksAsPaid()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide1 = CreateValidGuide(batchId);
            var guide2 = CreateValidGuide(batchId);

            var procedure1 = new TissGuideProcedure(guide1.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide1.AddProcedure(procedure1);
            guide1.MarkAsSent();
            guide1.Approve(150.00m);

            var procedure2 = new TissGuideProcedure(guide2.Id, "40101012", "Retorno", 1, 100.00m, TenantId);
            guide2.AddProcedure(procedure2);
            guide2.MarkAsSent();
            guide2.Approve(80.00m);

            batch.AddGuide(guide1);
            batch.AddGuide(guide2);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");
            batch.ProcessResponse("response.xml", 230.00m, 20.00m);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Paid" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.MarkAsPaidAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Paid");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Exactly(2));
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkAsPaidAsync_WithPartiallyPaidBatch_MarksAsPaid()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();
            guide.Approve(100.00m);

            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");
            batch.ProcessResponse("response.xml", 100.00m, 50.00m);

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Paid" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.MarkAsPaidAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Paid");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkAsPaidAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.MarkAsPaidAsync(batchId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region RejectAsync Tests

        [Fact]
        public async Task RejectAsync_WithSentBatch_RejectsBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Rejected" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.RejectAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Rejected");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_WithProcessingBatch_RejectsBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = CreateValidBatch(Guid.NewGuid(), Guid.NewGuid());
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);

            var guide = CreateValidGuide(batchId);
            batch.AddGuide(guide);
            batch.MarkAsReadyToSend();
            batch.GenerateXml("batch.xml", "/path/to/batch.xml");
            batch.Submit("PROT-123");
            batch.MarkAsProcessing();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync(batch);

            _batchRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissBatch>()))
                .Returns(Task.CompletedTask);

            _batchRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            var expectedDto = new TissBatchDto { Id = batchId, Status = "Rejected" };
            _mapperMock.Setup(m => m.Map<TissBatchDto>(It.IsAny<TissBatch>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.RejectAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Rejected");
            _batchRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissBatch>()), Times.Once);
            _batchRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _batchRepositoryMock.Setup(r => r.GetWithGuidesAsync(batchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.RejectAsync(batchId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{batchId}*not found*");
        }

        #endregion

        #region Helper Methods

        private Clinic CreateValidClinic(Guid clinicId)
        {
            var clinic = new Clinic(
                "Test Clinic",
                "Test Trade Name",
                "12345678901234",
                "11999999999",
                "clinic@test.com",
                "Test Address",
                TimeSpan.FromHours(8),
                TimeSpan.FromHours(18),
                TenantId
            );
            typeof(Clinic).GetProperty("Id")!.SetValue(clinic, clinicId);
            return clinic;
        }

        private HealthInsuranceOperator CreateValidOperator(Guid operatorId)
        {
            var operatorEntity = new HealthInsuranceOperator(
                "Test Operator",
                "Test Operator Company",
                "123456",
                "12345678901234",
                TenantId
            );
            typeof(HealthInsuranceOperator).GetProperty("Id")!.SetValue(operatorEntity, operatorId);
            return operatorEntity;
        }

        private TissBatch CreateValidBatch(Guid clinicId, Guid operatorId)
        {
            var batchNumber = $"BATCH-{DateTime.UtcNow:yyyyMMddHHmmss}";
            return new TissBatch(clinicId, operatorId, batchNumber, TenantId);
        }

        private TissGuide CreateValidGuide(Guid batchId)
        {
            var appointmentId = Guid.NewGuid();
            var insuranceId = Guid.NewGuid();
            var guideNumber = $"GUIDE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8)}";

            return new TissGuide(
                batchId,
                appointmentId,
                insuranceId,
                guideNumber,
                TissGuideType.Consultation,
                DateTime.UtcNow,
                TenantId,
                "AUTH123"
            );
        }

        #endregion
    }
}
