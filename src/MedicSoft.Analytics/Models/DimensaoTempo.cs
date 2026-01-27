using System;

namespace MedicSoft.Analytics.Models
{
    /// <summary>
    /// Time dimension table for analytics
    /// </summary>
    public class DimensaoTempo
    {
        public DateTime Data { get; set; }
        public int Ano { get; set; }
        public int Mes { get; set; }
        public int Dia { get; set; }
        public int DiaSemana { get; set; } // 0 = Sunday, 6 = Saturday
        public int Semana { get; set; }
        public int Trimestre { get; set; }
        public bool IsFeriado { get; set; }
        public bool IsFimDeSemana { get; set; }
        public string NomeMes { get; set; }
        public string NomeDiaSemana { get; set; }
        public string AnoMes { get; set; } // Format: "2026-01"
    }
}
