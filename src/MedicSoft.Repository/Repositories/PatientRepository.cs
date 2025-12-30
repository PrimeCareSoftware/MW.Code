using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByDocumentAsync(string document, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Document == document && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetByDocumentGlobalAsync(string document)
        {
            return await _dbSet
                .Where(p => p.Document == document)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetByEmailAsync(string email, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Email.Value == email && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(name) && p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByPhoneAsync(string phoneNumber, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Phone.Number.Contains(phoneNumber) && p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm, string tenantId)
        {
            return await _dbSet
                .Where(p => (p.Name.Contains(searchTerm) || 
                            p.Document.Contains(searchTerm) || 
                            p.Phone.Number.Contains(searchTerm)) && 
                            p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Document == document && p.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Email.Value == email && p.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Patient>> GetChildrenOfGuardianAsync(Guid guardianId, string tenantId)
        {
            return await _dbSet
                .Where(p => p.GuardianId == guardianId && p.TenantId == tenantId && p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(p => p.ClinicLinks)
                .Where(p => p.TenantId == tenantId && 
                           p.IsActive && 
                           p.ClinicLinks.Any(cl => cl.ClinicId == clinicId && cl.IsActive))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }
    }
}