using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MedicSoft.Auth.Api.Data;
using MedicSoft.Auth.Api.Services;
using MedicSoft.Shared.Authentication.Extensions;
using MedicSoft.Shared.Authentication.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedicWarehouse Auth Microservice",
        Version = "v1",
        Description = "Authentication microservice for MedicWarehouse system"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
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

// Configure database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    if (!string.IsNullOrEmpty(connectionString))
    {
        options.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(10),
                errorCodesToAdd: null);
            npgsqlOptions.CommandTimeout(60);
        });
    }
});

// Configure JWT settings
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

// Configure Session settings with validation
builder.Services.Configure<SessionSettings>(builder.Configuration.GetSection("SessionSettings"));
builder.Services.AddOptions<SessionSettings>()
    .Configure(options => builder.Configuration.GetSection("SessionSettings").Bind(options))
    .Validate(settings => 
    {
        if (settings.ExpiryHours <= 0)
        {
            throw new InvalidOperationException($"SessionSettings.ExpiryHours must be greater than 0. Current value: {settings.ExpiryHours}");
        }
        if (settings.ExpiryHours > 720) // 30 days
        {
            throw new InvalidOperationException($"SessionSettings.ExpiryHours is too large. Current value: {settings.ExpiryHours}. Maximum recommended: 720 hours (30 days)");
        }
        return true;
    }, "SessionSettings validation failed");

// Add JWT Authentication using shared library (for protected endpoints like validate-session)
builder.Services.AddMicroserviceAuthentication(builder.Configuration);

// Register services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:4200", "http://localhost:4201" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Ensure database is created and log configuration
// Note: In production, use proper EF Core migrations instead of EnsureCreated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    
    // Log session configuration
    try
    {
        var sessionSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<SessionSettings>>().Value;
        var jwtSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<JwtSettings>>().Value;
        
        logger.LogInformation("Session Configuration: ExpiryHours={ExpiryHours}, MaxConcurrentSessions={MaxConcurrentSessions}", 
            sessionSettings.ExpiryHours, sessionSettings.MaxConcurrentSessions);
        logger.LogInformation("JWT Configuration: ExpiryMinutes={ExpiryMinutes}, Issuer={Issuer}, Audience={Audience}", 
            jwtSettings.ExpiryMinutes, jwtSettings.Issuer, jwtSettings.Audience);
    }
    catch (Exception configEx)
    {
        logger.LogError(configEx, "Failed to log configuration settings");
    }
    
    try
    {
        // Only use EnsureCreated in development
        // In production, migrations should be applied separately
        if (env.IsDevelopment())
        {
            context.Database.EnsureCreated();
            logger.LogInformation("Database schema ensured in development mode");
        }
        else
        {
            // In production, verify database connectivity
            try
            {
                var canConnect = await context.Database.CanConnectAsync();
                if (canConnect)
                {
                    logger.LogInformation("Database connection verified successfully");
                }
                else
                {
                    logger.LogWarning("Could not connect to database. Please verify connection string and database availability.");
                }
            }
            catch (Exception dbEx)
            {
                logger.LogError(dbEx, "Failed to verify database connection. Error: {ErrorMessage}", dbEx.Message);
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth Microservice v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
