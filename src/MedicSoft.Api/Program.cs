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
    Log.Information("Iniciando PrimeCare Software API...");
    Log.Information("Configuração de logging Serilog aplicada com sucesso");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for all logging
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpClient(); // Add HttpClient factory for microservice proxying
builder.Services.AddSignalR(); // Add SignalR for real-time communication
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Add custom TimeSpan converter to serialize as "HH:mm" format for calendar compatibility
        options.JsonSerializerOptions.Converters.Add(new TimeSpanJsonConverter());
        // Add custom ProcedureCategory converter to accept both string names and numeric values
        options.JsonSerializerOptions.Converters.Add(new ProcedureCategoryJsonConverter());
    });
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "PrimeCare Software API",
        Version = "v1",
        Description = "PrimeCare Software - Sistema de Gestão para Consultórios Médicos",
        Contact = new OpenApiContact
        {
            Name = "PrimeCare Software",
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
var issuer = jwtSettings["Issuer"] ?? "PrimeCare Software";
var audience = jwtSettings["Audience"] ?? "PrimeCare Software-API";

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
builder.Services.AddScoped<IClinicSubscriptionRepository, ClinicSubscriptionRepository>();
builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
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
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IAccessProfileRepository, AccessProfileRepository>();
builder.Services.AddScoped<IClinicCustomizationRepository, ClinicCustomizationRepository>();
builder.Services.AddScoped<ISalesFunnelMetricRepository, SalesFunnelMetricRepository>();
builder.Services.AddScoped<IModuleConfigurationRepository, ModuleConfigurationRepository>();

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

// Register application services
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IMedicalRecordService, MedicalRecordService>();
builder.Services.AddScoped<ISoapRecordService, SoapRecordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOwnerService, OwnerService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
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
builder.Services.AddScoped<DataSeederService>();
builder.Services.AddScoped<ISalesFunnelService, SalesFunnelService>();

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
builder.Services.AddScoped<IDataProcessingConsentRepository, DataProcessingConsentRepository>();
builder.Services.AddScoped<IDataAccessLogRepository, DataAccessLogRepository>();
builder.Services.AddScoped<IDataConsentLogRepository, DataConsentLogRepository>();
builder.Services.AddScoped<IDataDeletionRequestRepository, DataDeletionRequestRepository>();
builder.Services.AddScoped<IConsentManagementService, ConsentManagementService>();
builder.Services.AddScoped<IDataDeletionService, DataDeletionService>();
builder.Services.AddScoped<IDataPortabilityService, DataPortabilityService>();

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
builder.Services.AddScoped<ISubscriptionService>(provider =>
{
    var notificationService = provider.GetRequiredService<INotificationService>();
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
// Email Service - Uses SendGrid for production, stub for development
var useRealEmailService = builder.Configuration.GetValue<bool>("Messaging:Email:Enabled");
if (useRealEmailService)
{
    builder.Services.AddScoped<MedicSoft.Application.Services.CRM.IEmailService, MedicSoft.Api.Services.CRM.SendGridEmailService>();
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PrimeCare Software API v1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at /swagger
    });
    
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
        DashboardTitle = "PrimeCare - Background Jobs"
    });
}

// LGPD Audit Middleware - Logs all sensitive data operations (LGPD Art. 37)
app.UseMiddleware<LgpdAuditMiddleware>();

// CFM 1.638/2002 - Add medical record audit middleware (after authentication)
app.UseMiddleware<MedicalRecordAuditMiddleware>();

// Enable rate limiting
if (rateLimitEnabled)
{
    app.UseRateLimiter();
}

app.MapControllers();
app.MapHub<MedicSoft.Api.Hubs.FilaHub>("/hubs/fila");

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
        Log.Information("Aplicando migrações do banco de dados...");
        context.Database.Migrate();
        Log.Information("Migrações do banco de dados aplicadas com sucesso");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados: {Message}", ex.Message);
        Console.WriteLine($"Database migration failed: {ex.Message}");
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
    
    Log.Information("Jobs recorrentes do Hangfire configurados com sucesso");
}
catch (Exception ex)
{
    Log.Error(ex, "Erro ao configurar jobs recorrentes do Hangfire: {Message}", ex.Message);
}

    Log.Information("PrimeCare Software API iniciada com sucesso");
    app.Run();
    Log.Information("PrimeCare Software API finalizada com sucesso");
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