using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
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
        private readonly TissBatchesController _controller;

        public TissBatchesControllerTests()
        {
            _batchServiceMock = new Mock<ITissBatchService>();
            _controller = new TissBatchesController(_batchServiceMock.Object);
            
            // Setup HttpContext with TenantId
            var httpContext = new DefaultHttpContext();
            httpContext.Items["TenantId"] = TenantId;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkWithBatches()
        {
            // Arrange
            var batches = new List<TissBatchDto>
            {
                new TissBatchDto { Id = 1, BatchNumber = "B001", TotalAmount = 1000.00m },
                new TissBatchDto { Id = 2, BatchNumber = "B002", TotalAmount = 2000.00m }
            };
            _batchServiceMock.Setup(s => s.GetAllAsync(TenantId)).ReturnsAsync(batches);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatches = okResult.Value.Should().BeAssignableTo<IEnumerable<TissBatchDto>>().Subject;
            returnedBatches.Should().HaveCount(2);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithBatch()
        {
            // Arrange
            var batch = new TissBatchDto { Id = 1, BatchNumber = "B001", TotalAmount = 1000.00m };
            _batchServiceMock.Setup(s => s.GetByIdAsync(1, TenantId)).ReturnsAsync(batch);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.BatchNumber.Should().Be("B001");
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.GetByIdAsync(999, TenantId)).ReturnsAsync((TissBatchDto)null);

            // Act
            var result = await _controller.GetById(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateTissBatchDto 
            { 
                OperatorId = 1,
                ClinicId = 1
            };
            var createdBatch = new TissBatchDto 
            { 
                Id = 1, 
                BatchNumber = "B001",
                Status = "Draft"
            };
            _batchServiceMock.Setup(s => s.CreateAsync(createDto, TenantId)).ReturnsAsync(createdBatch);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
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
            var updatedBatch = new TissBatchDto { Id = 1, BatchNumber = "B001", GuideCount = 1 };
            _batchServiceMock.Setup(s => s.AddGuideAsync(1, 1, TenantId)).ReturnsAsync(updatedBatch);

            // Act
            var result = await _controller.AddGuide(1, 1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.GuideCount.Should().Be(1);
        }

        [Fact]
        public async Task AddGuide_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.AddGuideAsync(999, 1, TenantId)).ReturnsAsync((TissBatchDto)null);

            // Act
            var result = await _controller.AddGuide(999, 1);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region GenerateXml Tests

        [Fact]
        public async Task GenerateXml_WithValidBatch_ReturnsOkWithXml()
        {
            // Arrange
            var xmlContent = "<?xml version=\"1.0\"?><tissLoteGuias>...</tissLoteGuias>";
            _batchServiceMock.Setup(s => s.GenerateXmlAsync(1, TenantId)).ReturnsAsync(xmlContent);

            // Act
            var result = await _controller.GenerateXml(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedXml = okResult.Value.Should().BeOfType<string>().Subject;
            returnedXml.Should().Contain("tissLoteGuias");
        }

        [Fact]
        public async Task GenerateXml_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.GenerateXmlAsync(999, TenantId)).ReturnsAsync((string)null);

            // Act
            var result = await _controller.GenerateXml(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region DownloadXml Tests

        [Fact]
        public async Task DownloadXml_WithValidBatch_ReturnsFileResult()
        {
            // Arrange
            var xmlBytes = System.Text.Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><tissLoteGuias>...</tissLoteGuias>");
            _batchServiceMock.Setup(s => s.DownloadXmlAsync(1, TenantId)).ReturnsAsync(xmlBytes);

            // Act
            var result = await _controller.DownloadXml(1);

            // Assert
            var fileResult = result.Should().BeOfType<FileContentResult>().Subject;
            fileResult.ContentType.Should().Be("application/xml");
            fileResult.FileDownloadName.Should().EndWith(".xml");
        }

        [Fact]
        public async Task DownloadXml_WithInvalidBatch_ReturnsNotFound()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.DownloadXmlAsync(999, TenantId)).ReturnsAsync((byte[])null);

            // Act
            var result = await _controller.DownloadXml(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region UpdateStatus Tests

        [Fact]
        public async Task UpdateStatus_WithValidData_ReturnsOk()
        {
            // Arrange
            var updatedBatch = new TissBatchDto { Id = 1, Status = "Sent" };
            _batchServiceMock.Setup(s => s.UpdateStatusAsync(1, "Sent", TenantId)).ReturnsAsync(updatedBatch);

            // Act
            var result = await _controller.UpdateStatus(1, "Sent");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedBatch = okResult.Value.Should().BeAssignableTo<TissBatchDto>().Subject;
            returnedBatch.Status.Should().Be("Sent");
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.DeleteAsync(1, TenantId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _batchServiceMock.Setup(s => s.DeleteAsync(999, TenantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion
    }
}
