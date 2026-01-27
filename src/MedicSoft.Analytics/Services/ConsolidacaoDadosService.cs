using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Analytics.Services
{
    public class ConsolidacaoDadosService : IConsolidacaoDadosService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ConsolidacaoDadosService> _logger;

        public ConsolidacaoDadosService(
            MedicSoftDbContext context,
            ILogger<ConsolidacaoDadosService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task ConsolidarDadosDiarioAsync(DateTime data, string tenantId)
        {
            try
            {
                _logger.LogInformation("Iniciando consolidação de dados para {Data} - Tenant: {TenantId}", data.Date, tenantId);

                var dataInicio = data.Date;
                var dataFim = data.Date.AddDays(1);

                // Buscar todos os agendamentos do dia
                var appointments = await _context.Appointments
                    .Where(a => a.TenantId == tenantId 
                        && a.ScheduledDate >= dataInicio 
                        && a.ScheduledDate < dataFim)
                    .Include(a => a.Patient)
                    .Include(a => a.HealthInsurancePlan)
                    .ToListAsync();

                // Buscar pagamentos do dia
                var payments = await _context.Payments
                    .Where(p => p.TenantId == tenantId 
                        && p.PaymentDate >= dataInicio 
                        && p.PaymentDate < dataFim
                        && p.AppointmentId.HasValue)
                    .ToListAsync();

                // Agrupar por clínica e médico
                var groupedData = appointments
                    .GroupBy(a => new { a.ClinicId, a.ProfessionalId })
                    .ToList();

                foreach (var group in groupedData)
                {
                    var consultas = group.ToList();
                    var consultaIds = consultas.Select(c => c.Id).ToList();
                    var paymentsForGroup = payments.Where(p => consultaIds.Contains(p.AppointmentId.Value)).ToList();

                    var consolidado = new ConsultaDiaria(data.Date, tenantId)
                    {
                        ClinicaId = group.Key.ClinicId,
                        MedicoId = group.Key.ProfessionalId,
                        
                        TotalConsultas = consultas.Count,
                        ConsultasRealizadas = consultas.Count(c => c.Status == AppointmentStatus.Completed),
                        ConsultasCanceladas = consultas.Count(c => c.Status == AppointmentStatus.Cancelled),
                        NoShow = consultas.Count(c => c.Status == AppointmentStatus.NoShow),
                        
                        ReceitaTotal = paymentsForGroup.Sum(p => p.Amount),
                        ReceitaRecebida = paymentsForGroup.Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount),
                        ReceitaPendente = paymentsForGroup.Where(p => p.Status == PaymentStatus.Pending).Sum(p => p.Amount),
                        
                        TempoMedioEsperaMinutos = CalcularTempoMedioEspera(consultas),
                        TempoMedioConsultaMinutos = CalcularTempoMedioConsulta(consultas),
                        
                        TotalPacientesNovos = await ContarPacientesNovos(consultas, dataInicio),
                        TotalPacientesRetorno = await ContarPacientesRetorno(consultas, dataInicio),
                        
                        TotalAvaliacoes = 0,
                        NpsMedio = null
                    };

                    // Verificar se já existe consolidação para este dia/clínica/médico
                    var existente = await _context.Set<ConsultaDiaria>()
                        .FirstOrDefaultAsync(c => 
                            c.Data == data.Date 
                            && c.TenantId == tenantId
                            && c.ClinicaId == group.Key.ClinicId 
                            && c.MedicoId == group.Key.ProfessionalId);

                    if (existente != null)
                    {
                        // Atualizar existente
                        existente.TotalConsultas = consolidado.TotalConsultas;
                        existente.ConsultasRealizadas = consolidado.ConsultasRealizadas;
                        existente.ConsultasCanceladas = consolidado.ConsultasCanceladas;
                        existente.NoShow = consolidado.NoShow;
                        existente.ReceitaTotal = consolidado.ReceitaTotal;
                        existente.ReceitaRecebida = consolidado.ReceitaRecebida;
                        existente.ReceitaPendente = consolidado.ReceitaPendente;
                        existente.TempoMedioEsperaMinutos = consolidado.TempoMedioEsperaMinutos;
                        existente.TempoMedioConsultaMinutos = consolidado.TempoMedioConsultaMinutos;
                        existente.TotalPacientesNovos = consolidado.TotalPacientesNovos;
                        existente.TotalPacientesRetorno = consolidado.TotalPacientesRetorno;
                        existente.UltimaAtualizacao = DateTime.UtcNow;
                        
                        _context.Set<ConsultaDiaria>().Update(existente);
                    }
                    else
                    {
                        await _context.Set<ConsultaDiaria>().AddAsync(consolidado);
                    }
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Consolidação de dados concluída para {Data} - {Count} registros processados", 
                    data.Date, groupedData.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consolidar dados para {Data} - Tenant: {TenantId}", data.Date, tenantId);
                throw;
            }
        }

        public async Task ConsolidarPeriodoAsync(DateTime dataInicio, DateTime dataFim, string tenantId)
        {
            _logger.LogInformation("Consolidando período de {Inicio} até {Fim}", dataInicio, dataFim);
            
            var dataAtual = dataInicio.Date;
            while (dataAtual <= dataFim.Date)
            {
                await ConsolidarDadosDiarioAsync(dataAtual, tenantId);
                dataAtual = dataAtual.AddDays(1);
            }
            
            _logger.LogInformation("Consolidação de período concluída");
        }

        private int CalcularTempoMedioEspera(System.Collections.Generic.List<Appointment> consultas)
        {
            var consultasComCheckin = consultas
                .Where(c => c.CheckInTime.HasValue)
                .ToList();

            if (!consultasComCheckin.Any())
                return 0;

            var tempos = consultasComCheckin.Select(c =>
            {
                var agendado = c.ScheduledDate.Add(c.ScheduledTime);
                var checkin = c.CheckInTime!.Value;
                return (checkin - agendado).TotalMinutes;
            }).Where(t => t >= 0).ToList();

            return tempos.Any() ? (int)tempos.Average() : 0;
        }

        private int CalcularTempoMedioConsulta(System.Collections.Generic.List<Appointment> consultas)
        {
            var consultasCompletas = consultas
                .Where(c => c.CheckInTime.HasValue && c.CheckOutTime.HasValue)
                .ToList();

            if (!consultasCompletas.Any())
                return 0;

            var tempos = consultasCompletas.Select(c =>
                (c.CheckOutTime!.Value - c.CheckInTime!.Value).TotalMinutes
            ).Where(t => t > 0).ToList();

            return tempos.Any() ? (int)tempos.Average() : 0;
        }

        private async Task<int> ContarPacientesNovos(System.Collections.Generic.List<Appointment> consultas, DateTime data)
        {
            var pacienteIds = consultas.Select(c => c.PatientId).Distinct().ToList();
            
            // Buscar primeiras consultas de todos os pacientes de uma vez (otimizado)
            var primeirasConsultas = await _context.Appointments
                .Where(a => pacienteIds.Contains(a.PatientId) && a.Status == AppointmentStatus.Completed)
                .GroupBy(a => a.PatientId)
                .Select(g => new { PatientId = g.Key, PrimeiraData = g.Min(a => a.ScheduledDate) })
                .ToListAsync();

            return primeirasConsultas.Count(pc => pc.PrimeiraData.Date == data.Date);
        }

        private async Task<int> ContarPacientesRetorno(System.Collections.Generic.List<Appointment> consultas, DateTime data)
        {
            var pacienteIds = consultas.Select(c => c.PatientId).Distinct().ToList();
            
            // Buscar pacientes com consultas anteriores de uma vez (otimizado)
            var pacientesComConsultasAnteriores = await _context.Appointments
                .Where(a => pacienteIds.Contains(a.PatientId)
                    && a.Status == AppointmentStatus.Completed
                    && a.ScheduledDate < data.Date)
                .Select(a => a.PatientId)
                .Distinct()
                .ToListAsync();

            return pacientesComConsultasAnteriores.Count;
        }
    }
}
