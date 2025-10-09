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
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.GetAge()));

            CreateMap<Address, AddressDto>();

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
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name));

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
        }
    }
}