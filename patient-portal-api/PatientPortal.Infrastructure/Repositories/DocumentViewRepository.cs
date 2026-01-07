using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

public class DocumentViewRepository : IDocumentViewRepository
{
    private readonly PatientPortalDbContext _context;

    public DocumentViewRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<DocumentView?> GetByIdAsync(Guid id, Guid patientId)
    {
        return await _context.DocumentViews
            .FirstOrDefaultAsync(d => d.Id == id && d.PatientId == patientId);
    }

    public async Task<List<DocumentView>> GetByPatientIdAsync(Guid patientId, int skip = 0, int take = 50)
    {
        return await _context.DocumentViews
            .Where(d => d.PatientId == patientId)
            .OrderByDescending(d => d.IssuedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<DocumentView>> GetByTypeAsync(Guid patientId, DocumentType documentType, int skip = 0, int take = 50)
    {
        return await _context.DocumentViews
            .Where(d => d.PatientId == patientId && d.DocumentType == documentType)
            .OrderByDescending(d => d.IssuedDate)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
    }

    public async Task<List<DocumentView>> GetRecentByPatientIdAsync(Guid patientId, int take = 10)
    {
        return await _context.DocumentViews
            .Where(d => d.PatientId == patientId)
            .OrderByDescending(d => d.IssuedDate)
            .Take(take)
            .ToListAsync();
    }

    public async Task<int> GetCountByPatientIdAsync(Guid patientId)
    {
        return await _context.DocumentViews
            .Where(d => d.PatientId == patientId)
            .CountAsync();
    }
}
