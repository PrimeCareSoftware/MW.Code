using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.SoapRecords;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public class SoapRecordService : ISoapRecordService
    {
        private readonly ISoapRecordRepository _soapRecordRepository;
        private readonly IRepository<Appointment> _appointmentRepository;

        public SoapRecordService(
            ISoapRecordRepository soapRecordRepository,
            IRepository<Appointment> appointmentRepository)
        {
            _soapRecordRepository = soapRecordRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<SoapRecordDto> CreateSoapRecord(Guid appointmentId, string tenantId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, tenantId);
            if (appointment == null)
                throw new InvalidOperationException($"Appointment with ID {appointmentId} not found");

            var existing = await _soapRecordRepository.GetByAppointmentIdAsync(appointmentId, tenantId);
            if (existing != null)
                throw new InvalidOperationException($"SOAP record already exists for appointment {appointmentId}");

            var doctorId = appointment.ProfessionalId ?? Guid.Empty;
            if (doctorId == Guid.Empty)
                throw new InvalidOperationException($"Appointment does not have a professional/doctor assigned");

            var soapRecord = new SoapRecord(appointmentId, appointment.PatientId, doctorId, tenantId);
            
            await _soapRecordRepository.AddAsync(soapRecord);
            await _soapRecordRepository.SaveChangesAsync();

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto> UpdateSubjective(Guid soapId, UpdateSubjectiveDto data, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);

            var subjectiveData = new SubjectiveData(
                data.ChiefComplaint,
                data.HistoryOfPresentIllness,
                data.CurrentSymptoms,
                data.SymptomDuration,
                data.AggravatingFactors,
                data.RelievingFactors,
                data.ReviewOfSystems,
                data.Allergies,
                data.CurrentMedications,
                data.PastMedicalHistory,
                data.FamilyHistory,
                data.SocialHistory
            );

            soapRecord.UpdateSubjective(subjectiveData);
            
            await _soapRecordRepository.UpdateAsync(soapRecord);

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto> UpdateObjective(Guid soapId, UpdateObjectiveDto data, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);

            VitalSigns? vitalSigns = null;
            if (data.VitalSigns != null)
            {
                vitalSigns = new VitalSigns(
                    data.VitalSigns.SystolicBP,
                    data.VitalSigns.DiastolicBP,
                    data.VitalSigns.HeartRate,
                    data.VitalSigns.RespiratoryRate,
                    data.VitalSigns.Temperature,
                    data.VitalSigns.OxygenSaturation,
                    data.VitalSigns.Weight,
                    data.VitalSigns.Height,
                    data.VitalSigns.Pain
                );
            }

            PhysicalExamination? physicalExam = null;
            if (data.PhysicalExam != null)
            {
                physicalExam = new PhysicalExamination(
                    data.PhysicalExam.GeneralAppearance,
                    data.PhysicalExam.Head,
                    data.PhysicalExam.Eyes,
                    data.PhysicalExam.Ears,
                    data.PhysicalExam.Nose,
                    data.PhysicalExam.Throat,
                    data.PhysicalExam.Neck,
                    data.PhysicalExam.Cardiovascular,
                    data.PhysicalExam.Respiratory,
                    data.PhysicalExam.Abdomen,
                    data.PhysicalExam.Musculoskeletal,
                    data.PhysicalExam.Neurological,
                    data.PhysicalExam.Skin,
                    data.PhysicalExam.OtherFindings
                );
            }

            var objectiveData = new ObjectiveData(
                vitalSigns,
                physicalExam,
                data.LabResults,
                data.ImagingResults,
                data.OtherExamResults
            );

            soapRecord.UpdateObjective(objectiveData);
            
            await _soapRecordRepository.UpdateAsync(soapRecord);

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto> UpdateAssessment(Guid soapId, UpdateAssessmentDto data, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);

            var differentialDiagnoses = data.DifferentialDiagnoses?
                .Select(d => new DifferentialDiagnosis(d.Diagnosis, d.Icd10Code, d.Justification, d.Priority))
                .ToList();

            var assessmentData = new AssessmentData(
                data.PrimaryDiagnosis,
                data.PrimaryDiagnosisIcd10,
                differentialDiagnoses,
                data.ClinicalReasoning,
                data.Prognosis,
                data.Evolution
            );

            soapRecord.UpdateAssessment(assessmentData);
            
            await _soapRecordRepository.UpdateAsync(soapRecord);

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto> UpdatePlan(Guid soapId, UpdatePlanDto data, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);

            var prescriptions = data.Prescriptions?
                .Select(p => new SoapPrescription(p.MedicationName, p.Dosage, p.Frequency, p.Duration, p.Instructions))
                .ToList();

            var examRequests = data.ExamRequests?
                .Select(e => new SoapExamRequest(e.ExamName, e.ExamType, e.ClinicalIndication, e.IsUrgent))
                .ToList();

            var procedures = data.Procedures?
                .Select(p => new SoapProcedure(p.ProcedureName, p.Description, p.ScheduledDate))
                .ToList();

            var referrals = data.Referrals?
                .Select(r => new SoapReferral(r.SpecialtyName, r.Reason, r.Priority))
                .ToList();

            var planData = new PlanData(
                prescriptions,
                examRequests,
                procedures,
                referrals,
                data.ReturnInstructions,
                data.NextAppointmentDate,
                data.PatientInstructions,
                data.DietaryRecommendations,
                data.ActivityRestrictions,
                data.WarningSymptoms
            );

            soapRecord.UpdatePlan(planData);
            
            await _soapRecordRepository.UpdateAsync(soapRecord);

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto> CompleteSoapRecord(Guid soapId, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);

            soapRecord.CompleteSoapRecord();
            
            await _soapRecordRepository.UpdateAsync(soapRecord);

            return MapToDto(soapRecord);
        }

        public async Task<SoapRecordDto?> GetBySoapId(Guid soapId, string tenantId)
        {
            var soapRecord = await _soapRecordRepository.GetByIdAsync(soapId, tenantId);
            return soapRecord != null ? MapToDto(soapRecord) : null;
        }

        public async Task<SoapRecordDto?> GetByAppointmentId(Guid appointmentId, string tenantId)
        {
            var soapRecord = await _soapRecordRepository.GetByAppointmentIdAsync(appointmentId, tenantId);
            return soapRecord != null ? MapToDto(soapRecord) : null;
        }

        public async Task<IEnumerable<SoapRecordDto>> GetByPatientId(Guid patientId, string tenantId)
        {
            var soapRecords = await _soapRecordRepository.GetByPatientIdAsync(patientId, tenantId);
            return soapRecords.Select(MapToDto);
        }

        public async Task<IEnumerable<SoapRecordDto>> GetByDoctorId(Guid doctorId, string tenantId)
        {
            var soapRecords = await _soapRecordRepository.GetByDoctorIdAsync(doctorId, tenantId);
            return soapRecords.Select(MapToDto);
        }

        public async Task<SoapRecordValidationDto> ValidateCompleteness(Guid soapId, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);
            var validation = soapRecord.ValidateCompleteness();

            return new SoapRecordValidationDto
            {
                IsValid = validation.IsValid,
                MissingFields = validation.MissingFields,
                Warnings = validation.Warnings,
                HasSubjective = validation.HasSubjective,
                HasObjective = validation.HasObjective,
                HasAssessment = validation.HasAssessment,
                HasPlan = validation.HasPlan
            };
        }

        public async Task UnlockSoapRecord(Guid soapId, string tenantId)
        {
            var soapRecord = await GetSoapRecordEntity(soapId, tenantId);
            soapRecord.Unlock();
            
            await _soapRecordRepository.UpdateAsync(soapRecord);
        }

        private async Task<SoapRecord> GetSoapRecordEntity(Guid soapId, string tenantId)
        {
            var soapRecord = await _soapRecordRepository.GetByIdAsync(soapId, tenantId);
            if (soapRecord == null)
                throw new InvalidOperationException($"SOAP record with ID {soapId} not found");
            return soapRecord;
        }

        private SoapRecordDto MapToDto(SoapRecord soapRecord)
        {
            return new SoapRecordDto
            {
                Id = soapRecord.Id,
                AppointmentId = soapRecord.AppointmentId,
                PatientId = soapRecord.PatientId,
                DoctorId = soapRecord.DoctorId,
                RecordDate = soapRecord.RecordDate,
                Subjective = MapSubjectiveToDto(soapRecord.Subjective),
                Objective = MapObjectiveToDto(soapRecord.Objective),
                Assessment = MapAssessmentToDto(soapRecord.Assessment),
                Plan = MapPlanToDto(soapRecord.Plan),
                IsComplete = soapRecord.IsComplete,
                CompletionDate = soapRecord.CompletionDate,
                IsLocked = soapRecord.IsLocked,
                CreatedAt = soapRecord.CreatedAt,
                UpdatedAt = soapRecord.UpdatedAt
            };
        }

        private SubjectiveDataDto? MapSubjectiveToDto(SubjectiveData? subjective)
        {
            if (subjective == null) return null;

            return new SubjectiveDataDto
            {
                ChiefComplaint = subjective.ChiefComplaint,
                HistoryOfPresentIllness = subjective.HistoryOfPresentIllness,
                CurrentSymptoms = subjective.CurrentSymptoms,
                SymptomDuration = subjective.SymptomDuration,
                AggravatingFactors = subjective.AggravatingFactors,
                RelievingFactors = subjective.RelievingFactors,
                ReviewOfSystems = subjective.ReviewOfSystems,
                Allergies = subjective.Allergies,
                CurrentMedications = subjective.CurrentMedications,
                PastMedicalHistory = subjective.PastMedicalHistory,
                FamilyHistory = subjective.FamilyHistory,
                SocialHistory = subjective.SocialHistory
            };
        }

        private ObjectiveDataDto? MapObjectiveToDto(ObjectiveData? objective)
        {
            if (objective == null) return null;

            return new ObjectiveDataDto
            {
                VitalSigns = MapVitalSignsToDto(objective.VitalSigns),
                PhysicalExam = MapPhysicalExamToDto(objective.PhysicalExam),
                LabResults = objective.LabResults,
                ImagingResults = objective.ImagingResults,
                OtherExamResults = objective.OtherExamResults
            };
        }

        private VitalSignsDto? MapVitalSignsToDto(VitalSigns? vitalSigns)
        {
            if (vitalSigns == null) return null;

            return new VitalSignsDto
            {
                SystolicBP = vitalSigns.SystolicBP,
                DiastolicBP = vitalSigns.DiastolicBP,
                HeartRate = vitalSigns.HeartRate,
                RespiratoryRate = vitalSigns.RespiratoryRate,
                Temperature = vitalSigns.Temperature,
                OxygenSaturation = vitalSigns.OxygenSaturation,
                Weight = vitalSigns.Weight,
                Height = vitalSigns.Height,
                BMI = vitalSigns.BMI,
                Pain = vitalSigns.Pain
            };
        }

        private PhysicalExaminationDto? MapPhysicalExamToDto(PhysicalExamination? physicalExam)
        {
            if (physicalExam == null) return null;

            return new PhysicalExaminationDto
            {
                GeneralAppearance = physicalExam.GeneralAppearance,
                Head = physicalExam.Head,
                Eyes = physicalExam.Eyes,
                Ears = physicalExam.Ears,
                Nose = physicalExam.Nose,
                Throat = physicalExam.Throat,
                Neck = physicalExam.Neck,
                Cardiovascular = physicalExam.Cardiovascular,
                Respiratory = physicalExam.Respiratory,
                Abdomen = physicalExam.Abdomen,
                Musculoskeletal = physicalExam.Musculoskeletal,
                Neurological = physicalExam.Neurological,
                Skin = physicalExam.Skin,
                OtherFindings = physicalExam.OtherFindings
            };
        }

        private AssessmentDataDto? MapAssessmentToDto(AssessmentData? assessment)
        {
            if (assessment == null) return null;

            return new AssessmentDataDto
            {
                PrimaryDiagnosis = assessment.PrimaryDiagnosis,
                PrimaryDiagnosisIcd10 = assessment.PrimaryDiagnosisIcd10,
                DifferentialDiagnoses = assessment.DifferentialDiagnoses?
                    .Select(d => new DifferentialDiagnosisDto
                    {
                        Diagnosis = d.Diagnosis,
                        Icd10Code = d.Icd10Code,
                        Justification = d.Justification,
                        Priority = d.Priority
                    }).ToList(),
                ClinicalReasoning = assessment.ClinicalReasoning,
                Prognosis = assessment.Prognosis,
                Evolution = assessment.Evolution
            };
        }

        private PlanDataDto? MapPlanToDto(PlanData? plan)
        {
            if (plan == null) return null;

            return new PlanDataDto
            {
                Prescriptions = plan.Prescriptions?
                    .Select(p => new SoapPrescriptionDto
                    {
                        MedicationName = p.MedicationName,
                        Dosage = p.Dosage,
                        Frequency = p.Frequency,
                        Duration = p.Duration,
                        Instructions = p.Instructions
                    }).ToList(),
                ExamRequests = plan.ExamRequests?
                    .Select(e => new SoapExamRequestDto
                    {
                        ExamName = e.ExamName,
                        ExamType = e.ExamType,
                        ClinicalIndication = e.ClinicalIndication,
                        IsUrgent = e.IsUrgent
                    }).ToList(),
                Procedures = plan.Procedures?
                    .Select(p => new SoapProcedureDto
                    {
                        ProcedureName = p.ProcedureName,
                        Description = p.Description,
                        ScheduledDate = p.ScheduledDate
                    }).ToList(),
                Referrals = plan.Referrals?
                    .Select(r => new SoapReferralDto
                    {
                        SpecialtyName = r.SpecialtyName,
                        Reason = r.Reason,
                        Priority = r.Priority
                    }).ToList(),
                ReturnInstructions = plan.ReturnInstructions,
                NextAppointmentDate = plan.NextAppointmentDate,
                PatientInstructions = plan.PatientInstructions,
                DietaryRecommendations = plan.DietaryRecommendations,
                ActivityRestrictions = plan.ActivityRestrictions,
                WarningSymptoms = plan.WarningSymptoms
            };
        }
    }
}
