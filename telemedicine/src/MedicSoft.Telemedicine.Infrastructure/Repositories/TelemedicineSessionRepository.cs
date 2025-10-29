using Microsoft.EntityFrameworkCore;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;
using MedicSoft.Telemedicine.Domain.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.Persistence;

namespace MedicSoft.Telemedicine.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of telemedicine session repository
/// </summary>
public class TelemedicineSessionRepository : ITelemedicineSessionRepository
{
    private readonly TelemedicineDbContext _context;

    public TelemedicineSessionRepository(TelemedicineDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TelemedicineSession?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);
    }

    public async Task<TelemedicineSession?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
    {
        return await _context.Sessions
            .FirstOrDefaultAsync(s => s.AppointmentId == appointmentId && s.TenantId == tenantId);
    }

    public async Task<IEnumerable<TelemedicineSession>> GetByClinicIdAsync(Guid clinicId, string tenantId, int skip = 0, int take = 50)
    {
        return await _context.Sessions
            .Where(s => s.ClinicId == clinicId && s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TelemedicineSession>> GetByProviderIdAsync(Guid providerId, string tenantId, int skip = 0, int take = 50)
    {
        return await _context.Sessions
            .Where(s => s.ProviderId == providerId && s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TelemedicineSession>> GetByPatientIdAsync(Guid patientId, string tenantId, int skip = 0, int take = 50)
    {
        return await _context.Sessions
            .Where(s => s.PatientId == patientId && s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TelemedicineSession>> GetByStatusAsync(SessionStatus status, string tenantId, int skip = 0, int take = 50)
    {
        return await _context.Sessions
            .Where(s => s.Status == status && s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<TelemedicineSession> AddAsync(TelemedicineSession session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task UpdateAsync(TelemedicineSession session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id, string tenantId)
    {
        return await _context.Sessions
            .AnyAsync(s => s.Id == id && s.TenantId == tenantId);
    }
}
