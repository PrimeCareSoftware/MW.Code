using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
    /// Unit tests for TissGuidesController
    /// Tests REST API endpoints for TISS guide management
    /// </summary>
    public class TissGuidesControllerTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<ITissGuideService> _guideServiceMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly TissGuidesController _controller;

        public TissGuidesControllerTests()
        {
            _guideServiceMock = new Mock<ITissGuideService>();
            _tenantContextMock = new Mock<ITenantContext>();
            _tenantContextMock.Setup(t => t.TenantId).Returns(TenantId);
            
            _controller = new TissGuidesController(_guideServiceMock.Object, _tenantContextMock.Object);
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ReturnsOkWithGuides()
        {
            // Arrange
            var guides = new List<TissGuideDto>
            {
                new TissGuideDto { Id = Guid.NewGuid(), GuideNumber = "G001", TotalAmount = 100.00m },
                new TissGuideDto { Id = Guid.NewGuid(), GuideNumber = "G002", TotalAmount = 200.00m }
            };
            _guideServiceMock.Setup(s => s.GetAllAsync(TenantId)).ReturnsAsync(guides);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuides = okResult.Value.Should().BeAssignableTo<IEnumerable<TissGuideDto>>().Subject;
            returnedGuides.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_WithEmptyResult_ReturnsOkWithEmptyList()
        {
            // Arrange
            _guideServiceMock.Setup(s => s.GetAllAsync(TenantId)).ReturnsAsync(new List<TissGuideDto>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuides = okResult.Value.Should().BeAssignableTo<IEnumerable<TissGuideDto>>().Subject;
            returnedGuides.Should().BeEmpty();
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithGuide()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = new TissGuideDto { Id = guideId, GuideNumber = "G001", TotalAmount = 100.00m };
            _guideServiceMock.Setup(s => s.GetByIdAsync(guideId, TenantId)).ReturnsAsync(guide);

            // Act
            var result = await _controller.GetById(guideId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.GuideNumber.Should().Be("G001");
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            _guideServiceMock.Setup(s => s.GetByIdAsync(guideId, TenantId)).ReturnsAsync((TissGuideDto)null);

            // Act
            var result = await _controller.GetById(guideId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreateTissGuideDto 
            { 
                GuideType = "Consultation",
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid()
            };
            var createdGuide = new TissGuideDto 
            { 
                Id = Guid.NewGuid(), 
                GuideNumber = "G001",
                GuideType = "Consultation"
            };
            _guideServiceMock.Setup(s => s.CreateAsync(createDto, TenantId)).ReturnsAsync(createdGuide);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            var returnedGuide = createdResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.GuideNumber.Should().Be("G001");
        }

        [Fact]
        public async Task Create_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateTissGuideDto();
            _controller.ModelState.AddModelError("GuideType", "Required");

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region AddProcedure Tests

        [Fact]
        public async Task AddProcedure_WithValidData_ReturnsOk()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var procedureDto = new AddProcedureToGuideDto 
            { 
                ProcedureCode = "10101012", 
                Quantity = 1,
                UnitPrice = 100.00m
            };
            var updatedGuide = new TissGuideDto { Id = guideId, GuideNumber = "G001" };
            _guideServiceMock.Setup(s => s.AddProcedureAsync(guideId, procedureDto, TenantId))
                .ReturnsAsync(updatedGuide);

            // Act
            var result = await _controller.AddProcedure(guideId, procedureDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeAssignableTo<TissGuideDto>();
        }

        #endregion

        #region RemoveProcedure Tests

        [Fact]
        public async Task RemoveProcedure_WithValidData_ReturnsOk()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var procedureId = Guid.NewGuid();
            var updatedGuide = new TissGuideDto { Id = guideId, GuideNumber = "G001" };
            _guideServiceMock.Setup(s => s.RemoveProcedureAsync(guideId, procedureId, TenantId))
                .ReturnsAsync(updatedGuide);

            // Act
            var result = await _controller.RemoveProcedure(guideId, procedureId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeAssignableTo<TissGuideDto>();
        }

        #endregion

        #region Finalize Tests

        [Fact]
        public async Task Finalize_WithValidId_ReturnsOk()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var finalizedGuide = new TissGuideDto { Id = guideId, Status = "Ready" };
            _guideServiceMock.Setup(s => s.FinalizeAsync(guideId, TenantId))
                .ReturnsAsync(finalizedGuide);

            // Act
            var result = await _controller.Finalize(guideId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.Status.Should().Be("Ready");
        }

        #endregion

        #region ProcessResponse Tests

        [Fact]
        public async Task ProcessResponse_WithValidData_ReturnsOk()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var responseDto = new ProcessGuideResponseDto 
            { 
                ApprovedAmount = 80.00m,
                GlosedAmount = 20.00m,
                GlossReason = "Procedimento não autorizado"
            };
            var processedGuide = new TissGuideDto { Id = guideId };
            _guideServiceMock.Setup(s => s.ProcessResponseAsync(guideId, responseDto, TenantId))
                .ReturnsAsync(processedGuide);

            // Act
            var result = await _controller.ProcessResponse(guideId, responseDto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeAssignableTo<TissGuideDto>();
        }

        #endregion

        #region MarkAsPaid Tests

        [Fact]
        public async Task MarkAsPaid_WithValidId_ReturnsOk()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var paidGuide = new TissGuideDto { Id = guideId, Status = "Paid" };
            _guideServiceMock.Setup(s => s.MarkAsPaidAsync(guideId, TenantId))
                .ReturnsAsync(paidGuide);

            // Act
            var result = await _controller.MarkAsPaid(guideId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.Status.Should().Be("Paid");
        }

        #endregion
    }
}
