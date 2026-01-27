using Microsoft.Extensions.Logging;
using Microsoft.ML;
using MedicSoft.ML.Models;

namespace MedicSoft.ML.Services
{
    public interface IPrevisaoNoShowService
    {
        Task TreinarModeloAsync(IEnumerable<DadosNoShow> dadosTreinamento);
        Task<bool> CarregarModeloAsync();
        double CalcularRiscoNoShow(DadosNoShow dados);
        List<string> SugerirAcoes(double riscoNoShow);
        List<(DadosNoShow dados, double risco)> IdentificarAgendamentosAltoRisco(
            IEnumerable<DadosNoShow> agendamentos, 
            double threshold = 0.5);
    }

    /// <summary>
    /// Service for no-show prediction using ML.NET
    /// Predicts probability of patient not showing up for appointment
    /// </summary>
    public class PrevisaoNoShowService : IPrevisaoNoShowService
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<PrevisaoNoShowService> _logger;
        private ITransformer? _model;
        private readonly string _modelPath;

        public PrevisaoNoShowService(ILogger<PrevisaoNoShowService> logger)
        {
            _mlContext = new MLContext(seed: 0);
            _logger = logger;
            _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MLModels", "modelo_noshow.zip");
        }

        /// <summary>
        /// Trains the no-show prediction model
        /// </summary>
        public async Task TreinarModeloAsync(IEnumerable<DadosNoShow> dadosTreinamento)
        {
            try
            {
                _logger.LogInformation("Iniciando treinamento do modelo de previsão de no-show");

                var dataView = _mlContext.Data.LoadFromEnumerable(dadosTreinamento);

                // Split data for training and testing
                var trainTestData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                // Build ML pipeline for binary classification
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(DadosNoShow.IdadePaciente),
                        nameof(DadosNoShow.DiasAteConsulta),
                        nameof(DadosNoShow.HoraDia),
                        nameof(DadosNoShow.HistoricoNoShow),
                        nameof(DadosNoShow.TempoDesdeUltimaConsulta),
                        nameof(DadosNoShow.IsConvenio),
                        nameof(DadosNoShow.TemLembrete))
                    .Append(_mlContext.BinaryClassification.Trainers.FastTree(
                        labelColumnName: "Label",
                        featureColumnName: "Features",
                        numberOfLeaves: 20,
                        minimumExampleCountPerLeaf: 10,
                        numberOfTrees: 100));

                // Train model
                _model = pipeline.Fit(trainTestData.TrainSet);

                // Evaluate model
                var predictions = _model.Transform(trainTestData.TestSet);
                var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");

                _logger.LogInformation(
                    "Modelo treinado com sucesso. Métricas: Accuracy={Accuracy}, AUC={Auc}, F1={F1}",
                    metrics.Accuracy,
                    metrics.AreaUnderRocCurve,
                    metrics.F1Score);

                // Save model
                var directory = Path.GetDirectoryName(_modelPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await Task.Run(() => _mlContext.Model.Save(_model, dataView.Schema, _modelPath));
                
                _logger.LogInformation("Modelo salvo em: {ModelPath}", _modelPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo de previsão de no-show");
                throw;
            }
        }

        /// <summary>
        /// Loads the trained model from disk
        /// </summary>
        public async Task<bool> CarregarModeloAsync()
        {
            try
            {
                if (!File.Exists(_modelPath))
                {
                    _logger.LogWarning("Modelo não encontrado em: {ModelPath}", _modelPath);
                    return false;
                }

                await Task.Run(() =>
                {
                    _model = _mlContext.Model.Load(_modelPath, out var modelSchema);
                });

                _logger.LogInformation("Modelo carregado com sucesso de: {ModelPath}", _modelPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modelo de previsão de no-show");
                return false;
            }
        }

        /// <summary>
        /// Calculates no-show risk for a specific appointment
        /// Returns probability between 0 (will attend) and 1 (will not attend)
        /// </summary>
        public double CalcularRiscoNoShow(DadosNoShow dados)
        {
            if (_model == null)
            {
                throw new InvalidOperationException("Modelo não treinado ou carregado");
            }

            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<DadosNoShow, PrevisaoNoShowResult>(_model);

            var previsao = predictionEngine.Predict(dados);
            
            // Return probability of NOT showing (1 - probability of showing)
            return previsao.VaiComparecer ? 1 - previsao.Probability : previsao.Probability;
        }

        /// <summary>
        /// Gets recommended actions based on no-show risk level
        /// </summary>
        public List<string> SugerirAcoes(double riscoNoShow)
        {
            var acoes = new List<string>();

            if (riscoNoShow > 0.7)
            {
                acoes.Add("🔴 ALTO RISCO: Ligar para confirmar presença");
                acoes.Add("Oferecer reagendamento se necessário");
                acoes.Add("Confirmar endereço e horário");
            }
            else if (riscoNoShow > 0.5)
            {
                acoes.Add("🟡 MÉDIO RISCO: Enviar lembrete adicional por WhatsApp");
                acoes.Add("Confirmar 2 horas antes da consulta");
            }
            else if (riscoNoShow > 0.3)
            {
                acoes.Add("🟢 BAIXO RISCO: Enviar lembrete padrão 24h antes");
            }
            else
            {
                acoes.Add("✅ RISCO MUITO BAIXO: Lembrete padrão suficiente");
            }

            return acoes;
        }

        /// <summary>
        /// Batch prediction for multiple appointments
        /// Returns only high-risk appointments (risk > threshold)
        /// </summary>
        public List<(DadosNoShow dados, double risco)> IdentificarAgendamentosAltoRisco(
            IEnumerable<DadosNoShow> agendamentos,
            double threshold = 0.5)
        {
            if (_model == null)
            {
                throw new InvalidOperationException("Modelo não treinado ou carregado");
            }

            var predictionEngine = _mlContext.Model
                .CreatePredictionEngine<DadosNoShow, PrevisaoNoShowResult>(_model);

            var altosRiscos = new List<(DadosNoShow, double)>();

            foreach (var agendamento in agendamentos)
            {
                var previsao = predictionEngine.Predict(agendamento);
                var risco = previsao.VaiComparecer ? 1 - previsao.Probability : previsao.Probability;

                if (risco > threshold)
                {
                    altosRiscos.Add((agendamento, risco));
                }
            }

            return altosRiscos.OrderByDescending(x => x.Item2).ToList();
        }
    }
}
