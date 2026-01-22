using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;
using MedicSoft.Api.Controllers;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Test.Api
{
    /// <summary>
    /// Tests for SessionsController proxy functionality
    /// </summary>
    public class SessionsControllerTests
    {
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ILogger<SessionsController>> _mockLogger;
        private readonly Mock<ITenantContext> _mockTenantContext;
        private readonly SessionsController _controller;

        public SessionsControllerTests()
        {
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<SessionsController>>();
            _mockTenantContext = new Mock<ITenantContext>();

            // Setup configuration to return telemedicine URL
            _mockConfiguration
                .Setup(c => c.GetValue<string>("Microservices:TelemedicineUrl", It.IsAny<string>()))
                .Returns("http://localhost:5084/api");

            _controller = new SessionsController(
                _mockHttpClientFactory.Object,
                _mockConfiguration.Object,
                _mockLogger.Object,
                _mockTenantContext.Object);

            // Setup HTTP context with tenant header
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Tenant-Id"] = "test-tenant";
            httpContext.Request.Headers["Authorization"] = "Bearer test-token";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetClinicSessions_WhenTelemedicineServiceReturnsOk_ShouldReturnOk()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var responseContent = "[{\"id\":\"123\",\"clinicId\":\"456\"}]";
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Act
            var result = await _controller.GetClinicSessions(clinicId, 0, 50);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
            Assert.Equal(responseContent, contentResult.Content);
        }

        [Fact]
        public async Task GetClinicSessions_WhenTelemedicineServiceReturnsNotFound_ShouldReturnNotFound()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Act
            var result = await _controller.GetClinicSessions(clinicId, 0, 50);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
        }

        [Fact]
        public async Task GetClinicSessions_WhenTelemedicineServiceUnavailable_ShouldReturn503()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("Service unavailable"));

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Act
            var result = await _controller.GetClinicSessions(clinicId, 0, 50);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(503, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetSessionById_WhenTelemedicineServiceReturnsOk_ShouldReturnOk()
        {
            // Arrange
            var sessionId = Guid.NewGuid();
            var responseContent = "{\"id\":\"123\",\"status\":\"active\"}";
            
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent)
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            _mockHttpClientFactory
                .Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            // Act
            var result = await _controller.GetSessionById(sessionId);

            // Assert
            var contentResult = Assert.IsType<ContentResult>(result);
            Assert.Equal("application/json", contentResult.ContentType);
            Assert.Equal(responseContent, contentResult.Content);
        }
    }
}
