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
    public class TissGuideServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<ITissGuideRepository> _guideRepositoryMock;
        private readonly Mock<ITissGuideProcedureRepository> _procedureRepositoryMock;
        private readonly Mock<ITissBatchRepository> _batchRepositoryMock;
        private readonly Mock<IAppointmentRepository> _appointmentRepositoryMock;
        private readonly Mock<IPatientHealthInsuranceRepository> _insuranceRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly TissGuideService _service;

        public TissGuideServiceTests()
        {
            _guideRepositoryMock = new Mock<ITissGuideRepository>();
            _procedureRepositoryMock = new Mock<ITissGuideProcedureRepository>();
            _batchRepositoryMock = new Mock<ITissBatchRepository>();
            _appointmentRepositoryMock = new Mock<IAppointmentRepository>();
            _insuranceRepositoryMock = new Mock<IPatientHealthInsuranceRepository>();
            _mapperMock = new Mock<IMapper>();
            
            _service = new TissGuideService(
                _guideRepositoryMock.Object,
                _procedureRepositoryMock.Object,
                _batchRepositoryMock.Object,
                _appointmentRepositoryMock.Object,
                _insuranceRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        #region CreateAsync Tests

        [Fact]
        public async Task CreateAsync_WithValidData_CreatesGuide()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "Consultation",
                ServiceDate = DateTime.UtcNow,
                AuthorizationNumber = "AUTH123"
            };

            var batch = CreateValidBatch(dto.TissBatchId);
            var appointment = CreateValidAppointment(dto.AppointmentId);
            var insurance = CreateValidInsurance(dto.PatientHealthInsuranceId);

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync(batch);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(dto.AppointmentId, TenantId))
                .ReturnsAsync(appointment);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientHealthInsuranceId, TenantId))
                .ReturnsAsync(insurance);

            _guideRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TissGuide>()))
                .ReturnsAsync((TissGuide g) => g);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var createdGuide = CreateValidGuide(dto.TissBatchId, dto.AppointmentId, dto.PatientHealthInsuranceId);
            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(createdGuide);

            var expectedDto = new TissGuideDto { Id = createdGuide.Id };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.CreateAsync(dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _guideRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentBatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "Consultation",
                ServiceDate = DateTime.UtcNow
            };

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync((TissBatch?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{dto.TissBatchId}*not found*");
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentAppointment_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "Consultation",
                ServiceDate = DateTime.UtcNow
            };

            var batch = CreateValidBatch(dto.TissBatchId);

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync(batch);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(dto.AppointmentId, TenantId))
                .ReturnsAsync((Appointment?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{dto.AppointmentId}*not found*");
        }

        [Fact]
        public async Task CreateAsync_WithNonExistentInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "Consultation",
                ServiceDate = DateTime.UtcNow
            };

            var batch = CreateValidBatch(dto.TissBatchId);
            var appointment = CreateValidAppointment(dto.AppointmentId);

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync(batch);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(dto.AppointmentId, TenantId))
                .ReturnsAsync(appointment);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientHealthInsuranceId, TenantId))
                .ReturnsAsync((PatientHealthInsurance?)null);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{dto.PatientHealthInsuranceId}*not found*");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidInsurance_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "Consultation",
                ServiceDate = DateTime.UtcNow
            };

            var batch = CreateValidBatch(dto.TissBatchId);
            var appointment = CreateValidAppointment(dto.AppointmentId);
            var insurance = CreateInvalidInsurance(dto.PatientHealthInsuranceId);

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync(batch);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(dto.AppointmentId, TenantId))
                .ReturnsAsync(appointment);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientHealthInsuranceId, TenantId))
                .ReturnsAsync(insurance);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*not valid*");
        }

        [Fact]
        public async Task CreateAsync_WithInvalidGuideType_ThrowsArgumentException()
        {
            // Arrange
            var dto = new CreateTissGuideDto
            {
                TissBatchId = Guid.NewGuid(),
                AppointmentId = Guid.NewGuid(),
                PatientHealthInsuranceId = Guid.NewGuid(),
                GuideType = "InvalidType",
                ServiceDate = DateTime.UtcNow
            };

            var batch = CreateValidBatch(dto.TissBatchId);
            var appointment = CreateValidAppointment(dto.AppointmentId);
            var insurance = CreateValidInsurance(dto.PatientHealthInsuranceId);

            _batchRepositoryMock.Setup(r => r.GetByIdAsync(dto.TissBatchId, TenantId))
                .ReturnsAsync(batch);

            _appointmentRepositoryMock.Setup(r => r.GetByIdAsync(dto.AppointmentId, TenantId))
                .ReturnsAsync(appointment);

            _insuranceRepositoryMock.Setup(r => r.GetByIdAsync(dto.PatientHealthInsuranceId, TenantId))
                .ReturnsAsync(insurance);

            // Act
            var act = async () => await _service.CreateAsync(dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*guide type*");
        }

        #endregion

        #region AddProcedureAsync Tests

        [Fact]
        public async Task AddProcedureAsync_WithValidData_AddsProcedure()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var dto = new AddProcedureToGuideDto
            {
                ProcedureCode = "40101010",
                ProcedureDescription = "Consulta médica",
                Quantity = 1,
                UnitPrice = 150.00m
            };

            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _procedureRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TissGuideProcedure>()))
                .ReturnsAsync((TissGuideProcedure p) => p);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.AddProcedureAsync(guideId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _procedureRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TissGuideProcedure>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddProcedureAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var dto = new AddProcedureToGuideDto
            {
                ProcedureCode = "40101010",
                ProcedureDescription = "Consulta médica",
                Quantity = 1,
                UnitPrice = 150.00m
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.AddProcedureAsync(guideId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region RemoveProcedureAsync Tests

        [Fact]
        public async Task RemoveProcedureAsync_WithValidData_RemovesProcedure()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var procedureId = Guid.NewGuid();

            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            typeof(TissGuideProcedure).GetProperty("Id")!.SetValue(procedure, procedureId);
            guide.AddProcedure(procedure);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _procedureRepositoryMock.Setup(r => r.DeleteAsync(procedureId, TenantId))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.RemoveProcedureAsync(guideId, procedureId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _procedureRepositoryMock.Verify(r => r.DeleteAsync(procedureId, TenantId), Times.Once);
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveProcedureAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var procedureId = Guid.NewGuid();

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.RemoveProcedureAsync(guideId, procedureId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region FinalizeAsync Tests

        [Fact]
        public async Task FinalizeAsync_WithValidGuide_MarksAsSent()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "Sent" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.FinalizeAsync(guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task FinalizeAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var guideId = Guid.NewGuid();

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.FinalizeAsync(guideId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsGuide()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            var expectedDto = new TissGuideDto { Id = guideId };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.GetByIdAsync(guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(expectedDto);
            _guideRepositoryMock.Verify(r => r.GetWithProceduresAsync(guideId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ReturnsNull()
        {
            // Arrange
            var guideId = Guid.NewGuid();

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var result = await _service.GetByIdAsync(guideId, TenantId);

            // Assert
            result.Should().BeNull();
            _guideRepositoryMock.Verify(r => r.GetWithProceduresAsync(guideId, TenantId), Times.Once);
        }

        #endregion

        #region GetByBatchIdAsync Tests

        [Fact]
        public async Task GetByBatchIdAsync_ReturnsGuides()
        {
            // Arrange
            var batchId = Guid.NewGuid();
            var guides = new List<TissGuide>
            {
                CreateValidGuide(batchId, Guid.NewGuid(), Guid.NewGuid()),
                CreateValidGuide(batchId, Guid.NewGuid(), Guid.NewGuid())
            };

            _guideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batchId, TenantId))
                .ReturnsAsync(guides);

            var expectedDtos = new List<TissGuideDto>
            {
                new TissGuideDto { Id = guides[0].Id },
                new TissGuideDto { Id = guides[1].Id }
            };
            _mapperMock.Setup(m => m.Map<IEnumerable<TissGuideDto>>(It.IsAny<IEnumerable<TissGuide>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _service.GetByBatchIdAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            _guideRepositoryMock.Verify(r => r.GetByBatchIdAsync(batchId, TenantId), Times.Once);
        }

        [Fact]
        public async Task GetByBatchIdAsync_WithNoBatches_ReturnsEmptyList()
        {
            // Arrange
            var batchId = Guid.NewGuid();

            _guideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batchId, TenantId))
                .ReturnsAsync(new List<TissGuide>());

            _mapperMock.Setup(m => m.Map<IEnumerable<TissGuideDto>>(It.IsAny<IEnumerable<TissGuide>>()))
                .Returns(new List<TissGuideDto>());

            // Act
            var result = await _service.GetByBatchIdAsync(batchId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region ProcessResponseAsync Tests

        [Fact]
        public async Task ProcessResponseAsync_WithFullApproval_ApprovesGuide()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            var dto = new ProcessGuideResponseDto
            {
                ApprovedAmount = 150.00m,
                GlosedAmount = 0m,
                ProcedureResponses = new List<ProcedureResponseDto>()
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "Approved" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(guideId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Approved");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithPartialApproval_PartiallyApprovesGuide()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            var dto = new ProcessGuideResponseDto
            {
                ApprovedAmount = 100.00m,
                GlosedAmount = 50.00m,
                GlossReason = "Partial gloss reason",
                ProcedureResponses = new List<ProcedureResponseDto>()
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "PartiallyApproved" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(guideId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("PartiallyApproved");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithRejection_RejectsGuide()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            var dto = new ProcessGuideResponseDto
            {
                ApprovedAmount = null,
                GlosedAmount = null,
                GlossReason = "Rejected due to missing documentation",
                ProcedureResponses = new List<ProcedureResponseDto>()
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "Rejected" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(guideId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Rejected");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithProcedureResponses_UpdatesProcedures()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var procedureId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            typeof(TissGuideProcedure).GetProperty("Id")!.SetValue(procedure, procedureId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();

            var dto = new ProcessGuideResponseDto
            {
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
                        GlossReason = "Procedure gloss reason"
                    }
                }
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _procedureRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuideProcedure>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.ProcessResponseAsync(guideId, dto, TenantId);

            // Assert
            result.Should().NotBeNull();
            _procedureRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuideProcedure>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task ProcessResponseAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var dto = new ProcessGuideResponseDto
            {
                ApprovedAmount = 150.00m,
                ProcedureResponses = new List<ProcedureResponseDto>()
            };

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.ProcessResponseAsync(guideId, dto, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region MarkAsPaidAsync Tests

        [Fact]
        public async Task MarkAsPaidAsync_WithApprovedGuide_MarksAsPaid()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();
            guide.Approve(150.00m);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "Paid" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.MarkAsPaidAsync(guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Paid");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkAsPaidAsync_WithPartiallyApprovedGuide_MarksAsPaid()
        {
            // Arrange
            var guideId = Guid.NewGuid();
            var guide = CreateValidGuide(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
            var procedure = new TissGuideProcedure(guide.Id, "40101010", "Consulta", 1, 150.00m, TenantId);
            guide.AddProcedure(procedure);
            guide.MarkAsSent();
            guide.Approve(100.00m);

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync(guide);

            _guideRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TissGuide>()))
                .Returns(Task.CompletedTask);

            _guideRepositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var expectedDto = new TissGuideDto { Id = guideId, Status = "Paid" };
            _mapperMock.Setup(m => m.Map<TissGuideDto>(It.IsAny<TissGuide>()))
                .Returns(expectedDto);

            // Act
            var result = await _service.MarkAsPaidAsync(guideId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be("Paid");
            _guideRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<TissGuide>()), Times.Once);
            _guideRepositoryMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MarkAsPaidAsync_WithNonExistentGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var guideId = Guid.NewGuid();

            _guideRepositoryMock.Setup(r => r.GetWithProceduresAsync(guideId, TenantId))
                .ReturnsAsync((TissGuide?)null);

            // Act
            var act = async () => await _service.MarkAsPaidAsync(guideId, TenantId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage($"*{guideId}*not found*");
        }

        #endregion

        #region Helper Methods

        private TissBatch CreateValidBatch(Guid batchId)
        {
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var batch = new TissBatch(clinicId, operatorId, "001", TenantId);
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, batchId);
            return batch;
        }

        private Appointment CreateValidAppointment(Guid appointmentId)
        {
            var patientId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var appointment = new Appointment(
                patientId,
                clinicId,
                DateTime.UtcNow.AddDays(1),
                TimeSpan.FromHours(10),
                30,
                AppointmentType.Consultation,
                TenantId,
                "Regular checkup"
            );
            typeof(Appointment).GetProperty("Id")!.SetValue(appointment, appointmentId);
            return appointment;
        }

        private PatientHealthInsurance CreateValidInsurance(Guid insuranceId)
        {
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(
                patientId,
                planId,
                "123456789",
                DateTime.UtcNow.AddYears(-1),
                TenantId,
                true,
                null,
                DateTime.UtcNow.AddYears(1)
            );
            typeof(PatientHealthInsurance).GetProperty("Id")!.SetValue(insurance, insuranceId);
            return insurance;
        }

        private PatientHealthInsurance CreateInvalidInsurance(Guid insuranceId)
        {
            var patientId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var insurance = new PatientHealthInsurance(
                patientId,
                planId,
                "123456789",
                DateTime.UtcNow.AddYears(-2),
                TenantId,
                true,
                null,
                DateTime.UtcNow.AddYears(-1)
            );
            insurance.Deactivate();
            typeof(PatientHealthInsurance).GetProperty("Id")!.SetValue(insurance, insuranceId);
            return insurance;
        }

        private TissGuide CreateValidGuide(Guid batchId, Guid appointmentId, Guid insuranceId)
        {
            var guideNumber = $"GUIDE-{DateTime.UtcNow:yyyyMMddHHmmss}";
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
