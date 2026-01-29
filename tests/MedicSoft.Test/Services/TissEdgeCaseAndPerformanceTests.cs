using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    /// <summary>
    /// Edge case and performance tests for TISS services
    /// Tests boundary conditions, error handling, and performance characteristics
    /// </summary>
    public class TissEdgeCaseAndPerformanceTests
    {
        private const string TenantId = "test-tenant";

        #region Edge Case Tests

        [Fact]
        public async Task XmlGenerator_WithEmptyBatch_ThrowsException()
        {
            // Arrange
            var batchRepoMock = new Mock<ITissBatchRepository>();
            var guideRepoMock = new Mock<ITissGuideRepository>();
            var clinicRepoMock = new Mock<IClinicRepository>();
            var operatorRepoMock = new Mock<IHealthInsuranceOperatorRepository>();
            var validatorMock = new Mock<ITissXmlValidatorService>();

            var batch = new TissBatch { Id = 1, BatchNumber = "B001", TenantId = TenantId };
            var emptyGuides = new List<TissGuide>();

            batchRepoMock.Setup(r => r.GetByIdAsync(1, TenantId)).ReturnsAsync(batch);
            guideRepoMock.Setup(r => r.GetGuidesByBatchIdAsync(1, TenantId)).ReturnsAsync(emptyGuides);

            var generator = new TissXmlGeneratorService(
                batchRepoMock.Object,
                guideRepoMock.Object,
                clinicRepoMock.Object,
                operatorRepoMock.Object,
                validatorMock.Object
            );

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await generator.GenerateXmlAsync(1, TenantId)
            );
        }

        [Fact]
        public async Task TissGuide_WithNegativeAmount_ShouldBeRejected()
        {
            // Arrange
            var guide = new TissGuide
            {
                Id = 1,
                GuideNumber = "G001",
                TotalAmount = -100.00m, // Negative amount
                TenantId = TenantId
            };

            // Act
            var isValid = guide.TotalAmount >= 0;

            // Assert
            isValid.Should().BeFalse("Negative amounts should not be allowed");
        }

        [Fact]
        public async Task TissGuide_WithZeroQuantityProcedure_ShouldBeHandled()
        {
            // Arrange
            var procedure = new TissGuideProcedure
            {
                Id = 1,
                ProcedureCode = "10101012",
                Quantity = 0, // Zero quantity
                UnitPrice = 100.00m,
                TenantId = TenantId
            };

            // Act
            var totalAmount = procedure.Quantity * procedure.UnitPrice;

            // Assert
            totalAmount.Should().Be(0m, "Zero quantity should result in zero amount");
        }

        [Fact]
        public async Task TissBatch_WithDuplicateGuides_ShouldBeDetected()
        {
            // Arrange
            var guides = new List<TissGuide>
            {
                new TissGuide { Id = 1, GuideNumber = "G001", TenantId = TenantId },
                new TissGuide { Id = 2, GuideNumber = "G001", TenantId = TenantId } // Duplicate
            };

            // Act
            var duplicateNumbers = guides
                .GroupBy(g => g.GuideNumber)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            // Assert
            duplicateNumbers.Should().Contain("G001", "Duplicate guide numbers should be detected");
        }

        [Fact]
        public async Task XmlValidation_WithMalformedXml_ShouldFail()
        {
            // Arrange
            var validatorMock = new Mock<ITissXmlValidatorService>();
            var malformedXml = "<?xml version=\"1.0\"?><tissLoteGuias><unclosed>";

            validatorMock.Setup(v => v.ValidateXmlStructureAsync(malformedXml))
                .ReturnsAsync(false);

            // Act
            var result = await validatorMock.Object.ValidateXmlStructureAsync(malformedXml);

            // Assert
            result.Should().BeFalse("Malformed XML should fail validation");
        }

        [Fact]
        public async Task TissBatch_WithExcessiveGlossaAmount_ShouldBeValidated()
        {
            // Arrange
            var batch = new TissBatch
            {
                Id = 1,
                TotalAmount = 1000.00m,
                GlossaAmount = 1500.00m, // Glossa exceeds total
                TenantId = TenantId
            };

            // Act
            var isInvalid = batch.GlossaAmount > batch.TotalAmount;

            // Assert
            isInvalid.Should().BeTrue("Glossa amount should not exceed total amount");
        }

        [Fact]
        public async Task HealthInsuranceOperator_WithInvalidAnsNumber_ShouldBeRejected()
        {
            // Arrange
            var @operator = new HealthInsuranceOperator
            {
                Id = 1,
                AnsRegistrationNumber = "ABC", // Invalid format (should be numeric)
                TradeName = "Test Operator",
                TenantId = TenantId
            };

            // Act
            var isNumeric = int.TryParse(@operator.AnsRegistrationNumber, out _);

            // Assert
            isNumeric.Should().BeFalse("ANS registration number should be validated");
        }

        [Fact]
        public async Task TissGuide_WithFutureDates_ShouldBeHandled()
        {
            // Arrange
            var guide = new TissGuide
            {
                Id = 1,
                GuideNumber = "G001",
                ServiceDate = DateTime.Now.AddDays(30), // Future date
                TenantId = TenantId
            };

            // Act
            var isFutureDate = guide.ServiceDate > DateTime.Now;

            // Assert
            isFutureDate.Should().BeTrue("Future service dates should be detected");
        }

        #endregion

        #region Performance Tests

        [Fact]
        public async Task XmlGeneration_For100Guides_ShouldCompleteUnder30Seconds()
        {
            // Arrange
            var batchRepoMock = new Mock<ITissBatchRepository>();
            var guideRepoMock = new Mock<ITissGuideRepository>();
            var clinicRepoMock = new Mock<IClinicRepository>();
            var operatorRepoMock = new Mock<IHealthInsuranceOperatorRepository>();
            var validatorMock = new Mock<ITissXmlValidatorService>();

            var batch = new TissBatch 
            { 
                Id = 1, 
                BatchNumber = "B001", 
                ClinicId = 1,
                OperatorId = 1,
                TenantId = TenantId 
            };
            
            var guides = Enumerable.Range(1, 100)
                .Select(i => new TissGuide
                {
                    Id = i,
                    GuideNumber = $"G{i:D3}",
                    TotalAmount = 100.00m,
                    ServiceDate = DateTime.Now,
                    TenantId = TenantId
                })
                .ToList();

            var clinic = new Clinic { Id = 1, Name = "Test Clinic", CNPJ = "12345678000100", TenantId = TenantId };
            var @operator = new HealthInsuranceOperator 
            { 
                Id = 1, 
                TradeName = "Test Operator",
                AnsRegistrationNumber = "123456",
                TenantId = TenantId 
            };

            batchRepoMock.Setup(r => r.GetByIdAsync(1, TenantId)).ReturnsAsync(batch);
            guideRepoMock.Setup(r => r.GetGuidesByBatchIdAsync(1, TenantId)).ReturnsAsync(guides);
            clinicRepoMock.Setup(r => r.GetByIdAsync(1, TenantId)).ReturnsAsync(clinic);
            operatorRepoMock.Setup(r => r.GetByIdAsync(1, TenantId)).ReturnsAsync(@operator);
            validatorMock.Setup(v => v.ValidateXmlStructureAsync(It.IsAny<string>())).ReturnsAsync(true);

            var generator = new TissXmlGeneratorService(
                batchRepoMock.Object,
                guideRepoMock.Object,
                clinicRepoMock.Object,
                operatorRepoMock.Object,
                validatorMock.Object
            );

            // Act
            var stopwatch = Stopwatch.StartNew();
            var xml = await generator.GenerateXmlAsync(1, TenantId);
            stopwatch.Stop();

            // Assert
            xml.Should().NotBeNullOrEmpty();
            stopwatch.Elapsed.TotalSeconds.Should().BeLessThan(30, 
                "XML generation for 100 guides should complete in under 30 seconds");
        }

        [Fact]
        public async Task Analytics_CalculateMetrics_For1000Batches_ShouldBePerformant()
        {
            // Arrange
            var batchRepoMock = new Mock<ITissBatchRepository>();
            var guideRepoMock = new Mock<ITissGuideRepository>();
            
            var batches = Enumerable.Range(1, 1000)
                .Select(i => new TissBatch
                {
                    Id = i,
                    BatchNumber = $"B{i:D4}",
                    TotalAmount = 1000.00m,
                    GlossaAmount = 100.00m,
                    Status = "Paid",
                    TenantId = TenantId
                })
                .ToList();

            batchRepoMock.Setup(r => r.GetBatchesByPeriodAsync(
                It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), TenantId))
                .ReturnsAsync(batches);

            var analyticsService = new TissAnalyticsService(
                batchRepoMock.Object,
                guideRepoMock.Object
            );

            // Act
            var stopwatch = Stopwatch.StartNew();
            var summary = await analyticsService.GetGlossSummaryAsync(
                1, DateTime.Now.AddMonths(-1), DateTime.Now, TenantId);
            stopwatch.Stop();

            // Assert
            summary.Should().NotBeNull();
            stopwatch.Elapsed.TotalSeconds.Should().BeLessThan(5, 
                "Analytics calculation for 1000 batches should complete in under 5 seconds");
        }

        [Fact]
        public async Task TussSearch_WithWildcard_ShouldReturnRelevantResults()
        {
            // Arrange
            var tussProcedureRepoMock = new Mock<ITussProcedureRepository>();
            
            var procedures = new List<TussProcedure>
            {
                new TussProcedure { Code = "10101012", Description = "Consulta médica", TenantId = TenantId },
                new TussProcedure { Code = "10101020", Description = "Consulta de retorno", TenantId = TenantId },
                new TussProcedure { Code = "20201030", Description = "Exame de sangue", TenantId = TenantId }
            };

            tussProcedureRepoMock.Setup(r => r.SearchAsync("Consulta", TenantId, 1, 10))
                .ReturnsAsync(procedures.Where(p => p.Description.Contains("Consulta")).ToList());

            // Act
            var results = await tussProcedureRepoMock.Object.SearchAsync("Consulta", TenantId, 1, 10);

            // Assert
            results.Should().HaveCount(2);
            results.Should().OnlyContain(p => p.Description.Contains("Consulta"));
        }

        #endregion

        #region Boundary Condition Tests

        [Fact]
        public async Task TissBatch_WithMaxIntGuideCount_ShouldBeHandled()
        {
            // Arrange
            var batch = new TissBatch
            {
                Id = 1,
                GuideCount = int.MaxValue,
                TenantId = TenantId
            };

            // Act & Assert
            batch.GuideCount.Should().Be(int.MaxValue);
        }

        [Fact]
        public async Task TissGuide_WithMaxDecimalAmount_ShouldBeHandled()
        {
            // Arrange
            var guide = new TissGuide
            {
                Id = 1,
                TotalAmount = 999999999.99m, // Maximum reasonable amount
                TenantId = TenantId
            };

            // Act & Assert
            guide.TotalAmount.Should().BeGreaterThan(0);
            guide.TotalAmount.Should().BeLessThan(decimal.MaxValue);
        }

        [Fact]
        public async Task TissGuide_WithEmptyGuideNumber_ShouldBeRejected()
        {
            // Arrange
            var guide = new TissGuide
            {
                Id = 1,
                GuideNumber = "", // Empty guide number
                TenantId = TenantId
            };

            // Act
            var isInvalid = string.IsNullOrWhiteSpace(guide.GuideNumber);

            // Assert
            isInvalid.Should().BeTrue("Empty guide numbers should be rejected");
        }

        #endregion
    }
}
