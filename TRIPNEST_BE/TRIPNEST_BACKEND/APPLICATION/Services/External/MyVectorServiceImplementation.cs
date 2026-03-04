using APPLICATION.Interfaces.External;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace APPLICATION.Services.External
{
    public class MyVectorServiceImplementation : IVectorService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<MyVectorServiceImplementation> _logger;

        private readonly string _provider;
        private readonly string? _openAiApiKey;
        private readonly string? _openAiBaseUrl;
        private readonly string _openAiModel;

        public MyVectorServiceImplementation(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<MyVectorServiceImplementation> logger)
        {
            _httpFactory = httpFactory;
            _config = config;
            _logger = logger;

            _provider = _config["Vector:Provider"]?.Trim().ToLowerInvariant() ?? "fallback";
            _openAiApiKey = _config["Vector:OpenAI:ApiKey"];
            _openAiBaseUrl = _config["Vector:OpenAI:BaseUrl"]?.TrimEnd('/') ?? "https://api.openai.com/v1";
            _openAiModel = _config["Vector:OpenAI:Model"] ?? "text-embedding-3-large";
        }

        public async Task<(string? vectorRef, byte[]? vectorBlob)> CreateEmbeddingAsync(string text, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _logger.LogWarning("CreateEmbeddingAsync called with empty text.");
                return (null, null);
            }

            try
            {
                if (_provider == "openai")
                {
                    if (string.IsNullOrWhiteSpace(_openAiApiKey))
                    {
                        _logger.LogWarning("OpenAI provider selected but API key missing. Falling back.");
                        return CreateFallbackEmbedding(text);
                    }

                    return await CreateOpenAiEmbeddingAsync(text, ct);
                }

                return CreateFallbackEmbedding(text);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create embedding for text (len={Len}).", text.Length);
                return (null, null);
            }
        }

        private async Task<(string? vectorRef, byte[]? vectorBlob)> CreateOpenAiEmbeddingAsync(string text, CancellationToken ct)
        {
            // OpenAI embeddings API: POST /v1/embeddings
            var client = _httpFactory.CreateClient("vectorService");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

            var url = $"{_openAiBaseUrl}/embeddings";

            var payload = new
            {
                model = _openAiModel,
                input = text
            };

            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var resp = await client.PostAsync(url, content, ct);
            var respText = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("OpenAI embeddings request failed: {Status} {Body}", resp.StatusCode, Truncate(respText, 1000));
                return (null, null);
            }

            using var doc = JsonDocument.Parse(respText);
            if (doc.RootElement.TryGetProperty("data", out var dataEl) && dataEl.GetArrayLength() > 0)
            {
                var first = dataEl[0];
                if (first.TryGetProperty("embedding", out var embEl) && embEl.ValueKind == JsonValueKind.Array)
                {
                    var floats = new float[embEl.GetArrayLength()];
                    int i = 0;
                    foreach (var v in embEl.EnumerateArray())
                    {
                        floats[i++] = v.GetSingle();
                    }

                    var bytes = FloatsToBytes(floats);

                    return (null, bytes);
                }
            }

            _logger.LogWarning("OpenAI response shape unexpected: {Body}", Truncate(respText, 500));
            return (null, null);
        }

        private (string? vectorRef, byte[]? vectorBlob) CreateFallbackEmbedding(string text)
        {
            // Simple deterministic embedding: compute SHA256, split into 8 floats by slicing hash bytes.
            // NOTE: This is NOT semantically meaningful — only for testing/local dev.
            using var sha = System.Security.Cryptography.SHA256.Create();
            var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text));

            // Expand or reduce to N floats (e.g., 128 floats) deterministically
            const int dims = 128;
            var floats = new float[dims];
            // build floats by repeating hash and mixing with index
            for (int i = 0; i < dims; i++)
            {
                // pick 4 bytes from hash with wrap
                int baseIdx = (i * 4) % hash.Length;
                uint val = (uint)(hash[baseIdx] | (hash[(baseIdx + 1) % hash.Length] << 8) | (hash[(baseIdx + 2) % hash.Length] << 16) | (hash[(baseIdx + 3) % hash.Length] << 24));
                // map to float in [-1,1]
                floats[i] = ((int)val / (float)uint.MaxValue) * 2f - 1f;
            }

            var bytes = FloatsToBytes(floats);

            // Return vectorRef as deterministic hex of sha (useful as id)
            var refId = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            return (refId, bytes);
        }

        private static byte[] FloatsToBytes(float[] floats)
        {
            var bytes = new byte[floats.Length * 4];
            for (int i = 0; i < floats.Length; i++)
            {
                var b = BitConverter.GetBytes(floats[i]);
                // ensure little-endian storage
                if (!BitConverter.IsLittleEndian) Array.Reverse(b);
                Buffer.BlockCopy(b, 0, bytes, i * 4, 4);
            }
            return bytes;
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return s ?? string.Empty;
            return s.Length <= max ? s : s.Substring(0, max) + "...";
        }
    }
}
