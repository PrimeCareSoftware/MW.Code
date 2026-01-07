using Microsoft.EntityFrameworkCore;
using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Infrastructure.Data;

namespace PatientPortal.Infrastructure.Repositories;

public class PatientUserRepository : IPatientUserRepository
{
    private readonly PatientPortalDbContext _context;

    public PatientUserRepository(PatientPortalDbContext context)
    {
        _context = context;
    }

    public async Task<PatientUser?> GetByIdAsync(Guid id)
    {
        return await _context.PatientUsers.FindAsync(id);
    }

    public async Task<PatientUser?> GetByEmailAsync(string email)
    {
        return await _context.PatientUsers
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<PatientUser?> GetByCPFAsync(string cpf)
    {
        return await _context.PatientUsers
            .FirstOrDefaultAsync(u => u.CPF == cpf);
    }

    public async Task<PatientUser?> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.PatientUsers
            .FirstOrDefaultAsync(u => u.PatientId == patientId);
    }

    public async Task<PatientUser> CreateAsync(PatientUser patientUser)
    {
        await _context.PatientUsers.AddAsync(patientUser);
        await _context.SaveChangesAsync();
        return patientUser;
    }

    public async Task UpdateAsync(PatientUser patientUser)
    {
        _context.PatientUsers.Update(patientUser);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.PatientUsers.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> ExistsByCPFAsync(string cpf)
    {
        return await _context.PatientUsers.AnyAsync(u => u.CPF == cpf);
    }
}
