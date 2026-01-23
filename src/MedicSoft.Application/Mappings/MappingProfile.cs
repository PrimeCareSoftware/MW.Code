using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.Value))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.ToString()))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()))
                .ForMember(dest => dest.IsChild, opt => opt.MapFrom(src => src.IsChild()))
                .ForMember(dest => dest.GuardianName, opt => opt.MapFrom(src => src.Guardian != null ? src.Guardian.Name : null));

            CreateMap<Address, AddressDto>();

            // Map AddressDto to Address value object
            CreateMap<AddressDto, Address>()
                .ConstructUsing(src => new Address(
                    src.Street,
                    src.Number,
                    src.Neighborhood,
                    src.City,
                    src.State,
                    src.ZipCode,
                    src.Country,
                    src.Complement
                ));

            CreateMap<CreatePatientDto, Patient>()
                .ConstructUsing((src, context) =>
                {
                    var tenantId = context.Items["TenantId"].ToString()!;
                    var email = new Email(src.Email);
                    var phone = new Phone(src.PhoneCountryCode, src.PhoneNumber);
                    var address = new Address(
                        src.Address.Street,
                        src.Address.Number,
                        src.Address.Neighborhood,
                        src.Address.City,
                        src.Address.State,
                        src.Address.ZipCode,
                        src.Address.Country,
                        src.Address.Complement
                    );

                    return new Patient(
                        src.Name,
                        src.Document,
                        src.DateOfBirth,
                        src.Gender,
                        email,
                        phone,
                        address,
                        tenantId,
                        src.MedicalHistory,
                        src.Allergies
                    );
                });

            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Clinic, ClinicDto>();

            CreateMap<CreateClinicDto, Clinic>()
                .ConstructUsing((src, context) =>
                {
                    var tenantId = context.Items["TenantId"].ToString()!;
                    return new Clinic(
                        src.Name,
                        src.TradeName,
                        src.Document,
                        src.Phone,
                        src.Email,
                        src.Address,
                        src.OpeningTime,
                        src.ClosingTime,
                        tenantId,
                        src.AppointmentDurationMinutes
                    );
                });

            CreateMap<MedicalRecord, MedicalRecordDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.Examinations, opt => opt.MapFrom(src => src.Examinations))
                .ForMember(dest => dest.Diagnoses, opt => opt.MapFrom(src => src.Diagnoses))
                .ForMember(dest => dest.Plans, opt => opt.MapFrom(src => src.Plans))
                .ForMember(dest => dest.Consents, opt => opt.MapFrom(src => src.Consents));

            // CFM 1.821 - Clinical Examination mappings
            CreateMap<ClinicalExamination, ClinicalExaminationDto>();

            // CFM 1.821 - Diagnostic Hypothesis mappings
            CreateMap<DiagnosticHypothesis, DiagnosticHypothesisDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => 
                    src.Type == DiagnosisType.Principal ? DiagnosisTypeDto.Principal : DiagnosisTypeDto.Secondary));

            // CFM 1.821 - Therapeutic Plan mappings
            CreateMap<TherapeuticPlan, TherapeuticPlanDto>();

            // CFM 1.821 - Informed Consent mappings
            CreateMap<InformedConsent, InformedConsentDto>();

            // Payment mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // Invoice mappings
            CreateMap<Invoice, InvoiceDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DaysUntilDue, opt => opt.MapFrom(src => src.DaysUntilDue()))
                .ForMember(dest => dest.DaysOverdue, opt => opt.MapFrom(src => src.DaysOverdue()))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()));

            // NotificationRoutine mappings
            CreateMap<NotificationRoutine, NotificationRoutineDto>()
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.Channel.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.ScheduleType, opt => opt.MapFrom(src => src.ScheduleType.ToString()))
                .ForMember(dest => dest.Scope, opt => opt.MapFrom(src => src.Scope.ToString()));

            // Procedure mappings
            CreateMap<Procedure, ProcedureDto>()
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));

            // AppointmentProcedure mappings
            CreateMap<AppointmentProcedure, AppointmentProcedureDto>()
                .ForMember(dest => dest.ProcedureName, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Name : string.Empty))
                .ForMember(dest => dest.ProcedureCode, opt => opt.MapFrom(src => src.Procedure != null ? src.Procedure.Code : string.Empty));

            // ExamRequest mappings
            CreateMap<ExamRequest, ExamRequestDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : string.Empty));

            // Digital Prescription mappings
            CreateMap<DigitalPrescription, DigitalPrescriptionDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
                .ForMember(dest => dest.DaysUntilExpiration, opt => opt.MapFrom(src => src.DaysUntilExpiration()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid()));

            CreateMap<DigitalPrescriptionItem, DigitalPrescriptionItemDto>()
                .ForMember(dest => dest.ControlledList, opt => opt.MapFrom(src => src.ControlledList.HasValue ? src.ControlledList.Value.ToString() : null));

            // SNGPC Report mappings
            CreateMap<SNGPCReport, SNGPCReportDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.DaysUntilDeadline, opt => opt.MapFrom(src => src.DaysUntilDeadline()))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue()));

            // TISS/TUSS mappings
            CreateMap<HealthInsuranceOperator, HealthInsuranceOperatorDto>()
                .ForMember(dest => dest.IntegrationType, opt => opt.MapFrom(src => src.IntegrationType.ToString()));

            CreateMap<PatientHealthInsurance, PatientHealthInsuranceDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : string.Empty))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.HealthInsurancePlan != null ? src.HealthInsurancePlan.PlanName : string.Empty))
                .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.HealthInsurancePlan != null && src.HealthInsurancePlan.Operator != null ? src.HealthInsurancePlan.Operator.TradeName : string.Empty))
                .ForMember(dest => dest.IsValid, opt => opt.MapFrom(src => src.IsValid(null)));

            CreateMap<AuthorizationRequest, AuthorizationRequestDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient != null ? src.Patient.Name : string.Empty))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.PatientHealthInsurance != null && src.PatientHealthInsurance.HealthInsurancePlan != null ? src.PatientHealthInsurance.HealthInsurancePlan.PlanName : string.Empty))
                .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.PatientHealthInsurance != null && src.PatientHealthInsurance.HealthInsurancePlan != null && src.PatientHealthInsurance.HealthInsurancePlan.Operator != null ? src.PatientHealthInsurance.HealthInsurancePlan.Operator.TradeName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.IsExpired()))
                .ForMember(dest => dest.IsValidForUse, opt => opt.MapFrom(src => src.IsValidForUse()));

            CreateMap<TussProcedure, TussProcedureDto>();

            CreateMap<TissGuideProcedure, TissGuideProcedureDto>();

            CreateMap<TissGuide, TissGuideDto>()
                .ForMember(dest => dest.BatchNumber, opt => opt.MapFrom(src => src.TissBatch != null ? src.TissBatch.BatchNumber : string.Empty))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientHealthInsurance != null && src.PatientHealthInsurance.Patient != null ? src.PatientHealthInsurance.Patient.Name : string.Empty))
                .ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.PatientHealthInsurance != null && src.PatientHealthInsurance.HealthInsurancePlan != null ? src.PatientHealthInsurance.HealthInsurancePlan.PlanName : string.Empty))
                .ForMember(dest => dest.GuideType, opt => opt.MapFrom(src => src.GuideType.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Procedures, opt => opt.MapFrom(src => src.Procedures));

            CreateMap<TissBatch, TissBatchDto>()
                .ForMember(dest => dest.ClinicName, opt => opt.MapFrom(src => src.Clinic != null ? src.Clinic.Name : string.Empty))
                .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.Operator != null ? src.Operator.TradeName : string.Empty))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.GetTotalAmount()))
                .ForMember(dest => dest.GuideCount, opt => opt.MapFrom(src => src.GetGuideCount()))
                .ForMember(dest => dest.Guides, opt => opt.MapFrom(src => src.Guides));

            CreateMap<HealthInsurancePlan, HealthInsurancePlanDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.OperatorName, opt => opt.MapFrom(src => src.Operator != null ? src.Operator.TradeName : null));

            // Electronic Invoice mappings
            CreateMap<ElectronicInvoice, ElectronicInvoiceDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.HasXml, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.XmlContent)));

            CreateMap<ElectronicInvoice, ElectronicInvoiceListDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<InvoiceConfiguration, InvoiceConfigurationDto>()
                .ForMember(dest => dest.Gateway, opt => opt.MapFrom(src => src.Gateway.ToString()))
                .ForMember(dest => dest.HasCertificate, opt => opt.MapFrom(src => src.DigitalCertificate != null && src.DigitalCertificate.Length > 0))
                .ForMember(dest => dest.IsCertificateExpired, opt => opt.MapFrom(src => src.IsCertificateExpired()))
                .ForMember(dest => dest.HasGatewayApiKey, opt => opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.GatewayApiKey)));
            
            // Anamnesis Template mappings
            CreateMap<AnamnesisTemplate, DTOs.Anamnesis.AnamnesisTemplateDto>();
            
            // Anamnesis Response mappings
            CreateMap<AnamnesisResponse, DTOs.Anamnesis.AnamnesisResponseDto>()
                .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.Template != null ? src.Template.Name : string.Empty));
        }
    }
}