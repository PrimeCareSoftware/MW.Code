using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Configurations;

namespace MedicSoft.Repository.Context
{
    public class MedicSoftDbContext : DbContext
    {
        public MedicSoftDbContext(DbContextOptions<MedicSoftDbContext> options) : base(options)
        {
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
        public DbSet<ModuleConfiguration> ModuleConfigurations { get; set; } = null!;
        public DbSet<Expense> Expenses { get; set; } = null!;
        public DbSet<Procedure> Procedures { get; set; } = null!;
        public DbSet<AppointmentProcedure> AppointmentProcedures { get; set; } = null!;
        public DbSet<Material> Materials { get; set; } = null!;
        public DbSet<ProcedureMaterial> ProcedureMaterials { get; set; } = null!;
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;

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
            modelBuilder.ApplyConfiguration(new ModuleConfigurationConfiguration());
            modelBuilder.ApplyConfiguration(new ExpenseConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentProcedureConfiguration());
            modelBuilder.ApplyConfiguration(new MaterialConfiguration());
            modelBuilder.ApplyConfiguration(new ProcedureMaterialConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordResetTokenConfiguration());

            // Global query filters for tenant isolation
            modelBuilder.Entity<Patient>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            modelBuilder.Entity<Clinic>().HasQueryFilter(c => EF.Property<string>(c, "TenantId") == GetTenantId());
            modelBuilder.Entity<Appointment>().HasQueryFilter(a => EF.Property<string>(a, "TenantId") == GetTenantId());
            modelBuilder.Entity<HealthInsurancePlan>().HasQueryFilter(h => EF.Property<string>(h, "TenantId") == GetTenantId());
            modelBuilder.Entity<MedicalRecord>().HasQueryFilter(mr => EF.Property<string>(mr, "TenantId") == GetTenantId());
            modelBuilder.Entity<PatientClinicLink>().HasQueryFilter(l => EF.Property<string>(l, "TenantId") == GetTenantId());
            modelBuilder.Entity<MedicalRecordTemplate>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            modelBuilder.Entity<PrescriptionTemplate>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            modelBuilder.Entity<Medication>().HasQueryFilter(m => EF.Property<string>(m, "TenantId") == GetTenantId());
            modelBuilder.Entity<PrescriptionItem>().HasQueryFilter(pi => EF.Property<string>(pi, "TenantId") == GetTenantId());
            modelBuilder.Entity<Payment>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            modelBuilder.Entity<Invoice>().HasQueryFilter(i => EF.Property<string>(i, "TenantId") == GetTenantId());
            modelBuilder.Entity<NotificationRoutine>().HasQueryFilter(nr => EF.Property<string>(nr, "TenantId") == GetTenantId());
            modelBuilder.Entity<SubscriptionPlan>().HasQueryFilter(sp => EF.Property<string>(sp, "TenantId") == GetTenantId());
            modelBuilder.Entity<ClinicSubscription>().HasQueryFilter(cs => EF.Property<string>(cs, "TenantId") == GetTenantId());
            modelBuilder.Entity<User>().HasQueryFilter(u => EF.Property<string>(u, "TenantId") == GetTenantId());
            modelBuilder.Entity<Owner>().HasQueryFilter(o => EF.Property<string>(o, "TenantId") == GetTenantId());
            modelBuilder.Entity<ModuleConfiguration>().HasQueryFilter(mc => EF.Property<string>(mc, "TenantId") == GetTenantId());
            modelBuilder.Entity<Expense>().HasQueryFilter(e => EF.Property<string>(e, "TenantId") == GetTenantId());
            modelBuilder.Entity<Procedure>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            modelBuilder.Entity<AppointmentProcedure>().HasQueryFilter(ap => EF.Property<string>(ap, "TenantId") == GetTenantId());
            modelBuilder.Entity<Material>().HasQueryFilter(m => EF.Property<string>(m, "TenantId") == GetTenantId());
            modelBuilder.Entity<PasswordResetToken>().HasQueryFilter(t => EF.Property<string>(t, "TenantId") == GetTenantId());
            modelBuilder.Entity<ProcedureMaterial>().HasQueryFilter(pm => EF.Property<string>(pm, "TenantId") == GetTenantId());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
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
    }
}