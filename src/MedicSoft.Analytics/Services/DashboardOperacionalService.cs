using System;
using System.Collections.Generic;
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
    /// <summary>
    /// Serviço para geração de dashboards operacionais
    /// </summary>
    public class DashboardOperacionalService : IDashboardOperacionalService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<DashboardOperacionalService> _logger;

        public DashboardOperacionalService(
            MedicSoftDbContext context,
            ILogger<DashboardOperacionalService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtém o dashboard operacional para o período especificado
        /// </summary>
        public async Task<DashboardOperacionalDto> GetDashboardAsync(
            DateTime inicio, 
            DateTime fim, 
            string tenantId)
        {
            try
            {
                _logger.LogInformation("Gerando dashboard operacional para período {Inicio} até {Fim}", inicio, fim);

                // Buscar agendamentos do período
                var agendamentos = await _context.Appointments
                    .Where(a => a.TenantId == tenantId 
                        && a.ScheduledDate >= inicio.Date 
                        && a.ScheduledDate <= fim.Date)
                    .Include(a => a.Professional)
                    .Include(a => a.Patient)
                    .ToListAsync();

                // Buscar dados da fila de espera (SenhaFila) para cálculos mais precisos
                var senhasFila = await _context.Set<SenhaFila>()
                    .Where(s => s.TenantId == tenantId 
                        && s.DataHoraEntrada >= inicio 
                        && s.DataHoraEntrada <= fim)
                    .Include(s => s.Medico)
                    .ToListAsync();

                var dashboard = new DashboardOperacionalDto
                {
                    Periodo = new PeriodoDto { Inicio = inicio, Fim = fim },
                    
                    // KPIs principais
                    TempoMedioEsperaMinutos = CalcularTempoMedioEspera(senhasFila, agendamentos),
                    TamanhoFilaAtual = CalcularTamanhoFilaAtual(tenantId),
                    PacientesEmAtendimento = CalcularPacientesEmAtendimento(tenantId),
                    TaxaAtendimentoNoPrazo = CalcularTaxaAtendimentoNoPrazo(senhasFila, agendamentos),
                    
                    // Análises detalhadas
                    TemposPorEtapa = GetTemposPorEtapa(senhasFila),
                    PerformanceMedicos = GetPerformanceMedicos(agendamentos, senhasFila),
                    DistribuicaoHorarios = GetDistribuicaoHorarios(agendamentos, senhasFila),
                    TemposPorEspecialidade = GetTemposPorEspecialidade(agendamentos, senhasFila),
                    TendenciaTempoEspera = GetTendenciaTempoEspera(senhasFila, agendamentos, inicio, fim)
                };

                _logger.LogInformation("Dashboard operacional gerado com sucesso");
                return dashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar dashboard operacional");
                throw;
            }
        }

        /// <summary>
        /// Calcula o tempo médio de espera em minutos
        /// </summary>
        private int CalcularTempoMedioEspera(List<SenhaFila> senhasFila, List<Appointment> agendamentos)
        {
            // Priorizar dados da fila se disponíveis
            var temposEspera = senhasFila
                .Where(s => s.TempoEsperaMinutos > 0)
                .Select(s => s.TempoEsperaMinutos)
                .ToList();

            if (temposEspera.Any())
            {
                return (int)temposEspera.Average();
            }

            // Fallback: calcular dos agendamentos com CheckInTime
            var temposAgendamento = agendamentos
                .Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                .Select(a => (a.CheckOutTime!.Value - a.CheckInTime!.Value).TotalMinutes)
                .Where(t => t > 0)
                .ToList();

            return temposAgendamento.Any() ? (int)temposAgendamento.Average() : 0;
        }

        /// <summary>
        /// Calcula o tamanho atual da fila (agendamentos agendados/confirmados para hoje)
        /// </summary>
        private int CalcularTamanhoFilaAtual(string tenantId)
        {
            var hoje = DateTime.Today;
            
            // Buscar na fila de senhas
            var filaAtual = _context.Set<SenhaFila>()
                .Count(s => s.TenantId == tenantId 
                    && s.DataHoraEntrada.Date == hoje
                    && (s.Status == StatusSenha.Aguardando || s.Status == StatusSenha.Chamando));

            if (filaAtual > 0)
                return filaAtual;

            // Fallback: agendamentos do dia
            return _context.Appointments
                .Count(a => a.TenantId == tenantId 
                    && a.ScheduledDate.Date == hoje
                    && (a.Status == AppointmentStatus.Scheduled || a.Status == AppointmentStatus.Confirmed));
        }

        /// <summary>
        /// Calcula quantos pacientes estão em atendimento no momento
        /// </summary>
        private int CalcularPacientesEmAtendimento(string tenantId)
        {
            var agora = DateTime.Now;
            var hoje = agora.Date;
            
            // Buscar na fila de senhas
            var emAtendimento = _context.Set<SenhaFila>()
                .Count(s => s.TenantId == tenantId 
                    && s.DataHoraEntrada.Date == hoje
                    && s.Status == StatusSenha.EmAtendimento);

            if (emAtendimento > 0)
                return emAtendimento;

            // Fallback: agendamentos em progresso
            return _context.Appointments
                .Count(a => a.TenantId == tenantId 
                    && a.ScheduledDate.Date == hoje
                    && a.Status == AppointmentStatus.InProgress);
        }

        /// <summary>
        /// Calcula a taxa de atendimento no prazo (%)
        /// </summary>
        private decimal CalcularTaxaAtendimentoNoPrazo(List<SenhaFila> senhasFila, List<Appointment> agendamentos)
        {
            // Usar senhas de fila se disponíveis
            if (senhasFila.Any(s => s.DataHoraAtendimento.HasValue))
            {
                var atendimentosFinalizados = senhasFila
                    .Where(s => s.Status == StatusSenha.Atendido && s.DataHoraAtendimento.HasValue)
                    .ToList();

                if (!atendimentosFinalizados.Any())
                    return 0;

                // Considerar "no prazo" se o tempo de espera foi <= 30 minutos
                var noPrazo = atendimentosFinalizados
                    .Count(s => s.TempoEsperaMinutos <= 30);

                return (decimal)noPrazo / atendimentosFinalizados.Count * 100;
            }

            // Fallback: usar agendamentos completados
            var completados = agendamentos
                .Where(a => a.Status == AppointmentStatus.Completed && a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                .ToList();

            if (!completados.Any())
                return 0;

            // Considerar "no prazo" se a duração foi <= duração agendada + 15 minutos
            var noPrazoAgendamento = completados
                .Count(a => 
                {
                    var duracao = (a.CheckOutTime!.Value - a.CheckInTime!.Value).TotalMinutes;
                    return duracao <= a.DurationMinutes + 15;
                });

            return (decimal)noPrazoAgendamento / completados.Count * 100;
        }

        /// <summary>
        /// Obtém tempos médios por etapa do atendimento
        /// </summary>
        private List<TempoPorEtapaDto> GetTemposPorEtapa(List<SenhaFila> senhasFila)
        {
            var resultado = new List<TempoPorEtapaDto>();

            if (!senhasFila.Any())
                return resultado;

            // Etapa 1: Tempo de Espera na Fila
            var temposEspera = senhasFila
                .Where(s => s.TempoEsperaMinutos > 0)
                .Select(s => s.TempoEsperaMinutos)
                .ToList();

            if (temposEspera.Any())
            {
                resultado.Add(new TempoPorEtapaDto
                {
                    Etapa = "Espera na Fila",
                    TempoMedioMinutos = (int)temposEspera.Average(),
                    TempoMinimoMinutos = temposEspera.Min(),
                    TempoMaximoMinutos = temposEspera.Max()
                });
            }

            // Etapa 2: Tempo de Atendimento
            var temposAtendimento = senhasFila
                .Where(s => s.TempoAtendimentoMinutos > 0)
                .Select(s => s.TempoAtendimentoMinutos)
                .ToList();

            if (temposAtendimento.Any())
            {
                resultado.Add(new TempoPorEtapaDto
                {
                    Etapa = "Atendimento",
                    TempoMedioMinutos = (int)temposAtendimento.Average(),
                    TempoMinimoMinutos = temposAtendimento.Min(),
                    TempoMaximoMinutos = temposAtendimento.Max()
                });
            }

            return resultado;
        }

        /// <summary>
        /// Obtém performance por médico
        /// </summary>
        private List<PerformanceMedicoDto> GetPerformanceMedicos(List<Appointment> agendamentos, List<SenhaFila> senhasFila)
        {
            var resultado = new List<PerformanceMedicoDto>();

            // Agrupar por médico usando senhas de fila
            var senhasPorMedico = senhasFila
                .Where(s => s.MedicoId.HasValue && s.Medico != null)
                .GroupBy(s => new { s.MedicoId, s.Medico })
                .ToList();

            foreach (var grupo in senhasPorMedico)
            {
                var senhas = grupo.ToList();
                var medico = grupo.Key.Medico!;

                var temposEspera = senhas
                    .Where(s => s.TempoEsperaMinutos > 0)
                    .Select(s => s.TempoEsperaMinutos)
                    .ToList();

                var temposConsulta = senhas
                    .Where(s => s.TempoAtendimentoMinutos > 0)
                    .Select(s => s.TempoAtendimentoMinutos)
                    .ToList();

                // Calcular pontualidade: % de atendimentos que começaram com <= 15 min de atraso
                var atendimentosNoPrazo = senhas
                    .Count(s => s.TempoEsperaMinutos <= 15);

                resultado.Add(new PerformanceMedicoDto
                {
                    NomeMedico = medico.FullName,
                    Crm = medico.ProfessionalId ?? "N/A",
                    TotalConsultas = senhas.Count,
                    TempoMedioConsultaMinutos = temposConsulta.Any() ? (int)temposConsulta.Average() : 0,
                    TempoMedioEsperaMinutos = temposEspera.Any() ? (int)temposEspera.Average() : 0,
                    TaxaPontualidade = senhas.Count > 0 ? (decimal)atendimentosNoPrazo / senhas.Count * 100 : 0
                });
            }

            // Fallback: usar agendamentos se não houver senhas
            if (!resultado.Any())
            {
                var agendamentosPorMedico = agendamentos
                    .Where(a => a.ProfessionalId.HasValue && a.Professional != null)
                    .GroupBy(a => new { a.ProfessionalId, a.Professional })
                    .ToList();

                foreach (var grupo in agendamentosPorMedico)
                {
                    var consultas = grupo.ToList();
                    var medico = grupo.Key.Professional!;

                    var consultasCompletas = consultas
                        .Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                        .ToList();

                    resultado.Add(new PerformanceMedicoDto
                    {
                        NomeMedico = medico.FullName,
                        Crm = medico.ProfessionalId ?? "N/A",
                        TotalConsultas = consultas.Count,
                        TempoMedioConsultaMinutos = consultasCompletas.Any() 
                            ? (int)consultasCompletas.Average(a => (a.CheckOutTime!.Value - a.CheckInTime!.Value).TotalMinutes)
                            : 0,
                        TempoMedioEsperaMinutos = 0, // TODO: Calcular quando campo WaitTimeMinutes estiver disponível
                        TaxaPontualidade = 0 // TODO: Calcular baseado em horário agendado vs real
                    });
                }
            }

            return resultado.OrderByDescending(p => p.TotalConsultas).ToList();
        }

        /// <summary>
        /// Obtém distribuição de consultas por horário
        /// </summary>
        private List<DistribuicaoHorarioDto> GetDistribuicaoHorarios(List<Appointment> agendamentos, List<SenhaFila> senhasFila)
        {
            // Usar senhas de fila se disponíveis
            if (senhasFila.Any())
            {
                var porHora = senhasFila
                    .GroupBy(s => s.DataHoraEntrada.Hour)
                    .Select(g => new DistribuicaoHorarioDto
                    {
                        Hora = g.Key,
                        TotalConsultas = g.Count(),
                        TempoMedioEsperaMinutos = g.Any(s => s.TempoEsperaMinutos > 0)
                            ? (int)g.Where(s => s.TempoEsperaMinutos > 0).Average(s => s.TempoEsperaMinutos)
                            : 0
                    })
                    .OrderBy(d => d.Hora)
                    .ToList();

                return porHora;
            }

            // Fallback: usar agendamentos
            var porHoraAgendamento = agendamentos
                .GroupBy(a => a.ScheduledTime.Hours)
                .Select(g => new DistribuicaoHorarioDto
                {
                    Hora = g.Key,
                    TotalConsultas = g.Count(),
                    TempoMedioEsperaMinutos = 0 // TODO: Calcular quando dados estiverem disponíveis
                })
                .OrderBy(d => d.Hora)
                .ToList();

            return porHoraAgendamento;
        }

        /// <summary>
        /// Obtém tempos médios por especialidade
        /// </summary>
        private List<TempoPorEspecialidadeDto> GetTemposPorEspecialidade(List<Appointment> agendamentos, List<SenhaFila> senhasFila)
        {
            // Usar senhas com médico (que tem especialidade)
            var senhasComMedico = senhasFila
                .Where(s => s.Medico != null && !string.IsNullOrEmpty(s.Medico.Specialty))
                .ToList();

            if (senhasComMedico.Any())
            {
                var porEspecialidade = senhasComMedico
                    .GroupBy(s => s.Medico!.Specialty)
                    .Select(g => new TempoPorEspecialidadeDto
                    {
                        Especialidade = g.Key ?? "Não informada",
                        TempoMedioConsultaMinutos = g.Any(s => s.TempoAtendimentoMinutos > 0)
                            ? (int)g.Where(s => s.TempoAtendimentoMinutos > 0).Average(s => s.TempoAtendimentoMinutos)
                            : 0,
                        TempoMedioEsperaMinutos = g.Any(s => s.TempoEsperaMinutos > 0)
                            ? (int)g.Where(s => s.TempoEsperaMinutos > 0).Average(s => s.TempoEsperaMinutos)
                            : 0
                    })
                    .OrderByDescending(t => t.TempoMedioConsultaMinutos)
                    .ToList();

                return porEspecialidade;
            }

            // Fallback: usar agendamentos
            var agendamentosComEspecialidade = agendamentos
                .Where(a => a.Professional != null && !string.IsNullOrEmpty(a.Professional.Specialty))
                .ToList();

            var porEspecialidadeAgendamento = agendamentosComEspecialidade
                .GroupBy(a => a.Professional!.Specialty)
                .Select(g => new TempoPorEspecialidadeDto
                {
                    Especialidade = g.Key ?? "Não informada",
                    TempoMedioConsultaMinutos = g.Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue).Any()
                        ? (int)g.Where(a => a.CheckInTime.HasValue && a.CheckOutTime.HasValue)
                            .Average(a => (a.CheckOutTime!.Value - a.CheckInTime!.Value).TotalMinutes)
                        : 0,
                    TempoMedioEsperaMinutos = 0 // TODO: Calcular quando dados estiverem disponíveis
                })
                .OrderByDescending(t => t.TempoMedioConsultaMinutos)
                .ToList();

            return porEspecialidadeAgendamento;
        }

        /// <summary>
        /// Obtém tendência de tempo de espera ao longo do período
        /// </summary>
        private List<TendenciaTempoEsperaDto> GetTendenciaTempoEspera(
            List<SenhaFila> senhasFila, 
            List<Appointment> agendamentos, 
            DateTime inicio, 
            DateTime fim)
        {
            var resultado = new List<TendenciaTempoEsperaDto>();

            // Agrupar por dia
            var dataAtual = inicio.Date;
            while (dataAtual <= fim.Date)
            {
                // Usar senhas de fila
                var senhasDoDia = senhasFila
                    .Where(s => s.DataHoraEntrada.Date == dataAtual)
                    .ToList();

                if (senhasDoDia.Any())
                {
                    var temposEspera = senhasDoDia
                        .Where(s => s.TempoEsperaMinutos > 0)
                        .Select(s => s.TempoEsperaMinutos)
                        .ToList();

                    resultado.Add(new TendenciaTempoEsperaDto
                    {
                        Data = dataAtual.ToString("dd/MM/yyyy"),
                        TempoMedioEsperaMinutos = temposEspera.Any() ? (int)temposEspera.Average() : 0,
                        TotalConsultas = senhasDoDia.Count
                    });
                }
                else
                {
                    // Fallback: usar agendamentos
                    var agendamentosDoDia = agendamentos
                        .Where(a => a.ScheduledDate.Date == dataAtual)
                        .ToList();

                    resultado.Add(new TendenciaTempoEsperaDto
                    {
                        Data = dataAtual.ToString("dd/MM/yyyy"),
                        TempoMedioEsperaMinutos = 0, // TODO: Calcular quando dados estiverem disponíveis
                        TotalConsultas = agendamentosDoDia.Count
                    });
                }

                dataAtual = dataAtual.AddDays(1);
            }

            return resultado;
        }
    }
}
