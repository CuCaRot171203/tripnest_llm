using Polly;
using Polly.Timeout;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;
using Polly.Extensions.Http;
using System.Net.Http.Headers;

namespace API.Config
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddLocalLlmClient(this IServiceCollection services, IConfiguration config)
        {
            var llmSection = config.GetSection("Llm");
            services.Configure<LlmOptions>(llmSection);
            var options = llmSection.Get<LlmOptions>() ?? new LlmOptions();

            // build retry policy with configurable backoffs
            IAsyncPolicy<HttpResponseMessage> BuildPolicy()
            {
                // Retry (handle transient errors + timeout rejections)
                var retryTimes = options.RetryCount;
                var backoffs = options.RetryBackoffMs ?? new[] { 200, 500, 1000 };

                var retryPolicy = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(msg => (int)msg.StatusCode >= 500)
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        retryTimes,
                        attempt => TimeSpan.FromMilliseconds(backoffs[Math.Min(attempt - 1, backoffs.Length - 1)]),
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            // optional: integrate logging via context if needed
                        });

                var timeoutPolicy = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(options.PerAttemptTimeoutSeconds), TimeoutStrategy.Optimistic);

                var breaker = Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .OrResult(msg => (int)msg.StatusCode >= 500)
                    .CircuitBreakerAsync(options.CircuitBreakerFailures, TimeSpan.FromSeconds(options.CircuitBreakerDurationSeconds));

                // Order: retry around timeout (so each retry has its own timeout), then breaker.
                return Policy.WrapAsync(retryPolicy, timeoutPolicy, breaker);
            }

            services.AddHttpClient("LocalLLM", client =>
            {
                client.BaseAddress = new Uri(options.BaseUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // Set HttpClient.Timeout to be large/Infinite because we control per-attempt timeout via Polly
                client.Timeout = TimeSpan.FromSeconds(options.HttpClientTimeoutSeconds);
            })
            .AddPolicyHandler(BuildPolicy());

            return services;
        }
    }
}
