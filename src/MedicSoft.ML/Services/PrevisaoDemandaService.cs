using Microsoft.Extensions.Logging;
using Microsoft.ML;
using MedicSoft.ML.Models;

namespace MedicSoft.ML.Services
{
    public interface IPrevisaoDemandaService
    {
        Task TreinarModeloAsync(IEnumerable<DadosTreinamentoDemanda> dadosTreinamento);
        Task<bool> CarregarModeloAsync();
        PrevisaoConsultas PreverProximaSemana();
        int PreverParaData(DateTime data);
    }

    /// <summary>
    /// Service for demand forecasting using ML.NET
    /// Predicts number of consultations based on historical data
    /// Thread-safe implementation with proper locking
    /// </summary>
    public class PrevisaoDemandaService : IPrevisaoDemandaService
    {
        private readonly MLContext _mlContext;
        private readonly ILogger<PrevisaoDemandaService> _logger;
        private ITransformer? _model;
        private readonly string _modelPath;
        private readonly object _modelLock = new object();

        public PrevisaoDemandaService(ILogger<PrevisaoDemandaService> logger)
        {
            _mlContext = new MLContext(seed: 0);
            _logger = logger;
            _modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MLModels", "modelo_demanda.zip");
        }

        /// <summary>
        /// Trains the demand forecasting model with historical data
        /// Thread-safe implementation with locking
        /// </summary>
        public async Task TreinarModeloAsync(IEnumerable<DadosTreinamentoDemanda> dadosTreinamento)
        {
            try
            {
                _logger.LogInformation("Iniciando treinamento do modelo de previsão de demanda");

                var dataView = _mlContext.Data.LoadFromEnumerable(dadosTreinamento);

                // Split data for training and testing
                var trainTestData = _mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                // Build ML pipeline
                var pipeline = _mlContext.Transforms.Concatenate("Features",
                        nameof(DadosTreinamentoDemanda.Mes),
                        nameof(DadosTreinamentoDemanda.DiaSemana),
                        nameof(DadosTreinamentoDemanda.Semana),
                        nameof(DadosTreinamentoDemanda.IsFeriado),
                        nameof(DadosTreinamentoDemanda.TemperaturaMedia))
                    .Append(_mlContext.Regression.Trainers.FastTree(
                        labelColumnName: "Label",
                        featureColumnName: "Features",
                        numberOfLeaves: 20,
                        minimumExampleCountPerLeaf: 10,
                        numberOfTrees: 100));

                // Train model
                var trainedModel = pipeline.Fit(trainTestData.TrainSet);

                // Evaluate model
                var predictions = trainedModel.Transform(trainTestData.TestSet);
                var metrics = _mlContext.Regression.Evaluate(predictions, labelColumnName: "Label");

                _logger.LogInformation(
                    "Modelo treinado com sucesso. Métricas: R²={RSquared}, MAE={Mae}, RMSE={Rmse}",
                    metrics.RSquared,
                    metrics.MeanAbsoluteError,
                    metrics.RootMeanSquaredError);

                // Save model
                var directory = Path.GetDirectoryName(_modelPath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await Task.Run(() => _mlContext.Model.Save(trainedModel, dataView.Schema, _modelPath));
                
                // Update model with thread-safety
                lock (_modelLock)
                {
                    _model = trainedModel;
                }
                
                _logger.LogInformation("Modelo salvo em: {ModelPath}", _modelPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo de previsão de demanda");
                throw;
            }
        }

        /// <summary>
        /// Loads the trained model from disk
        /// Thread-safe implementation with locking
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

                ITransformer? loadedModel = null;
                await Task.Run(() =>
                {
                    loadedModel = _mlContext.Model.Load(_modelPath, out var modelSchema);
                });

                // Verify model was loaded successfully
                if (loadedModel == null)
                {
                    _logger.LogError("Falha ao carregar modelo de: {ModelPath}", _modelPath);
                    return false;
                }

                // Update model with thread-safety
                lock (_modelLock)
                {
                    _model = loadedModel;
                }

                _logger.LogInformation("Modelo carregado com sucesso de: {ModelPath}", _modelPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modelo de previsão de demanda");
                return false;
            }
        }

        /// <summary>
        /// Predicts the number of consultations for the next 7 days
        /// Thread-safe implementation with locking
        /// NOTE: PredictionEngine creation per call is inefficient but thread-safe
        /// For high-frequency predictions, consider using PredictionEnginePool
        /// </summary>
        public PrevisaoConsultas PreverProximaSemana()
        {
            lock (_modelLock)
            {
                if (_model == null)
                {
                    throw new InvalidOperationException("Modelo não treinado ou carregado");
                }

                var predictionEngine = _mlContext.Model
                    .CreatePredictionEngine<DadosTreinamentoDemanda, PrevisaoConsultaResult>(_model);

                var proximaSemana = new List<PrevisaoDia>();

                for (int i = 1; i <= 7; i++)
                {
                    var data = DateTime.Now.Date.AddDays(i);
                    var input = new DadosTreinamentoDemanda
                    {
                        Mes = data.Month,
                        DiaSemana = (int)data.DayOfWeek,
                        Semana = GetNumeroSemana(data),
                        IsFeriado = IsFeriado(data) ? 1 : 0,
                        TemperaturaMedia = 25 // Default or integrate with weather API
                    };

                    var previsao = predictionEngine.Predict(input);

                    proximaSemana.Add(new PrevisaoDia
                    {
                        Data = data,
                        ConsultasPrevistas = (int)Math.Round(Math.Max(0, previsao.NumeroConsultas)),
                        ConfiancaPrevisao = 0.8f // Placeholder, calculate based on model metrics
                    });
                }

                return new PrevisaoConsultas
                {
                    Periodo = "Próxima Semana",
                    Previsoes = proximaSemana,
                    TotalPrevisto = proximaSemana.Sum(p => p.ConsultasPrevistas)
                };
            }
        }

        /// <summary>
        /// Predicts consultations for a specific date
        /// Thread-safe implementation with locking
        /// </summary>
        public int PreverParaData(DateTime data)
        {
            lock (_modelLock)
            {
                if (_model == null)
                {
                    throw new InvalidOperationException("Modelo não treinado ou carregado");
                }

                var predictionEngine = _mlContext.Model
                    .CreatePredictionEngine<DadosTreinamentoDemanda, PrevisaoConsultaResult>(_model);

                var input = new DadosTreinamentoDemanda
                {
                    Mes = data.Month,
                    DiaSemana = (int)data.DayOfWeek,
                    Semana = GetNumeroSemana(data),
                    IsFeriado = IsFeriado(data) ? 1 : 0,
                    TemperaturaMedia = 25
                };

                var previsao = predictionEngine.Predict(input);
                return (int)Math.Round(Math.Max(0, previsao.NumeroConsultas));
            }
        }

        private int GetNumeroSemana(DateTime data)
        {
            var jan1 = new DateTime(data.Year, 1, 1);
            var dias = (data - jan1).Days;
            return dias / 7 + 1;
        }

        private bool IsFeriado(DateTime data)
        {
            // TODO: Integrate with holiday API or database
            // For now, just check basic fixed holidays
            var feriadosFixos = new[]
            {
                new DateTime(data.Year, 1, 1),  // Ano Novo
                new DateTime(data.Year, 4, 21), // Tiradentes
                new DateTime(data.Year, 5, 1),  // Dia do Trabalho
                new DateTime(data.Year, 9, 7),  // Independência
                new DateTime(data.Year, 10, 12), // Nossa Senhora
                new DateTime(data.Year, 11, 2), // Finados
                new DateTime(data.Year, 11, 15), // Proclamação da República
                new DateTime(data.Year, 12, 25)  // Natal
            };

            return feriadosFixos.Contains(data.Date);
        }
    }
}
