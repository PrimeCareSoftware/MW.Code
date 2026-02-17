using System.Text;
using System.Threading.RateLimiting;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using MedicSoft.Api.Filters;
using MedicSoft.Api.JsonConverters;
using MedicSoft.Api.Middleware;
using MedicSoft.Application.Interfaces;
using MedicSoft.Application.Mappings;
using MedicSoft.Application.Services;
using MedicSoft.Application.Services.DigitalSignature;
using MedicSoft.CrossCutting.Extensions;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using MedicSoft.Repository.Context;
using MedicSoft.Repository.Repositories;

// Enable Npgsql legacy timestamp behavior to handle DateTime values with Kind=Unspecified or Local
// This must be set before any Npgsql/EF Core operations
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Configure Serilog before building the application
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .Build())
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProcessId()
    .CreateLogger();

try
{
    Log.Information("Iniciando Omni Care Software API...");
    Log.Information("Configuração de logging Serilog aplicada com sucesso");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for all logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpClient(); // Add HttpClient factory for microservice proxying
builder.Services.AddHttpClient("Telemedicine")
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();
        var allowInvalidSsl = builder.Configuration.GetValue<bool>("Microservices:AllowInvalidSsl");
        if (allowInvalidSsl && builder.Environment.IsDevelopment())
        {
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        return handler;
    });
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
}); // Add SignalR for real-time communication

// Configure Distributed Cache (Redis or Memory)
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "MedicSoft:";
    });
    Log.Information("Redis distributed cache configured");
}
else
{
    // Fallback to in-memory cache for development
    builder.Services.AddDistributedMemoryCache();
    Log.Warning("Using in-memory cache. Configure Redis for production environments.");
}

// Configure Response Caching
builder.Services.AddResponseCaching();

// Configure Response Compression for CRM endpoints
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.Configure<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Optimal;
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Add custom TimeSpan converter to serialize as "HH:mm" format for calendar compatibility
        options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
        // Add custom ProcedureCategory converter to accept both string names and numeric values
        options.JsonSerializerOptions.Converters.Add(new ProcedureCategoryJsonConverter());
        // Add JsonStringEnumConverter to support string-to-enum conversion for all enums
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        // Enable case-insensitive property name matching
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        // Use camelCase for JSON property names to match frontend expectations
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Omni Care Software API",
        Version = "v1",
        Description = "Omni Care Software - Sistema de Gestão para Consultórios Médicos. " +
                      "Esta API fornece endpoints para gestão completa de clínicas, incluindo: " +
                      "módulos configuráveis, gestão de pacientes, agendamentos, prontuários, e muito mais.",
        Contact = new OpenApiContact
        {
            Name = "Omni Care Software",
            Email = "contato@medicwarehouse.com"
        }
    });

    // Include XML comments with error handling
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
        }
        else
        {
            // Log warning if XML documentation file is not found
            // This helps diagnose build configuration issues
            Log.Warning("XML documentation file not found at {XmlPath}. " +
                       "API documentation will not include XML comments. " +
                       "Ensure GenerateDocumentationFile is set to true in the project file.", xmlPath);
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error loading XML comments for Swagger documentation");
    }

    // Configure Swagger to use fully qualified names to avoid schema ID conflicts
    // Fallback to Name if FullName is null to prevent Swagger generation failures
    c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);

    // Configure Swagger to handle IFormFile in multipart/form-data properly
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
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

    // Add operation filter to respect [AllowAnonymous] and [Authorize] attributes
    // This ensures swagger.json is accessible without authentication
    c.OperationFilter<MedicSoft.Api.Filters.AuthorizeCheckOperationFilter>();
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
            
            // Enable sensitive data logging and detailed errors in development
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                
                // Log SQL queries with execution time
                options.LogTo(
                    message => Log.Debug("Database: {Message}", message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }
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
            
            if (builder.Environment.IsDevelopment())
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                
                // Log SQL queries with execution time
                options.LogTo(
                    message => Log.Debug("Database: {Message}", message),
                    new[] { DbLoggerCategory.Database.Command.Name },
                    LogLevel.Information);
            }
        }
    }
});

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey not configured");
var issuer = jwtSettings["Issuer"] ?? "Omni Care Software";
var audience = jwtSettings["Audience"] ?? "Omni Care Software-API";

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
    
    // Configure JWT for SignalR
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            
            return Task.CompletedTask;
        }
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

// Configure MFA Policy
builder.Services.Configure<MedicSoft.Application.Configuration.MfaPolicySettings>(
    builder.Configuration.GetSection("MfaPolicy"));
Log.Information("MFA policy configured from appsettings.json");

// Configure Data Encryption Service for medical data protection (LGPD compliance)
var encryptionKey = builder.Configuration["Security:DataEncryptionKey"];
if (string.IsNullOrEmpty(encryptionKey))
{
    Log.Warning("Data encryption key not configured. Medical data will NOT be encrypted!");
}
else
{
    builder.Services.AddSingleton<IDataEncryptionService>(sp => 
        new DataEncryptionService(encryptionKey));
    Log.Information("Data encryption service configured for medical data protection (LGPD)");
}

// Configure MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
    typeof(MedicSoft.Application.Services.PatientService).Assembly));

