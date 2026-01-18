using System;
using System.Linq;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class TissBatchTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();
        private readonly Guid _operatorId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesTissBatch()
        {
            // Arrange
            var batchNumber = "BATCH-001";

            // Act
            var batch = new TissBatch(_clinicId, _operatorId, batchNumber, TenantId);

            // Assert
            batch.Id.Should().NotBeEmpty();
            batch.ClinicId.Should().Be(_clinicId);
            batch.OperatorId.Should().Be(_operatorId);
            batch.BatchNumber.Should().Be(batchNumber);
            batch.Status.Should().Be(BatchStatus.Draft);
            batch.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            batch.SubmittedDate.Should().BeNull();
            batch.ProcessedDate.Should().BeNull();
            batch.Guides.Should().BeEmpty();
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissBatch(Guid.Empty, _operatorId, "BATCH-001", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Clinic ID*");
        }

        [Fact]
        public void Constructor_WithEmptyOperatorId_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissBatch(_clinicId, Guid.Empty, "BATCH-001", TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Operator ID*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidBatchNumber_ThrowsArgumentException(string? batchNumber)
        {
            // Act
            var act = () => new TissBatch(_clinicId, _operatorId, batchNumber!, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Batch number*");
        }

        [Fact]
        public void AddGuide_WithValidGuide_AddsGuide()
        {
            // Arrange
            var batch = CreateValidBatch();
            var guide = CreateValidGuide(batch.Id);

            // Act
            batch.AddGuide(guide);

            // Assert
            batch.Guides.Should().ContainSingle()
                .Which.Should().Be(guide);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void AddGuide_WithNullGuide_ThrowsArgumentNullException()
        {
            // Arrange
            var batch = CreateValidBatch();

            // Act
            var act = () => batch.AddGuide(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void AddGuide_WithDuplicateGuide_ThrowsInvalidOperationException()
        {
            // Arrange
            var batch = CreateValidBatch();
            var guide = CreateValidGuide(batch.Id);
            batch.AddGuide(guide);

            // Act
            var act = () => batch.AddGuide(guide);

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*already in this batch*");
        }

        [Fact]
        public void AddGuide_WithSentStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var batch = CreateValidBatch();
            var guide = CreateValidGuide(batch.Id);
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.MarkAsReadyToSend();
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();

            // Act
            var act = () => batch.AddGuide(guide);

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void RemoveGuide_WithExistingGuide_RemovesGuide()
        {
            // Arrange
            var batch = CreateValidBatch();
            var guide = CreateValidGuide(batch.Id);
            batch.AddGuide(guide);

            // Act
            batch.RemoveGuide(guide.Id);

            // Assert
            batch.Guides.Should().BeEmpty();
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void RemoveGuide_WithNonExistentGuide_DoesNothing()
        {
            // Arrange
            var batch = CreateValidBatch();

            // Act
            batch.RemoveGuide(Guid.NewGuid());

            // Assert
            batch.Guides.Should().BeEmpty();
        }

        [Fact]
        public void MarkAsReadyToSend_WithGuides_ChangesStatus()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));

            // Act
            batch.MarkAsReadyToSend();

            // Assert
            batch.Status.Should().Be(BatchStatus.ReadyToSend);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void MarkAsReadyToSend_WithoutGuides_ThrowsInvalidOperationException()
        {
            // Arrange
            var batch = CreateValidBatch();

            // Act
            var act = () => batch.MarkAsReadyToSend();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*without guides*");
        }

        [Fact]
        public void MarkAsReadyToSend_WithInvalidStatus_ThrowsInvalidOperationException()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.MarkAsReadyToSend();

            // Act
            var act = () => batch.MarkAsReadyToSend();

            // Assert
            act.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GenerateXml_WithValidData_GeneratesXml()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            var xmlFileName = "batch-001.xml";
            var xmlFilePath = "/path/to/batch-001.xml";

            // Act
            batch.GenerateXml(xmlFileName, xmlFilePath);

            // Assert
            batch.XmlFileName.Should().Be(xmlFileName);
            batch.XmlFilePath.Should().Be(xmlFilePath);
            batch.Status.Should().Be(BatchStatus.ReadyToSend);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GenerateXml_WithInvalidFileName_ThrowsArgumentException(string? fileName)
        {
            // Arrange
            var batch = CreateValidBatch();

            // Act
            var act = () => batch.GenerateXml(fileName!, "/path");

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Submit_WithReadyStatus_SubmitsBatch()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            var protocolNumber = "PROT-12345";

            // Act
            batch.Submit(protocolNumber);

            // Assert
            batch.Status.Should().Be(BatchStatus.Sent);
            batch.ProtocolNumber.Should().Be(protocolNumber);
            batch.SubmittedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Submit_WithoutXml_ThrowsInvalidOperationException()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.MarkAsReadyToSend();

            // Act
            var act = () => batch.Submit();

            // Assert
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("*without generated XML*");
        }

        [Fact]
        public void MarkAsProcessing_WithSentStatus_ChangesStatus()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();

            // Act
            batch.MarkAsProcessing();

            // Assert
            batch.Status.Should().Be(BatchStatus.Processing);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void ProcessResponse_WithFullApproval_SetsStatusToProcessed()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();
            batch.MarkAsProcessing();

            // Act
            batch.ProcessResponse("response.xml", 1000m, 0m);

            // Assert
            batch.Status.Should().Be(BatchStatus.Processed);
            batch.ApprovedAmount.Should().Be(1000m);
            batch.GlosedAmount.Should().Be(0m);
            batch.ResponseXmlFileName.Should().Be("response.xml");
            batch.ProcessedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void ProcessResponse_WithPartialApproval_SetsStatusToPartiallyPaid()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();
            batch.MarkAsProcessing();

            // Act
            batch.ProcessResponse("response.xml", 800m, 200m);

            // Assert
            batch.Status.Should().Be(BatchStatus.PartiallyPaid);
            batch.ApprovedAmount.Should().Be(800m);
            batch.GlosedAmount.Should().Be(200m);
        }

        [Fact]
        public void ProcessResponse_WithFullRejection_SetsStatusToRejected()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();
            batch.MarkAsProcessing();

            // Act
            batch.ProcessResponse("response.xml", 0m, 1000m);

            // Assert
            batch.Status.Should().Be(BatchStatus.Rejected);
        }

        [Fact]
        public void MarkAsPaid_WithProcessedStatus_ChangesStatusToPaid()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();
            batch.MarkAsProcessing();
            batch.ProcessResponse("response.xml", 1000m, 0m);

            // Act
            batch.MarkAsPaid();

            // Assert
            batch.Status.Should().Be(BatchStatus.Paid);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Reject_WithSentStatus_RejectsBatch()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.GenerateXml("test.xml", "/path/test.xml");
            batch.Submit();

            // Act
            batch.Reject();

            // Assert
            batch.Status.Should().Be(BatchStatus.Rejected);
            batch.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void GetTotalAmount_WithMultipleGuides_ReturnsSumOfGuideAmounts()
        {
            // Arrange
            var batch = CreateValidBatch();
            var guide1 = CreateValidGuide(batch.Id);
            var guide2 = CreateValidGuide(batch.Id);
            
            // Add procedures to guides so they have non-zero amounts
            var procedure1 = CreateValidProcedure(guide1.Id);
            var procedure2 = CreateValidProcedure(guide2.Id);
            guide1.AddProcedure(procedure1);
            guide2.AddProcedure(procedure2);
            
            batch.AddGuide(guide1);
            batch.AddGuide(guide2);

            // Act
            var totalAmount = batch.GetTotalAmount();

            // Assert
            totalAmount.Should().Be(guide1.TotalAmount + guide2.TotalAmount);
            totalAmount.Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetGuideCount_ReturnsCorrectCount()
        {
            // Arrange
            var batch = CreateValidBatch();
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.AddGuide(CreateValidGuide(batch.Id));
            batch.AddGuide(CreateValidGuide(batch.Id));

            // Act
            var count = batch.GetGuideCount();

            // Assert
            count.Should().Be(3);
        }

        private TissBatch CreateValidBatch()
        {
            return new TissBatch(
                _clinicId,
                _operatorId,
                $"BATCH-{Guid.NewGuid().ToString().Substring(0, 8)}",
                TenantId
            );
        }

        private TissGuide CreateValidGuide(Guid batchId)
        {
            return new TissGuide(
                batchId,
                Guid.NewGuid(),
                Guid.NewGuid(),
                $"GUIDE-{Guid.NewGuid().ToString().Substring(0, 8)}",
                TissGuideType.Consultation,
                DateTime.UtcNow,
                TenantId
            );
        }

        private TissGuideProcedure CreateValidProcedure(Guid guideId)
        {
            return new TissGuideProcedure(
                guideId,
                "40101012",
                "Consulta médica",
                1,
                150.00m,
                TenantId
            );
        }
    }
}
