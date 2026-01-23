using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AnamnesisResponseRepository : BaseRepository<AnamnesisResponse>, IAnamnesisResponseRepository
    {
        public AnamnesisResponseRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<List<AnamnesisResponse>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Template)
                .Where(r => r.PatientId == patientId && r.TenantId == tenantId)
                .OrderByDescending(r => r.ResponseDate)
                .ToListAsync();
        }

        public async Task<AnamnesisResponse?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Template)
                .Where(r => r.AppointmentId == appointmentId && r.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<AnamnesisResponse>> GetByDoctorIdAsync(Guid doctorId, string tenantId)
        {
            return await _dbSet
                .Include(r => r.Template)
                .Include(r => r.Patient)
                .Where(r => r.DoctorId == doctorId && r.TenantId == tenantId)
                .OrderByDescending(r => r.ResponseDate)
                .ToListAsync();
        }
    }
}
