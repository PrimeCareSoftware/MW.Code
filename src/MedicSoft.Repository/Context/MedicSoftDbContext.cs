using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Configurations;
using MedicSoft.Repository.Extensions;

namespace MedicSoft.Repository.Context
{
    public class MedicSoftDbContext : DbContext
    {
        private readonly IConfiguration? _configuration;
        private readonly IDataEncryptionService? _encryptionService;

        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options) : base(options)
        {
            _configuration = null;
            _encryptionService = null;
        }

        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options, IConfiguration? configuration) : base(options)
        {
            _configuration = configuration;
            _encryptionService = null;
        }

        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options, IConfiguration? configuration, IDataEncryptionService? encryptionService) : base(options)
        {
            _configuration = configuration;
            _encryptionService = encryptionService;
        }

        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Clinic> Clinics { get; set; } = null!;
        public DbSet<Company> Companies { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
        public DbSet<BlockedTimeSlot> BlockedTimeSlots { get; set; } = null!;
        public DbSet<RecurringAppointmentPattern> RecurringAppointmentPatterns { get; set; } = null!;
        public DbSet<HealthInsurancePlan> HealthInsurancePlans { get; set; } = null!;
        public DbSet<MedicalRecord> MedicalRecords { get; set; } = null!;
        public DbSet<PatientClinicLink> PatientClinicLinks { get; set; } = null!;
        public DbSet<MedicalRecordTemplate> MedicalRecordTemplates { get; set; } = null!;
        public DbSet<PrescriptionTemplate> PrescriptionTemplates { get; set; } = null!;
        public DbSet<Medication> Medications { get; set; } = null!;
        public DbSet<PrescriptionItem> PrescriptionItems { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<NotificationRoutine> NotificationRoutines { get; set; } = null!;
        public DbSet<SubscriptionPlan> SubscriptionPlans { get; set; } = null!;
        public DbSet<ClinicSubscription> ClinicSubscriptions { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<UserClinicLink> UserClinicLinks { get; set; } = null!;
        public DbSet<Owner> Owners { get; set; } = null!;
        public DbSet<OwnerClinicLink> OwnerClinicLinks { get; set; } = null!;
        public DbSet<ModuleConfiguration> ModuleConfigurations { get; set; } = null!;
        public DbSet<ModuleConfigurationHistory> ModuleConfigurationHistories { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<AppointmentProcedure> AppointmentProcedures { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<ProcedureMaterial> ProcedureMaterials { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;
        public DbSet<ExamRequest> ExamRequests { get; set; } = null!;
        public DbSet<WaitingQueueEntry> WaitingQueueEntries { get; set; } = null!;
        public DbSet<WaitingQueueConfiguration> WaitingQueueConfigurations { get; set; } = null!;
        public DbSet<FilaEspera> FilasEspera { get; set; } = null!;
        public DbSet<SenhaFila> SenhasFila { get; set; } = null!;
        public DbSet<ExamCatalog> ExamCatalogs { get; set; } = null!;
        public DbSet<Notification> Notifications { get; set; } = null!;
        public DbSet<UserSession> UserSessions { get; set; } = null!;
        public DbSet<OwnerSession> OwnerSessions { get; set; } = null!;
        public DbSet<Ticket> Tickets { get; set; } = null!;
        public DbSet<TicketComment> TicketComments { get; set; } = null!;
        public DbSet<TicketAttachment> TicketAttachments { get; set; } = null!;
        public DbSet<TicketHistory> TicketHistory { get; set; } = null!;
        public DbSet<AccessProfile> AccessProfiles { get; set; } = null!;
        public DbSet<ProfilePermission> ProfilePermissions { get; set; } = null!;
        public DbSet<ClinicCustomization> ClinicCustomizations { get; set; } = null!;
        
        // CFM 1.821 - New entities for medical record compliance
        public DbSet<ClinicalExamination> ClinicalExaminations { get; set; } = null!;
        public DbSet<DiagnosticHypothesis> DiagnosticHypotheses { get; set; } = null!;
        public DbSet<TherapeuticPlan> TherapeuticPlans { get; set; } = null!;
        public DbSet<InformedConsent> InformedConsents { get; set; } = null!;
        public DbSet<SoapRecord> SoapRecords { get; set; } = null!;
        
        // CFM 1.638/2002 - Versioning and Audit
        public DbSet<MedicalRecordVersion> MedicalRecordVersions { get; set; } = null!;
        public DbSet<MedicalRecordAccessLog> MedicalRecordAccessLogs { get; set; } = null!;
        public DbSet<MedicalRecordSignature> MedicalRecordSignatures { get; set; } = null!;
        
        // Digital Prescriptions - CFM 1.643/2002 and ANVISA 344/1998
        public DbSet<DigitalPrescription> DigitalPrescriptions { get; set; } = null!;
        public DbSet<DigitalPrescriptionItem> DigitalPrescriptionItems { get; set; } = null!;
        public DbSet<PrescriptionSequenceControl> PrescriptionSequenceControls { get; set; } = null!;
        public DbSet<SNGPCReport> SNGPCReports { get; set; } = null!;
        public DbSet<ControlledMedicationRegistry> ControlledMedicationRegistries { get; set; } = null!;
        public DbSet<MonthlyControlledBalance> MonthlyControlledBalances { get; set; } = null!;
        public DbSet<SngpcTransmission> SngpcTransmissions { get; set; } = null!;
        public DbSet<SngpcAlert> SngpcAlerts { get; set; } = null!;
        
        // Sales Funnel Metrics
        public DbSet<SalesFunnelMetric> SalesFunnelMetrics { get; set; } = null!;
        
        // TISS Phase 1 - Health Insurance and Authorization
        public DbSet<HealthInsuranceOperator> HealthInsuranceOperators { get; set; } = null!;
        public DbSet<PatientHealthInsurance> PatientHealthInsurances { get; set; } = null!;
        public DbSet<AuthorizationRequest> AuthorizationRequests { get; set; } = null!;
        public DbSet<TissBatch> TissBatches { get; set; } = null!;
        public DbSet<TissGuide> TissGuides { get; set; } = null!;
        public DbSet<TissGuideProcedure> TissGuideProcedures { get; set; } = null!;
        public DbSet<TussProcedure> TussProcedures { get; set; } = null!;
        
        // TISS Phase 2 - Webservices and Glosas Management
        public DbSet<TissOperadoraConfig> TissOperadoraConfigs { get; set; } = null!;
        public DbSet<TissGlosa> TissGlosas { get; set; } = null!;
        public DbSet<TissRecursoGlosa> TissRecursosGlosa { get; set; } = null!;
        
        // Financial Module
        public DbSet<AccountsReceivable> AccountsReceivable { get; set; } = null!;
        public DbSet<ReceivablePayment> ReceivablePayments { get; set; } = null!;
        public DbSet<AccountsPayable> AccountsPayable { get; set; } = null!;
        public DbSet<PayablePayment> PayablePayments { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<CashFlowEntry> CashFlowEntries { get; set; } = null!;
        public DbSet<FinancialClosure> FinancialClosures { get; set; } = null!;
        public DbSet<FinancialClosureItem> FinancialClosureItems { get; set; } = null!;
        
        // Configurable Consultation Forms
        public DbSet<ConsultationFormProfile> ConsultationFormProfiles { get; set; } = null!;
        public DbSet<ConsultationFormConfiguration> ConsultationFormConfigurations { get; set; } = null!;
        
        // Electronic Invoices (NF-e/NFS-e)
        public DbSet<ElectronicInvoice> ElectronicInvoices { get; set; } = null!;
        public DbSet<Domain.Entities.InvoiceConfiguration> InvoiceConfigurations { get; set; } = null!;
        
        // LGPD Audit System
        public DbSet<AuditLog> AuditLogs { get; set; } = null!;
        public DbSet<DataProcessingConsent> DataProcessingConsents { get; set; } = null!;
        public DbSet<DataAccessLog> DataAccessLogs { get; set; } = null!;
        public DbSet<DataConsentLog> DataConsentLogs { get; set; } = null!;
        public DbSet<DataDeletionRequest> DataDeletionRequests { get; set; } = null!;
        
        // Anamnesis System
        public DbSet<AnamnesisTemplate> AnamnesisTemplates { get; set; } = null!;
        public DbSet<AnamnesisResponse> AnamnesisResponses { get; set; } = null!;
        
        // Brute Force Protection
        public DbSet<LoginAttempt> LoginAttempts { get; set; } = null!;
        public DbSet<AccountLockout> AccountLockouts { get; set; } = null!;
        
        // Two-Factor Authentication
        public DbSet<TwoFactorAuth> TwoFactorAuths { get; set; } = null!;

        // Analytics - Consolidated Data
        public DbSet<ConsultaDiaria> ConsultasDiarias { get; set; } = null!;
        
        // Digital Signature (ICP-Brasil) - CFM 1.821/2007
        public DbSet<CertificadoDigital> CertificadosDigitais { get; set; } = null!;
        public DbSet<AssinaturaDigital> AssinaturasDigitais { get; set; } = null!;

        // CRM - Customer Relationship Management
        public DbSet<MedicSoft.Domain.Entities.CRM.PatientJourney> PatientJourneys { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.JourneyStage> JourneyStages { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.PatientTouchpoint> PatientTouchpoints { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.MarketingAutomation> MarketingAutomations { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.AutomationAction> AutomationActions { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.Survey> Surveys { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.SurveyQuestion> SurveyQuestions { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.SurveyResponse> SurveyResponses { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.SurveyQuestionResponse> SurveyQuestionResponses { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.Complaint> Complaints { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.ComplaintInteraction> ComplaintInteractions { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.SentimentAnalysis> SentimentAnalyses { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.ChurnPrediction> ChurnPredictions { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.EmailTemplate> EmailTemplates { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.WebhookSubscription> WebhookSubscriptions { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.WebhookDelivery> WebhookDeliveries { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.Lead> Leads { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.CRM.LeadActivity> LeadActivities { get; set; } = null!;

        // Fiscal Management - Tax and Accounting
        public DbSet<MedicSoft.Domain.Entities.Fiscal.ConfiguracaoFiscal> ConfiguracoesFiscais { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.ImpostoNota> ImpostosNotas { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.ApuracaoImpostos> ApuracoesImpostos { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.PlanoContas> PlanoContas { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.LancamentoContabil> LancamentosContabeis { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.DRE> DREs { get; set; } = null!;
        public DbSet<MedicSoft.Domain.Entities.Fiscal.BalancoPatrimonial> BalancosPatrimoniais { get; set; } = null!;

        // System Admin - Notifications
        public DbSet<SystemNotification> SystemNotifications { get; set; } = null!;
        public DbSet<NotificationRule> NotificationRules { get; set; } = null!;
        
        // Alerts System
        public DbSet<Alert> Alerts { get; set; } = null!;
        public DbSet<AlertConfiguration> AlertConfigurations { get; set; } = null!;

        // System Admin - Tags (Phase 2)
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<ClinicTag> ClinicTags { get; set; } = null!;

        // System Admin - Analytics & BI (Phase 3)
        public DbSet<CustomDashboard> CustomDashboards { get; set; } = null!;
        public DbSet<DashboardWidget> DashboardWidgets { get; set; } = null!;
        public DbSet<WidgetTemplate> WidgetTemplates { get; set; } = null!;
        public DbSet<ReportTemplate> ReportTemplates { get; set; } = null!;
        public DbSet<ScheduledReport> ScheduledReports { get; set; } = null!;

        // System Admin - Workflow Automation (Phase 4)
        public DbSet<Workflow> Workflows { get; set; } = null!;
        public DbSet<WorkflowAction> WorkflowActions { get; set; } = null!;
        public DbSet<WorkflowExecution> WorkflowExecutions { get; set; } = null!;
        public DbSet<WorkflowActionExecution> WorkflowActionExecutions { get; set; } = null!;
        public DbSet<SubscriptionCredit> SubscriptionCredits { get; set; } = null!;

        // Encryption & Security
        public DbSet<EncryptionKey> EncryptionKeys { get; set; } = null!;
        
        // Business Configuration
        public DbSet<BusinessConfiguration> BusinessConfigurations { get; set; } = null!;
        public DbSet<DocumentTemplate> DocumentTemplates { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new HealthInsurancePlanConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new PatientClinicLinkConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new MedicationConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionItemConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationRoutineConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserClinicLinkConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerClinicLinkConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleConfigurationHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureMaterialConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
            modelBuilder.ApplyConfiguration(new EmailVerificationTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ExamRequestConfiguration());
            modelBuilder.ApplyConfiguration(new WaitingQueueEntryConfiguration());
            modelBuilder.ApplyConfiguration(new WaitingQueueConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new FilaEsperaConfiguration());
            modelBuilder.ApplyConfiguration(new SenhaFilaConfiguration());
            modelBuilder.ApplyConfiguration(new ExamCatalogConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new UserSessionConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerSessionConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new TicketCommentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketAttachmentConfiguration());
            modelBuilder.ApplyConfiguration(new TicketHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new AccessProfileConfiguration());
            modelBuilder.ApplyConfiguration(new ProfilePermissionConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicCustomizationConfiguration());
            
            // CFM 1.821 - New configurations for medical record compliance
            modelBuilder.ApplyConfiguration(new ClinicalExaminationConfiguration());
            modelBuilder.ApplyConfiguration(new DiagnosticHypothesisConfiguration());
            modelBuilder.ApplyConfiguration(new TherapeuticPlanConfiguration());
            modelBuilder.ApplyConfiguration(new InformedConsentConfiguration());
            modelBuilder.ApplyConfiguration(new SoapRecordConfiguration());
            
            // Digital Prescriptions - CFM 1.643/2002 and ANVISA 344/1998
            modelBuilder.ApplyConfiguration(new DigitalPrescriptionConfiguration());
            modelBuilder.ApplyConfiguration(new DigitalPrescriptionItemConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionSequenceControlConfiguration());
            modelBuilder.ApplyConfiguration(new SNGPCReportConfiguration());
            modelBuilder.ApplyConfiguration(new ControlledMedicationRegistryConfiguration());
            modelBuilder.ApplyConfiguration(new MonthlyControlledBalanceConfiguration());
            modelBuilder.ApplyConfiguration(new SngpcTransmissionConfiguration());
            modelBuilder.ApplyConfiguration(new SngpcAlertConfiguration());
            
            // Sales Funnel Metrics
            modelBuilder.ApplyConfiguration(new SalesFunnelMetricConfiguration());
            
            // TISS Phase 1 - Health Insurance and Authorization
            modelBuilder.ApplyConfiguration(new HealthInsuranceOperatorConfiguration());
            modelBuilder.ApplyConfiguration(new PatientHealthInsuranceConfiguration());
            modelBuilder.ApplyConfiguration(new AuthorizationRequestConfiguration());
            modelBuilder.ApplyConfiguration(new TissBatchConfiguration());
            modelBuilder.ApplyConfiguration(new TissGuideConfiguration());
            modelBuilder.ApplyConfiguration(new TissGuideProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new TussProcedureConfiguration());
            
            // TISS Phase 2 - Webservices and Glosas Management
            modelBuilder.ApplyConfiguration(new TissOperadoraConfigConfiguration());
            modelBuilder.ApplyConfiguration(new TissGlosaConfiguration());
            modelBuilder.ApplyConfiguration(new TissRecursoGlosaConfiguration());
            
            // Financial Module
            modelBuilder.ApplyConfiguration(new AccountsReceivableConfiguration());
            modelBuilder.ApplyConfiguration(new ReceivablePaymentConfiguration());
            modelBuilder.ApplyConfiguration(new AccountsPayableConfiguration());
            modelBuilder.ApplyConfiguration(new PayablePaymentConfiguration());
            modelBuilder.ApplyConfiguration(new SupplierConfiguration());
            modelBuilder.ApplyConfiguration(new CashFlowEntryConfiguration());
            modelBuilder.ApplyConfiguration(new FinancialClosureConfiguration());
            modelBuilder.ApplyConfiguration(new FinancialClosureItemConfiguration());
            
            // Configurable Consultation Forms
            modelBuilder.ApplyConfiguration(new ConsultationFormProfileConfiguration());
            modelBuilder.ApplyConfiguration(new ConsultationFormConfigurationConfiguration());
            
            // Electronic Invoices (NF-e/NFS-e)
            modelBuilder.ApplyConfiguration(new ElectronicInvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfigurationEntityConfiguration());
            
            // LGPD Audit System
            modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
            modelBuilder.ApplyConfiguration(new DataProcessingConsentConfiguration());
            
            // Anamnesis System
            modelBuilder.ApplyConfiguration(new AnamnesisTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new AnamnesisResponseConfiguration());
            
            // Brute Force Protection
            modelBuilder.ApplyConfiguration(new LoginAttemptConfiguration());
            modelBuilder.ApplyConfiguration(new AccountLockoutConfiguration());
            
            // Two-Factor Authentication
            modelBuilder.ApplyConfiguration(new TwoFactorAuthConfiguration());
            
            // Digital Signature (ICP-Brasil) - CFM 1.821/2007
            modelBuilder.ApplyConfiguration(new CertificadoDigitalConfiguration());
            modelBuilder.ApplyConfiguration(new AssinaturaDigitalConfiguration());

            // CRM - Customer Relationship Management
            modelBuilder.ApplyConfiguration(new Configurations.CRM.PatientJourneyConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.JourneyStageConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.PatientTouchpointConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.MarketingAutomationConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.AutomationActionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.SurveyConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.SurveyQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.SurveyResponseConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.SurveyQuestionResponseConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.ComplaintConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.ComplaintInteractionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.SentimentAnalysisConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.ChurnPredictionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.EmailTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.WebhookSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new Configurations.CRM.WebhookDeliveryConfiguration());

            // Fiscal Management - Tax and Accounting
            modelBuilder.ApplyConfiguration(new ConfiguracaoFiscalConfiguration());
            modelBuilder.ApplyConfiguration(new ImpostoNotaConfiguration());
            modelBuilder.ApplyConfiguration(new ApuracaoImpostosConfiguration());
            modelBuilder.ApplyConfiguration(new PlanoContasConfiguration());
            modelBuilder.ApplyConfiguration(new LancamentoContabilConfiguration());
            modelBuilder.ApplyConfiguration(new DREConfiguration());
            modelBuilder.ApplyConfiguration(new BalancoPatrimonialConfiguration());

            // System Admin - Analytics & BI (Phase 3)
            modelBuilder.ApplyConfiguration(new CustomDashboardConfiguration());
            modelBuilder.ApplyConfiguration(new DashboardWidgetConfiguration());
            modelBuilder.ApplyConfiguration(new WidgetTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ReportTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new ScheduledReportConfiguration());

            // System Admin - Tags (Phase 2)
            modelBuilder.ApplyConfiguration(new TagConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicTagConfiguration());

            // System Admin - Workflow Automation (Phase 4)
            modelBuilder.ApplyConfiguration(new WorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowActionConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowExecutionConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowActionExecutionConfiguration());

            // Encryption & Security
            modelBuilder.ApplyConfiguration(new EncryptionKeyConfiguration());
            
            // Business Configuration
            modelBuilder.ApplyConfiguration(new BusinessConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentTemplateConfiguration());

            // NOTE: Global query filters are disabled for now since GetTenantId() returns a hardcoded value.
            // All repositories explicitly filter by tenantId parameter, ensuring proper tenant isolation.
            // To enable global query filters in the future:
            // 1. Inject IHttpContextAccessor into DbContext
            // 2. Read tenantId from HttpContext.Items["TenantId"] in GetTenantId()
            // 3. Uncomment the filters below
            
            // Global query filters for tenant isolation (DISABLED - repositories handle tenant filtering explicitly)
            //modelBuilder.Entity<Patient>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Clinic>().HasQueryFilter(c => EF.Property<string>(c, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Appointment>().HasQueryFilter(a => EF.Property<string>(a, "TenantId") == GetTenantId());
            //modelBuilder.Entity<HealthInsurancePlan>().HasQueryFilter(h => EF.Property<string>(h, "TenantId") == GetTenantId());
            //modelBuilder.Entity<MedicalRecord>().HasQueryFilter(mr => EF.Property<string>(mr, "TenantId") == GetTenantId());
            //modelBuilder.Entity<PatientClinicLink>().HasQueryFilter(l => EF.Property<string>(l, "TenantId") == GetTenantId());
            //modelBuilder.Entity<MedicalRecordTemplate>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            //modelBuilder.Entity<PrescriptionTemplate>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Medication>().HasQueryFilter(m => EF.Property<string>(m, "TenantId") == GetTenantId());
            //modelBuilder.Entity<PrescriptionItem>().HasQueryFilter(pi => EF.Property<string>(pi, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Payment>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Invoice>().HasQueryFilter(i => EF.Property<string>(i, "TenantId") == GetTenantId());
            //modelBuilder.Entity<NotificationRoutine>().HasQueryFilter(nr => EF.Property<string>(nr, "TenantId") == GetTenantId());
            //modelBuilder.Entity<SubscriptionPlan>().HasQueryFilter(sp => EF.Property<string>(sp, "TenantId") == GetTenantId());
            //modelBuilder.Entity<ClinicSubscription>().HasQueryFilter(cs => EF.Property<string>(cs, "TenantId") == GetTenantId());
            //modelBuilder.Entity<User>().HasQueryFilter(u => EF.Property<string>(u, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Owner>().HasQueryFilter(o => EF.Property<string>(o, "TenantId") == GetTenantId());
            //modelBuilder.Entity<OwnerClinicLink>().HasQueryFilter(ocl => EF.Property<string>(ocl, "TenantId") == GetTenantId());
            //modelBuilder.Entity<ModuleConfiguration>().HasQueryFilter(mc => EF.Property<string>(mc, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Expense>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Procedure>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            //modelBuilder.Entity<AppointmentProcedure>().HasQueryFilter(ap => EF.Property<string>(ap, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Material>().HasQueryFilter(m => EF.Property<string>(m, "TenantId") == GetTenantId());
            //modelBuilder.Entity<PasswordResetToken>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            //modelBuilder.Entity<ProcedureMaterial>().HasQueryFilter(pm => EF.Property<string>(pm, "TenantId") == GetTenantId());
            //modelBuilder.Entity<ExamRequest>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetTenantId());
            //modelBuilder.Entity<WaitingQueueEntry>().HasQueryFilter(wqe => EF.Property<string>(wqe, "TenantId") == GetTenantId());
            //modelBuilder.Entity<WaitingQueueConfiguration>().HasQueryFilter(wqc => EF.Property<string>(wqc, "TenantId") == GetTenantId());
            //modelBuilder.Entity<ExamCatalog>().HasQueryFilter(ec => EF.Property<string>(ec, "TenantId") == GetTenantId());
            //modelBuilder.Entity<Notification>().HasQueryFilter(n => EF.Property<string>(n, "TenantId") == GetTenantId());

            // Apply encryption to sensitive medical data fields (LGPD compliance)
            if (_encryptionService != null)
            {
                modelBuilder.ApplyMedicalDataEncryption(_encryptionService);
            }

            // Seed initial data for Analytics & BI (Phase 3)
            Seeders.WidgetTemplateSeeder.Seed(modelBuilder);
            Seeders.ReportTemplateSeeder.Seed(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured && _configuration != null)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                
                if (!string.IsNullOrEmpty(connectionString))
                {
                    // Auto-detect database provider based on connection string
                    if (IsPostgreSQL(connectionString))
                    {
                        ConfigurePostgreSQL(optionsBuilder, connectionString);
                    }
                    else
                    {
                        ConfigureSqlServer(optionsBuilder, connectionString);
                    }
                }
            }
        }

        private bool IsPostgreSQL(string connectionString)
        {
            return connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase) ||
                   connectionString.Contains("postgres", StringComparison.OrdinalIgnoreCase) ||
                   connectionString.Contains("Server=", StringComparison.OrdinalIgnoreCase) && 
                   connectionString.Contains("Port=5432", StringComparison.OrdinalIgnoreCase);
        }

        private void ConfigurePostgreSQL(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseNpgsql(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorCodesToAdd: null);
                
                options.MigrationsHistoryTable("__EFMigrationsHistory", "public");
                options.CommandTimeout(60);
            });
        }

        private void ConfigureSqlServer(DbContextOptionsBuilder optionsBuilder, string connectionString)
        {
            optionsBuilder.UseSqlServer(connectionString, options =>
            {
                options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
                
                options.CommandTimeout(60);
            });
        }

        // This would typically be set through a service or context accessor
        private string GetTenantId()
        {
            // This is a placeholder - in a real implementation, this would come from 
            // a tenant context service or HTTP context
            return "default-tenant";
        }

        public void SetTenantId(string tenantId)
        {
            // Method to set tenant context - would be called by tenant middleware
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            ChangeTracker.Clear();
        }

        public override int SaveChanges()
        {
            NormalizeDateTimeValuesToUtc();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            NormalizeDateTimeValuesToUtc();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            NormalizeDateTimeValuesToUtc();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            NormalizeDateTimeValuesToUtc();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        /// <summary>
        /// Normalizes all DateTime and DateTime? properties to UTC before saving to PostgreSQL.
        /// PostgreSQL's 'timestamp with time zone' columns require DateTime values with Kind=Utc.
        /// This method converts any Unspecified or Local DateTimes to UTC.
        /// </summary>
        private void NormalizeDateTimeValuesToUtc()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList(); // Materialize to avoid collection modified errors

            Console.WriteLine($"[MedicSoftDbContext] NormalizeDateTimeValuesToUtc called. Processing {entries.Count} entities.");

            var diagnostics = new List<string>();

            foreach (var entry in entries)
            {
                Console.WriteLine($"[MedicSoftDbContext] Processing entity: {entry.Entity.GetType().Name}, State: {entry.State}");
                
                foreach (var property in entry.Properties)
                {
                    // Normalize scalar DateTime values
                    if (property.CurrentValue is DateTime dateTime)
                    {
                        if (dateTime.Kind == DateTimeKind.Unspecified)
                        {
                            var message = $"FIXING: Entity={entry.Entity.GetType().Name} Property={property.Metadata.Name} Value={dateTime:o} Kind=Unspecified -> UTC";
                            diagnostics.Add(message);
                            Console.WriteLine($"[MedicSoftDbContext] {message}");
                            property.CurrentValue = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                        }
                        else if (dateTime.Kind == DateTimeKind.Local)
                        {
                            var message = $"FIXING: Entity={entry.Entity.GetType().Name} Property={property.Metadata.Name} Value={dateTime:o} Kind=Local -> UTC";
                            diagnostics.Add(message);
                            Console.WriteLine($"[MedicSoftDbContext] {message}");
                            property.CurrentValue = dateTime.ToUniversalTime();
                        }

                        continue;
                    }

                    // If the property is a complex/owned object or collection, recursively inspect/normalize contained DateTimes
                    if (property.CurrentValue != null)
                    {
                        try
                        {
                            var visited = new HashSet<object>();
                            NormalizeObjectDateTimes(property.CurrentValue, visited, diagnostics, entry.Entity.GetType().Name + "." + property.Metadata.Name);
                        }
                        catch
                        {
                            // Swallow any reflection errors to avoid breaking save; best-effort normalization only
                        }
                    }
                }
            }

            if (diagnostics.Count > 0)
            {
                // Print diagnostics to console to make it easy to find offending values in logs
                Console.Error.WriteLine("[MedicSoftDbContext] DateTime normalization diagnostics (non-UTC values found):");
                foreach (var d in diagnostics.Take(50))
                    Console.Error.WriteLine(d);

                // If there were many, indicate how many were suppressed
                if (diagnostics.Count > 50)
                    Console.Error.WriteLine($"...and {diagnostics.Count - 50} more items.");
            }
        }

        private void NormalizeObjectDateTimes(object? obj, HashSet<object> visited, List<string> diagnostics, string? path = null)
        {
            if (obj == null) return;

            // Avoid cycles
            if (visited.Contains(obj)) return;
            visited.Add(obj);

            // Handle DateTime directly
            if (obj is DateTime dt)
            {
                if (dt.Kind == DateTimeKind.Unspecified)
                    diagnostics.Add($"ObjectPath={path ?? "(root)"} Value={dt:o} Kind=Unspecified (collection/owned element)");
                else if (dt.Kind == DateTimeKind.Local)
                    diagnostics.Add($"ObjectPath={path ?? "(root)"} Value={dt:o} Kind=Local (collection/owned element)");
                // Can't set the value here because caller must assign back; callers handle scalar properties separately
                return;
            }

            // Handle collections (IEnumerable) except string
            if (obj is System.Collections.IEnumerable enumerable && !(obj is string))
            {
                foreach (var item in enumerable)
                {
                    if (item == null) continue;

                    if (item is DateTime d)
                    {
                        if (d.Kind == DateTimeKind.Unspecified)
                            diagnostics.Add($"ObjectPath={path ?? "(root)"} CollectionElement Value={d:o} Kind=Unspecified");
                        else if (d.Kind == DateTimeKind.Local)
                            diagnostics.Add($"ObjectPath={path ?? "(root)"} CollectionElement Value={d:o} Kind=Local");

                        // Try to normalize in-place where possible (arrays and lists)
                        try
                        {
                            if (enumerable is System.Collections.IList list)
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i] is DateTime listDt)
                                    {
                                        if (listDt.Kind == DateTimeKind.Unspecified)
                                            list[i] = DateTime.SpecifyKind(listDt, DateTimeKind.Utc);
                                        else if (listDt.Kind == DateTimeKind.Local)
                                            list[i] = listDt.ToUniversalTime();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // ignore failures normalizing collection elements
                        }

                        continue;
                    }

                    NormalizeObjectDateTimes(item, visited, diagnostics, path == null ? item.GetType().Name : path + "." + item.GetType().Name);
                }

                return;
            }

            // For objects, reflect over writable properties and normalize DateTime and DateTime? values
            var type = obj.GetType();
            var props = type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                .Where(p => p.CanRead && (p.CanWrite || p.GetSetMethod(true) != null));

            foreach (var pi in props)
            {
                try
                {
                    var val = pi.GetValue(obj);
                    if (val == null) continue;

                    if (val is DateTime dtVal)
                    {
                        if (dtVal.Kind == DateTimeKind.Unspecified)
                        {
                            diagnostics.Add($"ObjectPath={path ?? type.Name}.{pi.Name} Value={dtVal:o} Kind=Unspecified");
                            // Try to set via public setter first, then fall back to private setter
                            if (pi.CanWrite)
                            {
                                pi.SetValue(obj, DateTime.SpecifyKind(dtVal, DateTimeKind.Utc));
                            }
                            else
                            {
                                var setter = pi.GetSetMethod(true);
                                setter?.Invoke(obj, new object[] { DateTime.SpecifyKind(dtVal, DateTimeKind.Utc) });
                            }
                        }
                        else if (dtVal.Kind == DateTimeKind.Local)
                        {
                            diagnostics.Add($"ObjectPath={path ?? type.Name}.{pi.Name} Value={dtVal:o} Kind=Local");
                            if (pi.CanWrite)
                            {
                                pi.SetValue(obj, dtVal.ToUniversalTime());
                            }
                            else
                            {
                                var setter = pi.GetSetMethod(true);
                                setter?.Invoke(obj, new object[] { dtVal.ToUniversalTime() });
                            }
                        }
                        continue;
                    }

                    // Nullable<DateTime>
                    var underlying = Nullable.GetUnderlyingType(pi.PropertyType);
                    if (underlying == typeof(DateTime) && val is DateTime nullableDt)
                    {
                        if (nullableDt.Kind == DateTimeKind.Unspecified)
                        {
                            diagnostics.Add($"ObjectPath={path ?? type.Name}.{pi.Name} Value={nullableDt:o} Kind=Unspecified");
                            if (pi.CanWrite)
                            {
                                pi.SetValue(obj, DateTime.SpecifyKind(nullableDt, DateTimeKind.Utc));
                            }
                            else
                            {
                                var setter = pi.GetSetMethod(true);
                                setter?.Invoke(obj, new object[] { DateTime.SpecifyKind(nullableDt, DateTimeKind.Utc) });
                            }
                        }
                        else if (nullableDt.Kind == DateTimeKind.Local)
                        {
                            diagnostics.Add($"ObjectPath={path ?? type.Name}.{pi.Name} Value={nullableDt:o} Kind=Local");
                            if (pi.CanWrite)
                            {
                                pi.SetValue(obj, nullableDt.ToUniversalTime());
                            }
                            else
                            {
                                var setter = pi.GetSetMethod(true);
                                setter?.Invoke(obj, new object[] { nullableDt.ToUniversalTime() });
                            }
                        }
                        continue;
                    }

                    // Recurse into complex properties
                    NormalizeObjectDateTimes(val, visited, diagnostics, path == null ? type.Name + "." + pi.Name : path + "." + pi.Name);
                }
                catch
                {
                    // Ignore reflection errors on individual properties
                }
            }
        }
    }
}