using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAnamnesisTemplateRepository : IRepository<AnamnesisTemplate>
    {
        Task<List<AnamnesisTemplate>> GetBySpecialtyAsync(MedicalSpecialty specialty, string tenantId);
        Task<AnamnesisTemplate?> GetDefaultTemplateAsync(MedicalSpecialty specialty, string tenantId);
        Task<List<AnamnesisTemplate>> GetActiveTemplatesAsync(string tenantId);
    }
}
