using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using PatientPortal.Infrastructure.Data;
using PatientPortal.Application.Interfaces;
using Moq;

namespace PatientPortal.Tests.Integration;

/// <summary>
/// Custom web application factory for integration tests
/// Uses in-memory database for testing
/// </summary>
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PatientPortalDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<PatientPortalDbContext>(options =>
            {
                options.UseInMemoryDatabase("InMemoryTestDb");
            });
            
            // Remove the AppointmentReminderService background service
            // This prevents it from trying to connect to PostgreSQL during tests
            var hostedServiceDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(IHostedService) &&
                     d.ImplementationType?.Name == "AppointmentReminderService");
            if (hostedServiceDescriptor != null)
            {
                services.Remove(hostedServiceDescriptor);
            }
            
            // Replace the notification service with a mock that doesn't actually send emails
            var notificationDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(INotificationService));
            if (notificationDescriptor != null)
            {
                services.Remove(notificationDescriptor);
            }
            
            // Add a mock notification service that doesn't throw
            var mockNotificationService = new Mock<INotificationService>();
            mockNotificationService
                .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            services.AddSingleton(mockNotificationService.Object);

            // Build the service provider and create the database
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<PatientPortalDbContext>();

            // Ensure database is created
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
        
        // Add configuration for testing
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["PortalBaseUrl"] = "https://portal.test.com",
                ["JwtSettings:SecretKey"] = "ThisIsATestSecretKeyThatIsAtLeast32CharactersLongForTesting",
                ["JwtSettings:Issuer"] = "TestIssuer",
                ["JwtSettings:Audience"] = "TestAudience",
                ["JwtSettings:ExpirationMinutes"] = "15"
            });
        });
    }
}