// Register repositories
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IClinicRepository, ClinicRepository>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IUserClinicLinkRepository, UserClinicLinkRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IBlockedTimeSlotRepository, BlockedTimeSlotRepository>();
builder.Services.AddScoped<IRecurringAppointmentPatternRepository, RecurringAppointmentPatternRepository>();
builder.Services.AddScoped<IRecurrenceExceptionRepository, RecurrenceExceptionRepository>();
builder.Services.AddScoped<IMedicalRecordRepository, MedicalRecordRepository>();
builder.Services.AddScoped<ISoapRecordRepository, SoapRecordRepository>();
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
builder.Services.AddScoped<IClinicPricingConfigurationRepository, ClinicPricingConfigurationRepository>();
builder.Services.AddScoped<IProcedurePricingConfigurationRepository, ProcedurePricingConfigurationRepository>();
builder.Services.AddScoped<IClinicSubscriptionRepository, ClinicSubscriptionRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
builder.Services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
// Register TISS repositories
builder.Services.AddScoped<ITissGlosaRepository, TissGlosaRepository>();
builder.Services.AddScoped<ITissOperadoraConfigRepository, TissOperadoraConfigRepository>();
builder.Services.AddScoped<ITissRecursoGlosaRepository, TissRecursoGlosaRepository>();
builder.Services.AddScoped<IMedicationRepository, MedicationRepository>();
builder.Services.AddScoped<IPrescriptionItemRepository, PrescriptionItemRepository>();
builder.Services.AddScoped<IPrescriptionTemplateRepository, PrescriptionTemplateRepository>();
builder.Services.AddScoped<IMedicalRecordTemplateRepository, MedicalRecordTemplateRepository>();
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IExamRequestRepository, ExamRequestRepository>();
builder.Services.AddScoped<IWaitingQueueRepository, WaitingQueueRepository>();
builder.Services.AddScoped<IWaitingQueueConfigurationRepository, WaitingQueueConfigurationRepository>();
builder.Services.AddScoped<IFilaEsperaRepository, FilaEsperaRepository>();
builder.Services.AddScoped<ISenhaFilaRepository, SenhaFilaRepository>();
builder.Services.AddScoped<IOwnerClinicLinkRepository, OwnerClinicLinkRepository>();
builder.Services.AddScoped<IExamCatalogRepository, ExamCatalogRepository>();
builder.Services.AddScoped<IUserSessionRepository, UserSessionRepository>();
builder.Services.AddScoped<IOwnerSessionRepository, OwnerSessionRepository>();
builder.Services.AddScoped<ITwoFactorAuthRepository, TwoFactorAuthRepository>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IAccessProfileRepository, AccessProfileRepository>();
builder.Services.AddScoped<IClinicCustomizationRepository, ClinicCustomizationRepository>();
builder.Services.AddScoped<ISalesFunnelMetricRepository, SalesFunnelMetricRepository>();
// Also register as generic IRepository for DI compatibility
builder.Services.AddScoped<IRepository<MedicSoft.Domain.Entities.SalesFunnelMetric>>(sp => sp.GetRequiredService<ISalesFunnelMetricRepository>());
builder.Services.AddScoped<IModuleConfigurationRepository, ModuleConfigurationRepository>();

// System Admin - Notification repositories
builder.Services.AddScoped<ISystemNotificationRepository, SystemNotificationRepository>();
builder.Services.AddScoped<INotificationRuleRepository, NotificationRuleRepository>();

// CFM 1.821 - Register new repositories
builder.Services.AddScoped<IClinicalExaminationRepository, ClinicalExaminationRepository>();
builder.Services.AddScoped<IDiagnosticHypothesisRepository, DiagnosticHypothesisRepository>();
builder.Services.AddScoped<ITherapeuticPlanRepository, TherapeuticPlanRepository>();
builder.Services.AddScoped<IInformedConsentRepository, InformedConsentRepository>();

// CFM 1.638/2002 - Register versioning and audit repositories
builder.Services.AddScoped<IMedicalRecordVersionRepository, MedicalRecordVersionRepository>();
builder.Services.AddScoped<IMedicalRecordAccessLogRepository, MedicalRecordAccessLogRepository>();

// Digital Prescriptions and SNGPC - CFM 1.643/2002 + ANVISA
builder.Services.AddScoped<IDigitalPrescriptionRepository, DigitalPrescriptionRepository>();
builder.Services.AddScoped<IDigitalPrescriptionItemRepository, DigitalPrescriptionItemRepository>();
builder.Services.AddScoped<ISNGPCReportRepository, SNGPCReportRepository>();
builder.Services.AddScoped<IControlledMedicationRegistryRepository, ControlledMedicationRegistryRepository>();
builder.Services.AddScoped<IMonthlyControlledBalanceRepository, MonthlyControlledBalanceRepository>();
builder.Services.AddScoped<ISngpcTransmissionRepository, SngpcTransmissionRepository>();
builder.Services.AddScoped<ISngpcAlertRepository, SngpcAlertRepository>();
builder.Services.AddScoped<IPrescriptionSequenceControlRepository, PrescriptionSequenceControlRepository>();
builder.Services.AddScoped<IPrescriptionPdfService, PrescriptionPdfService>();
builder.Services.AddScoped<ISNGPCXmlGeneratorService, SNGPCXmlGeneratorService>();
builder.Services.AddScoped<IICPBrasilDigitalSignatureService, ICPBrasilDigitalSignatureService>();

// Electronic Invoices (NF-e/NFS-e)
builder.Services.AddScoped<IElectronicInvoiceRepository, ElectronicInvoiceRepository>();
builder.Services.AddScoped<IInvoiceConfigurationRepository, InvoiceConfigurationRepository>();

// TISS/TUSS - Health Insurance Integration
builder.Services.AddScoped<IHealthInsuranceOperatorRepository, HealthInsuranceOperatorRepository>();
builder.Services.AddScoped<IHealthInsurancePlanRepository, HealthInsurancePlanRepository>();
builder.Services.AddScoped<IPatientHealthInsuranceRepository, PatientHealthInsuranceRepository>();
builder.Services.AddScoped<IAuthorizationRequestRepository, AuthorizationRequestRepository>();
builder.Services.AddScoped<ITissGuideRepository, TissGuideRepository>();
builder.Services.AddScoped<ITissBatchRepository, TissBatchRepository>();
builder.Services.AddScoped<ITussProcedureRepository, TussProcedureRepository>();
builder.Services.AddScoped<ITissGuideProcedureRepository, TissGuideProcedureRepository>();

// Financial Module - Repositories
builder.Services.AddScoped<IAccountsReceivableRepository, AccountsReceivableRepository>();
builder.Services.AddScoped<IAccountsPayableRepository, AccountsPayableRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ICashFlowEntryRepository, CashFlowEntryRepository>();
builder.Services.AddScoped<IFinancialClosureRepository, FinancialClosureRepository>();

// Fiscal Management - Tax and Accounting Repositories
builder.Services.AddScoped<IConfiguracaoFiscalRepository, ConfiguracaoFiscalRepository>();
builder.Services.AddScoped<IImpostoNotaRepository, ImpostoNotaRepository>();
builder.Services.AddScoped<IApuracaoImpostosRepository, ApuracaoImpostosRepository>();
builder.Services.AddScoped<IPlanoContasRepository, PlanoContasRepository>();
builder.Services.AddScoped<ILancamentoContabilRepository, LancamentoContabilRepository>();
builder.Services.AddScoped<IDRERepository, DRERepository>();
builder.Services.AddScoped<IBalancoPatrimonialRepository, BalancoPatrimonialRepository>();

