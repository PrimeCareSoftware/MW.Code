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
    public class SenhaFilaRepository : BaseRepository<SenhaFila>, ISenhaFilaRepository
    {
        public SenhaFilaRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public override async Task<SenhaFila?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Fila)
                .Include(s => s.Paciente)
                .Include(s => s.Medico)
                .Include(s => s.Agendamento)
                .Where(s => s.Id == id && s.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<SenhaFila?> GetByNumeroSenhaAsync(string numeroSenha, Guid filaId, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Fila)
                .Include(s => s.Paciente)
                .Where(s => s.NumeroSenha == numeroSenha && s.FilaId == filaId && s.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SenhaFila>> GetByFilaIdAsync(Guid filaId, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Include(s => s.Medico)
                .Where(s => s.FilaId == filaId && s.TenantId == tenantId)
                .OrderBy(s => s.DataHoraEntrada)
                .ToListAsync();
        }

        public async Task<List<SenhaFila>> GetActiveSenhasByFilaAsync(Guid filaId, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Include(s => s.Medico)
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           (s.Status == StatusSenha.Aguardando || 
                            s.Status == StatusSenha.Chamando || 
                            s.Status == StatusSenha.EmAtendimento))
                .OrderByDescending(s => s.Prioridade)
                .ThenBy(s => s.DataHoraEntrada)
                .ToListAsync();
        }

        public async Task<List<SenhaFila>> GetAguardandoByFilaAsync(Guid filaId, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           s.Status == StatusSenha.Aguardando)
                .OrderByDescending(s => s.Prioridade)
                .ThenBy(s => s.DataHoraEntrada)
                .ToListAsync();
        }

        public async Task<SenhaFila?> GetProximaSenhaAsync(Guid filaId, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           s.Status == StatusSenha.Aguardando)
                .OrderByDescending(s => s.Prioridade)
                .ThenBy(s => s.DataHoraEntrada)
                .FirstOrDefaultAsync();
        }

        public async Task<List<SenhaFila>> GetProximasSenhasAsync(Guid filaId, int quantidade, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           s.Status == StatusSenha.Aguardando)
                .OrderByDescending(s => s.Prioridade)
                .ThenBy(s => s.DataHoraEntrada)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<List<SenhaFila>> GetSenhasByDataAsync(DateTime data, Guid clinicaId, string tenantId)
        {
            var dataInicio = data.Date;
            var dataFim = dataInicio.AddDays(1);

            return await _dbSet
                .Include(s => s.Fila)
                .Include(s => s.Paciente)
                .Where(s => s.Fila.ClinicaId == clinicaId &&
                           s.TenantId == tenantId &&
                           s.DataHoraEntrada >= dataInicio &&
                           s.DataHoraEntrada < dataFim)
                .OrderBy(s => s.DataHoraEntrada)
                .ToListAsync();
        }

        public async Task<int> GetPosicaoNaFilaAsync(Guid senhaId, string tenantId)
        {
            var senha = await _dbSet
                .Where(s => s.Id == senhaId && s.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (senha == null || senha.Status != StatusSenha.Aguardando)
                return 0;

            // Conta quantas senhas estão na frente (maior prioridade ou mesma prioridade mas chegou antes)
            var posicao = await _dbSet
                .Where(s => s.FilaId == senha.FilaId && 
                           s.TenantId == tenantId &&
                           s.Status == StatusSenha.Aguardando &&
                           (s.Prioridade > senha.Prioridade ||
                            (s.Prioridade == senha.Prioridade && s.DataHoraEntrada < senha.DataHoraEntrada)))
                .CountAsync();

            return posicao + 1;
        }

        public async Task<int> CountSenhasAFrenteAsync(Guid senhaId, string tenantId)
        {
            var senha = await _dbSet
                .Where(s => s.Id == senhaId && s.TenantId == tenantId)
                .FirstOrDefaultAsync();

            if (senha == null || senha.Status != StatusSenha.Aguardando)
                return 0;

            return await _dbSet
                .Where(s => s.FilaId == senha.FilaId && 
                           s.TenantId == tenantId &&
                           s.Status == StatusSenha.Aguardando &&
                           (s.Prioridade > senha.Prioridade ||
                            (s.Prioridade == senha.Prioridade && s.DataHoraEntrada < senha.DataHoraEntrada)))
                .CountAsync();
        }

        public async Task<List<SenhaFila>> GetUltimasChamadasAsync(Guid filaId, int quantidade, string tenantId)
        {
            return await _dbSet
                .Include(s => s.Paciente)
                .Include(s => s.Medico)
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           s.DataHoraChamada != null &&
                           (s.Status == StatusSenha.EmAtendimento || 
                            s.Status == StatusSenha.Atendido ||
                            s.Status == StatusSenha.Chamando))
                .OrderByDescending(s => s.DataHoraChamada)
                .Take(quantidade)
                .ToListAsync();
        }

        public async Task<List<SenhaFila>> GetSenhasByFilaAndDateRangeAsync(Guid filaId, DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            return await _dbSet
                .Where(s => s.FilaId == filaId && 
                           s.TenantId == tenantId &&
                           s.DataHoraEntrada >= dataInicio &&
                           s.DataHoraEntrada <= dataFim)
                .ToListAsync();
        }
    }
}
