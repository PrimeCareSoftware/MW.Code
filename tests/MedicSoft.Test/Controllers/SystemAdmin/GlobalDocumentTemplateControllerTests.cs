using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MedicSoft.Api.Controllers.SystemAdmin;
using MedicSoft.Application.Commands.GlobalDocumentTemplates;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Application.Queries.GlobalDocumentTemplates;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Controllers.SystemAdmin
{
    /// <summary>
    /// Integration tests for GlobalDocumentTemplateController
    /// Tests REST API endpoints for global document template management
    /// </summary>
    public class GlobalDocumentTemplateControllerTests
    {
        private const string TenantId = "test-tenant";
        private const string UserId = "test-user-id";
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ITenantContext> _tenantContextMock;
        private readonly GlobalDocumentTemplateController _controller;

        public GlobalDocumentTemplateControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _tenantContextMock = new Mock<ITenantContext>();
            _tenantContextMock.Setup(t => t.TenantId).Returns(TenantId);

            _controller = new GlobalDocumentTemplateController(_mediatorMock.Object, _tenantContextMock.Object);

            // Setup controller context with user claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim("sub", UserId)
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        #region GetAll Tests

        [Fact]
        public async Task GetAll_WithNoFilters_ReturnsOkWithAllTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Template 1" },
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Template 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetAllGlobalTemplatesQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_WithTypeFilter_ReturnsFilteredTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Certificate Template" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllGlobalTemplatesQuery>(q => 
                    q.Filter != null && q.Filter.Type == DocumentTemplateType.MedicalCertificate), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetAll(type: DocumentTemplateType.MedicalCertificate);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAll_WithSpecialtyFilter_ReturnsFilteredTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Medical Template" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllGlobalTemplatesQuery>(q => 
                    q.Filter != null && q.Filter.Specialty == ProfessionalSpecialty.Medico), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetAll(specialty: ProfessionalSpecialty.Medico);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAll_WithSearchTerm_ReturnsMatchingTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Medical Certificate" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetAllGlobalTemplatesQuery>(q => 
                    q.Filter != null && q.Filter.SearchTerm == "medical"), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetAll(searchTerm: "medical");

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(1);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkWithTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new GlobalDocumentTemplateDto 
            { 
                Id = templateId, 
                Name = "Test Template",
                Description = "Description"
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetGlobalTemplateByIdQuery>(q => q.Id == templateId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(template);

            // Act
            var result = await _controller.GetById(templateId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplate = okResult.Value.Should().BeAssignableTo<GlobalDocumentTemplateDto>().Subject;
            returnedTemplate.Id.Should().Be(templateId);
        }

        [Fact]
        public async Task GetById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetGlobalTemplateByIdQuery>(q => q.Id == templateId), It.IsAny<CancellationToken>()))
                .ReturnsAsync((GlobalDocumentTemplateDto?)null);

            // Act
            var result = await _controller.GetById(templateId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region GetByType Tests

        [Fact]
        public async Task GetByType_WithValidType_ReturnsOkWithTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Certificate 1" },
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Certificate 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetGlobalTemplatesByTypeQuery>(q => 
                    q.Type == DocumentTemplateType.MedicalCertificate), It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetByType(DocumentTemplateType.MedicalCertificate);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(2);
        }

        #endregion

        #region GetBySpecialty Tests

        [Fact]
        public async Task GetBySpecialty_WithValidSpecialty_ReturnsOkWithTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplateDto>
            {
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Medical Template 1" },
                new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = "Medical Template 2" }
            };

            _mediatorMock
                .Setup(m => m.Send(It.Is<GetGlobalTemplatesBySpecialtyQuery>(q => 
                    q.Specialty == ProfessionalSpecialty.Medico), It.IsAny<CancellationToken>()))
                .ReturnsAsync(templates);

            // Act
            var result = await _controller.GetBySpecialty(ProfessionalSpecialty.Medico);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplates = okResult.Value.Should().BeAssignableTo<List<GlobalDocumentTemplateDto>>().Subject;
            returnedTemplates.Should().HaveCount(2);
        }

        #endregion

        #region Create Tests

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new CreateGlobalTemplateDto
            {
                Name = "New Template",
                Description = "Description",
                Type = DocumentTemplateType.MedicalCertificate,
                Specialty = ProfessionalSpecialty.Medico,
                Content = "<p>Content</p>",
                Variables = "{}"
            };

            var createdTemplate = new GlobalDocumentTemplateDto
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateGlobalTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(createdTemplate);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.ActionName.Should().Be(nameof(_controller.GetById));
            var returnedTemplate = createdResult.Value.Should().BeAssignableTo<GlobalDocumentTemplateDto>().Subject;
            returnedTemplate.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task Create_WithDuplicateName_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGlobalTemplateDto
            {
                Name = "Duplicate Template",
                Description = "Description",
                Type = DocumentTemplateType.MedicalCertificate,
                Specialty = ProfessionalSpecialty.Medico,
                Content = "<p>Content</p>",
                Variables = "{}"
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<CreateGlobalTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Template already exists"));

            // Act
            var result = await _controller.Create(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task Update_WithValidData_ReturnsOkWithUpdatedTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var dto = new UpdateGlobalTemplateDto
            {
                Name = "Updated Template",
                Description = "Updated Description",
                Content = "<p>Updated Content</p>",
                Variables = "{}",
                IsActive = true
            };

            var updatedTemplate = new GlobalDocumentTemplateDto
            {
                Id = templateId,
                Name = dto.Name,
                Description = dto.Description
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateGlobalTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(updatedTemplate);

            // Act
            var result = await _controller.Update(templateId, dto);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedTemplate = okResult.Value.Should().BeAssignableTo<GlobalDocumentTemplateDto>().Subject;
            returnedTemplate.Name.Should().Be(dto.Name);
        }

        [Fact]
        public async Task Update_WithNonExistentTemplate_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var dto = new UpdateGlobalTemplateDto
            {
                Name = "Updated Template",
                Description = "Updated Description",
                Content = "<p>Content</p>",
                Variables = "{}",
                IsActive = true
            };

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<UpdateGlobalTemplateCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Template not found"));

            // Act
            var result = await _controller.Update(templateId, dto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithUnusedTemplate_ReturnsNoContent()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteGlobalTemplateCommand>(c => c.Id == templateId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.Delete(templateId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_WithUsedTemplate_ReturnsBadRequest()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteGlobalTemplateCommand>(c => c.Id == templateId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Template is in use"));

            // Act
            var result = await _controller.Delete(templateId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Delete_WithNonExistentTemplate_ReturnsBadRequest()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<DeleteGlobalTemplateCommand>(c => c.Id == templateId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Template not found"));

            // Act
            var result = await _controller.Delete(templateId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region SetActiveStatus Tests

        [Fact]
        public async Task SetActiveStatus_ToFalse_ReturnsNoContent()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<SetGlobalTemplateActiveStatusCommand>(c => 
                    c.Id == templateId && c.IsActive == false), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.SetActiveStatus(templateId, false);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task SetActiveStatus_ToTrue_ReturnsNoContent()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<SetGlobalTemplateActiveStatusCommand>(c => 
                    c.Id == templateId && c.IsActive == true), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Unit.Value);

            // Act
            var result = await _controller.SetActiveStatus(templateId, true);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task SetActiveStatus_WithNonExistentTemplate_ReturnsNotFound()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mediatorMock
                .Setup(m => m.Send(It.Is<SetGlobalTemplateActiveStatusCommand>(c => c.Id == templateId), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new InvalidOperationException("Template not found"));

            // Act
            var result = await _controller.SetActiveStatus(templateId, false);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion
    }
}
