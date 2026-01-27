using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Analytics.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Analytics.Services
{
    /// <summary>
    /// Serviço para geração de dashboards de qualidade
    /// </summary>
    public class DashboardQualidadeService : IDashboardQualidadeService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<DashboardQualidadeService> _logger;

        public DashboardQualidadeService(
            MedicSoftDbContext context,
            ILogger<DashboardQualidadeService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém o dashboard de qualidade para o período especificado
        /// </summary>
        public async Task<DashboardQualidadeDto> GetDashboardAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId)
        {
            try
            {
                _logger.LogInformation("Gerando dashboard de qualidade para período {Inicio} até {Fim}", inicio, fim);

                // Buscar dados consolidados (que contém NPS agregado)
                var dadosConsolidados = await _context.Set<ConsultaDiaria>()
                    .Where(c => c.TenantId == tenantId 
                        && c.Data >= inicio.Date 
                        && c.Data <= fim.Date)
                    .ToListAsync();

                // TODO: Quando tabela PatientFeedback for implementada, buscar avaliações detalhadas
                // var avaliacoes = await _context.Set<PatientFeedback>()
                //     .Where(f => f.TenantId == tenantId && f.CreatedAt >= inicio && f.CreatedAt <= fim)
                //     .Include(f => f.Professional)
                //     .Include(f => f.Patient)
                //     .ToListAsync();

                var dashboard = new DashboardQualidadeDto
                {
                    Periodo = new PeriodoDto { Inicio = inicio, Fim = fim },
                    
                    // KPIs principais
                    NpsMedio = CalcularNpsMedio(dadosConsolidados),
                    TotalAvaliacoes = CalcularTotalAvaliacoes(dadosConsolidados),
                    TaxaSatisfacao = CalcularTaxaSatisfacao(dadosConsolidados),
                    TaxaRecomendacao = CalcularTaxaRecomendacao(dadosConsolidados),
                    
                    // Distribuição NPS
                    DistribuicaoNps = GetDistribuicaoNps(dadosConsolidados),
                    
                    // Avaliações por médico
                    AvaliacoesMedicos = GetAvaliacoesMedicos(dadosConsolidados),
                    
                    // Avaliações por especialidade
                    AvaliacoesEspecialidades = GetAvaliacoesEspecialidades(dadosConsolidados),
                    
                    // Comentários recentes (mock data - TODO: implementar quando tabela de feedback existir)
                    ComentariosPositivos = GetComentariosPositivos(),
                    ComentariosNegativos = GetComentariosNegativos(),
                    
                    // Tendência NPS
                    TendenciaNps = GetTendenciaNps(dadosConsolidados, inicio, fim),
                    
                    // Palavras-chave (mock data - TODO: implementar análise de texto quando feedback existir)
                    PrincipaisElogios = GetPrincipaisElogios(),
                    PrincipaisCriticas = GetPrincipaisCriticas()
                };

                _logger.LogInformation("Dashboard de qualidade gerado com sucesso");
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar dashboard de qualidade");
                throw;
            }
        }

        /// <summary>
        /// Calcula o NPS médio do período
        /// </summary>
        private decimal CalcularNpsMedio(List<ConsultaDiaria> dadosConsolidados)
        {
            var dadosComNps = dadosConsolidados
                .Where(d => d.NpsMedio.HasValue && d.TotalAvaliacoes > 0)
                .ToList();

            if (!dadosComNps.Any())
                return 0;

            // Calcular média ponderada pelo número de avaliações
            var somaPonderada = dadosComNps.Sum(d => d.NpsMedio!.Value * d.TotalAvaliacoes);
            var totalAvaliacoes = dadosComNps.Sum(d => d.TotalAvaliacoes);

            return totalAvaliacoes > 0 ? somaPonderada / totalAvaliacoes : 0;
        }

        /// <summary>
        /// Calcula o total de avaliações
        /// </summary>
        private int CalcularTotalAvaliacoes(List<ConsultaDiaria> dadosConsolidados)
        {
            return dadosConsolidados.Sum(d => d.TotalAvaliacoes);
        }

        /// <summary>
        /// Calcula a taxa de satisfação (% de avaliações >= 8)
        /// </summary>
        private decimal CalcularTaxaSatisfacao(List<ConsultaDiaria> dadosConsolidados)
        {
            // TODO: Quando tabela de feedback existir, calcular baseado em avaliações individuais
            // Por enquanto, estimar baseado no NPS médio
            var npsMedio = CalcularNpsMedio(dadosConsolidados);
            
            if (npsMedio >= 9)
                return 90m; // Estimativa: 90% satisfeitos
            else if (npsMedio >= 7)
                return 70m; // Estimativa: 70% satisfeitos
            else if (npsMedio >= 5)
                return 50m; // Estimativa: 50% satisfeitos
            else
                return 30m; // Estimativa: 30% satisfeitos
        }

        /// <summary>
        /// Calcula a taxa de recomendação (% de avaliações 9-10)
        /// </summary>
        private decimal CalcularTaxaRecomendacao(List<ConsultaDiaria> dadosConsolidados)
        {
            // TODO: Quando tabela de feedback existir, calcular baseado em avaliações individuais
            // Por enquanto, estimar baseado no NPS médio
            var npsMedio = CalcularNpsMedio(dadosConsolidados);
            
            if (npsMedio >= 9)
                return 80m; // Estimativa: 80% recomendariam
            else if (npsMedio >= 7)
                return 60m; // Estimativa: 60% recomendariam
            else if (npsMedio >= 5)
                return 40m; // Estimativa: 40% recomendariam
            else
                return 20m; // Estimativa: 20% recomendariam
        }

        /// <summary>
        /// Obtém a distribuição de NPS (promotores, neutros, detratores)
        /// </summary>
        private DistribuicaoNpsDto GetDistribuicaoNps(List<ConsultaDiaria> dadosConsolidados)
        {
            var totalAvaliacoes = CalcularTotalAvaliacoes(dadosConsolidados);
            
            if (totalAvaliacoes == 0)
            {
                return new DistribuicaoNpsDto
                {
                    Promotores = 0,
                    Neutros = 0,
                    Detratores = 0,
                    PercentualPromotores = 0,
                    PercentualNeutros = 0,
                    PercentualDetratores = 0
                };
            }

            // TODO: Quando tabela de feedback existir, calcular baseado em avaliações individuais
            // Por enquanto, estimar baseado no NPS médio
            var npsMedio = CalcularNpsMedio(dadosConsolidados);
            
            int promotores, neutros, detratores;
            
            if (npsMedio >= 9)
            {
                promotores = (int)(totalAvaliacoes * 0.7); // 70% promotores
                neutros = (int)(totalAvaliacoes * 0.25);    // 25% neutros
                detratores = totalAvaliacoes - promotores - neutros; // 5% detratores
            }
            else if (npsMedio >= 7)
            {
                promotores = (int)(totalAvaliacoes * 0.5); // 50% promotores
                neutros = (int)(totalAvaliacoes * 0.35);    // 35% neutros
                detratores = totalAvaliacoes - promotores - neutros; // 15% detratores
            }
            else if (npsMedio >= 5)
            {
                promotores = (int)(totalAvaliacoes * 0.3); // 30% promotores
                neutros = (int)(totalAvaliacoes * 0.4);     // 40% neutros
                detratores = totalAvaliacoes - promotores - neutros; // 30% detratores
            }
            else
            {
                promotores = (int)(totalAvaliacoes * 0.2); // 20% promotores
                neutros = (int)(totalAvaliacoes * 0.3);     // 30% neutros
                detratores = totalAvaliacoes - promotores - neutros; // 50% detratores
            }

            return new DistribuicaoNpsDto
            {
                Promotores = promotores,
                Neutros = neutros,
                Detratores = detratores,
                PercentualPromotores = totalAvaliacoes > 0 ? (decimal)promotores / totalAvaliacoes * 100 : 0,
                PercentualNeutros = totalAvaliacoes > 0 ? (decimal)neutros / totalAvaliacoes * 100 : 0,
                PercentualDetratores = totalAvaliacoes > 0 ? (decimal)detratores / totalAvaliacoes * 100 : 0
            };
        }

        /// <summary>
        /// Obtém avaliações por médico
        /// </summary>
        private List<AvaliacaoMedicoDto> GetAvaliacoesMedicos(List<ConsultaDiaria> dadosConsolidados)
        {
            // Buscar médicos relacionados aos dados consolidados
            var medicosIds = dadosConsolidados
                .Where(d => d.MedicoId.HasValue && d.TotalAvaliacoes > 0)
                .Select(d => d.MedicoId!.Value)
                .Distinct()
                .ToList();

            if (!medicosIds.Any())
                return new List<AvaliacaoMedicoDto>();

            var medicos = _context.Users
                .Where(u => medicosIds.Contains(u.Id))
                .ToDictionary(u => u.Id, u => u);

            var dadosPorMedico = dadosConsolidados
                .Where(d => d.MedicoId.HasValue && d.TotalAvaliacoes > 0)
                .GroupBy(d => d.MedicoId!.Value)
                .Select(g =>
                {
                    var medicoId = g.Key;
                    var dados = g.ToList();
                    var totalAvaliacoes = dados.Sum(d => d.TotalAvaliacoes);
                    var npsPonderado = dados
                        .Where(d => d.NpsMedio.HasValue)
                        .Sum(d => d.NpsMedio!.Value * d.TotalAvaliacoes);

                    var medico = medicos.ContainsKey(medicoId) ? medicos[medicoId] : null;

                    return new AvaliacaoMedicoDto
                    {
                        NomeMedico = medico?.FullName ?? "Desconhecido",
                        Crm = medico?.ProfessionalId ?? "N/A",
                        NpsMedio = totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0,
                        TotalAvaliacoes = totalAvaliacoes,
                        TaxaSatisfacao = EstimarTaxaSatisfacao(totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0)
                    };
                })
                .OrderByDescending(a => a.NpsMedio)
                .ToList();

            return dadosPorMedico;
        }

        /// <summary>
        /// Obtém avaliações por especialidade
        /// </summary>
        private List<AvaliacaoEspecialidadeDto> GetAvaliacoesEspecialidades(List<ConsultaDiaria> dadosConsolidados)
        {
            // Buscar médicos relacionados aos dados consolidados
            var medicosIds = dadosConsolidados
                .Where(d => d.MedicoId.HasValue && d.TotalAvaliacoes > 0)
                .Select(d => d.MedicoId!.Value)
                .Distinct()
                .ToList();

            if (!medicosIds.Any())
                return new List<AvaliacaoEspecialidadeDto>();

            var medicos = _context.Users
                .Where(u => medicosIds.Contains(u.Id) && !string.IsNullOrEmpty(u.Specialty))
                .ToDictionary(u => u.Id, u => u);

            var dadosPorEspecialidade = dadosConsolidados
                .Where(d => d.MedicoId.HasValue && d.TotalAvaliacoes > 0 && medicos.ContainsKey(d.MedicoId.Value))
                .GroupBy(d => medicos[d.MedicoId!.Value].Specialty)
                .Select(g =>
                {
                    var dados = g.ToList();
                    var totalAvaliacoes = dados.Sum(d => d.TotalAvaliacoes);
                    var npsPonderado = dados
                        .Where(d => d.NpsMedio.HasValue)
                        .Sum(d => d.NpsMedio!.Value * d.TotalAvaliacoes);

                    return new AvaliacaoEspecialidadeDto
                    {
                        Especialidade = g.Key ?? "Não informada",
                        NpsMedio = totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0,
                        TotalAvaliacoes = totalAvaliacoes,
                        TaxaSatisfacao = EstimarTaxaSatisfacao(totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0)
                    };
                })
                .OrderByDescending(a => a.NpsMedio)
                .ToList();

            return dadosPorEspecialidade;
        }

        /// <summary>
        /// Obtém comentários positivos recentes
        /// </summary>
        private List<ComentarioAvaliacaoDto> GetComentariosPositivos()
        {
            // TODO: Implementar quando tabela PatientFeedback existir
            // Por enquanto, retornar lista vazia ou dados de exemplo
            return new List<ComentarioAvaliacaoDto>
            {
                // Exemplo de como seria:
                // new ComentarioAvaliacaoDto
                // {
                //     Id = Guid.NewGuid(),
                //     PacienteNome = "Maria S.",
                //     MedicoNome = "Dr. João Silva",
                //     DataAvaliacao = DateTime.Now.AddDays(-2),
                //     Nota = 10,
                //     Comentario = "Excelente atendimento, muito atencioso!"
                // }
            };
        }

        /// <summary>
        /// Obtém comentários negativos recentes
        /// </summary>
        private List<ComentarioAvaliacaoDto> GetComentariosNegativos()
        {
            // TODO: Implementar quando tabela PatientFeedback existir
            // Por enquanto, retornar lista vazia ou dados de exemplo
            return new List<ComentarioAvaliacaoDto>
            {
                // Exemplo de como seria:
                // new ComentarioAvaliacaoDto
                // {
                //     Id = Guid.NewGuid(),
                //     PacienteNome = "José A.",
                //     MedicoNome = "Dra. Ana Costa",
                //     DataAvaliacao = DateTime.Now.AddDays(-1),
                //     Nota = 4,
                //     Comentario = "Tempo de espera muito longo."
                // }
            };
        }

        /// <summary>
        /// Obtém tendência de NPS ao longo do tempo
        /// </summary>
        private List<TendenciaNpsDto> GetTendenciaNps(
            List<ConsultaDiaria> dadosConsolidados,
            DateTime inicio,
            DateTime fim)
        {
            var resultado = new List<TendenciaNpsDto>();

            // Agrupar por mês se o período for maior que 60 dias, senão por semana
            var diasPeriodo = (fim - inicio).Days;
            
            if (diasPeriodo > 60)
            {
                // Agrupar por mês
                var dadosPorMes = dadosConsolidados
                    .Where(d => d.TotalAvaliacoes > 0)
                    .GroupBy(d => new { d.Data.Year, d.Data.Month })
                    .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                    .ToList();

                foreach (var grupo in dadosPorMes)
                {
                    var dados = grupo.ToList();
                    var totalAvaliacoes = dados.Sum(d => d.TotalAvaliacoes);
                    var npsPonderado = dados
                        .Where(d => d.NpsMedio.HasValue)
                        .Sum(d => d.NpsMedio!.Value * d.TotalAvaliacoes);

                    resultado.Add(new TendenciaNpsDto
                    {
                        Mes = $"{grupo.Key.Month:00}/{grupo.Key.Year}",
                        NpsMedio = totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0,
                        TotalAvaliacoes = totalAvaliacoes
                    });
                }
            }
            else
            {
                // Agrupar por semana
                var dataAtual = inicio.Date;
                while (dataAtual <= fim.Date)
                {
                    var fimSemana = dataAtual.AddDays(6);
                    if (fimSemana > fim.Date)
                        fimSemana = fim.Date;

                    var dadosSemana = dadosConsolidados
                        .Where(d => d.Data >= dataAtual && d.Data <= fimSemana && d.TotalAvaliacoes > 0)
                        .ToList();

                    if (dadosSemana.Any())
                    {
                        var totalAvaliacoes = dadosSemana.Sum(d => d.TotalAvaliacoes);
                        var npsPonderado = dadosSemana
                            .Where(d => d.NpsMedio.HasValue)
                            .Sum(d => d.NpsMedio!.Value * d.TotalAvaliacoes);

                        resultado.Add(new TendenciaNpsDto
                        {
                            Mes = $"{dataAtual:dd/MM} - {fimSemana:dd/MM}",
                            NpsMedio = totalAvaliacoes > 0 ? npsPonderado / totalAvaliacoes : 0,
                            TotalAvaliacoes = totalAvaliacoes
                        });
                    }

                    dataAtual = dataAtual.AddDays(7);
                }
            }

            return resultado;
        }

        /// <summary>
        /// Obtém principais elogios (palavras-chave em comentários positivos)
        /// </summary>
        private List<PalavraChaveDto> GetPrincipaisElogios()
        {
            // TODO: Implementar análise de texto quando tabela PatientFeedback existir
            // Por enquanto, retornar palavras-chave de exemplo
            return new List<PalavraChaveDto>
            {
                new PalavraChaveDto { Palavra = "atencioso", Frequencia = 45 },
                new PalavraChaveDto { Palavra = "profissional", Frequencia = 38 },
                new PalavraChaveDto { Palavra = "pontual", Frequencia = 32 },
                new PalavraChaveDto { Palavra = "educado", Frequencia = 28 },
                new PalavraChaveDto { Palavra = "competente", Frequencia = 25 }
            };
        }

        /// <summary>
        /// Obtém principais críticas (palavras-chave em comentários negativos)
        /// </summary>
        private List<PalavraChaveDto> GetPrincipaisCriticas()
        {
            // TODO: Implementar análise de texto quando tabela PatientFeedback existir
            // Por enquanto, retornar palavras-chave de exemplo
            return new List<PalavraChaveDto>
            {
                new PalavraChaveDto { Palavra = "espera", Frequencia = 22 },
                new PalavraChaveDto { Palavra = "atraso", Frequencia = 18 },
                new PalavraChaveDto { Palavra = "recepção", Frequencia = 12 },
                new PalavraChaveDto { Palavra = "fila", Frequencia = 10 },
                new PalavraChaveDto { Palavra = "rápido", Frequencia = 8 }
            };
        }

        /// <summary>
        /// Estima taxa de satisfação baseada no NPS
        /// </summary>
        private decimal EstimarTaxaSatisfacao(decimal nps)
        {
            if (nps >= 9)
                return 90m;
            else if (nps >= 7)
                return 70m;
            else if (nps >= 5)
                return 50m;
            else
                return 30m;
        }
    }
}
