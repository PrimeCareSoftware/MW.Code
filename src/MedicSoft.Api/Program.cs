using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MedicSoft.Api.JsonConverters;
using MedicSoft.Api.Middleware;
using MedicSoft.Application.Mappings;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Extensions;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using MedicSoft.Repository.Context;
using MedicSoft.Repository.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Add custom TimeSpan converter to serialize as "HH:mm" format for calendar compatibility
        options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedicWarehouse API",
        Version = "v1",
        Description = "MedicWarehouse - Sistema de Gestão para Consultórios Médicos",
        Contact = new OpenApiContact
        {
            Name = "MedicWarehouse",
            Email = "contato@medicwarehouse.com"
        }
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure database with auto-detection for SQL Server or PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MedicSoftDbContext>((serviceProvider, options) =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        // Auto-detect database provider based on connection string
        if (connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            // PostgreSQL
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                npgsqlOptions.CommandTimeout(60);
            });
        }
        else
        {
            // SQL Server (backward compatibility)
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                sqlOptions.CommandTimeout(60);
            });
        }
    }
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var issuer = jwtSettings["Issuer"] ?? "MedicWarehouse";
var audience = jwtSettings["Audience"] ?? "MedicWarehouse-API";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.FromMinutes(5) // Allow 5 minutes tolerance for time sync issues
    };
});

builder.Services.AddAuthorization();

// Configure Rate Limiting
var rateLimitEnabled = builder.Configuration.GetValue<bool>("RateLimiting:EnableRateLimiting", true);
if (rateLimitEnabled)
{
    builder.Services.AddRateLimiter(options =>
    {
        var permitLimit = builder.Configuration.GetValue<int>("RateLimiting:PermitLimit", 10);
        var windowSeconds = builder.Configuration.GetValue<int>("RateLimiting:WindowSeconds", 60);
        var queueLimit = builder.Configuration.GetValue<int>("RateLimiting:QueueLimit", 0);

        options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
                factory: partition => new FixedWindowRateLimiterOptions
                {
                    AutoReplenishment = true,
                    PermitLimit = permitLimit,
                    QueueLimit = queueLimit,
                    Window = TimeSpan.FromSeconds(windowSeconds)
                }));

        options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    });
}

// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Configure MediatR License
builder.Services.Configure<MedicSoft.Application.Configuration.MediatRLicenseSettings>(
    builder.Configuration.GetSection("MediatRLicense"));
builder.Services.AddSingleton<MedicSoft.Application.Services.MediatRLicenseService>();

// Configure MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(MedicSoft.Application.Services.PatientService).Assembly));

// Register repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IOwnerRepository, OwnerRepository>();
builder.Services.AddScoped<INotificationRoutineRepository, NotificationRoutineRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPatientClinicLinkRepository, PatientClinicLinkRepository>();
builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
builder.Services.AddScoped<IAppointmentProcedureRepository, AppointmentProcedureRepository>();
builder.Services.AddScoped<IClinicSubscriptionRepository, ClinicSubscriptionRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
builder.Services.AddScoped<IPrescriptionTemplateRepository, PrescriptionTemplateRepository>();
builder.Services.AddScoped<IMedicalRecordTemplateRepository, MedicalRecordTemplateRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExamRequestRepository, ExamRequestRepository>();
builder.Services.AddScoped<IWaitingQueueRepository, WaitingQueueRepository>();
builder.Services.AddScoped<IWaitingQueueConfigurationRepository, WaitingQueueConfigurationRepository>();
builder.Services.AddScoped<IOwnerClinicLinkRepository, OwnerClinicLinkRepository>();
builder.Services.AddScoped<IExamCatalogRepository, ExamCatalogRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IOwnerSessionRepository, OwnerSessionRepository>();

// Register application services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExamRequestService, ExamRequestService>();
builder.Services.AddScoped<IWaitingQueueService, WaitingQueueService>();
builder.Services.AddScoped<IOwnerClinicLinkService, OwnerClinicLinkService>();
builder.Services.AddScoped<DataSeederService>();

// Register domain services
builder.Services.AddScoped<AppointmentSchedulingService>();
builder.Services.AddScoped<ISubscriptionService>(provider =>
{
    var notificationService = provider.GetRequiredService<INotificationService>();
    var environment = builder.Environment.EnvironmentName;
    return new SubscriptionService(notificationService, environment);
});

// Register cross-cutting services (includes security services)
builder.Services.AddMedicSoftCrossCutting();

// Configure CORS with secure settings
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "MedicWarehouse API v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
}
else
{
    // Enable HSTS in production
    app.UseHsts();
}

// Add security headers
app.UseSecurityHeaders();

// Use HTTPS redirection only if required by configuration
var requireHttps = builder.Configuration.GetValue<bool>("Security:RequireHttps", false);
if (requireHttps)
{
    app.UseHttpsRedirection();
}

// Use secure CORS policy
app.UseCors("SecurePolicy");

// Add tenant resolution middleware (before authentication)
app.UseTenantResolution();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Enable rate limiting
if (rateLimitEnabled)
{
    app.UseRateLimiter();
}

app.MapControllers();

// Initialize MediatR License
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var licenseService = scope.ServiceProvider.GetRequiredService<MedicSoft.Application.Services.MediatRLicenseService>();
        licenseService.InitializeLicense();
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "MediatR license initialization warning: {Message}", ex.Message);
    }
}

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed: {Message}", ex.Message);
        Console.WriteLine($"Database migration failed: {ex.Message}");
    }
}

app.Run();