// Consultation Form Configuration - Repositories
builder.Services.AddScoped<IConsultationFormProfileRepository, ConsultationFormProfileRepository>();
builder.Services.AddScoped<IConsultationFormConfigurationRepository, ConsultationFormConfigurationRepository>();
builder.Services.AddScoped<IBusinessConfigurationRepository, BusinessConfigurationRepository>();
builder.Services.AddScoped<IDocumentTemplateRepository, DocumentTemplateRepository>();
builder.Services.AddScoped<IGlobalDocumentTemplateRepository, GlobalDocumentTemplateRepository>();
builder.Services.AddScoped<IExternalServiceConfigurationRepository, ExternalServiceConfigurationRepository>();

// Blog System
builder.Services.AddScoped<IBlogPostRepository, BlogPostRepository>();

// Register application services
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<ISoapRecordService, SoapRecordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ITwoFactorAuthService, TwoFactorAuthService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IClinicSelectionService, ClinicSelectionService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IExamRequestService, ExamRequestService>();
builder.Services.AddScoped<IWaitingQueueService, WaitingQueueService>();
builder.Services.AddScoped<IFilaService, FilaService>();
builder.Services.AddScoped<IFilaNotificationService, FilaNotificationService>();
builder.Services.AddScoped<IFilaAnalyticsService, FilaAnalyticsService>();
builder.Services.AddScoped<IOwnerClinicLinkService, OwnerClinicLinkService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IAccessProfileService, AccessProfileService>();
builder.Services.AddSingleton<IInAppNotificationService, InAppNotificationService>();
builder.Services.AddScoped<IAlertService, AlertService>();
builder.Services.AddScoped<DataSeederService>();
builder.Services.AddScoped<ISalesFunnelService, SalesFunnelService>();

// Chat System Services
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IPresenceService, PresenceService>();

// System Admin - Phase 1 Services
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.ISaasMetricsService, MedicSoft.Application.Services.SystemAdmin.SaasMetricsService>();
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.IGlobalSearchService, MedicSoft.Application.Services.SystemAdmin.GlobalSearchService>();
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.ISystemNotificationService, MedicSoft.Api.Services.SystemAdmin.SystemNotificationService>();

// System Admin - Phase 2: Clinic Management
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.IClinicManagementService, MedicSoft.Application.Services.SystemAdmin.ClinicManagementService>();
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.ICrossTenantUserService, MedicSoft.Application.Services.SystemAdmin.CrossTenantUserService>();
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.ITagService, MedicSoft.Application.Services.SystemAdmin.TagService>();
builder.Services.AddSingleton<MedicSoft.Application.Services.SystemAdmin.IMonitoringService, MedicSoft.Application.Services.SystemAdmin.MonitoringService>();

// System Admin - Background Jobs
builder.Services.AddScoped<MedicSoft.Api.Jobs.SystemAdmin.NotificationJobs>();
builder.Services.AddScoped<MedicSoft.Api.Jobs.AlertProcessingJob>();

// CFM 1.821 - Register new services
builder.Services.AddScoped<IClinicalExaminationService, ClinicalExaminationService>();
builder.Services.AddScoped<IDiagnosticHypothesisService, DiagnosticHypothesisService>();
builder.Services.AddScoped<ITherapeuticPlanService, TherapeuticPlanService>();
builder.Services.AddScoped<IInformedConsentService, InformedConsentService>();
builder.Services.AddScoped<ICfm1821ValidationService, Cfm1821ValidationService>();

// CFM 1.638/2002 - Register versioning and audit services
builder.Services.AddScoped<IMedicalRecordVersionService, MedicalRecordVersionService>();
builder.Services.AddScoped<IMedicalRecordAuditService, MedicalRecordAuditService>();

// Consultation Form Configuration - Services
builder.Services.AddScoped<IConsultationFormConfigurationService, ConsultationFormConfigurationService>();
builder.Services.AddScoped<BusinessConfigurationService>();
builder.Services.AddScoped<ExternalServiceConfigurationService>();

// Module Configuration Services
builder.Services.AddScoped<IModuleConfigurationService, ModuleConfigurationService>();
builder.Services.AddScoped<IModuleAnalyticsService, ModuleAnalyticsService>();
builder.Services.AddScoped<IModuleConfigurationValidator, ModuleConfigurationValidator>();

// Digital Prescriptions and SNGPC Services
builder.Services.AddScoped<ISNGPCXmlGeneratorService, SNGPCXmlGeneratorService>();
builder.Services.AddScoped<IICPBrasilDigitalSignatureService, ICPBrasilDigitalSignatureService>();
builder.Services.AddScoped<IControlledMedicationRegistryService, ControlledMedicationRegistryService>();
builder.Services.AddScoped<IMonthlyBalanceService, MonthlyBalanceService>();
builder.Services.AddScoped<ISngpcTransmissionService, SngpcTransmissionService>();
builder.Services.AddHttpClient<IAnvisaSngpcClient, AnvisaSngpcClient>();
builder.Services.AddScoped<ISngpcAlertService, SngpcAlertService>();

// Electronic Invoice Services
builder.Services.AddScoped<IElectronicInvoiceService, ElectronicInvoiceService>();

// Fiscal Management Services - Phase 3
builder.Services.AddScoped<ICalculoImpostosService, MedicSoft.Application.Services.Fiscal.CalculoImpostosService>();
builder.Services.AddScoped<IApuracaoImpostosService, MedicSoft.Application.Services.Fiscal.ApuracaoImpostosService>();

// Fiscal Management Services - Phase 4
builder.Services.AddScoped<IDREService, MedicSoft.Application.Services.Fiscal.DREService>();
builder.Services.AddScoped<IBalancoPatrimonialService, MedicSoft.Application.Services.Fiscal.BalancoPatrimonialService>();

// Fiscal Management Services - Phase 6 (SPED)
builder.Services.AddScoped<ISPEDFiscalService, MedicSoft.Application.Services.Fiscal.SPEDFiscalService>();
builder.Services.AddScoped<ISPEDContabilService, MedicSoft.Application.Services.Fiscal.SPEDContabilService>();

// Payment Flow Orchestration Service
builder.Services.AddScoped<IPaymentFlowService, PaymentFlowService>();

