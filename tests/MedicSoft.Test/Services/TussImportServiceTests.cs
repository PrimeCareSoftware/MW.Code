using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Moq;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class TussImportServiceTests
    {
        private readonly Mock<ITussProcedureRepository> _repositoryMock;
        private readonly Mock<ILogger<TussImportService>> _loggerMock;
        private readonly TussImportService _service;
        private const string TenantId = "test-tenant";

        public TussImportServiceTests()
        {
            _repositoryMock = new Mock<ITussProcedureRepository>();
            _loggerMock = new Mock<ILogger<TussImportService>>();
            _service = new TussImportService(_repositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task ImportFromCsvAsync_WithEmptyStream_ShouldReturnError()
        {
            using var stream = new MemoryStream();
            var result = await _service.ImportFromCsvAsync(stream, TenantId);
            result.Success.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
            result.Errors[0].Message.Should().Contain("empty");
        }

        [Fact]
        public async Task ImportFromCsvAsync_WithValidSingleRecord_ShouldImportSuccessfully()
        {
            var csv = @"Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
10101012,Consulta médica,01,Consulta médica em consultório,100.00,false";
            using var stream = CreateStream(csv);

            _repositoryMock.Setup(r => r.GetByCodeAsync(It.IsAny<string>(), TenantId))
                .ReturnsAsync((TussProcedure?)null);
            _repositoryMock.Setup(r => r.AddAsync(It.IsAny<TussProcedure>()))
                .ReturnsAsync((TussProcedure p) => p);
            _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.ImportFromCsvAsync(stream, TenantId);

            result.Success.Should().BeTrue();
            result.TotalRecords.Should().Be(1);
            result.SuccessfulImports.Should().Be(1);
        }

        [Fact]
        public async Task ImportFromCsvAsync_WithExistingProcedure_ShouldUpdateRecord()
        {
            var csv = @"Code,Name,Category,Description,ReferencePrice,RequiresAuthorization
10101012,Consulta médica,01,Consulta médica atualizada,150.00,true";
            using var stream = CreateStream(csv);

            var existingProcedure = new TussProcedure("10101012", "Old description", "01", 100.00m, TenantId);

            _repositoryMock.Setup(r => r.GetByCodeAsync("10101012", TenantId))
                .ReturnsAsync(existingProcedure);
            _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<TussProcedure>()))
                .Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var result = await _service.ImportFromCsvAsync(stream, TenantId);

            result.Success.Should().BeTrue();
            result.UpdatedRecords.Should().Be(1);
        }

        [Fact]
        public async Task ImportFromExcelAsync_ShouldReturnNotImplemented()
        {
            using var stream = new MemoryStream();
            var result = await _service.ImportFromExcelAsync(stream, TenantId);
            result.Success.Should().BeFalse();
            result.Errors.Should().Contain(e => e.Message.Contains("not yet implemented"));
        }

        private static MemoryStream CreateStream(string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            return new MemoryStream(bytes);
        }
    }
}
