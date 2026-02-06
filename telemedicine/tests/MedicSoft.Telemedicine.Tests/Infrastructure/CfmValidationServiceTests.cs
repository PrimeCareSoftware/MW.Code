using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MedicSoft.Telemedicine.Application.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.ExternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace MedicSoft.Telemedicine.Tests.Infrastructure;

public class CfmValidationServiceTests
{
    private readonly Mock<ILogger<CfmValidationService>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;

    public CfmValidationServiceTests()
    {
        _mockLogger = new Mock<ILogger<CfmValidationService>>();
        _mockConfiguration = new Mock<IConfiguration>();
    }

    [Fact]
    public async Task ValidateCrmAsync_WithValidCrm_ReturnsSuccessResult()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new
                {
                    nome = "Dr. João Silva",
                    numero = "123456",
                    uf = "SP",
                    especialidade = "Cardiologia",
                    situacao = "Ativo",
                    dataInscricao = DateTime.UtcNow.AddYears(-5)
                })
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCrmAsync("123456", "SP");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.DoctorName.Should().Be("Dr. João Silva");
        result.CrmNumber.Should().Be("123456");
        result.CrmState.Should().Be("SP");
        result.Specialty.Should().Be("Cardiologia");
        result.Status.Should().Be("Ativo");
    }

    [Fact]
    public async Task ValidateCrmAsync_WithInvalidCrm_ReturnsNotFoundResult()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCrmAsync("999999", "SP");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not found");
    }

    [Fact]
    public async Task ValidateCrmAsync_WithEmptyCrmNumber_ReturnsInvalidResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCrmAsync("", "SP");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task ValidateCrmAsync_WithEmptyState_ReturnsInvalidResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCrmAsync("123456", "");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task ValidateCpfAsync_WithValidCpf_ReturnsSuccessResult()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = JsonContent.Create(new
                {
                    valido = true,
                    cpf = "12345678901"
                })
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCpfAsync("123.456.789-01");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Cpf.Should().Be("12345678901");
    }

    [Fact]
    public async Task ValidateCpfAsync_WithInvalidCpf_ReturnsInvalidResult()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound
            });

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCpfAsync("00000000000");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ValidateCpfAsync_WithEmptyCpf_ReturnsInvalidResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCpfAsync("");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("required");
    }

    [Fact]
    public async Task ValidateCpfAsync_WithInvalidLength_ReturnsInvalidResult()
    {
        // Arrange
        var httpClient = new HttpClient();
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCpfAsync("123456");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("11 digits");
    }

    [Fact]
    public async Task ValidateCrmAsync_HandlesHttpRequestException()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCrmAsync("123456", "SP");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("connect");
    }

    [Fact]
    public async Task ValidateCpfAsync_HandlesHttpRequestException()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHttpMessageHandler.Object);
        var service = new CfmValidationService(httpClient, _mockConfiguration.Object, _mockLogger.Object);

        // Act
        var result = await service.ValidateCpfAsync("12345678901");

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("connect");
    }
}
