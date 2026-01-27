using Microsoft.ML.Data;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.ML.Models
{
    /// <summary>
    /// Training data for no-show prediction
    /// </summary>
    public class DadosNoShow
    {
        [LoadColumn(0)]
        [Range(0, 120, ErrorMessage = "Idade deve estar entre 0 e 120 anos")]
        public float IdadePaciente { get; set; }
        
        [LoadColumn(1)]
        [Range(0, 365, ErrorMessage = "Dias até consulta deve estar entre 0 e 365")]
        public float DiasAteConsulta { get; set; }
        
        [LoadColumn(2)]
        [Range(0, 23, ErrorMessage = "Hora do dia deve estar entre 0 e 23")]
        public float HoraDia { get; set; }
        
        [LoadColumn(3)]
        [Range(0, 1, ErrorMessage = "Histórico de no-show deve estar entre 0 e 1")]
        public float HistoricoNoShow { get; set; } // % de no-show anterior
        
        [LoadColumn(4)]
        [Range(0, 9999, ErrorMessage = "Tempo desde última consulta deve ser positivo")]
        public float TempoDesdeUltimaConsulta { get; set; } // dias
        
        [LoadColumn(5)]
        [Range(0, 1, ErrorMessage = "IsConvenio deve ser 0 ou 1")]
        public float IsConvenio { get; set; }
        
        [LoadColumn(6)]
        [Range(0, 1, ErrorMessage = "TemLembrete deve ser 0 ou 1")]
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