// TISS/TUSS Services
builder.Services.AddScoped<IHealthInsuranceOperatorService, HealthInsuranceOperatorService>();
builder.Services.AddScoped<IHealthInsurancePlanService, HealthInsurancePlanService>();
builder.Services.AddScoped<IPatientHealthInsuranceService, PatientHealthInsuranceService>();
builder.Services.AddScoped<IAuthorizationRequestService, AuthorizationRequestService>();
builder.Services.AddScoped<ITissGuideService, TissGuideService>();
builder.Services.AddScoped<ITissBatchService, TissBatchService>();
builder.Services.AddScoped<ITussProcedureService, TussProcedureService>();
builder.Services.AddScoped<ITissXmlGeneratorService, TissXmlGeneratorService>();
builder.Services.AddScoped<ITissXmlValidatorService, TissXmlValidatorService>();
builder.Services.AddScoped<ITussImportService, TussImportService>();
builder.Services.AddScoped<ITissAnalyticsService, TissAnalyticsService>();

// TISS Phase 2 Services - Glosas, Recursos and Notifications
builder.Services.AddScoped<ITissOperadoraConfigService, TissOperadoraConfigService>();
builder.Services.AddScoped<ITissGlosaService, TissGlosaService>();
builder.Services.AddScoped<ITissRecursoGlosaService, TissRecursoGlosaService>();
builder.Services.AddScoped<ITissNotificationService, TissNotificationService>();

// LGPD Audit System
builder.Services.AddScoped<IAuditService, AuditService>();
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<ISuspiciousActivityDetector, SuspiciousActivityDetector>();
builder.Services.AddScoped<IDataProcessingConsentRepository, DataProcessingConsentRepository>();
builder.Services.AddScoped<IDataAccessLogRepository, DataAccessLogRepository>();
builder.Services.AddScoped<IDataConsentLogRepository, DataConsentLogRepository>();
builder.Services.AddScoped<IDataDeletionRequestRepository, DataDeletionRequestRepository>();
builder.Services.AddScoped<IConsentManagementService, ConsentManagementService>();
builder.Services.AddScoped<IDataDeletionService, DataDeletionService>();
builder.Services.AddScoped<IDataPortabilityService, DataPortabilityService>();

// Audit Retention Background Job
builder.Services.AddScoped<MedicSoft.Api.Jobs.AuditRetentionJob>();

// Digital Signature System (ICP-Brasil)
builder.Services.AddScoped<ICertificateManager, CertificateManager>();
builder.Services.AddScoped<ITimestampService, TimestampService>();
builder.Services.AddScoped<IAssinaturaDigitalService, AssinaturaDigitalService>();
builder.Services.AddScoped<ICertificadoDigitalRepository, CertificadoDigitalRepository>();
builder.Services.AddScoped<IAssinaturaDigitalRepository, AssinaturaDigitalRepository>();

// Register IHttpContextAccessor for Digital Signature Service
builder.Services.AddHttpContextAccessor();

// Anamnesis System
builder.Services.AddScoped<IAnamnesisTemplateRepository, AnamnesisTemplateRepository>();
builder.Services.AddScoped<IAnamnesisResponseRepository, AnamnesisResponseRepository>();

// Register domain services
builder.Services.AddScoped<AppointmentSchedulingService>();
builder.Services.AddScoped<RecurringPatternExpansionService>();
builder.Services.AddScoped<ISubscriptionService>(provider =>
{
    var notificationService = provider.GetRequiredService<MedicSoft.Domain.Services.INotificationService>();
    var environment = builder.Environment.EnvironmentName;
    return new SubscriptionService(notificationService, environment);
});

// Register Analytics services
builder.Services.AddScoped<MedicSoft.Analytics.Services.IConsolidacaoDadosService, MedicSoft.Analytics.Services.ConsolidacaoDadosService>();
builder.Services.AddScoped<MedicSoft.Analytics.Services.IDashboardClinicoService, MedicSoft.Analytics.Services.DashboardClinicoService>();
builder.Services.AddScoped<MedicSoft.Analytics.Services.IDashboardFinanceiroService, MedicSoft.Analytics.Services.DashboardFinanceiroService>();
builder.Services.AddScoped<MedicSoft.Analytics.Jobs.ConsolidacaoDiariaJob>();

// Register ML services
builder.Services.AddSingleton<MedicSoft.ML.Services.IPrevisaoDemandaService, MedicSoft.ML.Services.PrevisaoDemandaService>();
builder.Services.AddSingleton<MedicSoft.ML.Services.IPrevisaoNoShowService, MedicSoft.ML.Services.PrevisaoNoShowService>();

// Configure messaging services (Email, SMS, WhatsApp)
builder.Services.Configure<MedicSoft.Api.Configuration.MessagingConfiguration>(
    builder.Configuration.GetSection(MedicSoft.Api.Configuration.MessagingConfiguration.SectionName));

// CRM Advanced - Email Template Repository
builder.Services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

// CRM Advanced - Phase 2: Marketing Automation
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IMarketingAutomationService, MedicSoft.Api.Services.CRM.MarketingAutomationService>();
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IAutomationEngine, MedicSoft.Api.Services.CRM.AutomationEngine>();

// CRM Advanced - Messaging Services (Real implementations replacing stubs)
// Email Service - Uses SMTP for production (direct email sending), stub for development
// Configure SMTP email settings
builder.Services.Configure<MedicSoft.Application.Services.EmailService.SmtpEmailSettings>(
    builder.Configuration.GetSection(MedicSoft.Application.Services.EmailService.SmtpEmailSettings.SectionName));

var useRealEmailService = builder.Configuration.GetValue<bool>("Email:Enabled", true);
if (useRealEmailService)
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IEmailService, MedicSoft.Application.Services.EmailService.SmtpEmailService>();
}
else
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IEmailService, MedicSoft.Api.Services.CRM.StubEmailService>();
}

// SMS Service - Uses Twilio for production, stub for development
var useRealSmsService = builder.Configuration.GetValue<bool>("Messaging:Sms:Enabled");
if (useRealSmsService)
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.ISmsService, MedicSoft.Api.Services.CRM.TwilioSmsService>();
}
else
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.ISmsService, MedicSoft.Api.Services.CRM.StubSmsService>();
}

