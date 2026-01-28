using Polly;
using Polly.Retry;

namespace MedicSoft.Api.Configuration
{
    /// <summary>
    /// Resilience policies for external service integrations
    /// Implements retry logic with exponential backoff
    /// </summary>
    public static class ResiliencePolicies
    {
        /// <summary>
        /// Creates a generic async retry policy for transient errors
        /// Retries on common transient exceptions with exponential backoff
        /// </summary>
        public static ResiliencePipeline CreateGenericRetryPolicy()
        {
            return new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder()
                        .Handle<HttpRequestException>() // Network failures
                        .Handle<TaskCanceledException>() // Timeouts
                        .Handle<TimeoutException>() // Explicit timeouts
                        .Handle<TransientMessagingException>() // Custom transient errors
                })
                .Build();
        }
    }

    /// <summary>
    /// Custom exception for transient messaging errors that should be retried
    /// </summary>
    public class TransientMessagingException : Exception
    {
        public TransientMessagingException(string message) : base(message) { }
        public TransientMessagingException(string message, Exception innerException) : base(message, innerException) { }
    }
}
