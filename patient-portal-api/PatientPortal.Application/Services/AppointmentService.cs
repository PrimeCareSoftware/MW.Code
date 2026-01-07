using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentViewRepository _appointmentViewRepository;
    private readonly IPatientUserRepository _patientUserRepository;

    public AppointmentService(
        IAppointmentViewRepository appointmentViewRepository,
        IPatientUserRepository patientUserRepository)
    {
        _appointmentViewRepository = appointmentViewRepository;
        _patientUserRepository = patientUserRepository;
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid appointmentId, Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return null;

        var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
        
        return appointment == null ? null : MapToDto(appointment);
    }

    public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid patientUserId, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetByPatientIdAsync(patientUser.PatientId, skip, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(Guid patientUserId, int take = 10)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetUpcomingByPatientIdAsync(patientUser.PatientId, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<List<AppointmentDto>> GetByStatusAsync(Guid patientUserId, AppointmentStatus status, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetByStatusAsync(patientUser.PatientId, status, skip, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<int> GetCountAsync(Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return 0;

        return await _appointmentViewRepository.GetCountByPatientIdAsync(patientUser.PatientId);
    }

    private static AppointmentDto MapToDto(Domain.Entities.AppointmentView appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            DoctorName = appointment.DoctorName,
            DoctorSpecialty = appointment.DoctorSpecialty,
            ClinicName = appointment.ClinicName,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status.ToString(),
            AppointmentType = appointment.AppointmentType,
            IsTelehealth = appointment.IsTelehealth,
            TelehealthLink = appointment.TelehealthLink,
            Notes = appointment.Notes,
            CanReschedule = appointment.CanReschedule,
            CanCancel = appointment.CanCancel
        };
    }
}
