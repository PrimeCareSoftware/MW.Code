using System;

namespace MedicSoft.Domain.Attributes
{
    /// <summary>
    /// Marks a property for automatic encryption.
    /// This attribute is used to identify sensitive fields that should be encrypted at rest.
    /// </summary>
    /// <remarks>
    /// When applied to a string property, the property will be automatically encrypted
    /// before being stored in the database and decrypted when retrieved.
    /// This helps ensure LGPD compliance for sensitive medical data.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EncryptedAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets whether the field should be searchable using a hash.
        /// When true, a corresponding {PropertyName}Hash field should exist on the entity.
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// Gets or sets the encryption priority level.
        /// Higher priority fields are encrypted first during migration.
        /// Default is Normal.
        /// </summary>
        public EncryptionPriority Priority { get; set; } = EncryptionPriority.Normal;

        /// <summary>
        /// Gets or sets a description of why this field needs encryption.
        /// Useful for documentation and compliance audits.
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Defines the priority level for field encryption.
    /// </summary>
    public enum EncryptionPriority
    {
        /// <summary>
        /// Low priority - less sensitive data
        /// </summary>
        Low = 0,

        /// <summary>
        /// Normal priority - standard sensitive data
        /// </summary>
        Normal = 1,

        /// <summary>
        /// High priority - very sensitive data (medical records, diagnoses, etc.)
        /// </summary>
        High = 2,

        /// <summary>
        /// Critical priority - ultra-sensitive data (must be encrypted first)
        /// </summary>
        Critical = 3
    }
}
