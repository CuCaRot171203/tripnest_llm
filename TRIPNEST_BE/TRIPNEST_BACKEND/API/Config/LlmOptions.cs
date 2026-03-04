namespace API.Config
{
    public class LlmOptions
    {
        public string BaseUrl { get; set; } = "http://localhost:8000/";
        public int HttpClientTimeoutSeconds { get; set; } = 100;
        public int PerAttemptTimeoutSeconds { get; set; } = 15;
        public int RetryCount { get; set; } = 3;
        public int[] RetryBackoffMs { get; set; } = new[] { 200, 500, 1000 };
        public int CircuitBreakerFailures { get; set; } = 3;
        public int CircuitBreakerDurationSeconds { get; set; } = 30;
    }
}
