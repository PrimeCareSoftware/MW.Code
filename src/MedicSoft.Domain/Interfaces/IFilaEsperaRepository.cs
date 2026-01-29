using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IFilaEsperaRepository : IRepository<FilaEspera>
    {
        Task<FilaEspera?> GetByClinicaIdAsync(Guid clinicaId, string tenantId);
        Task<List<FilaEspera>> GetActiveByClinicaAsync(Guid clinicaId, string tenantId);
        Task<List<FilaEspera>> GetAllByClinicaAsync(Guid clinicaId, string tenantId);
    }
}
