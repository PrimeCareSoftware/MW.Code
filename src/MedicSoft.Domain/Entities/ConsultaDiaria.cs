using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Consolidated daily consultation metrics for analytics
    /// </summary>
    public class ConsultaDiaria : BaseEntity
    {
        public DateTime Data { get; set; }
        public Guid? ClinicaId { get; set; }
        public Guid? MedicoId { get; set; }
        public Guid? EspecialidadeId { get; set; }
        
        // Consultation counts
        public int TotalConsultas { get; set; }
        public int ConsultasRealizadas { get; set; }
        public int ConsultasCanceladas { get; set; }
        public int NoShow { get; set; }
        
        // Revenue metrics
        public decimal ReceitaTotal { get; set; }
        public decimal ReceitaRecebida { get; set; }
        public decimal ReceitaPendente { get; set; }
        
        // Time metrics
        public int TempoMedioEsperaMinutos { get; set; }
        public int TempoMedioConsultaMinutos { get; set; }
        
        // Patient metrics
        public int TotalPacientesNovos { get; set; }
        public int TotalPacientesRetorno { get; set; }
        
        // Quality metrics
        public decimal? NpsMedio { get; set; }
        public int TotalAvaliacoes { get; set; }
        
        // Timestamps
        public DateTime DataConsolidacao { get; set; }
        public DateTime UltimaAtualizacao { get; set; }

        // EF Core constructor
        private ConsultaDiaria() : base() { }

        public ConsultaDiaria(DateTime data, string tenantId) : base(tenantId)
        {
            Data = data;
            DataConsolidacao = DateTime.UtcNow;
            UltimaAtualizacao = DateTime.UtcNow;
        }
    }
}
