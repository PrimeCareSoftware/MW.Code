using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using MedicSoft.Telemedicine.Api.Filters;
using MedicSoft.Telemedicine.Application.Interfaces;
using MedicSoft.Telemedicine.Application.Services;
using MedicSoft.Telemedicine.Domain.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.ExternalServices;
using MedicSoft.Telemedicine.Infrastructure.Persistence;
using MedicSoft.Telemedicine.Infrastructure.Repositories;
using MedicSoft.Telemedicine.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MedicSoft Telemedicine API",
        Version = "v1",
        Description = "Microservice for telemedicine video consultations",
        Contact = new OpenApiContact
        {
            Name = "Omni Care Software Team"
        }
    });
    
    // Add tenant header parameter globally
    c.AddSecurityDefinition("TenantId", new OpenApiSecurityScheme
    {
        Description = "Tenant identifier for multi-tenancy",
        Name = "X-Tenant-Id",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "TenantId"
    });
    
    // Add operation filter for file uploads
    c.OperationFilter<FileUploadOperationFilter>();
    
    // Configure Swagger to handle file uploads with [FromForm] parameters
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<TelemedicineDbContext>(options =>
{
    if (connectionString?.Contains("postgres", StringComparison.OrdinalIgnoreCase) == true)
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        // Fallback to in-memory for development
        options.UseInMemoryDatabase("TelemedicineDb");
    }
});

// Register Application Services
builder.Services.AddScoped<ITelemedicineService, TelemedicineService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

// Register Domain Services
builder.Services.AddScoped<ITelemedicineSessionRepository, TelemedicineSessionRepository>();
builder.Services.AddScoped<ITelemedicineConsentRepository, TelemedicineConsentRepository>();
builder.Services.AddScoped<IIdentityVerificationRepository, IdentityVerificationRepository>();
builder.Services.AddScoped<ITelemedicineRecordingRepository, TelemedicineRecordingRepository>();

// Register Video Call Service based on configuration
var videoProvider = builder.Configuration["VideoProvider"] ?? "DailyCo";
if (videoProvider.Equals("Twilio", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddScoped<IVideoCallService, TwilioVideoService>();
}
else
{
    builder.Services.AddHttpClient<IVideoCallService, DailyCoVideoService>();
}

builder.Services.AddHttpClient<ICfmValidationService, CfmValidationService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Telemedicine API V1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
