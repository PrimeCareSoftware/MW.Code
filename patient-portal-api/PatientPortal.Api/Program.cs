using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PatientPortal.Application.Configuration;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;
using PatientPortal.Infrastructure.Repositories;
using PatientPortal.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Explicitly configure camelCase for JSON serialization to ensure consistency
        // Frontend expects: accessToken, refreshToken, expiresAt, user, etc.
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Patient Portal API",
        Version = "v1",
        Description = "API for Patient Portal - Omni Care Software\n\n" +
                      "This API provides secure access for patients to view their medical appointments, " +
                      "documents, and manage their profile information.\n\n" +
                      "**Authentication:** JWT Bearer Token\n" +
                      "**Base URL:** /api\n\n" +
                      "**Getting Started:**\n" +
                      "1. Register a new account via POST /api/auth/register\n" +
                      "2. Login via POST /api/auth/login to receive access and refresh tokens\n" +
                      "3. Use the access token in the Authorization header for protected endpoints\n" +
                      "4. Refresh tokens when needed via POST /api/auth/refresh\n\n" +
                      "**Security Features:**\n" +
                      "- JWT tokens with 15-minute expiry\n" +
                      "- Refresh tokens with 7-day validity\n" +
                      "- Password hashing with PBKDF2\n" +
                      "- Account lockout after 5 failed attempts\n" +
                      "- LGPD compliant data handling",
        Contact = new OpenApiContact
        {
            Name = "Omni Care Software",
            Email = "support@omnicaresoftware.com"
        },
        License = new OpenApiLicense
        {
            Name = "Proprietary"
        }
    });

    // Include XML comments
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\n\n" +
                      "Example: 'Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// Configure Database - Using the same database as Omni Care Software
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<PatientPortalDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Configure Options
builder.Services.Configure<AppointmentReminderSettings>(
    builder.Configuration.GetSection(AppointmentReminderSettings.SectionName));
builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(EmailSettings.SectionName));
builder.Services.Configure<PortalSettings>(
    builder.Configuration.GetSection(PortalSettings.SectionName));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Application Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();
builder.Services.AddScoped<IClinicSettingsService, ClinicSettingsService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ITwoFactorService, TwoFactorService>();
builder.Services.AddScoped<IMainDatabaseContext, PatientPortal.Infrastructure.Services.MainDatabaseContext>();

// Register Background Services
builder.Services.AddHostedService<PatientPortal.Infrastructure.Services.AppointmentReminderService>();

// Register Repositories
builder.Services.AddScoped<IPatientUserRepository, PatientUserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<ITwoFactorTokenRepository, TwoFactorTokenRepository>();
builder.Services.AddScoped<IAppointmentViewRepository, AppointmentViewRepository>();
builder.Services.AddScoped<IDocumentViewRepository, DocumentViewRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Portal API v1");
        c.RoutePrefix = string.Empty; // Set Swagger UI at root
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }

