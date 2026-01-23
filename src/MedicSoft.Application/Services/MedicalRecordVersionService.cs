using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class MedicalRecordVersionService : IMedicalRecordVersionService
    {
        private readonly IMedicalRecordVersionRepository _versionRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public MedicalRecordVersionService(
            IMedicalRecordVersionRepository versionRepository,
            IMedicalRecordRepository medicalRecordRepository)
        {
            _versionRepository = versionRepository;
            _medicalRecordRepository = medicalRecordRepository;
        }

        public async Task<MedicalRecordVersion> CreateVersionAsync(
            Guid medicalRecordId, 
            string changeType, 
            Guid userId, 
            string tenantId,
            string? reason = null)
        {
            var record = await _medicalRecordRepository.GetByIdAsync(medicalRecordId, tenantId);
            if (record == null)
                throw new InvalidOperationException($"Medical record {medicalRecordId} not found");

            // Generate snapshot
            var snapshot = SerializeMedicalRecord(record);
            
            // Generate content hash
            var contentHash = await GenerateContentHashAsync(record);
            
            // Get previous version hash for blockchain-like chain
            var latestVersion = await _versionRepository.GetLatestVersionAsync(medicalRecordId, tenantId);
            var previousVersionHash = latestVersion?.ContentHash;
            
            // Generate changes summary if not the first version
            string? changesSummary = null;
            if (latestVersion != null && changeType == "Updated")
            {
                var previousRecord = DeserializeMedicalRecord(latestVersion.SnapshotJson);
                changesSummary = await GenerateChangesSummaryAsync(previousRecord, record);
            }

            var version = new MedicalRecordVersion(
                medicalRecordId: medicalRecordId,
                version: record.CurrentVersion,
                changeType: changeType,
                changedByUserId: userId,
                snapshotJson: snapshot,
                contentHash: contentHash,
                tenantId: tenantId,
                changeReason: reason,
                changesSummary: changesSummary,
                previousVersionHash: previousVersionHash
            );

            return await _versionRepository.CreateAsync(version);
        }

        public async Task<List<MedicalRecordVersion>> GetVersionHistoryAsync(Guid medicalRecordId, string tenantId)
        {
            return await _versionRepository.GetVersionHistoryAsync(medicalRecordId, tenantId);
        }

        public async Task<MedicalRecordVersion?> GetVersionAsync(Guid medicalRecordId, int version, string tenantId)
        {
            return await _versionRepository.GetVersionAsync(medicalRecordId, version, tenantId);
        }

        public Task<string> GenerateContentHashAsync(MedicalRecord record)
        {
            // Serialize the record in a deterministic way
            var json = SerializeMedicalRecord(record);
            
            // Calculate SHA-256 hash
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(json);
            var hash = sha256.ComputeHash(bytes);
            return Task.FromResult(Convert.ToBase64String(hash));
        }

        public Task<string> GenerateChangesSummaryAsync(MedicalRecord? oldState, MedicalRecord newState)
        {
            if (oldState == null)
                return Task.FromResult("Initial version");

            var changes = new List<string>();

            if (oldState.ChiefComplaint != newState.ChiefComplaint)
                changes.Add("Chief complaint updated");

            if (oldState.HistoryOfPresentIllness != newState.HistoryOfPresentIllness)
                changes.Add("History of present illness updated");

            if (oldState.PastMedicalHistory != newState.PastMedicalHistory)
                changes.Add("Past medical history updated");

            if (oldState.FamilyHistory != newState.FamilyHistory)
                changes.Add("Family history updated");

            if (oldState.LifestyleHabits != newState.LifestyleHabits)
                changes.Add("Lifestyle habits updated");

            if (oldState.CurrentMedications != newState.CurrentMedications)
                changes.Add("Current medications updated");

            if (oldState.Diagnosis != newState.Diagnosis)
                changes.Add("Diagnosis updated");

            if (oldState.Prescription != newState.Prescription)
                changes.Add("Prescription updated");

            if (oldState.Notes != newState.Notes)
                changes.Add("Notes updated");

            if (changes.Count == 0)
                return Task.FromResult("No significant changes");

            return Task.FromResult(string.Join(", ", changes));
        }

        private string SerializeMedicalRecord(MedicalRecord record)
        {
            // Create a snapshot object with all relevant fields
            var snapshot = new
            {
                record.Id,
                record.AppointmentId,
                record.PatientId,
                record.ChiefComplaint,
                record.HistoryOfPresentIllness,
                record.PastMedicalHistory,
                record.FamilyHistory,
                record.LifestyleHabits,
                record.CurrentMedications,
                record.Diagnosis,
                record.Prescription,
                record.Notes,
                record.ConsultationStartTime,
                record.ConsultationEndTime,
                record.ConsultationDurationMinutes,
                record.IsClosed,
                record.ClosedAt,
                record.ClosedByUserId,
                record.CurrentVersion,
                record.ReopenedAt,
                record.ReopenedByUserId,
                record.ReopenReason,
                record.CreatedAt,
                record.UpdatedAt
            };

            return JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                WriteIndented = false,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        }

        private MedicalRecord? DeserializeMedicalRecord(string json)
        {
            try
            {
                // This is a simplified deserialization for comparison purposes
                // In a real scenario, you might need a more robust approach
                using var document = JsonDocument.Parse(json);
                // For now, return null as we're mainly using this for change comparison
                // The actual implementation would need to reconstruct the object properly
                return null;
            }
            catch
            {
                return null;
            }
        }
    }
}
