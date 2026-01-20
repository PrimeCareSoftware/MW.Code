using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Telemedicine.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for telemedicine consent management
/// </summary>
public class TelemedicineConsentRepository : ITelemedicineConsentRepository
{
    private readonly TelemedicineDbContext _context;

    public TelemedicineConsentRepository(TelemedicineDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TelemedicineConsent> AddAsync(TelemedicineConsent consent)
    {
        if (consent == null)
            throw new ArgumentNullException(nameof(consent));
            
        await _context.Consents.AddAsync(consent);
        await _context.SaveChangesAsync();
        return consent;
    }

    public async Task UpdateAsync(TelemedicineConsent consent)
    {
        if (consent == null)
            throw new ArgumentNullException(nameof(consent));
            
        _context.Consents.Update(consent);
        await _context.SaveChangesAsync();
    }

    public async Task<TelemedicineConsent?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.Consents
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
    }

    public async Task<IEnumerable<TelemedicineConsent>> GetByPatientIdAsync(Guid patientId, string tenantId, bool activeOnly = true)
    {
        var query = _context.Consents
            .Where(c => c.PatientId == patientId && c.TenantId == tenantId);
            
        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }
        
        return await query
            .OrderByDescending(c => c.ConsentDate)
            .ToListAsync();
    }

    public async Task<TelemedicineConsent?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
    {
        return await _context.Consents
            .FirstOrDefaultAsync(c => c.AppointmentId == appointmentId && c.TenantId == tenantId);
    }

    public async Task<bool> HasValidConsentAsync(Guid patientId, string tenantId)
    {
        return await _context.Consents
            .AnyAsync(c => c.PatientId == patientId 
                        && c.TenantId == tenantId 
                        && c.IsActive
                        && c.ConsentDate <= DateTime.UtcNow);
    }

    public async Task<TelemedicineConsent?> GetMostRecentConsentAsync(Guid patientId, string tenantId)
    {
        return await _context.Consents
            .Where(c => c.PatientId == patientId && c.TenantId == tenantId)
            .OrderByDescending(c => c.ConsentDate)
            .FirstOrDefaultAsync();
    }
}
