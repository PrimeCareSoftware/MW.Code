using Polly;
using Polly.Retry;

namespace MedicSoft.Api.Configuration
{
    /// <summary>
    /// Resilience policies for external service integrations
    /// Implements retry logic with exponential backoff and rate limiting
    /// </summary>
    public static class ResiliencePolicies
    {
        /// <summary>
        /// Creates a retry policy with exponential backoff for email services
        /// </summary>
        public static ResiliencePipeline<HttpResponseMessage> CreateEmailRetryPolicy()
        {
            return new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(response => 
                            (int)response.StatusCode >= 500 || // Server errors
                            response.StatusCode == System.Net.HttpStatusCode.TooManyRequests) // Rate limiting
                })
                .Build();
        }

        /// <summary>
        /// Creates a retry policy with exponential backoff for SMS services
        /// </summary>
        public static ResiliencePipeline<HttpResponseMessage> CreateSmsRetryPolicy()
        {
            return new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(response => 
                            (int)response.StatusCode >= 500 || 
                            response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                })
                .Build();
        }

        /// <summary>
        /// Creates a retry policy with exponential backoff for WhatsApp services
        /// </summary>
        public static ResiliencePipeline<HttpResponseMessage> CreateWhatsAppRetryPolicy()
        {
            return new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(2),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true,
                    ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                        .HandleResult(response => 
                            (int)response.StatusCode >= 500 || 
                            response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                })
                .Build();
        }

        /// <summary>
        /// Creates a generic async retry policy for exceptions
        /// </summary>
        public static ResiliencePipeline CreateGenericRetryPolicy()
        {
            return new ResiliencePipelineBuilder()
                .AddRetry(new RetryStrategyOptions
                {
                    MaxRetryAttempts = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    BackoffType = DelayBackoffType.Exponential,
                    UseJitter = true
                })
                .Build();
        }
    }
}
