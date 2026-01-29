using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using Xunit;

namespace MedicSoft.Test.Controllers
{
    /// <summary>
    /// Unit tests for TissBatchesController
    /// Tests REST API endpoints for TISS batch management and XML generation
    /// </summary>
    public class TissBatchesControllerTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<ITissBatchService> _batchServiceMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly TissBatchesController _controller;

        public TissBatchesControllerTests()
        {
            _batchServiceMock = new Mock<ITissBatchService>();
            _tenantContextMock = new Mock<ITenantContext>();
            _tenantContextMock.Setup(t => t.TenantId).Returns(TenantId);
            
            _controller = new TissBatchesController(_batchServiceMock.Object, _tenantContextMock.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkWithBatches()
        {
            // Arrange
            var batches = new List<TissBatchDto>
            {
                new TissBatchDto { Id = Guid.NewGuid(), BatchNumber = "B001", TotalAmount = 1000.00m },
                new TissBatchDto { Id = Guid.NewGuid(), BatchNumber = "B002", TotalAmount = 2000.00m }
            };
            _batchServiceMock.Setup(s => s.GetAllAsync(TenantId)).ReturnsAsync(batches);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatches = okResult.Value.Should().BeAssignableTo<IEnumerable<TissBatchDto>>().Subject;
            returnedBatches.Should().HaveCount(2);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithBatch()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var batch = new TissBatchDto { Id = batchId, BatchNumber = "B001", TotalAmount = 1000.00m };
            _batchServiceMock.Setup(s => s.GetByIdAsync(batchId, TenantId)).ReturnsAsync(batch);

            // Act
            var result = await _controller.GetById(batchId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.BatchNumber.Should().Be("B001");
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            _batchServiceMock.Setup(s => s.GetByIdAsync(batchId, TenantId)).ReturnsAsync((TissBatchDto)null);

            // Act
            var result = await _controller.GetById(batchId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateTissBatchDto 
            { 
                OperatorId = Guid.NewGuid(),
                ClinicId = Guid.NewGuid()
            };
            var createdBatch = new TissBatchDto 
            { 
                Id = Guid.NewGuid(), 
                BatchNumber = "B001",
                Status = "Draft"
            };
            _batchServiceMock.Setup(s => s.CreateAsync(createDto, TenantId)).ReturnsAsync(createdBatch);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            var returnedBatch = createdResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.BatchNumber.Should().Be("B001");
        }

        #endregion

        #region AddGuide Tests

        [Fact]
        public async Task AddGuide_WithValidData_ReturnsOk()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();
            var updatedBatch = new TissBatchDto { Id = batchId, BatchNumber = "B001", GuideCount = 1 };
            _batchServiceMock.Setup(s => s.AddGuideAsync(batchId, guideId, TenantId)).ReturnsAsync(updatedBatch);

            // Act
            var result = await _controller.AddGuide(batchId, guideId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.GuideCount.Should().Be(1);
        }

        [Fact]
        public async Task AddGuide_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();
            _batchServiceMock.Setup(s => s.AddGuideAsync(batchId, guideId, TenantId))
                .ThrowsAsync(new InvalidOperationException("Batch not found"));

            // Act
            var result = await _controller.AddGuide(batchId, guideId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region RemoveGuide Tests

        [Fact]
        public async Task RemoveGuide_WithValidData_ReturnsOk()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guideId = Guid.NewGuid();
            var updatedBatch = new TissBatchDto { Id = batchId, BatchNumber = "B001", GuideCount = 0 };
            _batchServiceMock.Setup(s => s.RemoveGuideAsync(batchId, guideId, TenantId)).ReturnsAsync(updatedBatch);

            // Act
            var result = await _controller.RemoveGuide(batchId, guideId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.GuideCount.Should().Be(0);
        }

        #endregion

        #region GenerateXml Tests

        [Fact]
        public async Task GenerateXml_WithValidBatch_ReturnsOkWithXml()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var xmlContent = "<?xml version=\"1.0\"?><tissLoteGuias>...</tissLoteGuias>";
            _batchServiceMock.Setup(s => s.GenerateXmlAsync(batchId, TenantId)).ReturnsAsync(xmlContent);

            // Act
            var result = await _controller.GenerateXml(batchId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedXml = okResult.Value.Should().BeOfType<string>().Subject;
            returnedXml.Should().Contain("tissLoteGuias");
        }

        [Fact]
        public async Task GenerateXml_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            _batchServiceMock.Setup(s => s.GenerateXmlAsync(batchId, TenantId))
                .ThrowsAsync(new InvalidOperationException("Batch not found"));

            // Act
            var result = await _controller.GenerateXml(batchId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region DownloadXml Tests

        [Fact]
        public async Task DownloadXml_WithValidBatch_ReturnsFileResult()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var xmlBytes = System.Text.Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><tissLoteGuias>...</tissLoteGuias>");
            var batch = new TissBatchDto { Id = batchId, XmlFileName = "batch_001.xml" };
            
            _batchServiceMock.Setup(s => s.GetByIdAsync(batchId, TenantId)).ReturnsAsync(batch);
            _batchServiceMock.Setup(s => s.DownloadXmlAsync(batchId, TenantId)).ReturnsAsync(xmlBytes);

            // Act
            var result = await _controller.DownloadXml(batchId);

            // Assert
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.ContentType.Should().Be("application/xml");
        }

        [Fact]
        public async Task DownloadXml_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            _batchServiceMock.Setup(s => s.GetByIdAsync(batchId, TenantId)).ReturnsAsync((TissBatchDto)null);

            // Act
            var result = await _controller.DownloadXml(batchId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region MarkReadyToSend Tests

        [Fact]
        public async Task MarkReadyToSend_WithValidData_ReturnsOk()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var updatedBatch = new TissBatchDto { Id = batchId, Status = "Ready" };
            _batchServiceMock.Setup(s => s.MarkReadyToSendAsync(batchId, TenantId)).ReturnsAsync(updatedBatch);

            // Act
            var result = await _controller.MarkReadyToSend(batchId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.Status.Should().Be("Ready");
        }

        #endregion

        #region ProcessResponse Tests

        [Fact]
        public async Task ProcessResponse_WithValidData_ReturnsOk()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var responseDto = new ProcessBatchResponseDto 
            { 
                ProtocolNumber = "PROT-12345",
                ApprovedAmount = 8000.00m,
                GlosedAmount = 2000.00m
            };
            var processedBatch = new TissBatchDto { Id = batchId, Status = "Processed" };
            _batchServiceMock.Setup(s => s.ProcessResponseAsync(batchId, responseDto, TenantId))
                .ReturnsAsync(processedBatch);

            // Act
            var result = await _controller.ProcessResponse(batchId, responseDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.Status.Should().Be("Processed");
        }

        #endregion
    }
}
