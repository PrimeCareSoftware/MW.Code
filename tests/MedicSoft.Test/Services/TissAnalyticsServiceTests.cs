using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class TissAnalyticsServiceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Mock<ITissBatchRepository> _tissBatchRepositoryMock;
        private readonly Mock<ITissGuideRepository> _tissGuideRepositoryMock;
        private readonly Mock<IAuthorizationRequestRepository> _authorizationRequestRepositoryMock;
        private readonly Mock<IHealthInsuranceOperatorRepository> _operatorRepositoryMock;
        private readonly TissAnalyticsService _service;

        public TissAnalyticsServiceTests()
        {
            _tissBatchRepositoryMock = new Mock<ITissBatchRepository>();
            _tissGuideRepositoryMock = new Mock<ITissGuideRepository>();
            _authorizationRequestRepositoryMock = new Mock<IAuthorizationRequestRepository>();
            _operatorRepositoryMock = new Mock<IHealthInsuranceOperatorRepository>();

            _service = new TissAnalyticsService(
                _tissBatchRepositoryMock.Object,
                _tissGuideRepositoryMock.Object,
                _authorizationRequestRepositoryMock.Object,
                _operatorRepositoryMock.Object
            );
        }

        #region GetGlosasSummaryAsync Tests

        [Fact]
        public async Task GetGlosasSummaryAsync_WithValidData_ReturnsSummary()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetGlosasSummaryAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.TotalBilled.Should().Be(3000m);
            result.TotalApproved.Should().Be(2400m);
            result.TotalGlosed.Should().Be(600m);
            result.GlosaPercentage.Should().Be(20m);
            result.TotalBatches.Should().Be(2);
            result.TotalGuides.Should().Be(3);
            result.GlosedGuides.Should().Be(2);
        }

        [Fact]
        public async Task GetGlosasSummaryAsync_WithNoBatches_ReturnsZeroSummary()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(new List<TissBatch>());

            // Act
            var result = await _service.GetGlosasSummaryAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.TotalBilled.Should().Be(0m);
            result.TotalApproved.Should().Be(0m);
            result.TotalGlosed.Should().Be(0m);
            result.GlosaPercentage.Should().Be(0m);
            result.TotalBatches.Should().Be(0);
            result.TotalGuides.Should().Be(0);
            result.GlosedGuides.Should().Be(0);
        }

        [Fact]
        public async Task GetGlosasSummaryAsync_WithBatchesOutsideDateRange_ReturnsZeroSummary()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, Guid.NewGuid(), new DateTime(2023, 12, 15))
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            // Act
            var result = await _service.GetGlosasSummaryAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.TotalBatches.Should().Be(0);
            result.TotalGuides.Should().Be(0);
        }

        [Fact]
        public async Task GetGlosasSummaryAsync_WithZeroTotalBilled_ReturnsZeroGlosaPercentage()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = new List<TissGuide>(); // Empty guides

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetGlosasSummaryAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.GlosaPercentage.Should().Be(0m);
        }

        #endregion

        #region GetGlosasByOperatorAsync Tests

        [Fact]
        public async Task GetGlosasByOperatorAsync_WithValidData_ReturnsGlosasByOperator()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId1 = Guid.NewGuid();
            var operatorId2 = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, operatorId1, new DateTime(2024, 1, 15)),
                CreateBatch(clinicId, operatorId2, new DateTime(2024, 1, 20))
            };

            var guidesOp1 = CreateGuidesWithGlosedAmounts(batches[0].Id);
            var guidesOp2 = CreateGuidesWithGlosedAmounts(batches[1].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[0].Id, TenantId))
                .ReturnsAsync(guidesOp1);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[1].Id, TenantId))
                .ReturnsAsync(guidesOp2);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId1, TenantId))
                .ReturnsAsync(CreateOperator(operatorId1, "Operator 1"));

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId2, TenantId))
                .ReturnsAsync(CreateOperator(operatorId2, "Operator 2"));

            // Act
            var result = await _service.GetGlosasByOperatorAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(2);
            resultList[0].OperatorName.Should().Be("Operator 1");
            resultList[0].TotalBilled.Should().Be(3000m);
            resultList[0].TotalGlosed.Should().Be(600m);
            resultList[0].GlosaPercentage.Should().Be(20m);
        }

        [Fact]
        public async Task GetGlosasByOperatorAsync_WithNonExistentOperator_UsesDefaultName()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, operatorId, new DateTime(2024, 1, 15))
            };

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[0].Id, TenantId))
                .ReturnsAsync(guides);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync((HealthInsuranceOperator?)null);

            // Act
            var result = await _service.GetGlosasByOperatorAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].OperatorName.Should().Be("Desconhecida");
        }

        [Fact]
        public async Task GetGlosasByOperatorAsync_OrdersByGlosaPercentageDescending()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId1 = Guid.NewGuid();
            var operatorId2 = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, operatorId1, new DateTime(2024, 1, 15)),
                CreateBatch(clinicId, operatorId2, new DateTime(2024, 1, 20))
            };

            // Operator 1: 20% glosa rate
            var guidesOp1 = CreateGuidesWithGlosedAmounts(batches[0].Id);
            
            // Operator 2: 30% glosa rate (higher)
            var guidesOp2 = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[1].Id, 1000m, 700m, 300m)
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[0].Id, TenantId))
                .ReturnsAsync(guidesOp1);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[1].Id, TenantId))
                .ReturnsAsync(guidesOp2);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId1, TenantId))
                .ReturnsAsync(CreateOperator(operatorId1, "Low Glosa Operator"));

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId2, TenantId))
                .ReturnsAsync(CreateOperator(operatorId2, "High Glosa Operator"));

            // Act
            var result = await _service.GetGlosasByOperatorAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList[0].OperatorName.Should().Be("High Glosa Operator");
            resultList[0].GlosaPercentage.Should().Be(30m);
            resultList[1].OperatorName.Should().Be("Low Glosa Operator");
            resultList[1].GlosaPercentage.Should().Be(20m);
        }

        #endregion

        #region GetGlosasTrendAsync Tests

        [Fact]
        public async Task GetGlosasTrendAsync_WithValidData_ReturnsTrendData()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 3;

            var now = DateTime.UtcNow;
            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, Guid.NewGuid(), now.AddMonths(-1)),
                CreateBatch(clinicId, Guid.NewGuid(), now.AddMonths(-2))
            };

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetGlosasTrendAsync(clinicId, months, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(2);
            resultList[0].Year.Should().BeGreaterThan(0);
            resultList[0].Month.Should().BeInRange(1, 12);
            resultList[0].MonthName.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetGlosasTrendAsync_OrdersByYearAndMonth()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 12;

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, Guid.NewGuid(), new DateTime(2024, 3, 15)),
                CreateBatch(clinicId, Guid.NewGuid(), new DateTime(2024, 1, 15)),
                CreateBatch(clinicId, Guid.NewGuid(), new DateTime(2024, 2, 15))
            };

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetGlosasTrendAsync(clinicId, months, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().BeInAscendingOrder(x => x.Year);
            
            var sameYearItems = resultList.Where(x => x.Year == 2024).ToList();
            if (sameYearItems.Count > 1)
            {
                sameYearItems.Should().BeInAscendingOrder(x => x.Month);
            }
        }

        [Fact]
        public async Task GetGlosasTrendAsync_WithNoBatches_ReturnsEmptyList()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 3;

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(new List<TissBatch>());

            // Act
            var result = await _service.GetGlosasTrendAsync(clinicId, months, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetProcedureGlosasAsync Tests

        [Fact]
        public async Task GetProcedureGlosasAsync_WithValidData_ReturnsProcedureGlosas()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = CreateGuidesWithProcedures(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetProcedureGlosasAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().NotBeEmpty();
            resultList[0].ProcedureCode.Should().NotBeNullOrEmpty();
            resultList[0].ProcedureName.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetProcedureGlosasAsync_GroupsProceduresByCodeAndName()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = CreateGuidesWithProcedures(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetProcedureGlosasAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().NotBeEmpty();
            resultList.Select(r => r.ProcedureCode).Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public async Task GetProcedureGlosasAsync_ReturnsTop10Only()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = CreateGuidesWithManyProcedures(batches[0].Id, 15);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetProcedureGlosasAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().HaveCountLessOrEqualTo(10);
        }

        [Fact]
        public async Task GetProcedureGlosasAsync_OrdersByTotalGlosedDescending()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            var guides = CreateGuidesWithProcedures(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetProcedureGlosasAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            if (resultList.Count > 1)
            {
                resultList.Should().BeInDescendingOrder(x => x.TotalGlosed);
            }
        }

        #endregion

        #region GetAuthorizationRateAsync Tests

        [Fact]
        public async Task GetAuthorizationRateAsync_WithValidData_ReturnsAuthorizationRates()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var authorizations = CreateAuthorizationRequests(operatorId, startDate, endDate);

            _authorizationRequestRepositoryMock.Setup(r => r.GetAllWithDetailsAsync(TenantId))
                .ReturnsAsync(authorizations);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(CreateOperator(operatorId, "Test Operator"));

            // Act
            var result = await _service.GetAuthorizationRateAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].OperatorName.Should().Be("Test Operator");
            resultList[0].TotalRequests.Should().Be(10);
            resultList[0].ApprovedRequests.Should().Be(7);
            resultList[0].RejectedRequests.Should().Be(2);
            resultList[0].PendingRequests.Should().Be(1);
            resultList[0].ApprovalRate.Should().Be(70m);
        }

        [Fact]
        public async Task GetAuthorizationRateAsync_WithZeroRequests_ReturnsZeroApprovalRate()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            _authorizationRequestRepositoryMock.Setup(r => r.GetAllWithDetailsAsync(TenantId))
                .ReturnsAsync(new List<AuthorizationRequest>());

            // Act
            var result = await _service.GetAuthorizationRateAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAuthorizationRateAsync_OrdersByApprovalRateDescending()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId1 = Guid.NewGuid();
            var operatorId2 = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var authsOp1 = CreateAuthorizationRequests(operatorId1, startDate, endDate); // 70% approval
            var authsOp2 = CreateAuthorizationRequestsHighApproval(operatorId2, startDate, endDate); // 90% approval

            var allAuths = authsOp1.Concat(authsOp2).ToList();

            _authorizationRequestRepositoryMock.Setup(r => r.GetAllWithDetailsAsync(TenantId))
                .ReturnsAsync(allAuths);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId1, TenantId))
                .ReturnsAsync(CreateOperator(operatorId1, "Low Approval Operator"));

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId2, TenantId))
                .ReturnsAsync(CreateOperator(operatorId2, "High Approval Operator"));

            // Act
            var result = await _service.GetAuthorizationRateAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList[0].OperatorName.Should().Be("High Approval Operator");
            resultList[0].ApprovalRate.Should().BeGreaterThan(resultList[1].ApprovalRate);
        }

        [Fact]
        public async Task GetAuthorizationRateAsync_FiltersAuthorizationsWithoutHealthInsurance()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var authsWithInsurance = CreateAuthorizationRequests(operatorId, startDate, endDate);
            var authWithoutInsurance = CreateAuthorizationRequestWithoutInsurance(startDate);

            var allAuths = authsWithInsurance.Concat(new[] { authWithoutInsurance }).ToList();

            _authorizationRequestRepositoryMock.Setup(r => r.GetAllWithDetailsAsync(TenantId))
                .ReturnsAsync(allAuths);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(CreateOperator(operatorId, "Test Operator"));

            // Act
            var result = await _service.GetAuthorizationRateAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].TotalRequests.Should().Be(10); // Should not include the one without insurance
        }

        #endregion

        #region GetApprovalTimeAsync Tests

        [Fact]
        public async Task GetApprovalTimeAsync_WithValidData_ReturnsApprovalTimes()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithSubmittedAndProcessedDates(clinicId, operatorId, startDate, endDate);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId, TenantId))
                .ReturnsAsync(CreateOperator(operatorId, "Test Operator"));

            // Act
            var result = await _service.GetApprovalTimeAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().HaveCount(1);
            resultList[0].OperatorName.Should().Be("Test Operator");
            resultList[0].AverageApprovalDays.Should().BeGreaterThan(0);
            resultList[0].TotalProcessed.Should().Be(2);
            resultList[0].MinApprovalDays.Should().BeGreaterThan(0);
            resultList[0].MaxApprovalDays.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetApprovalTimeAsync_OrdersByAverageApprovalDaysAscending()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId1 = Guid.NewGuid();
            var operatorId2 = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batchesOp1 = CreateBatchesWithSubmittedAndProcessedDates(clinicId, operatorId1, startDate, endDate, 5); // 5 days avg
            var batchesOp2 = CreateBatchesWithSubmittedAndProcessedDates(clinicId, operatorId2, startDate, endDate, 10); // 10 days avg

            var allBatches = batchesOp1.Concat(batchesOp2).ToList();

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(allBatches);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId1, TenantId))
                .ReturnsAsync(CreateOperator(operatorId1, "Fast Operator"));

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(operatorId2, TenantId))
                .ReturnsAsync(CreateOperator(operatorId2, "Slow Operator"));

            // Act
            var result = await _service.GetApprovalTimeAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList[0].OperatorName.Should().Be("Fast Operator");
            resultList[0].AverageApprovalDays.Should().BeLessThan(resultList[1].AverageApprovalDays);
        }

        [Fact]
        public async Task GetApprovalTimeAsync_WithNoBatches_ReturnsEmptyList()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(new List<TissBatch>());

            // Act
            var result = await _service.GetApprovalTimeAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetMonthlyPerformanceAsync Tests

        [Fact]
        public async Task GetMonthlyPerformanceAsync_WithValidData_ReturnsMonthlyPerformance()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 3;

            var now = DateTime.UtcNow;
            var batches = CreateBatchesWithSubmittedAndProcessedDates(
                clinicId, 
                Guid.NewGuid(), 
                now.AddMonths(-2), 
                now, 
                5
            );

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetMonthlyPerformanceAsync(clinicId, months, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().NotBeEmpty();
            resultList[0].Year.Should().BeGreaterThan(0);
            resultList[0].Month.Should().BeInRange(1, 12);
            resultList[0].MonthName.Should().NotBeNullOrEmpty();
            resultList[0].TotalBilled.Should().BeGreaterThan(0);
            resultList[0].AverageApprovalDays.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetMonthlyPerformanceAsync_OrdersByYearAndMonth()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 12;

            var batches = new List<TissBatch>
            {
                CreateBatchWithSubmittedAndProcessedDates(clinicId, Guid.NewGuid(), 
                    new DateTime(2024, 3, 1), new DateTime(2024, 3, 15)),
                CreateBatchWithSubmittedAndProcessedDates(clinicId, Guid.NewGuid(), 
                    new DateTime(2024, 1, 1), new DateTime(2024, 1, 15)),
                CreateBatchWithSubmittedAndProcessedDates(clinicId, Guid.NewGuid(), 
                    new DateTime(2024, 2, 1), new DateTime(2024, 2, 15))
            };

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetMonthlyPerformanceAsync(clinicId, months, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().BeInAscendingOrder(x => x.Year);
        }

        [Fact]
        public async Task GetMonthlyPerformanceAsync_WithNoApprovalTimes_ReturnsZeroAverageDays()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var months = 1;

            var now = DateTime.UtcNow;
            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, Guid.NewGuid(), now) // No submitted date
            };

            var guides = CreateGuidesWithGlosedAmounts(batches[0].Id);

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            // Act
            var result = await _service.GetMonthlyPerformanceAsync(clinicId, months, TenantId);

            // Assert
            var resultList = result.ToList();
            if (resultList.Any())
            {
                resultList[0].AverageApprovalDays.Should().Be(0);
            }
        }

        #endregion

        #region GetGlosaAlertsAsync Tests

        [Fact]
        public async Task GetGlosaAlertsAsync_WithHighGlosaRate_ReturnsAlerts()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            
            // Create guides with high glosa rate (40%)
            var guides = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[0].Id, 1000m, 600m, 400m)
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(CreateOperator(operatorId, "Test Operator"));

            // Act
            var result = await _service.GetGlosaAlertsAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().NotBeEmpty();
            resultList.Should().Contain(a => a.AlertType == "Taxa de Glosa Geral");
        }

        [Fact]
        public async Task GetGlosaAlertsAsync_WithLowGlosaRate_ReturnsNoAlerts()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            
            // Create guides with low glosa rate (5%)
            var guides = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[0].Id, 1000m, 950m, 50m)
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(CreateOperator(Guid.NewGuid(), "Test Operator"));

            // Act
            var result = await _service.GetGlosaAlertsAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            result.Should().NotBeNull();
            var resultList = result.ToList();
            resultList.Should().BeEmpty();
        }

        [Fact]
        public async Task GetGlosaAlertsAsync_WithVeryHighGlosaRate_ReturnsHighSeverityAlert()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = CreateBatchesWithProcessedDates(clinicId, startDate, endDate);
            
            // Create guides with very high glosa rate (30%)
            var guides = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[0].Id, 1000m, 700m, 300m)
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(guides);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync(CreateOperator(operatorId, "Test Operator"));

            // Act
            var result = await _service.GetGlosaAlertsAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            resultList.Should().Contain(a => a.Severity == "high");
        }

        [Fact]
        public async Task GetGlosaAlertsAsync_OrdersBySeverityAndValue()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var operatorId1 = Guid.NewGuid();
            var operatorId2 = Guid.NewGuid();
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2024, 1, 31);

            var batches = new List<TissBatch>
            {
                CreateBatch(clinicId, operatorId1, new DateTime(2024, 1, 15)),
                CreateBatch(clinicId, operatorId2, new DateTime(2024, 1, 20))
            };

            // Operator 1: 40% glosa (very high - should be high severity)
            var guidesOp1 = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[0].Id, 1000m, 600m, 400m)
            };

            // Operator 2: 20% glosa (medium severity)
            var guidesOp2 = new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batches[1].Id, 1000m, 800m, 200m)
            };

            _tissBatchRepositoryMock.Setup(r => r.GetByClinicIdAsync(clinicId, TenantId))
                .ReturnsAsync(batches);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[0].Id, TenantId))
                .ReturnsAsync(guidesOp1);

            _tissGuideRepositoryMock.Setup(r => r.GetByBatchIdAsync(batches[1].Id, TenantId))
                .ReturnsAsync(guidesOp2);

            _operatorRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TenantId))
                .ReturnsAsync((Guid id, string tenantId) => CreateOperator(id, $"Operator {id}"));

            // Act
            var result = await _service.GetGlosaAlertsAsync(clinicId, startDate, endDate, TenantId);

            // Assert
            var resultList = result.ToList();
            if (resultList.Count > 1)
            {
                var firstAlert = resultList[0];
                firstAlert.Severity.Should().Be("high");
            }
        }

        #endregion

        #region Helper Methods

        private TissBatch CreateBatch(Guid clinicId, Guid operatorId, DateTime? processedDate)
        {
            var batchNumber = $"BATCH-{Guid.NewGuid().ToString().Substring(0, 16)}";
            var batch = new TissBatch(clinicId, operatorId, batchNumber, TenantId);
            
            if (processedDate.HasValue)
            {
                typeof(TissBatch).GetProperty("ProcessedDate")!.SetValue(batch, processedDate.Value);
            }
            
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, Guid.NewGuid());
            return batch;
        }

        private TissBatch CreateBatchWithSubmittedAndProcessedDates(
            Guid clinicId, 
            Guid operatorId, 
            DateTime submittedDate, 
            DateTime processedDate)
        {
            var batchNumber = $"BATCH-{Guid.NewGuid().ToString().Substring(0, 16)}";
            var batch = new TissBatch(clinicId, operatorId, batchNumber, TenantId);
            
            typeof(TissBatch).GetProperty("SubmittedDate")!.SetValue(batch, submittedDate);
            typeof(TissBatch).GetProperty("ProcessedDate")!.SetValue(batch, processedDate);
            typeof(TissBatch).GetProperty("Id")!.SetValue(batch, Guid.NewGuid());
            
            return batch;
        }

        private List<TissBatch> CreateBatchesWithProcessedDates(Guid clinicId, DateTime startDate, DateTime endDate)
        {
            var operatorId = Guid.NewGuid();
            return new List<TissBatch>
            {
                CreateBatch(clinicId, operatorId, startDate.AddDays(5)),
                CreateBatch(clinicId, operatorId, startDate.AddDays(10))
            };
        }

        private List<TissBatch> CreateBatchesWithSubmittedAndProcessedDates(
            Guid clinicId, 
            Guid operatorId, 
            DateTime startDate, 
            DateTime endDate,
            int approvalDays = 5)
        {
            return new List<TissBatch>
            {
                CreateBatchWithSubmittedAndProcessedDates(
                    clinicId, 
                    operatorId, 
                    startDate.AddDays(1), 
                    startDate.AddDays(1 + approvalDays)),
                CreateBatchWithSubmittedAndProcessedDates(
                    clinicId, 
                    operatorId, 
                    startDate.AddDays(10), 
                    startDate.AddDays(10 + approvalDays))
            };
        }

        private TissGuide CreateGuideWithGlosedAmount(
            Guid batchId, 
            decimal totalAmount, 
            decimal? approvedAmount, 
            decimal? glosedAmount)
        {
            var appointmentId = Guid.NewGuid();
            var insuranceId = Guid.NewGuid();
            var guideNumber = $"GUIDE-{Guid.NewGuid().ToString().Substring(0, 16)}";
            
            var guide = new TissGuide(
                batchId,
                appointmentId,
                insuranceId,
                guideNumber,
                TissGuideType.Consultation,
                new DateTime(2024, 1, 15),
                TenantId,
                "AUTH123"
            );
            
            typeof(TissGuide).GetProperty("TotalAmount")!.SetValue(guide, totalAmount);
            typeof(TissGuide).GetProperty("ApprovedAmount")!.SetValue(guide, approvedAmount);
            typeof(TissGuide).GetProperty("GlosedAmount")!.SetValue(guide, glosedAmount);
            typeof(TissGuide).GetProperty("Id")!.SetValue(guide, Guid.NewGuid());
            
            return guide;
        }

        private List<TissGuide> CreateGuidesWithGlosedAmounts(Guid batchId)
        {
            return new List<TissGuide>
            {
                CreateGuideWithGlosedAmount(batchId, 1000m, 800m, 200m),
                CreateGuideWithGlosedAmount(batchId, 1000m, 800m, 200m),
                CreateGuideWithGlosedAmount(batchId, 1000m, 800m, 200m)
            };
        }

        private List<TissGuide> CreateGuidesWithProcedures(Guid batchId)
        {
            var guide1 = CreateGuideWithGlosedAmount(batchId, 500m, 400m, 100m);
            var guide2 = CreateGuideWithGlosedAmount(batchId, 300m, 250m, 50m);

            var procedures1 = new List<TissGuideProcedure>
            {
                CreateProcedure(guide1.Id, "10101012", "Consulta médica", 300m, 100m),
                CreateProcedure(guide1.Id, "20101020", "Exame de sangue", 200m, 0m)
            };

            var procedures2 = new List<TissGuideProcedure>
            {
                CreateProcedure(guide2.Id, "10101012", "Consulta médica", 300m, 50m)
            };

            typeof(TissGuide).GetProperty("Procedures")!.SetValue(guide1, procedures1);
            typeof(TissGuide).GetProperty("Procedures")!.SetValue(guide2, procedures2);

            return new List<TissGuide> { guide1, guide2 };
        }

        private List<TissGuide> CreateGuidesWithManyProcedures(Guid batchId, int procedureCount)
        {
            var guide = CreateGuideWithGlosedAmount(batchId, 1000m, 800m, 200m);
            
            var procedures = new List<TissGuideProcedure>();
            for (int i = 0; i < procedureCount; i++)
            {
                procedures.Add(CreateProcedure(
                    guide.Id, 
                    $"CODE{i:D4}", 
                    $"Procedure {i}", 
                    100m, 
                    10m * i));
            }

            typeof(TissGuide).GetProperty("Procedures")!.SetValue(guide, procedures);

            return new List<TissGuide> { guide };
        }

        private TissGuideProcedure CreateProcedure(
            Guid guideId, 
            string code, 
            string description, 
            decimal totalPrice, 
            decimal? glosedAmount)
        {
            var procedure = new TissGuideProcedure(
                guideId,
                code,
                description,
                1,
                totalPrice,
                TenantId
            );

            typeof(TissGuideProcedure).GetProperty("GlosedAmount")!.SetValue(procedure, glosedAmount);
            typeof(TissGuideProcedure).GetProperty("Id")!.SetValue(procedure, Guid.NewGuid());

            return procedure;
        }

        private HealthInsuranceOperator CreateOperator(Guid operatorId, string tradeName)
        {
            var operatorEntity = new HealthInsuranceOperator(
                tradeName,
                $"{tradeName} Company",
                "123456",
                "12345678901234",
                TenantId
            );
            typeof(HealthInsuranceOperator).GetProperty("Id")!.SetValue(operatorEntity, operatorId);
            return operatorEntity;
        }

        private List<AuthorizationRequest> CreateAuthorizationRequests(
            Guid operatorId, 
            DateTime startDate, 
            DateTime endDate)
        {
            var patientId = Guid.NewGuid();
            var requests = new List<AuthorizationRequest>();

            // Create 10 requests: 7 approved, 2 denied, 1 pending
            for (int i = 0; i < 10; i++)
            {
                var status = i < 7 ? AuthorizationStatus.Approved : 
                           i < 9 ? AuthorizationStatus.Denied : 
                           AuthorizationStatus.Pending;

                var request = CreateAuthorizationRequest(patientId, operatorId, startDate.AddDays(i), status);
                requests.Add(request);
            }

            return requests;
        }

        private List<AuthorizationRequest> CreateAuthorizationRequestsHighApproval(
            Guid operatorId, 
            DateTime startDate, 
            DateTime endDate)
        {
            var patientId = Guid.NewGuid();
            var requests = new List<AuthorizationRequest>();

            // Create 10 requests: 9 approved, 1 denied
            for (int i = 0; i < 10; i++)
            {
                var status = i < 9 ? AuthorizationStatus.Approved : AuthorizationStatus.Denied;
                var request = CreateAuthorizationRequest(patientId, operatorId, startDate.AddDays(i), status);
                requests.Add(request);
            }

            return requests;
        }

        private AuthorizationRequest CreateAuthorizationRequest(
            Guid patientId, 
            Guid operatorId, 
            DateTime requestDate, 
            AuthorizationStatus status)
        {
            var insuranceId = Guid.NewGuid();
            var requestNumber = $"REQ-{Guid.NewGuid().ToString().Substring(0, 16)}";

            var request = new AuthorizationRequest(
                patientId,
                insuranceId,
                requestNumber,
                "10101012",
                "Consulta médica",
                1,
                TenantId
            );

            // Setup PatientHealthInsurance with HealthInsurancePlan that has OperatorId
            var plan = new HealthInsurancePlan(
                operatorId,
                "Test Plan",
                "123",
                HealthInsurancePlanType.Individual,
                TenantId
            );
            typeof(HealthInsurancePlan).GetProperty("Id")!.SetValue(plan, Guid.NewGuid());

            var insurance = new PatientHealthInsurance(
                patientId,
                plan.Id,
                "CARD123",
                new DateTime(2023, 1, 1),
                TenantId
            );
            typeof(PatientHealthInsurance).GetProperty("Id")!.SetValue(insurance, insuranceId);
            typeof(PatientHealthInsurance).GetProperty("HealthInsurancePlan")!.SetValue(insurance, plan);

            typeof(AuthorizationRequest).GetProperty("PatientHealthInsurance")!.SetValue(request, insurance);
            typeof(AuthorizationRequest).GetProperty("RequestDate")!.SetValue(request, requestDate);
            typeof(AuthorizationRequest).GetProperty("Status")!.SetValue(request, status);
            typeof(AuthorizationRequest).GetProperty("Id")!.SetValue(request, Guid.NewGuid());

            return request;
        }

        private AuthorizationRequest CreateAuthorizationRequestWithoutInsurance(DateTime requestDate)
        {
            var patientId = Guid.NewGuid();
            var insuranceId = Guid.NewGuid();
            var requestNumber = $"REQ-{Guid.NewGuid().ToString().Substring(0, 16)}";

            var request = new AuthorizationRequest(
                patientId,
                insuranceId,
                requestNumber,
                "10101012",
                "Consulta médica",
                1,
                TenantId
            );

            typeof(AuthorizationRequest).GetProperty("RequestDate")!.SetValue(request, requestDate);
            typeof(AuthorizationRequest).GetProperty("PatientHealthInsurance")!.SetValue(request, null);
            typeof(AuthorizationRequest).GetProperty("Id")!.SetValue(request, Guid.NewGuid());

            return request;
        }

        #endregion
    }
}
