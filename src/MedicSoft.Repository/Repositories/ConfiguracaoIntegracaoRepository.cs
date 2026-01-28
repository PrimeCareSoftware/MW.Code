using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Implementação do repositório de configuração de integração
    /// </summary>
    public class ConfiguracaoIntegracaoRepository : IConfiguracaoIntegracaoRepository
    {
        private readonly MedicSoftDbContext _context;

        public ConfiguracaoIntegracaoRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<ConfiguracaoIntegracao?> ObterConfiguracaoAtivaAsync(Guid clinicaId)
        {
            return await _context.Set<ConfiguracaoIntegracao>()
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ClinicaId == clinicaId && c.Ativa);
        }

        public async Task AtualizarUltimaSincronizacaoAsync(Guid clinicaId, DateTime data)
        {
            var configuracao = await _context.Set<ConfiguracaoIntegracao>()
                .FirstOrDefaultAsync(c => c.ClinicaId == clinicaId && c.Ativa);

            if (configuracao != null)
            {
                configuracao.UltimaSincronizacao = data;
                configuracao.TentativasErro = 0;
                configuracao.UltimoErro = null;
                _context.Set<ConfiguracaoIntegracao>().Update(configuracao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RegistrarErroAsync(Guid clinicaId, string mensagem)
        {
            var configuracao = await _context.Set<ConfiguracaoIntegracao>()
                .FirstOrDefaultAsync(c => c.ClinicaId == clinicaId && c.Ativa);

            if (configuracao != null)
            {
                configuracao.UltimoErro = mensagem;
                configuracao.TentativasErro++;
                
                // Desativar após 5 tentativas consecutivas de erro
                if (configuracao.TentativasErro >= 5)
                {
                    configuracao.Ativa = false;
                }
                
                _context.Set<ConfiguracaoIntegracao>().Update(configuracao);
                await _context.SaveChangesAsync();
            }
        }

        public async Task LimparErrosAsync(Guid clinicaId)
        {
            var configuracao = await _context.Set<ConfiguracaoIntegracao>()
                .FirstOrDefaultAsync(c => c.ClinicaId == clinicaId);

            if (configuracao != null)
            {
                configuracao.TentativasErro = 0;
                configuracao.UltimoErro = null;
                _context.Set<ConfiguracaoIntegracao>().Update(configuracao);
                await _context.SaveChangesAsync();
            }
        }

        // Minimal IRepository<ConfiguracaoIntegracao> implementation
        public async Task<ConfiguracaoIntegracao?> GetByIdAsync(Guid id, string tenantId) =>
            await _context.Set<ConfiguracaoIntegracao>()
                .FirstOrDefaultAsync(c => c.Id == id && c.ClinicaId.ToString() == tenantId);

        public async Task<IEnumerable<ConfiguracaoIntegracao>> GetAllAsync(string tenantId) =>
            await _context.Set<ConfiguracaoIntegracao>()
                .Where(c => c.ClinicaId.ToString() == tenantId)
                .ToListAsync();

        public async Task<IEnumerable<ConfiguracaoIntegracao>> FindAsync(Expression<Func<ConfiguracaoIntegracao, bool>> predicate, string tenantId) =>
            await _context.Set<ConfiguracaoIntegracao>()
                .Where(c => c.ClinicaId.ToString() == tenantId)
                .Where(predicate)
                .ToListAsync();

        public async Task<ConfiguracaoIntegracao> AddAsync(ConfiguracaoIntegracao entity)
        {
            await _context.Set<ConfiguracaoIntegracao>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<ConfiguracaoIntegracao> AddWithoutSaveAsync(ConfiguracaoIntegracao entity)
        {
            await _context.Set<ConfiguracaoIntegracao>().AddAsync(entity);
            return entity;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _context.SaveChangesAsync(cancellationToken);

        public async Task UpdateAsync(ConfiguracaoIntegracao entity)
        {
            _context.Set<ConfiguracaoIntegracao>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, string tenantId)
        {
            var entity = await GetByIdAsync(id, tenantId);
            if (entity != null)
            {
                _context.Set<ConfiguracaoIntegracao>().Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteWithoutSaveAsync(Guid id, string tenantId)
        {
            var entity = await GetByIdAsync(id, tenantId);
            if (entity != null)
            {
                _context.Set<ConfiguracaoIntegracao>().Remove(entity);
            }
        }

        public async Task<bool> ExistsAsync(Guid id, string tenantId) =>
            await _context.Set<ConfiguracaoIntegracao>()
                .AnyAsync(c => c.Id == id && c.ClinicaId.ToString() == tenantId);

        public async Task<int> CountAsync(string tenantId) =>
            await _context.Set<ConfiguracaoIntegracao>()
                .CountAsync(c => c.ClinicaId.ToString() == tenantId);

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var result = await operation();
                await transaction.CommitAsync(cancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await operation();
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<ConfiguracaoIntegracao?> GetByIdWithoutTenantAsync(Guid id) =>
            await _context.Set<ConfiguracaoIntegracao>().FindAsync(id);

        public IQueryable<ConfiguracaoIntegracao> GetAllQueryable() =>
            _context.Set<ConfiguracaoIntegracao>().AsQueryable();
    }
}
