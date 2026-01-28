using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Data;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Implementação do repositório de configuração de integração
    /// </summary>
    public class ConfiguracaoIntegracaoRepository : Repository<ConfiguracaoIntegracao>, IConfiguracaoIntegracaoRepository
    {
        public ConfiguracaoIntegracaoRepository(ApplicationDbContext context) : base(context)
        {
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
    }
}
