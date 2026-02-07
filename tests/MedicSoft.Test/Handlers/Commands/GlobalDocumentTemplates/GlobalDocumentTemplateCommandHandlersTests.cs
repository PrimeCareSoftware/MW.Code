using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using MedicSoft.Application.Commands.GlobalDocumentTemplates;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Application.Handlers.Commands.GlobalDocumentTemplates;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Handlers.Commands.GlobalDocumentTemplates
{
    public class CreateGlobalTemplateCommandHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly CreateGlobalTemplateCommandHandler _handler;
        private readonly string _tenantId = "test-tenant";
        private readonly string _createdBy = "admin-user-id";

        public CreateGlobalTemplateCommandHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new CreateGlobalTemplateCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_CreatesTemplate()
        {
            // Arrange
            var dto = new CreateGlobalTemplateDto
            {
                Name = "Test Template",
                Description = "Test Description",
                Type = DocumentTemplateType.MedicalCertificate,
                Specialty = ProfessionalSpecialty.Medico,
                Content = "<p>Content</p>",
                Variables = "{}"
            };

            var command = new CreateGlobalTemplateCommand(dto, _tenantId, _createdBy);

            _mockRepository
                .Setup(r => r.ExistsByNameAndTypeAsync(dto.Name, dto.Type, _tenantId))
                .ReturnsAsync(false);

            var createdTemplate = new GlobalDocumentTemplate(
                dto.Name, dto.Description, dto.Type, dto.Specialty,
                dto.Content, dto.Variables, _tenantId, _createdBy);

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<GlobalDocumentTemplate>()))
                .ReturnsAsync(createdTemplate);

            var resultDto = new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = dto.Name };
            _mockMapper
                .Setup(m => m.Map<GlobalDocumentTemplateDto>(It.IsAny<GlobalDocumentTemplate>()))
                .Returns(resultDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            _mockRepository.Verify(r => r.ExistsByNameAndTypeAsync(dto.Name, dto.Type, _tenantId), Times.Once);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<GlobalDocumentTemplate>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithDuplicateName_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateGlobalTemplateDto
            {
                Name = "Existing Template",
                Description = "Description",
                Type = DocumentTemplateType.MedicalCertificate,
                Specialty = ProfessionalSpecialty.Medico,
                Content = "<p>Content</p>",
                Variables = "{}"
            };

            var command = new CreateGlobalTemplateCommand(dto, _tenantId, _createdBy);

            _mockRepository
                .Setup(r => r.ExistsByNameAndTypeAsync(dto.Name, dto.Type, _tenantId))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains(dto.Name, exception.Message);
            _mockRepository.Verify(r => r.AddAsync(It.IsAny<GlobalDocumentTemplate>()), Times.Never);
        }
    }

    public class UpdateGlobalTemplateCommandHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UpdateGlobalTemplateCommandHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public UpdateGlobalTemplateCommandHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new UpdateGlobalTemplateCommandHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_UpdatesTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var existingTemplate = new GlobalDocumentTemplate(
                "Original Name", "Original Description",
                DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico,
                "Original Content", "{}", _tenantId, "admin");

            var dto = new UpdateGlobalTemplateDto
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Content = "<p>Updated Content</p>",
                Variables = "{\"updated\":true}",
                IsActive = true
            };

            var command = new UpdateGlobalTemplateCommand(templateId, dto, _tenantId);

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync(existingTemplate);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<GlobalDocumentTemplate>()))
                .Returns(Task.CompletedTask);

            var resultDto = new GlobalDocumentTemplateDto { Id = templateId, Name = dto.Name };
            _mockMapper
                .Setup(m => m.Map<GlobalDocumentTemplateDto>(It.IsAny<GlobalDocumentTemplate>()))
                .Returns(resultDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            _mockRepository.Verify(r => r.GetByIdAsync(templateId, _tenantId), Times.Once);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<GlobalDocumentTemplate>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentTemplate_ThrowsInvalidOperationException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var dto = new UpdateGlobalTemplateDto
            {
                Name = "Updated Name",
                Description = "Updated Description",
                Content = "<p>Content</p>",
                Variables = "{}",
                IsActive = true
            };

            var command = new UpdateGlobalTemplateCommand(templateId, dto, _tenantId);

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync((GlobalDocumentTemplate?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains(templateId.ToString(), exception.Message);
            _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<GlobalDocumentTemplate>()), Times.Never);
        }
    }

    public class DeleteGlobalTemplateCommandHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IDocumentTemplateRepository> _mockDocumentTemplateRepository;
        private readonly DeleteGlobalTemplateCommandHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public DeleteGlobalTemplateCommandHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockDocumentTemplateRepository = new Mock<IDocumentTemplateRepository>();
            _handler = new DeleteGlobalTemplateCommandHandler(
                _mockRepository.Object,
                _mockDocumentTemplateRepository.Object);
        }

        [Fact]
        public async Task Handle_WithUnusedTemplate_DeletesSuccessfully()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var existingTemplate = new GlobalDocumentTemplate(
                "Template", "Description",
                DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico,
                "Content", "{}", _tenantId, "admin");

            var command = new DeleteGlobalTemplateCommand(templateId, _tenantId);

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync(existingTemplate);

            _mockDocumentTemplateRepository
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DocumentTemplate, bool>>>(), _tenantId))
                .ReturnsAsync(new List<DocumentTemplate>());

            _mockRepository
                .Setup(r => r.DeleteAsync(templateId, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _mockRepository.Verify(r => r.GetByIdAsync(templateId, _tenantId), Times.Once);
            _mockRepository.Verify(r => r.DeleteAsync(templateId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_WithUsedTemplate_ThrowsInvalidOperationException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var existingTemplate = new GlobalDocumentTemplate(
                "Template", "Description",
                DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico,
                "Content", "{}", _tenantId, "admin");

            var command = new DeleteGlobalTemplateCommand(templateId, _tenantId);

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync(existingTemplate);

            var usedTemplates = new List<DocumentTemplate>
            {
                new DocumentTemplate("Template 1", "Desc", DocumentTemplateType.MedicalCertificate, _tenantId)
            };

            _mockDocumentTemplateRepository
                .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<DocumentTemplate, bool>>>(), _tenantId))
                .ReturnsAsync(usedTemplates);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains("1", exception.Message);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WithNonExistentTemplate_ThrowsInvalidOperationException()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var command = new DeleteGlobalTemplateCommand(templateId, _tenantId);

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync((GlobalDocumentTemplate?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _handler.Handle(command, CancellationToken.None));

            Assert.Contains(templateId.ToString(), exception.Message);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }
    }

    public class SetGlobalTemplateActiveStatusCommandHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly SetGlobalTemplateActiveStatusCommandHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public SetGlobalTemplateActiveStatusCommandHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _handler = new SetGlobalTemplateActiveStatusCommandHandler(_mockRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidData_SetsActiveStatus()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var command = new SetGlobalTemplateActiveStatusCommand(templateId, false, _tenantId);

            _mockRepository
                .Setup(r => r.SetActiveStatusAsync(templateId, false, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _mockRepository.Verify(r => r.SetActiveStatusAsync(templateId, false, _tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_WithTrueStatus_SetsActiveStatusToTrue()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var command = new SetGlobalTemplateActiveStatusCommand(templateId, true, _tenantId);

            _mockRepository
                .Setup(r => r.SetActiveStatusAsync(templateId, true, _tenantId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result);
            _mockRepository.Verify(r => r.SetActiveStatusAsync(templateId, true, _tenantId), Times.Once);
        }
    }
}
