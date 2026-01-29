using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;

namespace PatientPortal.Infrastructure.Data;

/// <summary>
/// Database context for Patient Portal
/// Uses the same database as PrimeCare Software main application
/// </summary>
public class PatientPortalDbContext : DbContext
{
    public PatientPortalDbContext(DbContextOptions<PatientPortalDbContext> options) 
        : base(options)
    {
    }

    // Patient Portal specific tables
    public DbSet<PatientUser> PatientUsers { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; } = null!;
    public DbSet<PasswordResetToken> PasswordResetTokens { get; set; } = null!;
    public DbSet<TwoFactorToken> TwoFactorTokens { get; set; } = null!;

    // Read-only views from main application
    public DbSet<AppointmentView> AppointmentViews { get; set; } = null!;
    public DbSet<DocumentView> DocumentViews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure PatientUser entity
        modelBuilder.Entity<PatientUser>(entity =>
        {
            entity.ToTable("PatientUsers");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(e => e.CPF)
                .IsRequired()
                .HasMaxLength(11);

            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(e => e.TwoFactorSecret)
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.UpdatedAt);

            entity.Property(e => e.LastLoginAt);

            // Indexes for performance
            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.CPF)
                .IsUnique();

            entity.HasIndex(e => new { e.ClinicId, e.PatientId })
                .IsUnique();
        });

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.RevokedAt);

            entity.Property(e => e.ReplacedByToken)
                .HasMaxLength(512);

            // Indexes for performance
            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.HasIndex(e => new { e.PatientUserId, e.ExpiresAt });
        });

        // Configure EmailVerificationToken entity
        modelBuilder.Entity<EmailVerificationToken>(entity =>
        {
            entity.ToTable("EmailVerificationTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.UsedAt);

            // Add foreign key relationship with cascade delete
            entity.HasOne<PatientUser>()
                .WithMany()
                .HasForeignKey(e => e.PatientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.HasIndex(e => new { e.PatientUserId, e.ExpiresAt });
        });

        // Configure PasswordResetToken entity
        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.ToTable("PasswordResetTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(512);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.UsedAt);

            entity.Property(e => e.CreatedByIp)
                .HasMaxLength(50);

            // Add foreign key relationship with cascade delete
            entity.HasOne<PatientUser>()
                .WithMany()
                .HasForeignKey(e => e.PatientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.HasIndex(e => new { e.PatientUserId, e.ExpiresAt });
        });

        // Configure TwoFactorToken entity
        modelBuilder.Entity<TwoFactorToken>(entity =>
        {
            entity.ToTable("TwoFactorTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(10);

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.UsedAt);

            entity.Property(e => e.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            // Add foreign key relationship with cascade delete
            entity.HasOne<PatientUser>()
                .WithMany()
                .HasForeignKey(e => e.PatientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            entity.HasIndex(e => new { e.Code, e.PatientUserId });
            entity.HasIndex(e => new { e.PatientUserId, e.ExpiresAt });
            entity.HasIndex(e => e.CreatedAt);
        });

        // Configure AppointmentView as a view (read-only)
        modelBuilder.Entity<AppointmentView>(entity =>
        {
            entity.ToView("vw_PatientAppointments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.AppointmentDate)
                .IsRequired();

            entity.Property(e => e.DoctorName)
                .HasMaxLength(200);

            entity.Property(e => e.DoctorSpecialty)
                .HasMaxLength(100);

            entity.Property(e => e.ClinicName)
                .HasMaxLength(200);

            entity.Property(e => e.Notes)
                .HasMaxLength(1000);
        });

        // Configure DocumentView as a view (read-only)
        modelBuilder.Entity<DocumentView>(entity =>
        {
            entity.ToView("vw_PatientDocuments");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.IssuedDate)
                .IsRequired();

            entity.Property(e => e.DoctorName)
                .HasMaxLength(200);

            entity.Property(e => e.FileUrl)
                .HasMaxLength(500);
        });
    }
}
