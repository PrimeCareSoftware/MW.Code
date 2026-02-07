using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Application.Handlers.Queries.GlobalDocumentTemplates;
using MedicSoft.Application.Queries.GlobalDocumentTemplates;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Handlers.Queries.GlobalDocumentTemplates
{
    public class GetAllGlobalTemplatesQueryHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetAllGlobalTemplatesQueryHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public GetAllGlobalTemplatesQueryHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetAllGlobalTemplatesQueryHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithNoFilters_ReturnsAllTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Template 1", "Desc 1", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 2", "Desc 2", DocumentTemplateType.Prescription, ProfessionalSpecialty.Psicologo, "Content2", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetAllAsync(_tenantId))
                .ReturnsAsync(templates);

            var dtos = templates.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name }).ToList();
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns(dtos);

            var query = new GetAllGlobalTemplatesQuery(_tenantId, null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(r => r.GetAllAsync(_tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_WithTypeFilter_ReturnsFilteredTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Template 1", "Desc 1", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 2", "Desc 2", DocumentTemplateType.Prescription, ProfessionalSpecialty.Medico, "Content2", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 3", "Desc 3", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content3", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetAllAsync(_tenantId))
                .ReturnsAsync(templates);

            var filter = new GlobalDocumentTemplateFilterDto { Type = DocumentTemplateType.MedicalCertificate };
            
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns((List<GlobalDocumentTemplate> source) => 
                    source.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name }).ToList());

            var query = new GetAllGlobalTemplatesQuery(_tenantId, filter);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task Handle_WithSpecialtyFilter_ReturnsFilteredTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Template 1", "Desc 1", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 2", "Desc 2", DocumentTemplateType.Prescription, ProfessionalSpecialty.Psicologo, "Content2", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetAllAsync(_tenantId))
                .ReturnsAsync(templates);

            var filter = new GlobalDocumentTemplateFilterDto { Specialty = ProfessionalSpecialty.Medico };
            
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns((List<GlobalDocumentTemplate> source) => 
                    source.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name }).ToList());

            var query = new GetAllGlobalTemplatesQuery(_tenantId, filter);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task Handle_WithActiveFilter_ReturnsOnlyActiveTemplates()
        {
            // Arrange
            var template1 = new GlobalDocumentTemplate("Template 1", "Desc 1", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin");
            var template2 = new GlobalDocumentTemplate("Template 2", "Desc 2", DocumentTemplateType.Prescription, ProfessionalSpecialty.Medico, "Content2", "{}", _tenantId, "admin");
            template2.SetActiveStatus(false);

            var templates = new List<GlobalDocumentTemplate> { template1, template2 };

            _mockRepository
                .Setup(r => r.GetAllAsync(_tenantId))
                .ReturnsAsync(templates);

            var filter = new GlobalDocumentTemplateFilterDto { IsActive = true };
            
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns((List<GlobalDocumentTemplate> source) => 
                    source.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name, IsActive = t.IsActive }).ToList());

            var query = new GetAllGlobalTemplatesQuery(_tenantId, filter);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task Handle_WithSearchTerm_ReturnsMatchingTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Medical Template", "Description for medical", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Prescription Template", "Description", DocumentTemplateType.Prescription, ProfessionalSpecialty.Medico, "Content2", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetAllAsync(_tenantId))
                .ReturnsAsync(templates);

            var filter = new GlobalDocumentTemplateFilterDto { SearchTerm = "medical" };
            
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns((List<GlobalDocumentTemplate> source) => 
                    source.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name, Description = t.Description }).ToList());

            var query = new GetAllGlobalTemplatesQuery(_tenantId, filter);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Medical Template", result[0].Name);
        }
    }

    public class GetGlobalTemplateByIdQueryHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetGlobalTemplateByIdQueryHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public GetGlobalTemplateByIdQueryHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetGlobalTemplateByIdQueryHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithExistingId_ReturnsTemplate()
        {
            // Arrange
            var templateId = Guid.NewGuid();
            var template = new GlobalDocumentTemplate(
                "Template", "Description", DocumentTemplateType.MedicalCertificate,
                ProfessionalSpecialty.Medico, "Content", "{}", _tenantId, "admin");

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync(template);

            var dto = new GlobalDocumentTemplateDto { Id = templateId, Name = "Template" };
            _mockMapper
                .Setup(m => m.Map<GlobalDocumentTemplateDto>(template))
                .Returns(dto);

            var query = new GetGlobalTemplateByIdQuery(templateId, _tenantId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(templateId, result.Id);
            _mockRepository.Verify(r => r.GetByIdAsync(templateId, _tenantId), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var templateId = Guid.NewGuid();

            _mockRepository
                .Setup(r => r.GetByIdAsync(templateId, _tenantId))
                .ReturnsAsync((GlobalDocumentTemplate?)null);

            var query = new GetGlobalTemplateByIdQuery(templateId, _tenantId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(r => r.GetByIdAsync(templateId, _tenantId), Times.Once);
        }
    }

    public class GetGlobalTemplatesByTypeQueryHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetGlobalTemplatesByTypeQueryHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public GetGlobalTemplatesByTypeQueryHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetGlobalTemplatesByTypeQueryHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidType_ReturnsTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Template 1", "Desc", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 2", "Desc", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Psicologo, "Content2", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetByTypeAsync(DocumentTemplateType.MedicalCertificate, _tenantId))
                .ReturnsAsync(templates);

            var dtos = templates.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name }).ToList();
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns(dtos);

            var query = new GetGlobalTemplatesByTypeQuery(DocumentTemplateType.MedicalCertificate, _tenantId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(r => r.GetByTypeAsync(DocumentTemplateType.MedicalCertificate, _tenantId), Times.Once);
        }
    }

    public class GetGlobalTemplatesBySpecialtyQueryHandlerTests
    {
        private readonly Mock<IGlobalDocumentTemplateRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetGlobalTemplatesBySpecialtyQueryHandler _handler;
        private readonly string _tenantId = "test-tenant";

        public GetGlobalTemplatesBySpecialtyQueryHandlerTests()
        {
            _mockRepository = new Mock<IGlobalDocumentTemplateRepository>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetGlobalTemplatesBySpecialtyQueryHandler(_mockRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidSpecialty_ReturnsTemplates()
        {
            // Arrange
            var templates = new List<GlobalDocumentTemplate>
            {
                new GlobalDocumentTemplate("Template 1", "Desc", DocumentTemplateType.MedicalCertificate, ProfessionalSpecialty.Medico, "Content1", "{}", _tenantId, "admin"),
                new GlobalDocumentTemplate("Template 2", "Desc", DocumentTemplateType.Prescription, ProfessionalSpecialty.Medico, "Content2", "{}", _tenantId, "admin")
            };

            _mockRepository
                .Setup(r => r.GetBySpecialtyAsync(ProfessionalSpecialty.Medico, _tenantId))
                .ReturnsAsync(templates);

            var dtos = templates.Select(t => new GlobalDocumentTemplateDto { Id = Guid.NewGuid(), Name = t.Name }).ToList();
            _mockMapper
                .Setup(m => m.Map<List<GlobalDocumentTemplateDto>>(It.IsAny<List<GlobalDocumentTemplate>>()))
                .Returns(dtos);

            var query = new GetGlobalTemplatesBySpecialtyQuery(ProfessionalSpecialty.Medico, _tenantId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockRepository.Verify(r => r.GetBySpecialtyAsync(ProfessionalSpecialty.Medico, _tenantId), Times.Once);
        }
    }
}
