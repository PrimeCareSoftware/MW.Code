using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.CrossCutting.Security;

namespace MedicSoft.Repository.Extensions
{
    /// <summary>
    /// Extension methods for configuring encrypted properties in Entity Framework Core.
    /// </summary>
    public static class EncryptionExtensions
    {
        /// <summary>
        /// Configures a property to be encrypted when storing and decrypted when retrieving from database.
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="propertyBuilder">The property builder</param>
        /// <param name="encryptionService">The encryption service to use</param>
        /// <returns>The property builder for chaining</returns>
        public static PropertyBuilder<string?> IsEncrypted<TEntity>(
            this PropertyBuilder<string?> propertyBuilder,
            IDataEncryptionService encryptionService)
            where TEntity : class
        {
            var converter = new Converters.EncryptedStringConverter(encryptionService);
            propertyBuilder.HasConversion(converter);
            return propertyBuilder;
        }

        /// <summary>
        /// Applies encryption to sensitive medical data fields in the model.
        /// This method configures encryption for:
        /// - Patient: MedicalHistory, Allergies
        /// - MedicalRecord: ChiefComplaint, HistoryOfPresentIllness, PastMedicalHistory, FamilyHistory, 
        ///   LifestyleHabits, CurrentMedications, Diagnosis, Prescription, Notes
        /// - DigitalPrescription: Notes
        /// </summary>
        /// <param name="modelBuilder">The model builder</param>
        /// <param name="encryptionService">The encryption service to use</param>
        public static void ApplyMedicalDataEncryption(
            this ModelBuilder modelBuilder,
            IDataEncryptionService encryptionService)
        {
            var converter = new Converters.EncryptedStringConverter(encryptionService);

            // Patient sensitive fields
            modelBuilder.Entity<Domain.Entities.Patient>(entity =>
            {
                entity.Property(e => e.MedicalHistory).HasConversion(converter);
                entity.Property(e => e.Allergies).HasConversion(converter);
            });

            // MedicalRecord sensitive fields
            modelBuilder.Entity<Domain.Entities.MedicalRecord>(entity =>
            {
                entity.Property(e => e.ChiefComplaint).HasConversion(converter);
                entity.Property(e => e.HistoryOfPresentIllness).HasConversion(converter);
                entity.Property(e => e.PastMedicalHistory).HasConversion(converter);
                entity.Property(e => e.FamilyHistory).HasConversion(converter);
                entity.Property(e => e.LifestyleHabits).HasConversion(converter);
                entity.Property(e => e.CurrentMedications).HasConversion(converter);
                entity.Property(e => e.Diagnosis).HasConversion(converter);
                entity.Property(e => e.Prescription).HasConversion(converter);
                entity.Property(e => e.Notes).HasConversion(converter);
            });

            // DigitalPrescription sensitive fields
            modelBuilder.Entity<Domain.Entities.DigitalPrescription>(entity =>
            {
                entity.Property(e => e.Notes).HasConversion(converter);
            });
        }
    }
}
