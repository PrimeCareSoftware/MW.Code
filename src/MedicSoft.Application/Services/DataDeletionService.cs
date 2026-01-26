using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public class DataDeletionService : IDataDeletionService
    {
        private readonly IDataDeletionRequestRepository _deletionRequestRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly IAuditService _auditService;
        private readonly ILogger<DataDeletionService> _logger;

        public DataDeletionService(
            IDataDeletionRequestRepository deletionRequestRepository,
            IPatientRepository patientRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            IDigitalPrescriptionRepository prescriptionRepository,
            IAuditService auditService,
            ILogger<DataDeletionService> logger)
        {
            _deletionRequestRepository = deletionRequestRepository ?? throw new ArgumentNullException(nameof(deletionRequestRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _medicalRecordRepository = medicalRecordRepository ?? throw new ArgumentNullException(nameof(medicalRecordRepository));
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> RequestDataDeletionAsync(
            Guid patientId,
            string patientName,
            string patientEmail,
            string reason,
            DeletionRequestType requestType,
            string ipAddress,
            string userAgent,
            bool requiresLegalApproval,
            string tenantId)
        {
            _logger.LogInformation("Creating data deletion request for patient {PatientId}, type: {Type}",
                patientId, requestType);

            var request = new DataDeletionRequest(
                patientId,
                patientName,
                patientEmail,
                reason,
                requestType,
                ipAddress,
                userAgent,
                requiresLegalApproval,
                tenantId
            );

            await _deletionRequestRepository.AddAsync(request);

            _logger.LogInformation("Data deletion request created with ID {RequestId}", request.Id);

            return request.Id;
        }

        public async Task ProcessDataDeletionRequestAsync(
            Guid requestId,
            string userId,
            string userName,
            string notes,
            string tenantId)
        {
            _logger.LogInformation("Processing data deletion request {RequestId}", requestId);

            var request = await _deletionRequestRepository.GetByIdAsync(requestId, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Data deletion request {requestId} not found");
            }

            request.Process(userId, userName, notes);
            await _deletionRequestRepository.UpdateAsync(request);

            _logger.LogInformation("Data deletion request {RequestId} is now being processed", requestId);
        }

        public async Task CompleteDataDeletionRequestAsync(Guid requestId, string tenantId)
        {
            _logger.LogInformation("Completing data deletion request {RequestId}", requestId);

            var request = await _deletionRequestRepository.GetByIdAsync(requestId, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Data deletion request {requestId} not found");
            }

            // Execute the actual deletion/anonymization based on request type
            if (request.RequestType == DeletionRequestType.Anonymization || 
                request.RequestType == DeletionRequestType.Complete)
            {
                await AnonymizePatientDataAsync(request.PatientId, tenantId);
            }

            request.Complete();
            await _deletionRequestRepository.UpdateAsync(request);

            _logger.LogInformation("Data deletion request {RequestId} completed", requestId);
        }

        public async Task RejectDataDeletionRequestAsync(Guid requestId, string reason, string tenantId)
        {
            _logger.LogInformation("Rejecting data deletion request {RequestId}", requestId);

            var request = await _deletionRequestRepository.GetByIdAsync(requestId, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Data deletion request {requestId} not found");
            }

            request.Reject(reason);
            await _deletionRequestRepository.UpdateAsync(request);

            _logger.LogInformation("Data deletion request {RequestId} rejected", requestId);
        }

        public async Task ApproveLegalAsync(Guid requestId, string approver, string tenantId)
        {
            _logger.LogInformation("Approving legal for data deletion request {RequestId}", requestId);

            var request = await _deletionRequestRepository.GetByIdAsync(requestId, tenantId);
            if (request == null)
            {
                throw new InvalidOperationException($"Data deletion request {requestId} not found");
            }

            request.ApproveLegal(approver);
            await _deletionRequestRepository.UpdateAsync(request);

            _logger.LogInformation("Data deletion request {RequestId} legally approved", requestId);
        }

        public async Task<List<DataDeletionRequest>> GetPendingRequestsAsync(string tenantId)
        {
            return await _deletionRequestRepository.GetPendingRequestsAsync(tenantId);
        }

        public async Task<List<DataDeletionRequest>> GetPatientRequestsAsync(Guid patientId, string tenantId)
        {
            return await _deletionRequestRepository.GetByPatientIdAsync(patientId, tenantId);
        }

        public async Task AnonymizePatientDataAsync(Guid patientId, string tenantId)
        {
            _logger.LogInformation("Starting anonymization process for patient {PatientId}", patientId);

            try
            {
                // Get patient
                var patient = await _patientRepository.GetByIdAsync(patientId, tenantId);
                if (patient == null)
                {
                    throw new InvalidOperationException($"Patient {patientId} not found");
                }

                var anonymizedId = Guid.NewGuid();

                // IMPORTANT: According to CFM Resolution 1.821/2007, medical records must be kept for 20 years
                // Therefore, we anonymize personal data but keep clinical data for legal compliance

                _logger.LogInformation("Anonymizing personal data for patient {PatientId}", patientId);
                
                // Anonymize patient - replace with anonymized data while keeping the entity structure
                // Note: This requires implementing AnonymizeData method on Patient entity
                // For now, we'll update via the repository directly with anonymized values
                
                // The actual anonymization would be done by calling UpdatePersonalInfo with anonymized data
                // patient.UpdatePersonalInfo(...) with anonymized values
                
                // Since the Patient entity has value objects (Email, Phone, Address), 
                // we need to create anonymized versions of these
                var anonymizedEmail = new Email($"anonymized.{anonymizedId:N}@example.com");
                var anonymizedPhone = new Phone("+55", "00000000000");
                var anonymizedAddress = new Address(
                    "Rua Anonimizada",
                    "0000",
                    "Bairro Anonimizado",
                    "Cidade",
                    "XX",
                    "00000000",
                    "Brasil"
                );

                // Update with anonymized data
                patient.UpdatePersonalInfo(
                    $"Paciente Anonimizado {anonymizedId:N}",
                    anonymizedEmail,
                    anonymizedPhone,
                    anonymizedAddress
                );

                // Note: Document, DateOfBirth, Gender are kept for statistical purposes (CFM 1.821/2007)
                // Medical History and Allergies should be kept for clinical research

                await _patientRepository.UpdateAsync(patient);

                // Anonymize references in medical records (keep clinical data, anonymize personal references)
                // Medical records should keep diagnosis and treatment but remove personal notes
                _logger.LogInformation("Medical records kept for legal compliance (CFM 1.821/2007 - 20 years retention)");

                // Log the anonymization for audit purposes
                _logger.LogInformation("Patient {PatientId} data anonymized successfully. Clinical data retained per CFM 1.821/2007", 
                    patientId);

                // Note: Actual implementation should also mark the patient as "anonymized" in a flag
                // This would require adding a boolean IsAnonymized property to the Patient entity
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error anonymizing patient {PatientId} data", patientId);
                throw;
            }
        }

        private string GenerateAnonymizedCpf()
        {
            // Generate a random but valid-format CPF for anonymization
            var random = new Random();
            var cpf = new List<int>();
            
            // Generate 9 random digits
            for (int i = 0; i < 9; i++)
            {
                cpf.Add(random.Next(0, 10));
            }
            
            // Calculate first verification digit
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += cpf[i] * (10 - i);
            }
            int digit1 = (sum * 10) % 11;
            if (digit1 == 10) digit1 = 0;
            cpf.Add(digit1);
            
            // Calculate second verification digit
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += cpf[i] * (11 - i);
            }
            int digit2 = (sum * 10) % 11;
            if (digit2 == 10) digit2 = 0;
            cpf.Add(digit2);
            
            return string.Join("", cpf);
        }
    }
}
