using Microsoft.EntityFrameworkCore;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Interfaces;
using MedicSoft.Telemedicine.Infrastructure.Persistence;

namespace MedicSoft.Telemedicine.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for telemedicine recordings
/// </summary>
public class TelemedicineRecordingRepository : ITelemedicineRecordingRepository
{
    private readonly TelemedicineDbContext _context;

    public TelemedicineRecordingRepository(TelemedicineDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<TelemedicineRecording?> GetByIdAsync(Guid id, string tenantId)
    {
        return await _context.TelemedicineRecordings
            .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId && !r.IsDeleted);
    }

    public async Task<TelemedicineRecording?> GetBySessionIdAsync(Guid sessionId, string tenantId)
    {
        return await _context.TelemedicineRecordings
            .FirstOrDefaultAsync(r => r.SessionId == sessionId && r.TenantId == tenantId && !r.IsDeleted);
    }

    public async Task<IEnumerable<TelemedicineRecording>> GetByTenantIdAsync(string tenantId, int skip = 0, int take = 50)
    {
        return await _context.TelemedicineRecordings
            .Where(r => r.TenantId == tenantId && !r.IsDeleted)
            .OrderByDescending(r => r.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TelemedicineRecording>> GetAvailableRecordingsAsync(string tenantId, int skip = 0, int take = 50)
    {
        return await _context.TelemedicineRecordings
            .Where(r => r.TenantId == tenantId && r.Status == RecordingStatus.Available && !r.IsDeleted)
            .OrderByDescending(r => r.RecordingCompletedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<IEnumerable<TelemedicineRecording>> GetRecordingsDueForDeletionAsync(string tenantId, int skip = 0, int take = 50)
    {
        var now = DateTime.UtcNow;
        return await _context.TelemedicineRecordings
            .Where(r => r.TenantId == tenantId && r.RetentionUntil <= now && !r.IsDeleted)
            .OrderBy(r => r.RetentionUntil)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<long> GetTotalStorageSizeAsync(string tenantId)
    {
        return await _context.TelemedicineRecordings
            .Where(r => r.TenantId == tenantId && r.Status == RecordingStatus.Available && !r.IsDeleted)
            .SumAsync(r => r.FileSizeBytes);
    }

    public async Task AddAsync(TelemedicineRecording recording)
    {
        await _context.TelemedicineRecordings.AddAsync(recording);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(TelemedicineRecording recording)
    {
        _context.TelemedicineRecordings.Update(recording);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id, string tenantId)
    {
        var recording = await _context.TelemedicineRecordings
            .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);
            
        if (recording != null)
        {
            _context.TelemedicineRecordings.Remove(recording);
            await _context.SaveChangesAsync();
        }
    }
}
