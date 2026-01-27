using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Analytics.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Analytics.Services
{
    public class DashboardClinicoService : IDashboardClinicoService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<DashboardClinicoService> _logger;

        public DashboardClinicoService(
            MedicSoftDbContext context,
            ILogger<DashboardClinicoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardClinicoDto> GetDashboardAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId, 
            Guid? medicoId = null)
        {
            try
            {
                _logger.LogInformation("Gerando dashboard clínico para período {Inicio} até {Fim}", inicio, fim);

                // Buscar dados consolidados
                var query = _context.Set<ConsultaDiaria>()
                    .Where(c => c.Data >= inicio.Date && c.Data <= fim.Date && c.TenantId == tenantId);

                if (medicoId.HasValue)
                {
                    query = query.Where(c => c.MedicoId == medicoId.Value);
                }

                var dados = await query.ToListAsync();

                if (!dados.Any())
                {
                    _logger.LogWarning("Nenhum dado consolidado encontrado para o período");
                    return CriarDashboardVazio(inicio, fim);
                }

                var totalConsultas = dados.Sum(d => d.TotalConsultas);
                var consultasRealizadas = dados.Sum(d => d.ConsultasRealizadas);

                var dashboard = new DashboardClinicoDto
                {
                    Periodo = new PeriodoDto { Inicio = inicio, Fim = fim },
                    
                    TotalConsultas = totalConsultas,
                    TaxaOcupacao = CalcularTaxaOcupacao(dados),
                    TempoMedioConsulta = dados.Any() ? (int)dados.Average(d => d.TempoMedioConsultaMinutos) : 0,
                    TaxaNoShow = totalConsultas > 0 
                        ? (decimal)dados.Sum(d => d.NoShow) / totalConsultas * 100 
                        : 0,
                    
                    ConsultasPorEspecialidade = await GetConsultasPorEspecialidadeAsync(inicio, fim, tenantId, medicoId),
                    ConsultasPorMedico = await GetConsultasPorMedicoAsync(inicio, fim, tenantId, medicoId),
                    ConsultasPorDiaSemana = GetConsultasPorDiaSemana(dados),
                    ConsultasPorHorario = await GetConsultasPorHorarioAsync(inicio, fim, tenantId, medicoId),
                    
                    DiagnosticosMaisFrequentes = await GetTopDiagnosticosAsync(inicio, fim, tenantId, medicoId),
                    
                    PacientesNovos = dados.Sum(d => d.TotalPacientesNovos),
                    PacientesRetorno = dados.Sum(d => d.TotalPacientesRetorno),
                    
                    TendenciaConsultas = GetTendenciaMensal(dados)
                };

                _logger.LogInformation("Dashboard clínico gerado com sucesso");
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar dashboard clínico");
                throw;
            }
        }

        private decimal CalcularTaxaOcupacao(List<ConsultaDiaria> dados)
        {
            var totalConsultas = dados.Sum(d => d.TotalConsultas);
            var consultasRealizadas = dados.Sum(d => d.ConsultasRealizadas);
            
            if (totalConsultas == 0)
                return 0;
            
            return (decimal)consultasRealizadas / totalConsultas * 100;
        }

        private async Task<List<ConsultasPorEspecialidadeDto>> GetConsultasPorEspecialidadeAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId, 
            Guid? medicoId)
        {
            var query = _context.Appointments
                .Where(a => a.TenantId == tenantId 
                    && a.ScheduledDate >= inicio 
                    && a.ScheduledDate <= fim);

            if (medicoId.HasValue)
            {
                query = query.Where(a => a.ProfessionalId == medicoId.Value);
            }

            var appointments = await query
                .Include(a => a.Professional)
                .ToListAsync();

            // Agrupar por especialidade (simplificado - você pode ter uma tabela de especialidades)
            var grouped = appointments
                .Where(a => a.Professional != null)
                .GroupBy(a => "Especialidade Geral") // Placeholder - ajustar conforme modelo
                .Select(g => new ConsultasPorEspecialidadeDto
                {
                    Especialidade = g.Key,
                    Total = g.Count(),
                    Percentual = appointments.Any() ? (decimal)g.Count() / appointments.Count * 100 : 0
                })
                .ToList();

            if (!grouped.Any())
            {
                grouped.Add(new ConsultasPorEspecialidadeDto
                {
                    Especialidade = "Geral",
                    Total = appointments.Count,
                    Percentual = 100
                });
            }

            return grouped;
        }

        private async Task<List<ConsultasPorMedicoDto>> GetConsultasPorMedicoAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId, 
            Guid? medicoId)
        {
            var query = _context.Appointments
                .Where(a => a.TenantId == tenantId 
                    && a.ScheduledDate >= inicio 
                    && a.ScheduledDate <= fim
                    && a.ProfessionalId.HasValue);

            if (medicoId.HasValue)
            {
                query = query.Where(a => a.ProfessionalId == medicoId.Value);
            }

            var appointments = await query
                .Include(a => a.Professional)
                .ToListAsync();

            var grouped = appointments
                .GroupBy(a => new { a.ProfessionalId, NomeMedico = a.Professional != null ? a.Professional.FullName : "Desconhecido" })
                .Select(g =>
                {
                    var totalAgendadas = g.Count();
                    var realizadas = g.Count(a => a.Status == AppointmentStatus.Completed);
                    
                    return new ConsultasPorMedicoDto
                    {
                        NomeMedico = g.Key.NomeMedico,
                        CRM = "CRM/UF", // Placeholder - ajustar conforme modelo
                        Total = totalAgendadas,
                        TaxaOcupacao = totalAgendadas > 0 ? (decimal)realizadas / totalAgendadas * 100 : 0
                    };
                })
                .OrderByDescending(m => m.Total)
                .ToList();

            return grouped;
        }

        private List<ConsultasPorDiaDto> GetConsultasPorDiaSemana(List<ConsultaDiaria> dados)
        {
            var culture = new CultureInfo("pt-BR");
            
            var grouped = dados
                .GroupBy(d => d.Data.DayOfWeek)
                .Select(g => new ConsultasPorDiaDto
                {
                    DiaSemana = culture.DateTimeFormat.GetDayName(g.Key),
                    Total = g.Sum(d => d.TotalConsultas),
                    MediaPorDia = (decimal)g.Average(d => d.TotalConsultas)
                })
                .OrderBy(d => 
                {
                    var dia = culture.DateTimeFormat.DayNames.ToList().IndexOf(d.DiaSemana);
                    return dia == 0 ? 7 : dia; // Domingo no final
                })
                .ToList();

            return grouped;
        }

        private async Task<List<ConsultasPorHorarioDto>> GetConsultasPorHorarioAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId, 
            Guid? medicoId)
        {
            var query = _context.Appointments
                .Where(a => a.TenantId == tenantId 
                    && a.ScheduledDate >= inicio 
                    && a.ScheduledDate <= fim);

            if (medicoId.HasValue)
            {
                query = query.Where(a => a.ProfessionalId == medicoId.Value);
            }

            var appointments = await query.ToListAsync();

            var grouped = appointments
                .GroupBy(a => a.ScheduledTime.Hours)
                .Select(g => new ConsultasPorHorarioDto
                {
                    Hora = g.Key,
                    Total = g.Count()
                })
                .OrderBy(h => h.Hora)
                .ToList();

            return grouped;
        }

        private async Task<List<DiagnosticoFrequenciaDto>> GetTopDiagnosticosAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId, 
            Guid? medicoId,
            int top = 10)
        {
            var query = _context.DiagnosticHypotheses
                .Where(d => d.TenantId == tenantId 
                    && d.CreatedAt >= inicio 
                    && d.CreatedAt <= fim);

            var diagnosticos = await query.ToListAsync();

            if (!diagnosticos.Any())
            {
                return new List<DiagnosticoFrequenciaDto>();
            }

            var grouped = diagnosticos
                .GroupBy(d => new { d.ICD10Code, Descricao = d.Description ?? "Sem descrição" })
                .Select(g => new DiagnosticoFrequenciaDto
                {
                    CodigoCid = g.Key.ICD10Code ?? "N/A",
                    Descricao = g.Key.Descricao,
                    Frequencia = g.Count(),
                    Percentual = (decimal)g.Count() / diagnosticos.Count * 100
                })
                .OrderByDescending(d => d.Frequencia)
                .Take(top)
                .ToList();

            return grouped;
        }

        private List<TendenciaMensalDto> GetTendenciaMensal(List<ConsultaDiaria> dados)
        {
            var culture = new CultureInfo("pt-BR");
            
            var grouped = dados
                .GroupBy(d => new { d.Data.Year, d.Data.Month })
                .Select(g => new TendenciaMensalDto
                {
                    Mes = $"{culture.DateTimeFormat.GetMonthName(g.Key.Month)} {g.Key.Year}",
                    Agendadas = g.Sum(d => d.TotalConsultas),
                    Realizadas = g.Sum(d => d.ConsultasRealizadas),
                    Canceladas = g.Sum(d => d.ConsultasCanceladas),
                    NoShow = g.Sum(d => d.NoShow)
                })
                .OrderBy(t => t.Mes)
                .ToList();

            return grouped;
        }

        private DashboardClinicoDto CriarDashboardVazio(DateTime inicio, DateTime fim)
        {
            return new DashboardClinicoDto
            {
                Periodo = new PeriodoDto { Inicio = inicio, Fim = fim },
                TotalConsultas = 0,
                TaxaOcupacao = 0,
                TempoMedioConsulta = 0,
                TaxaNoShow = 0,
                ConsultasPorEspecialidade = new List<ConsultasPorEspecialidadeDto>(),
                ConsultasPorMedico = new List<ConsultasPorMedicoDto>(),
                ConsultasPorDiaSemana = new List<ConsultasPorDiaDto>(),
                ConsultasPorHorario = new List<ConsultasPorHorarioDto>(),
                DiagnosticosMaisFrequentes = new List<DiagnosticoFrequenciaDto>(),
                PacientesNovos = 0,
                PacientesRetorno = 0,
                TendenciaConsultas = new List<TendenciaMensalDto>()
            };
        }
    }
}