// WhatsApp Service - Uses WhatsApp Business API for production, stub for development
var useRealWhatsAppService = builder.Configuration.GetValue<bool>("Messaging:WhatsApp:Enabled");
if (useRealWhatsAppService)
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IWhatsAppService, MedicSoft.Api.Services.CRM.WhatsAppBusinessService>();
}
else
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IWhatsAppService, MedicSoft.Api.Services.CRM.StubWhatsAppService>();
}

// CRM Advanced - Complaint/Ouvidoria System
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IComplaintService, MedicSoft.Api.Services.CRM.ComplaintService>();

// CRM Advanced - Lead Management (Standalone, no Salesforce dependency)
// Register CRM entity repositories for Lead Management
builder.Services.AddScoped<IRepository<MedicSoft.Domain.Entities.CRM.Lead>, LeadRepository>();
builder.Services.AddScoped<IRepository<MedicSoft.Domain.Entities.CRM.LeadActivity>, LeadActivityRepository>();
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.ILeadManagementService, MedicSoft.Api.Services.CRM.LeadManagementService>();
builder.Services.AddHostedService<MedicSoft.Api.Services.CRM.LeadCaptureHostedService>();

// CRM Advanced - Patient Journey
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IPatientJourneyService, MedicSoft.Api.Services.CRM.PatientJourneyService>();

// CRM Advanced - Survey System
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.ISurveyService, MedicSoft.Api.Services.CRM.SurveyService>();

// CRM Advanced - Sentiment Analysis
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.ISentimentAnalysisService, MedicSoft.Api.Services.CRM.SentimentAnalysisService>();

// CRM Advanced - Churn Prediction
builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IChurnPredictionService, MedicSoft.Api.Services.CRM.ChurnPredictionService>();

// Register CRM Background Jobs
builder.Services.AddScoped<MedicSoft.Api.Jobs.CRM.AutomationExecutorJob>();
builder.Services.AddScoped<MedicSoft.Api.Jobs.CRM.SurveyTriggerJob>();
builder.Services.AddScoped<MedicSoft.Api.Jobs.CRM.ChurnPredictionJob>();
builder.Services.AddScoped<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>();

// System Admin - Workflow Automation (Phase 4)
builder.Services.AddScoped<MedicSoft.Application.Services.Workflows.IWorkflowEngine, MedicSoft.Api.Services.Workflows.WorkflowEngine>();
builder.Services.AddScoped<MedicSoft.Application.Services.Workflows.IEventPublisher, MedicSoft.Application.Services.Workflows.EventPublisher>();
// Register Reports.IEmailService adapter that delegates to CRM.IEmailService
builder.Services.AddScoped<MedicSoft.Application.Services.Reports.IEmailService, MedicSoft.Api.Services.Reports.EmailServiceAdapter>();
// Now we can register SmartActionService with its dependencies resolved
builder.Services.AddScoped<MedicSoft.Application.Services.SystemAdmin.ISmartActionService, MedicSoft.Application.Services.SystemAdmin.SmartActionService>();
builder.Services.AddScoped<MedicSoft.Api.Jobs.Workflows.WorkflowJobs>();
builder.Services.AddScoped<MedicSoft.Api.Data.Seeders.WorkflowTemplateSeeder>();

// Configure Hangfire for background jobs
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UsePostgreSqlStorage(options => 
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Add Hangfire server
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 2; // Number of concurrent job processing workers
    options.SchedulePollingInterval = TimeSpan.FromMinutes(1);
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

// Apply EF Core migrations automatically when enabled (recommended for development)
var applyMigrations = app.Configuration.GetValue<bool>("Database:ApplyMigrations");
if (applyMigrations)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var dbConnectionString = app.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        if (dbConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
            dbConnectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"GlobalDocumentTemplates\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Name\" character varying(200) NOT NULL, " +
                "\"Description\" character varying(500) NOT NULL, " +
                "\"Type\" integer NOT NULL, " +
                "\"Specialty\" integer NOT NULL, " +
                "\"Content\" text NOT NULL, " +
                "\"Variables\" text NOT NULL, " +
                "\"IsActive\" boolean NOT NULL DEFAULT true, " +
                "\"CreatedBy\" character varying(100) NOT NULL, " +
                "\"TenantId\" character varying(100) NOT NULL, " +
                "\"CreatedAt\" timestamp with time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp with time zone NULL, " +
                "CONSTRAINT \"PK_GlobalDocumentTemplates\" PRIMARY KEY (\"Id\")" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"CustomDashboards\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Name\" character varying(200) NOT NULL, " +
                "\"Description\" character varying(1000) NULL, " +
                "\"Layout\" text NULL, " +
                "\"IsDefault\" boolean NOT NULL DEFAULT false, " +
                "\"IsPublic\" boolean NOT NULL DEFAULT false, " +
                "\"CreatedBy\" character varying(450) NOT NULL, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp with time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp with time zone NULL, " +
                "CONSTRAINT \"PK_CustomDashboards\" PRIMARY KEY (\"Id\")" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"DashboardWidgets\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"DashboardId\" uuid NOT NULL, " +
                "\"Type\" character varying(50) NOT NULL, " +
                "\"Title\" character varying(200) NOT NULL, " +
                "\"Config\" text NULL, " +
                "\"Query\" text NULL, " +
                "\"RefreshInterval\" integer NOT NULL DEFAULT 0, " +
                "\"GridX\" integer NOT NULL DEFAULT 0, " +
                "\"GridY\" integer NOT NULL DEFAULT 0, " +
                "\"GridWidth\" integer NOT NULL DEFAULT 4, " +
                "\"GridHeight\" integer NOT NULL DEFAULT 3, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp with time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp with time zone NULL, " +
                "CONSTRAINT \"PK_DashboardWidgets\" PRIMARY KEY (\"Id\"), " +
                "CONSTRAINT \"FK_DashboardWidgets_CustomDashboards_DashboardId\" FOREIGN KEY (\"DashboardId\") REFERENCES \"CustomDashboards\" (\"Id\") ON DELETE CASCADE" +
                ");");
        }
        dbContext.Database.Migrate();

        // Defensive repair: ensure recurrence columns exist (some environments have partial migrations)
        if (dbConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
            dbConnectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"SystemNotifications\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Type\" text NOT NULL, " +
                "\"Category\" text NOT NULL, " +
                "\"Title\" text NOT NULL, " +
                "\"Message\" text NOT NULL, " +
                "\"ActionUrl\" text NULL, " +
                "\"ActionLabel\" text NULL, " +
                "\"IsRead\" boolean NOT NULL DEFAULT false, " +
                "\"ReadAt\" timestamp without time zone NULL, " +
                "\"Data\" text NULL, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp without time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp without time zone NULL, " +
                "CONSTRAINT \"PK_SystemNotifications\" PRIMARY KEY (\"Id\")" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"NotificationRules\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Trigger\" text NOT NULL, " +
                "\"IsEnabled\" boolean NOT NULL DEFAULT true, " +
                "\"Conditions\" text NULL, " +
                "\"Actions\" text NULL, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp without time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp without time zone NULL, " +
                "CONSTRAINT \"PK_NotificationRules\" PRIMARY KEY (\"Id\")" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"Users\" ADD COLUMN IF NOT EXISTS \"ProfessionalSpecialty\" integer NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "CREATE INDEX IF NOT EXISTS \"IX_Users_ProfessionalSpecialty\" ON \"Users\" (\"ProfessionalSpecialty\");");

            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"DocumentTemplates\" ADD COLUMN IF NOT EXISTS \"GlobalTemplateId\" uuid NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "CREATE INDEX IF NOT EXISTS \"ix_documenttemplates_globaltemplateid\" ON \"DocumentTemplates\" (\"GlobalTemplateId\");");

            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"RecurringAppointmentPatterns\" ADD COLUMN IF NOT EXISTS \"EffectiveEndDate\" timestamp with time zone NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"RecurringAppointmentPatterns\" ADD COLUMN IF NOT EXISTS \"ParentPatternId\" uuid NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"RecurringAppointmentPatterns\" ADD COLUMN IF NOT EXISTS \"IsActive\" boolean NOT NULL DEFAULT true;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"RecurringAppointmentPatterns\" ALTER COLUMN \"IsActive\" SET DEFAULT true;");
            dbContext.Database.ExecuteSqlRaw(
                "UPDATE \"RecurringAppointmentPatterns\" SET \"IsActive\" = true WHERE \"IsActive\" IS NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"BlockedTimeSlots\" ADD COLUMN IF NOT EXISTS \"RecurringSeriesId\" uuid NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"BlockedTimeSlots\" ADD COLUMN IF NOT EXISTS \"OriginalOccurrenceDate\" timestamp with time zone NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"BlockedTimeSlots\" ADD COLUMN IF NOT EXISTS \"IsException\" boolean NOT NULL DEFAULT false;");
        }

        Log.Information("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Failed to apply database migrations");
        throw;
    }
}

