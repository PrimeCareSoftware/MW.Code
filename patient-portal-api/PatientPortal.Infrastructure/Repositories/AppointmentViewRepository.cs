using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

public class AppointmentViewRepository : IAppointmentViewRepository
{
    private readonly PatientPortalDbContext _context;

    public AppointmentViewRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentView?> GetByIdAsync(Guid id, Guid patientId)
    {
        return await _context.AppointmentViews
            .FirstOrDefaultAsync(a => a.Id == id && a.PatientId == patientId);
    }

    public async Task<List<AppointmentView>> GetByPatientIdAsync(Guid patientId, int skip = 0, int take = 50)
    {
        return await _context.AppointmentViews
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<AppointmentView>> GetUpcomingByPatientIdAsync(Guid patientId, int take = 10)
    {
        var now = DateTime.UtcNow;
        return await _context.AppointmentViews
            .Where(a => a.PatientId == patientId && a.AppointmentDate > now)
            .OrderBy(a => a.AppointmentDate)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<AppointmentView>> GetByStatusAsync(Guid patientId, AppointmentStatus status, int skip = 0, int take = 50)
    {
        return await _context.AppointmentViews
            .Where(a => a.PatientId == patientId && a.Status == status)
            .OrderByDescending(a => a.AppointmentDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountByPatientIdAsync(Guid patientId)
    {
        return await _context.AppointmentViews
            .Where(a => a.PatientId == patientId)
            .CountAsync();
    }
}
