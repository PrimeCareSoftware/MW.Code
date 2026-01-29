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
        private readonly TissGuidesController _controller;

        public TissGuidesControllerTests()
        {
            _guideServiceMock = new Mock<ITissGuideService>();
            _controller = new TissGuidesController(_guideServiceMock.Object);
            
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
        public async Task GetAll_ReturnsOkWithGuides()
        {
            // Arrange
            var guides = new List<TissGuideDto>
            {
                new TissGuideDto { Id = 1, GuideNumber = "G001", TotalAmount = 100.00m },
                new TissGuideDto { Id = 2, GuideNumber = "G002", TotalAmount = 200.00m }
            };
            _guideServiceMock.Setup(s => s.GetAllAsync(TenantId)).ReturnsAsync(guides);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
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
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuides = okResult.Value.Should().BeAssignableTo<IEnumerable<TissGuideDto>>().Subject;
            returnedGuides.Should().BeEmpty();
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithGuide()
        {
            // Arrange
            var guide = new TissGuideDto { Id = 1, GuideNumber = "G001", TotalAmount = 100.00m };
            _guideServiceMock.Setup(s => s.GetByIdAsync(1, TenantId)).ReturnsAsync(guide);

            // Act
            var result = await _controller.GetById(1);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.GuideNumber.Should().Be("G001");
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            _guideServiceMock.Setup(s => s.GetByIdAsync(999, TenantId)).ReturnsAsync((TissGuideDto)null);

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
            var createDto = new CreateTissGuideDto 
            { 
                GuideType = "Consultation",
                OperatorId = 1,
                PatientId = 1
            };
            var createdGuide = new TissGuideDto 
            { 
                Id = 1, 
                GuideNumber = "G001",
                GuideType = "Consultation"
            };
            _guideServiceMock.Setup(s => s.CreateAsync(createDto, TenantId)).ReturnsAsync(createdGuide);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
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
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidData_ReturnsOk()
        {
            // Arrange
            var updateDto = new UpdateTissGuideDto { Id = 1, GuideNumber = "G001-UPD" };
            var updatedGuide = new TissGuideDto { Id = 1, GuideNumber = "G001-UPD" };
            _guideServiceMock.Setup(s => s.UpdateAsync(1, updateDto, TenantId)).ReturnsAsync(updatedGuide);

            // Act
            var result = await _controller.Update(1, updateDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.GuideNumber.Should().Be("G001-UPD");
        }

        [Fact]
        public async Task Update_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateTissGuideDto { Id = 999 };
            _guideServiceMock.Setup(s => s.UpdateAsync(999, updateDto, TenantId)).ReturnsAsync((TissGuideDto)null);

            // Act
            var result = await _controller.Update(999, updateDto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            _guideServiceMock.Setup(s => s.DeleteAsync(1, TenantId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistentId_ReturnsNotFound()
        {
            // Arrange
            _guideServiceMock.Setup(s => s.DeleteAsync(999, TenantId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        #endregion

        #region AddProcedure Tests

        [Fact]
        public async Task AddProcedure_WithValidData_ReturnsOk()
        {
            // Arrange
            var procedureDto = new TissGuideProcedureDto { ProcedureCode = "10101012", Quantity = 1 };
            var updatedGuide = new TissGuideDto { Id = 1, GuideNumber = "G001" };
            _guideServiceMock.Setup(s => s.AddProcedureAsync(1, procedureDto, TenantId)).ReturnsAsync(updatedGuide);

            // Act
            var result = await _controller.AddProcedure(1, procedureDto);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeAssignableTo<TissGuideDto>();
        }

        #endregion

        #region Authorization Tests

        [Fact]
        public async Task GetByAuthorizationNumber_WithValidNumber_ReturnsOk()
        {
            // Arrange
            var guide = new TissGuideDto { Id = 1, AuthorizationNumber = "AUTH-12345" };
            _guideServiceMock.Setup(s => s.GetByAuthorizationNumberAsync("AUTH-12345", TenantId))
                .ReturnsAsync(guide);

            // Act
            var result = await _controller.GetByAuthorizationNumber("AUTH-12345");

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedGuide = okResult.Value.Should().BeAssignableTo<TissGuideDto>().Subject;
            returnedGuide.AuthorizationNumber.Should().Be("AUTH-12345");
        }

        #endregion
    }
}