// Defensive repair for partial migrations (runs even when ApplyMigrations is false)
var enableDefensiveRepair = app.Configuration.GetValue<bool?>("Database:EnableDefensiveRepair") ?? true;
if (enableDefensiveRepair)
{
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<MedicSoftDbContext>();
        var dbConnectionString = app.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        if (dbConnectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
            dbConnectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase))
        {
            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"CustomDashboards\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"Name\" character varying(200) NOT NULL, " +
                "\"Description\" character varying(1000) NULL, " +
                "\"Layout\" text NULL, " +
                "\"IsDefault\" boolean NOT NULL DEFAULT false, " +
                "\"IsPublic\" boolean NOT NULL DEFAULT false, " +
                "\"CreatedBy\" character varying(450) NOT NULL, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp with time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp with time zone NULL, " +
                "CONSTRAINT \"PK_CustomDashboards\" PRIMARY KEY (\"Id\")" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "CREATE TABLE IF NOT EXISTS \"DashboardWidgets\" (" +
                "\"Id\" uuid NOT NULL, " +
                "\"DashboardId\" uuid NOT NULL, " +
                "\"Type\" character varying(50) NOT NULL, " +
                "\"Title\" character varying(200) NOT NULL, " +
                "\"Config\" text NULL, " +
                "\"Query\" text NULL, " +
                "\"RefreshInterval\" integer NOT NULL DEFAULT 0, " +
                "\"GridX\" integer NOT NULL DEFAULT 0, " +
                "\"GridY\" integer NOT NULL DEFAULT 0, " +
                "\"GridWidth\" integer NOT NULL DEFAULT 4, " +
                "\"GridHeight\" integer NOT NULL DEFAULT 3, " +
                "\"TenantId\" text NOT NULL DEFAULT '', " +
                "\"CreatedAt\" timestamp with time zone NOT NULL, " +
                "\"UpdatedAt\" timestamp with time zone NULL, " +
                "CONSTRAINT \"PK_DashboardWidgets\" PRIMARY KEY (\"Id\"), " +
                "CONSTRAINT \"FK_DashboardWidgets_CustomDashboards_DashboardId\" FOREIGN KEY (\"DashboardId\") REFERENCES \"CustomDashboards\" (\"Id\") ON DELETE CASCADE" +
                ");");

            dbContext.Database.ExecuteSqlRaw(
                "ALTER TABLE \"DocumentTemplates\" ADD COLUMN IF NOT EXISTS \"GlobalTemplateId\" uuid NULL;");
            dbContext.Database.ExecuteSqlRaw(
                "CREATE INDEX IF NOT EXISTS \"ix_documenttemplates_globaltemplateid\" ON \"DocumentTemplates\" (\"GlobalTemplateId\");");
        }
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to apply defensive database repair");
    }
}

// Configure the HTTP request pipeline
// Enable Swagger - configurable via SwaggerSettings:Enabled (default: true in Development, false in Production)
// Swagger is placed early in the pipeline to bypass authentication/authorization
// This ensures the swagger.json endpoint is accessible without authentication
var enableSwagger = builder.Configuration.GetValue<bool?>("SwaggerSettings:Enabled") 
    ?? app.Environment.IsDevelopment(); // Default to true in Development, false otherwise

if (enableSwagger)
{
    app.UseSwagger(c =>
    {
        // Enable caching to improve Swagger loading performance
        // Swagger JSON will be cached for 24 hours to avoid regeneration on every request
        c.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            httpReq.HttpContext.Response.Headers.Append("Cache-Control", "public, max-age=86400"); // 24 hours
        });
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Omni Care Software API v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
}

if (app.Environment.IsDevelopment())
{
    // Enable static files for local development tools (ONLY in Development)
    app.UseStaticFiles();
}
else
{
    // Enable HSTS in production
    app.UseHsts();
}

// Add global exception handler (should be first to catch all exceptions)
app.UseGlobalExceptionHandler();

