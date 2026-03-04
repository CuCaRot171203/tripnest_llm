// APPLICATION/Services/Search/SearchService.cs
using APPLICATION.DTOs.Search;
using APPLICATION.Interfaces.Search;
using APPLICATION.Options;
using INFRASTRUCTURE.DTOs.Search;
using INFRASTRUCTURE.Interfaces.Search;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace APPLICATION.Services.Search
{
    public class SearchService : ISearchService
    {
        private readonly ISearchRepository _repo;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<SearchService> _logger;
        private readonly LlmOptions _llmOptions;

        public SearchService(ISearchRepository repo, IHttpClientFactory httpClientFactory, ILogger<SearchService> logger, IOptions<LlmOptions> llmOptions)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _llmOptions = llmOptions?.Value ?? new LlmOptions();
        }

        public async Task<SearchResponseDto> SearchAsync(SearchQueryDto query, CancellationToken ct = default)
        {
            if (query.Page <= 0) query.Page = 1;
            if (query.PageSize <= 0) query.PageSize = 20;

            var (items, total) = await _repo.SearchPropertiesAsync(
                q: query.Q,
                city: query.City,
                lat: query.Lat,
                lng: query.Lng,
                radius: query.Radius,
                checkin: query.Checkin,
                checkout: query.Checkout,
                amenities: query.Amenities,
                minPrice: query.MinPrice,
                maxPrice: query.MaxPrice,
                sort: query.Sort,
                page: query.Page,
                pageSize: query.PageSize,
                ct: ct
            );

            var resultItems = items.Select(p => new SearchResultItemDto
            {
                PropertyId = p.PropertyId,
                Title = p.TitleEn ?? p.TitleVi,
                Address = p.AddressFormatted ?? $"{p.Street}, {p.City}",
                City = p.City,
                PriceBase = p.PriceBase,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ThumbnailUrl = GetPhotoUrlSafe(p)
            }).ToList();

            return new SearchResponseDto
            {
                Items = resultItems,
                Total = total,
                Page = query.Page,
                PageSize = query.PageSize
            };
        }

        public async Task<List<SuggestionDto>> GetSuggestionsAsync(string q, int limit = 10, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(q)) return new List<SuggestionDto>();

            var raw = await _repo.GetSuggestionsAsync(q, limit, ct);

            return raw.Select(r => new SuggestionDto
            {
                Text = r.Text,
                Type = r.Type,
                PropertyId = r.PropertyId
            }).ToList();
        }

        public async Task<SearchResponseDto> SemanticSearchAsync(SemanticSearchRequestDto request, CancellationToken ct = default)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Text))
                return new SearchResponseDto { Items = new List<SearchResultItemDto>(), Total = 0, Page = 1, PageSize = request?.TopK ?? 10 };

            var client = _httpClientFactory.CreateClient("LocalLLM");

            var payload = new { origin = "", destination = "", user_query = request.Text };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage resp = null;
            try
            {
                using var requestMsg = new HttpRequestMessage(HttpMethod.Post, "api/ai/route") { Content = content };
                resp = await client.SendAsync(requestMsg, HttpCompletionOption.ResponseHeadersRead, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("SemanticSearch canceled by caller for text: {Text}", request.Text);
                throw; // preserve cancellation
            }
            catch (TaskCanceledException tex)
            {
                _logger.LogWarning(tex, "LocalLLM request timed out/cancelled for query: {Query}", request.Text);
                return EmptySearchResponse(request.TopK);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TimeoutException)
            {
                _logger.LogError(ex, "Error calling LocalLLM for query: {Query}", request.Text);
                return EmptySearchResponse(request.TopK);
            }

            if (resp == null)
            {
                _logger.LogWarning("LocalLLM returned null response for query: {Query}", request.Text);
                return EmptySearchResponse(request.TopK);
            }

            if (resp.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                _logger.LogWarning("Local LLM returned 422 for query: {Query}", request.Text);
                return EmptySearchResponse(request.TopK);
            }

            if (!resp.IsSuccessStatusCode)
            {
                var bodyErr = await SafeReadContentAsync(resp, ct);
                _logger.LogError("Local LLM call failed: {Status}. Body: {Body}", resp.StatusCode, bodyErr);
                return EmptySearchResponse(request.TopK);
            }

            var respBody = await SafeReadContentAsync(resp, ct);
            if (string.IsNullOrWhiteSpace(respBody))
            {
                _logger.LogWarning("Local LLM returned empty body for query: {Query}", request.Text);
                return EmptySearchResponse(request.TopK);
            }

            using var doc = JsonDocument.Parse(respBody);
            var root = doc.RootElement;

            // 1) property_ids
            if (root.TryGetProperty("property_ids", out var idsEl) && idsEl.ValueKind == JsonValueKind.Array)
            {
                var ids = new List<long>();
                foreach (var e in idsEl.EnumerateArray())
                {
                    if (e.TryGetInt64(out var id)) ids.Add(id);
                    else if (e.ValueKind == JsonValueKind.String && long.TryParse(e.GetString(), out var sid)) ids.Add(sid);
                }

                if (ids.Any())
                {
                    var props = await _repo.GetPropertiesByIdsAsync(ids, ct);
                    return MapPropertiesToResponse(props, request.TopK ?? ids.Count);
                }
            }

            // 2) embedding
            if (root.TryGetProperty("embedding", out var embEl) && embEl.ValueKind == JsonValueKind.Array)
            {
                var embedding = new List<float>();
                foreach (var e in embEl.EnumerateArray())
                {
                    if (e.TryGetSingle(out var f)) embedding.Add(f);
                    else if (e.TryGetDouble(out var d)) embedding.Add((float)d);
                    else if (e.ValueKind == JsonValueKind.String && float.TryParse(e.GetString(), out var fs)) embedding.Add(fs);
                }

                if (embedding.Any())
                {
                    try
                    {
                        var hits = await _repo.SemanticSearchAsync(embedding.ToArray(), request.Filters, request.TopK ?? 10, ct);

                        var propIds = hits
                            .Where(h => string.Equals(h.ItemType, "property", StringComparison.OrdinalIgnoreCase))
                            .Select(h => long.TryParse(h.ItemId, out var id) ? id : (long?)null)
                            .Where(id => id.HasValue)
                            .Select(id => id!.Value)
                            .ToList();

                        if (propIds.Any())
                        {
                            var props = await _repo.GetPropertiesByIdsAsync(propIds, ct);
                            return MapPropertiesToResponse(props, request.TopK ?? propIds.Count);
                        }
                    }
                    catch (NotImplementedException)
                    {
                        _logger.LogWarning("Repository.SemanticSearchAsync not implemented.");
                        return EmptySearchResponse(request.TopK);
                    }
                }
            }

            // 3) hits[] with id
            if (root.TryGetProperty("hits", out var hitsEl) && hitsEl.ValueKind == JsonValueKind.Array)
            {
                var ids = new List<long>();
                foreach (var h in hitsEl.EnumerateArray())
                {
                    if (h.TryGetProperty("id", out var idEl))
                    {
                        var idStr = idEl.GetString();
                        if (!string.IsNullOrWhiteSpace(idStr))
                        {
                            var m = System.Text.RegularExpressions.Regex.Match(idStr, @"\d+");
                            if (m.Success && long.TryParse(m.Value, out var id)) ids.Add(id);
                        }
                    }
                }

                if (ids.Any())
                {
                    var props = await _repo.GetPropertiesByIdsAsync(ids, ct);
                    return MapPropertiesToResponse(props, request.TopK ?? ids.Count);
                }
            }

            _logger.LogWarning("Unrecognized response from LLM for query: {Query}. Body: {Body}", request.Text, respBody);
            return EmptySearchResponse(request.TopK);
        }

        private static SearchResponseDto EmptySearchResponse(int? topK) =>
            new SearchResponseDto { Items = new List<SearchResultItemDto>(), Total = 0, Page = 1, PageSize = topK ?? 10 };

        private async Task<string> SafeReadContentAsync(HttpResponseMessage resp, CancellationToken ct)
        {
            try
            {
                return await resp.Content.ReadAsStringAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read response content safely.");
                return string.Empty;
            }
        }

        private SearchResponseDto MapPropertiesToResponse(List<DOMAIN.Models.Properties> properties, int topK)
        {
            var dtoItems = properties.Select(p => new SearchResultItemDto
            {
                PropertyId = p.PropertyId,
                Title = p.TitleEn ?? p.TitleVi,
                Address = p.AddressFormatted ?? $"{p.Street}, {p.City}",
                City = p.City,
                PriceBase = p.PriceBase,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                ThumbnailUrl = GetPhotoUrlSafe(p)
            }).ToList();

            return new SearchResponseDto
            {
                Items = dtoItems,
                Total = dtoItems.Count,
                Page = 1,
                PageSize = Math.Min(topK, dtoItems.Count)
            };
        }

        private string? GetPhotoUrlSafe(DOMAIN.Models.Properties p)
        {
            try
            {
                var photosProp = p.GetType().GetProperty("Propertyphotos");
                if (photosProp == null) return null;

                var photosVal = photosProp.GetValue(p) as System.Collections.IEnumerable;
                if (photosVal == null) return null;

                var enumerator = photosVal.GetEnumerator();
                if (!enumerator.MoveNext()) return null;
                var firstPhoto = enumerator.Current;
                if (firstPhoto == null) return null;

                string[] candidateNames = new[] { "Url", "PhotoUrl", "FilePath", "Path", "UrlFull", "Src", "Thumbnail", "ImageUrl" };
                foreach (var name in candidateNames)
                {
                    var prop = firstPhoto.GetType().GetProperty(name);
                    if (prop != null)
                    {
                        var val = prop.GetValue(firstPhoto);
                        if (val != null) return val.ToString();
                    }
                }

                return firstPhoto.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
