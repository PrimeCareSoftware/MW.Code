using Microsoft.ML.Data;

namespace MedicSoft.ML.Models
{
    /// <summary>
    /// Training data model for demand forecasting
    /// Used to train ML model to predict number of consultations
    /// </summary>
    public class DadosTreinamentoDemanda
    {
        [LoadColumn(0)]
        public float Mes { get; set; }
        
        [LoadColumn(1)]
        public float DiaSemana { get; set; }
        
        [LoadColumn(2)]
        public float Semana { get; set; }
        
        [LoadColumn(3)]
        public float IsFeriado { get; set; }
        
        [LoadColumn(4)]
        public float TemperaturaMedia { get; set; }
        
        [LoadColumn(5)]
        [ColumnName("Label")]
        public float NumeroConsultas { get; set; }
    }

    /// <summary>
    /// Prediction result for demand forecasting
    /// </summary>
    public class PrevisaoConsultaResult
    {
        [ColumnName("Score")]
        public float NumeroConsultas { get; set; }
    }

    /// <summary>
    /// Prediction for a specific day
    /// </summary>
    public class PrevisaoDia
    {
        public DateTime Data { get; set; }
        public int ConsultasPrevistas { get; set; }
        public float ConfiancaPrevisao { get; set; }
    }

    /// <summary>
    /// Prediction result for a period
    /// </summary>
    public class PrevisaoConsultas
    {
        public string Periodo { get; set; } = string.Empty;
        public List<PrevisaoDia> Previsoes { get; set; } = new();
        public int TotalPrevisto { get; set; }
    }
}
