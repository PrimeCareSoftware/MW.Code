using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Configurations;

namespace MedicSoft.Repository.Context
{
    public class MedicSoftDbContext : DbContext
    {
        private readonly IConfiguration? _configuration;

        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options) : base(options)
        {
            _configuration = null;
        }

        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options, IConfiguration? configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Patient> Patients { get; set; } = null!;
        public DbSet<Clinic> Clinics { get; set; } = null!;
        public DbSet<Appointment> Appointments { get; set; } = null!;
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
        public DbSet<Owner> Owners { get; set; } = null!;
        public DbSet<OwnerClinicLink> OwnerClinicLinks { get; set; } = null!;
        public DbSet<ModuleConfiguration> ModuleConfigurations { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<AppointmentProcedure> AppointmentProcedures { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<ProcedureMaterial> ProcedureMaterials { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
        public DbSet<ExamRequest> ExamRequests { get; set; } = null!;
        public DbSet<WaitingQueueEntry> WaitingQueueEntries { get; set; } = null!;
        public DbSet<WaitingQueueConfiguration> WaitingQueueConfigurations { get; set; } = null!;
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
        
        // Digital Prescriptions - CFM 1.643/2002 and ANVISA 344/1998
        public DbSet<DigitalPrescription> DigitalPrescriptions { get; set; } = null!;
        public DbSet<DigitalPrescriptionItem> DigitalPrescriptionItems { get; set; } = null!;
        public DbSet<PrescriptionSequenceControl> PrescriptionSequenceControls { get; set; } = null!;
        public DbSet<SNGPCReport> SNGPCReports { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());
            modelBuilder.ApplyConfiguration(new HealthInsurancePlanConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordConfiguration());
            modelBuilder.ApplyConfiguration(new PatientClinicLinkConfiguration());
            modelBuilder.ApplyConfiguration(new MedicalRecordTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionTemplateConfiguration());
            modelBuilder.ApplyConfiguration(new MedicationConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionItemConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());
            modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationRoutineConfiguration());
            modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicSubscriptionConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerConfiguration());
            modelBuilder.ApplyConfiguration(new OwnerClinicLinkConfiguration());
            modelBuilder.ApplyConfiguration(new ModuleConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureMaterialConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());
            modelBuilder.ApplyConfiguration(new ExamRequestConfiguration());
            modelBuilder.ApplyConfiguration(new WaitingQueueEntryConfiguration());
            modelBuilder.ApplyConfiguration(new WaitingQueueConfigurationConfiguration());
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
            
            // Digital Prescriptions - CFM 1.643/2002 and ANVISA 344/1998
            modelBuilder.ApplyConfiguration(new DigitalPrescriptionConfiguration());
            modelBuilder.ApplyConfiguration(new DigitalPrescriptionItemConfiguration());
            modelBuilder.ApplyConfiguration(new PrescriptionSequenceControlConfiguration());
            modelBuilder.ApplyConfiguration(new SNGPCReportConfiguration());

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