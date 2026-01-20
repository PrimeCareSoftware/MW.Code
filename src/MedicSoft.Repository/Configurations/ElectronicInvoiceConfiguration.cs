using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ElectronicInvoiceConfiguration : IEntityTypeConfiguration<ElectronicInvoice>
    {
        public void Configure(EntityTypeBuilder<ElectronicInvoice> builder)
        {
            builder.ToTable("ElectronicInvoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedNever();

            // Basic Info
            builder.Property(i => i.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(i => i.Number)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.Series)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(i => i.IssueDate)
                .IsRequired();

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<int>();

            // Provider
            builder.Property(i => i.ProviderCnpj)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.ProviderName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.ProviderMunicipalRegistration)
                .HasMaxLength(50);

            builder.Property(i => i.ProviderStateRegistration)
                .HasMaxLength(50);

            builder.Property(i => i.ProviderAddress)
                .HasMaxLength(500);

            builder.Property(i => i.ProviderCity)
                .HasMaxLength(100);

            builder.Property(i => i.ProviderState)
                .HasMaxLength(2);

            builder.Property(i => i.ProviderZipCode)
                .HasMaxLength(10);

            // Client
            builder.Property(i => i.ClientCpfCnpj)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(i => i.ClientName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(i => i.ClientEmail)
                .HasMaxLength(200);

            builder.Property(i => i.ClientPhone)
                .HasMaxLength(20);

            builder.Property(i => i.ClientAddress)
                .HasMaxLength(500);

            builder.Property(i => i.ClientCity)
                .HasMaxLength(100);

            builder.Property(i => i.ClientState)
                .HasMaxLength(2);

            builder.Property(i => i.ClientZipCode)
                .HasMaxLength(10);

            // Service
            builder.Property(i => i.ServiceDescription)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(i => i.ServiceCode)
                .HasMaxLength(20);

            builder.Property(i => i.ItemCode)
                .HasMaxLength(20);

            builder.Property(i => i.ServiceAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            // Taxes
            builder.Property(i => i.IssRate)
                .HasColumnType("decimal(5,2)");

            builder.Property(i => i.IssAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.IssRetained)
                .IsRequired();

            builder.Property(i => i.IrAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.PisAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.CofinsAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.CsllAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.InssAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.TotalTaxes)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.NetAmount)
                .HasColumnType("decimal(18,2)");

            // SEFAZ Response
            builder.Property(i => i.AuthorizationCode)
                .HasMaxLength(50);

            builder.Property(i => i.AccessKey)
                .HasMaxLength(50);

            builder.Property(i => i.VerificationCode)
                .HasMaxLength(50);

            builder.Property(i => i.Protocol)
                .HasMaxLength(50);

            builder.Property(i => i.AuthorizationDate);

            // Documents
            builder.Property(i => i.XmlContent)
                .HasColumnType("text");

            builder.Property(i => i.PdfUrl)
                .HasMaxLength(500);

            builder.Property(i => i.RpsNumber)
                .HasMaxLength(20);

            // Cancellation
            builder.Property(i => i.CancellationDate);

            builder.Property(i => i.CancellationReason)
                .HasMaxLength(500);

            builder.Property(i => i.CancellationProtocol)
                .HasMaxLength(50);

            // Replacement
            builder.Property(i => i.ReplacedInvoiceId);

            builder.Property(i => i.ReplacementInvoiceId);

            // Error
            builder.Property(i => i.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(i => i.ErrorCode)
                .HasMaxLength(50);

            // References
            builder.Property(i => i.PaymentId);

            builder.Property(i => i.AppointmentId);

            // Base Entity
            builder.Property(i => i.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt);

            // Relationships
            builder.HasOne(i => i.Payment)
                .WithMany()
                .HasForeignKey(i => i.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Appointment)
                .WithMany()
                .HasForeignKey(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(i => new { i.TenantId, i.Number, i.Series })
                .IsUnique()
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_Number_Series")
                .HasFilter("\"Number\" <> ''");

            builder.HasIndex(i => i.AccessKey)
                .IsUnique()
                .HasDatabaseName("IX_ElectronicInvoices_AccessKey")
                .HasFilter("\"AccessKey\" IS NOT NULL");

            builder.HasIndex(i => new { i.TenantId, i.Status })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_Status");

            builder.HasIndex(i => new { i.TenantId, i.IssueDate })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_IssueDate");

            builder.HasIndex(i => new { i.TenantId, i.ClientCpfCnpj })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_ClientCpfCnpj");

            builder.HasIndex(i => new { i.TenantId, i.PaymentId })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_PaymentId");

            builder.HasIndex(i => new { i.TenantId, i.AppointmentId })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_AppointmentId");

            builder.HasIndex(i => new { i.TenantId, i.Type })
                .HasDatabaseName("IX_ElectronicInvoices_TenantId_Type");
        }
    }
}