// Add Response Compression (before other middleware to compress responses)
app.UseResponseCompression();

// Add Response Caching (after compression, before other middleware)
// This improves performance by caching responses including Swagger JSON
app.UseResponseCaching();

// Add request logging middleware (logs all requests with timing)
if (builder.Configuration.GetValue<bool>("Monitoring:EnableRequestLogging", true))
{
    app.UseRequestLogging();
}

// Add performance monitoring middleware
if (builder.Configuration.GetValue<bool>("Monitoring:EnablePerformanceMonitoring", true))
{
    app.UsePerformanceMonitoring();
}

// Add security headers
app.UseSecurityHeaders();

// Use HTTPS redirection only if required by configuration
var requireHttps = builder.Configuration.GetValue<bool>("Security:RequireHttps", false);
if (requireHttps)
{
    app.UseHttpsRedirection();
}

// Enable routing (required before CORS for proper endpoint matching)
app.UseRouting();

// Use secure CORS policy
app.UseCors("SecurePolicy");

// Add tenant resolution middleware (before authentication)
app.UseTenantResolution();

// Enable authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Hangfire Dashboard (only in Development or with proper authentication)
if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = new[] { new HangfireAuthorizationFilter() },
        DashboardTitle = "Omni Care - Background Jobs"
    });
}

// LGPD Audit Middleware - Logs all sensitive data operations (LGPD Art. 37)
app.UseMiddleware<LgpdAuditMiddleware>();

// Automatic Audit Middleware - Global audit logging for all operations
app.UseMiddleware<AutomaticAuditMiddleware>();

// CFM 1.638/2002 - Add medical record audit middleware (after authentication)
app.UseMiddleware<MedicalRecordAuditMiddleware>();

// MFA Enforcement Middleware - Enforce MFA for administrative roles
app.UseMiddleware<MfaEnforcementMiddleware>();

// Enable rate limiting
if (rateLimitEnabled)
{
    app.UseRateLimiter();
}

app.MapControllers();
app.MapHub<MedicSoft.Api.Hubs.FilaHub>("/hubs/fila");
app.MapHub<MedicSoft.Api.Hubs.SystemNotificationHub>("/hubs/system-notifications");
app.MapHub<MedicSoft.Api.Hubs.AlertHub>("/hubs/alerts");
app.MapHub<MedicSoft.Api.Hubs.ChatHub>("/hubs/chat");

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
    
    try
    {
        // Check for pending migrations before applying
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            Log.Warning("Existem {Count} migrações pendentes que serão aplicadas: {Migrations}", 
                pendingMigrations.Count(), 
                string.Join(", ", pendingMigrations));
            
            Log.Information("Aplicando {Count} migrações pendentes...", pendingMigrations.Count());
            context.Database.Migrate();
            Log.Information("Migrações do banco de dados aplicadas com sucesso");
        }
        else
        {
            Log.Information("Nenhuma migração pendente encontrada. Verificando estado do banco...");
            context.Database.Migrate(); // Ensure we're at the latest version
        }
        
        // Verify critical CRM tables exist to prevent runtime errors
        Log.Information("Verificando existência de tabelas críticas do CRM...");
        var canConnect = await context.Database.CanConnectAsync();
        if (canConnect)
        {
            // Test if critical CRM tables exist by attempting a simple query
            // This will throw if tables don't exist
            var _marketingCheck = await context.MarketingAutomations.AnyAsync();
            var _surveyCheck = await context.SurveyQuestionResponses.AnyAsync();
            Log.Information("Verificação de tabelas CRM concluída com sucesso");
        }
    }
    catch (Npgsql.PostgresException pgEx) when (pgEx.SqlState == "42P01") // Table does not exist
    {
        var tableName = pgEx.TableName ?? "desconhecida";
        Log.Fatal(pgEx, "ERRO CRÍTICO: Tabela '{TableName}' não existe no banco de dados. " +
            "Isso indica que as migrações não foram aplicadas corretamente.", tableName);
        
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine($"❌ ERRO: Tabela '{tableName}' não encontrada no banco de dados");
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("Existem migrações pendentes que precisam ser aplicadas.");
        Console.WriteLine();
        Console.WriteLine("POSSÍVEIS CAUSAS:");
        Console.WriteLine("  1. As migrações foram revertidas manualmente");
        Console.WriteLine("  2. O banco de dados foi recriado sem aplicar as migrações");
        Console.WriteLine("  3. Há um problema de permissões ao criar schemas/tabelas");
        Console.WriteLine("  4. A migração que cria esta tabela não foi aplicada");
        Console.WriteLine();
        Console.WriteLine("SOLUÇÕES:");
        Console.WriteLine("  1. Reinicie a aplicação (migrações são aplicadas automaticamente)");
        Console.WriteLine("  2. Execute manualmente: dotnet ef database update");
        Console.WriteLine("  3. Use o script: ./run-all-migrations.sh");
        Console.WriteLine();
        Console.WriteLine($"Tabela faltando: {tableName}");
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine();
        Console.WriteLine("Para mais informações, consulte: TROUBLESHOOTING_MIGRATIONS.md");
        Console.WriteLine();
        
        throw; // Halt application startup
    }
    catch (Npgsql.PostgresException pgEx)
    {
        // Handle other PostgreSQL errors during migrations (permissions, connectivity, etc.)
        Log.Fatal(pgEx, "Falha ao aplicar migrações do banco de dados (PostgreSQL): {Message}", pgEx.Message);
        Console.WriteLine($"Database migration failed (PostgreSQL error): {pgEx.Message}");
        Console.WriteLine($"SQL State: {pgEx.SqlState}");
        Console.WriteLine("A aplicação não pode iniciar sem as migrações do banco de dados.");
        Console.WriteLine("Por favor, verifique:");
        Console.WriteLine("1. A string de conexão está correta?");
        Console.WriteLine("2. O banco de dados PostgreSQL está rodando?");
        Console.WriteLine("3. O usuário tem permissões para criar schemas e tabelas?");
        Console.WriteLine("4. Execute: dotnet ef database update --project src/MedicSoft.Api");
        throw; // Halt application startup if migrations fail
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados: {Message}", ex.Message);
        Console.WriteLine($"Database migration failed: {ex.Message}");
        Console.WriteLine("A aplicação não pode iniciar sem as migrações do banco de dados.");
        Console.WriteLine("Por favor, verifique:");
        Console.WriteLine("1. A string de conexão está correta?");
        Console.WriteLine("2. O banco de dados PostgreSQL está rodando?");
        Console.WriteLine("3. O usuário tem permissões para criar schemas e tabelas?");
        Console.WriteLine("4. Execute: dotnet ef database update --project src/MedicSoft.Api");
        throw; // Halt application startup if migrations fail
    }
}

