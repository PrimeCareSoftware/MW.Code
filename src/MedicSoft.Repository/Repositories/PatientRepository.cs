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

        public override async Task<IEnumerable<Patient>> GetAllAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.TenantId == tenantId && p.IsActive)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
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
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByPhoneAsync(string phoneNumber, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Phone.Number.Contains(phoneNumber) && p.TenantId == tenantId)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm, string tenantId)
        {
            var searchTermLower = searchTerm.ToLower();

            return await _dbSet
                .Where(p => (p.Name.ToLower().Contains(searchTermLower) || 
                            p.Document.Contains(searchTerm) ||  // CPF is numeric, case-insensitive not needed
                            p.Phone.Number.Contains(searchTerm)) &&  // Phone is numeric, case-insensitive not needed
                            p.TenantId == tenantId &&
                            p.IsActive)
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .Take(20)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> SearchAsync(string searchTerm, string tenantId, Guid clinicId)
        {
            var searchTermLower = searchTerm.ToLower();
            
            // Optimized query using JOIN instead of .Any() to avoid N+1 issue - Category 4.2
            var query = from p in _dbSet
                        join pcl in _context.Set<PatientClinicLink>() on p.Id equals pcl.PatientId
                        where (p.Name.ToLower().Contains(searchTermLower) || 
                               p.Document.Contains(searchTerm) ||
                               p.Phone.Number.Contains(searchTerm)) &&
                              p.TenantId == tenantId &&
                              p.IsActive &&
                              pcl.ClinicId == clinicId &&
                              pcl.IsActive
                        orderby p.Name
                        select p;
            
            return await query
                .AsNoTracking()
                .Distinct()
                .Take(20)
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
                .AsNoTracking()
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            // Optimized query using JOIN instead of .Any() to avoid N+1 issue
            var query = from p in _dbSet
                        join pcl in _context.Set<PatientClinicLink>() on p.Id equals pcl.PatientId
                        where p.TenantId == tenantId && 
                              p.IsActive && 
                              pcl.ClinicId == clinicId && 
                              pcl.IsActive
                        orderby p.Name
                        select p;
            
            return await query.AsNoTracking().Distinct().ToListAsync();
        }
    }
}