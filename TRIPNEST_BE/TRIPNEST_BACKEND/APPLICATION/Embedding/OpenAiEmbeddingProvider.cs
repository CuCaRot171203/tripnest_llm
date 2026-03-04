using APPLICATION.Interfaces.Embedding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APPLICATION.Embedding
{
    public class OpenAiEmbeddingProvider : IEmbeddingProvider
    {
        private readonly HttpClient _http;
        private readonly string _apiKey;
        private readonly string _model;
        private readonly ILogger<OpenAiEmbeddingProvider>? _logger;

        public OpenAiEmbeddingProvider(HttpClient http, IConfiguration config, ILogger<OpenAiEmbeddingProvider>? logger = null)
        {
            _http = http ?? throw new ArgumentNullException(nameof(http));
            _logger = logger;
            // Prefer configuration keys: "OpenAI:ApiKey" and "OpenAI:Model"
            _apiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? throw new InvalidOperationException("OpenAI API key not configured.");
            _model = config["OpenAI:Model"] ?? "text-embedding-3-small"; // change default if you want
            // Configure HttpClient base address if desired; otherwise use full URL below
        }

        public async Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                return Array.Empty<float>();

            // Build request payload
            var payload = new
            {
                model = _model,
                input = text
            };

            var json = JsonSerializer.Serialize(payload);
            using var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/embeddings")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);

            using var res = await _http.SendAsync(req, ct);
            if (!res.IsSuccessStatusCode)
            {
                var errorBody = await res.Content.ReadAsStringAsync(ct);
                _logger?.LogError("OpenAI embeddings API error: {Status} {Body}", res.StatusCode, errorBody);
                throw new InvalidOperationException($"OpenAI embeddings request failed: {res.StatusCode}");
            }

            using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            // Response shape (OpenAI standard): { data: [ { embedding: [f,f,...], index:0, ... } ], ... }
            if (!doc.RootElement.TryGetProperty("data", out var dataElem) || dataElem.GetArrayLength() == 0)
                return Array.Empty<float>();

            var embElem = dataElem[0].GetProperty("embedding");
            var floatList = new List<float>(embElem.GetArrayLength());

            foreach (var item in embElem.EnumerateArray())
            {
                // sometimes numbers are doubles — try to get as double then cast
                if (item.TryGetSingle(out float f))
                    floatList.Add(f);
                else if (item.TryGetDouble(out double d))
                    floatList.Add((float)d);
                else
                    floatList.Add(0f);
            }

            return floatList.ToArray();
        }
    }
}
