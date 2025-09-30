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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfiguration(new PatientConfiguration());
            modelBuilder.ApplyConfiguration(new ClinicConfiguration());
            modelBuilder.ApplyConfiguration(new AppointmentConfiguration());

            // Global query filters for tenant isolation
            modelBuilder.Entity<Patient>().HasQueryFilter(p => EF.Property<string>(p, "TenantId") == GetTenantId());
            modelBuilder.Entity<Clinic>().HasQueryFilter(c => EF.Property<string>(c, "TenantId") == GetTenantId());
            modelBuilder.Entity<Appointment>().HasQueryFilter(a => EF.Property<string>(a, "TenantId") == GetTenantId());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql();
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