using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Base TISS webservice client with retry logic and error handling
    /// </summary>
    public class TissWebServiceClient : ITissWebServiceClient
    {
        private readonly ILogger<TissWebServiceClient> _logger;

        public TissWebServiceClient(ILogger<TissWebServiceClient> logger)
        {
            _logger = logger;
        }

        public virtual async Task<TissRetornoLote> EnviarLoteAsync(Guid loteId)
        {
            _logger.LogInformation("Sending batch {LoteId} to operator", loteId);
            
            return await ExecutarComRetryAsync(async () =>
            {
                // Base implementation - to be overridden by specific operator clients
                await Task.Delay(100);
                
                return new TissRetornoLote
                {
                    ProtocoloOperadora = Guid.NewGuid().ToString(),
                    DataRecebimento = DateTime.UtcNow,
                    Status = "Recebido",
                    Mensagem = "Lote recebido com sucesso"
                };
            });
        }

        public virtual async Task<TissRetornoLote> ConsultarLoteAsync(string protocoloOperadora)
        {
            _logger.LogInformation("Querying batch with protocol {Protocolo}", protocoloOperadora);
            
            return await ExecutarComRetryAsync(async () =>
            {
                await Task.Delay(100);
                
                return new TissRetornoLote
                {
                    ProtocoloOperadora = protocoloOperadora,
                    DataRecebimento = DateTime.UtcNow,
                    Status = "Processado"
                };
            });
        }

        public virtual async Task<TissRetornoGuia> ConsultarGuiaAsync(string numeroGuia)
        {
            _logger.LogInformation("Querying guide {NumeroGuia}", numeroGuia);
            
            return await ExecutarComRetryAsync(async () =>
            {
                await Task.Delay(100);
                
                return new TissRetornoGuia
                {
                    NumeroGuia = numeroGuia,
                    Status = "Aprovada"
                };
            });
        }

        public virtual async Task<bool> CancelarGuiaAsync(string numeroGuia, string motivo)
        {
            _logger.LogInformation("Canceling guide {NumeroGuia}. Reason: {Motivo}", numeroGuia, motivo);
            
            return await ExecutarComRetryAsync(async () =>
            {
                await Task.Delay(100);
                return true;
            });
        }

        public virtual async Task<TissRetornoRecurso> EnviarRecursoAsync(Guid recursoId)
        {
            _logger.LogInformation("Sending appeal {RecursoId}", recursoId);
            
            return await ExecutarComRetryAsync(async () =>
            {
                await Task.Delay(100);
                
                return new TissRetornoRecurso
                {
                    ProtocoloRecurso = Guid.NewGuid().ToString(),
                    DataRecebimento = DateTime.UtcNow,
                    Status = "Recebido"
                };
            });
        }

        /// <summary>
        /// Execute operation with retry policy
        /// </summary>
        protected async Task<T> ExecutarComRetryAsync<T>(Func<Task<T>> operation, int maxRetries = 3)
        {
            int attempts = 0;
            Exception? lastException = null;

            while (attempts < maxRetries)
            {
                try
                {
                    attempts++;
                    return await operation();
                }
                catch (HttpRequestException ex)
                {
                    lastException = ex;
                    _logger.LogWarning(
                        "Attempt {Attempt} of {MaxRetries} failed: {Error}",
                        attempts, maxRetries, ex.Message);
                    
                    if (attempts < maxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempts));
                        _logger.LogInformation("Waiting {Delay}s before retry...", delay.TotalSeconds);
                        await Task.Delay(delay);
                    }
                }
                catch (TimeoutException ex)
                {
                    lastException = ex;
                    _logger.LogWarning(
                        "Attempt {Attempt} of {MaxRetries} timed out: {Error}",
                        attempts, maxRetries, ex.Message);
                    
                    if (attempts < maxRetries)
                    {
                        var delay = TimeSpan.FromSeconds(Math.Pow(2, attempts));
                        await Task.Delay(delay);
                    }
                }
            }

            _logger.LogError(lastException, "All {MaxRetries} attempts failed", maxRetries);
            throw new InvalidOperationException($"Operation failed after {maxRetries} attempts", lastException);
        }
    }
}