// Configure Hangfire recurring jobs
try
{
    Log.Information("Configurando jobs recorrentes do Hangfire...");
    
    // Schedule daily data consolidation job at 00:00 UTC
    RecurringJob.AddOrUpdate<MedicSoft.Analytics.Jobs.ConsolidacaoDiariaJob>(
        "consolidacao-diaria",
        job => job.ExecutarConsolidacaoDiariaAsync(),
        Cron.Daily(0, 0), // Every day at 00:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // CRM Jobs - Marketing Automation
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.AutomationExecutorJob>(
        "crm-automation-executor",
        job => job.ExecuteAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.AutomationExecutorJob>(
        "crm-automation-metrics",
        job => job.UpdateMetricsAsync(),
        Cron.Daily(1, 0), // Daily at 01:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // CRM Jobs - Survey Triggers
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SurveyTriggerJob>(
        "crm-survey-trigger",
        job => job.TriggerSurveysAsync(),
        Cron.Daily(10, 0), // Daily at 10:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SurveyTriggerJob>(
        "crm-survey-process-responses",
        job => job.ProcessSurveyResponsesAsync(),
        Cron.Daily(2, 0), // Daily at 02:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // CRM Jobs - Churn Prediction
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.ChurnPredictionJob>(
        "crm-churn-prediction",
        job => job.PredictChurnAsync(),
        Cron.Weekly(DayOfWeek.Sunday, 3, 0), // Weekly on Sunday at 03:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.ChurnPredictionJob>(
        "crm-churn-high-risk-notification",
        job => job.NotifyHighRiskPatientsAsync(),
        Cron.Daily(8, 0), // Daily at 08:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.ChurnPredictionJob>(
        "crm-churn-recalculate-old",
        job => job.RecalculateOldPredictionsAsync(),
        Cron.Weekly(DayOfWeek.Wednesday, 4, 0), // Weekly on Wednesday at 04:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.ChurnPredictionJob>(
        "crm-churn-retention-analysis",
        job => job.AnalyzeRetentionEffectivenessAsync(),
        Cron.Monthly(1, 5, 0), // Monthly on 1st day at 05:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // Workflow Automation Jobs (Phase 4)
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.Workflows.WorkflowJobs>(
        "workflow-check-subscriptions",
        job => job.CheckSubscriptionExpirationsAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.Workflows.WorkflowJobs>(
        "workflow-check-trials",
        job => job.CheckTrialExpiringAsync(),
        Cron.Daily(9, 0), // Daily at 09:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.Workflows.WorkflowJobs>(
        "workflow-check-inactive",
        job => job.CheckInactiveClientsAsync(),
        Cron.Daily(10, 0), // Daily at 10:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // CRM Jobs - Sentiment Analysis
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>(
        "crm-sentiment-survey-comments",
        job => job.AnalyzeSurveyCommentsAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>(
        "crm-sentiment-complaints",
        job => job.AnalyzeComplaintsAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>(
        "crm-sentiment-interactions",
        job => job.AnalyzeComplaintInteractionsAsync(),
        Cron.Daily(11, 0), // Daily at 11:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>(
        "crm-sentiment-alerts",
        job => job.GenerateNegativeSentimentAlertsAsync(),
        "*/30 * * * *", // Every 30 minutes
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.CRM.SentimentAnalysisJob>(
        "crm-sentiment-trends",
        job => job.AnalyzeSentimentTrendsAsync(),
        Cron.Daily(12, 0), // Daily at 12:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // System Admin Jobs - Notification monitoring
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.SystemAdmin.NotificationJobs>(
        "sysadmin-check-subscription-expirations",
        job => job.CheckSubscriptionExpirationsAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.SystemAdmin.NotificationJobs>(
        "sysadmin-check-trial-expiring",
        job => job.CheckTrialExpiringAsync(),
        Cron.Daily(9, 0), // Daily at 09:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.SystemAdmin.NotificationJobs>(
        "sysadmin-check-inactive-clinics",
        job => job.CheckInactiveClinicsAsync(),
        Cron.Daily(10, 0), // Daily at 10:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.SystemAdmin.NotificationJobs>(
        "sysadmin-check-unresponded-tickets",
        job => job.CheckUnrespondedTicketsAsync(),
        "0 */6 * * *", // Every 6 hours
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // Alert Processing Jobs - Automated alert generation
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-mark-expired",
        job => job.MarkExpiredAlertsAsync(),
        Cron.Hourly(), // Every hour
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-cleanup-old",
        job => job.CleanupOldAlertsAsync(),
        Cron.Daily(3, 0), // Daily at 03:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-check-overdue-appointments",
        job => job.CheckOverdueAppointmentsAsync(),
        "*/15 * * * *", // Every 15 minutes
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-check-overdue-payments",
        job => job.CheckOverduePaymentsAsync(),
        Cron.Daily(8, 0), // Daily at 08:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-check-low-stock",
        job => job.CheckLowStockAsync(),
        Cron.Daily(7, 0), // Daily at 07:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AlertProcessingJob>(
        "alert-check-expiring-subscriptions",
        job => job.CheckExpiringSubscriptionsAsync(),
        Cron.Daily(6, 0), // Daily at 06:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    // LGPD Audit Retention Job - Clean up old audit logs (7 years retention)
    RecurringJob.AddOrUpdate<MedicSoft.Api.Jobs.AuditRetentionJob>(
        "audit-retention-policy",
        job => job.ExecuteAsync(),
        Cron.Daily(2, 0), // Daily at 02:00 UTC
        new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Utc
        });
    
    Log.Information("Jobs recorrentes do Hangfire configurados com sucesso");
}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao configurar jobs recorrentes do Hangfire: {Message}", ex.Message);
}

    Log.Information("Omni Care Software API iniciada com sucesso");
    app.Run();
    Log.Information("Omni Care Software API finalizada com sucesso");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha crítica ao iniciar a aplicação: {Message}", ex.Message);
    throw;
}
finally
{
    Log.CloseAndFlush();
}