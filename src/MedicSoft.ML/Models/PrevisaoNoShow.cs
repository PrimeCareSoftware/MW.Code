using Microsoft.ML.Data;

namespace MedicSoft.ML.Models
{
    /// <summary>
    /// Training data for no-show prediction
    /// </summary>
    public class DadosNoShow
    {
        [LoadColumn(0)]
        public float IdadePaciente { get; set; }
        
        [LoadColumn(1)]
        public float DiasAteConsulta { get; set; }
        
        [LoadColumn(2)]
        public float HoraDia { get; set; }
        
        [LoadColumn(3)]
        public float HistoricoNoShow { get; set; } // % de no-show anterior
        
        [LoadColumn(4)]
        public float TempoDesdeUltimaConsulta { get; set; } // dias
        
        [LoadColumn(5)]
        public float IsConvenio { get; set; }
        
        [LoadColumn(6)]
        public float TemLembrete { get; set; }
        
        [LoadColumn(7)]
        [ColumnName("Label")]
        public bool VaiComparecer { get; set; }
    }

    /// <summary>
    /// Prediction result for no-show
    /// </summary>
    public class PrevisaoNoShowResult
    {
        [ColumnName("PredictedLabel")]
        public bool VaiComparecer { get; set; }
        
        [ColumnName("Probability")]
        public float Probability { get; set; }
        
        [ColumnName("Score")]
        public float Score { get; set; }
    }

    /// <summary>
    /// Appointment with no-show risk
    /// </summary>
    public class AgendamentoRisco
    {
        public Guid AgendamentoId { get; set; }
        public string PacienteNome { get; set; } = string.Empty;
        public DateTime DataHora { get; set; }
        public double RiscoNoShow { get; set; }
        public List<string> AcoesRecomendadas { get; set; } = new();
    }
}
