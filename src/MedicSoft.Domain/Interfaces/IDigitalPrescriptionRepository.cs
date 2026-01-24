using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IDigitalPrescriptionRepository : IRepository<DigitalPrescription>
    {
        /// <summary>
        /// Gets all active prescriptions for a patient.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetByPatientIdAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Gets all prescriptions for a medical record.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);

        /// <summary>
        /// Gets all prescriptions issued by a specific doctor.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetByDoctorIdAsync(Guid doctorId, string tenantId);

        /// <summary>
        /// Gets prescriptions by type.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetByTypeAsync(PrescriptionType type, string tenantId);

        /// <summary>
        /// Gets all prescriptions that require SNGPC reporting.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetRequiringSNGPCReportAsync(string tenantId);

        /// <summary>
        /// Gets prescriptions that haven't been reported to SNGPC yet within a date range.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetUnreportedToSNGPCAsync(DateTime startDate, DateTime endDate, string tenantId);

        /// <summary>
        /// Gets controlled substance prescriptions within a specific period for SNGPC reporting.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetControlledPrescriptionsByPeriodAsync(string tenantId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets prescriptions by verification code (for QR code lookup).
        /// </summary>
        Task<DigitalPrescription?> GetByVerificationCodeAsync(string verificationCode);

        /// <summary>
        /// Gets active (non-expired) prescriptions for a patient.
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetActivePrescriptionsForPatientAsync(Guid patientId, string tenantId);

        /// <summary>
        /// Gets prescriptions that are expiring soon (within specified days).
        /// </summary>
        Task<IEnumerable<DigitalPrescription>> GetExpiringSoonAsync(int days, string tenantId);

        /// <summary>
        /// Gets a prescription by ID with all its items included.
        /// </summary>
        Task<DigitalPrescription?> GetByIdWithItemsAsync(Guid id, string tenantId);

        /// <summary>
        /// Checks if a prescription with the given sequence number exists.
        /// </summary>
        Task<bool> SequenceNumberExistsAsync(string sequenceNumber, string tenantId);
    }
}
