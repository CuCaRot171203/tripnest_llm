using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Search
{
    public class SearchRepository : ISearchRepository
    {
        private readonly TripnestDbContext _db;

        public SearchRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<(List<Properties> items, int total)> SearchPropertiesAsync(
           string? q,
           string? city,
           double? lat,
           double? lng,
           double? radius,
           DateTime? checkin,
           DateTime? checkout,
           List<int>? amenities,
           decimal? minPrice,
           decimal? maxPrice,
           string? sort,
           int page = 1,
           int pageSize = 10,
           CancellationToken ct = default)
        {
            var query = _db.Properties
                .Include(p => p.Rooms)
                .Include(p => p.Propertyamenities)
                    .ThenInclude(pa => pa.Amenity)
                .AsQueryable();

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(p =>
                    (p.TitleEn ?? "").Contains(q) ||
                    (p.TitleVi ?? "").Contains(q) ||
                    (p.DescriptionEn ?? "").Contains(q) ||
                    (p.DescriptionVi ?? "").Contains(q));
            }

            if (!string.IsNullOrEmpty(city))
            {
                var cityLower = city.ToLower();
                query = query.Where(p => (p.City ?? "").ToLower() == cityLower);
            }

            if (amenities != null && amenities.Count > 0)
            {
                query = query.Where(p =>
                    p.Propertyamenities.Any(a => amenities.Contains(a.AmenityId)));
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.PriceBase >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.PriceBase <= maxPrice.Value);
            }

            if (lat.HasValue && lng.HasValue && radius.HasValue)
            {
                double deg = radius.Value / 111.0;
                query = query.Where(p =>
                    p.Latitude.HasValue && p.Longitude.HasValue &&
                    p.Latitude >= lat - deg && p.Latitude <= lat + deg &&
                    p.Longitude >= lng - deg && p.Longitude <= lng + deg);
            }

            switch (sort)
            {
                case "price_asc":
                    query = query.OrderBy(p => p.PriceBase);
                    break;
                case "price_desc":
                    query = query.OrderByDescending(p => p.PriceBase);
                    break;
                case "newest":
                    query = query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = query.OrderBy(p => p.PropertyId);
                    break;
            }

            var total = await query.CountAsync(ct);
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, total);
        }

        public async Task<List<(string Text, string? Type, long? PropertyId)>> GetSuggestionsAsync(string q, int limit = 10, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(q)) return new List<(string, string?, long?)>();

            var results = await _db.Properties
                .Where(p => (p.TitleEn ?? "").Contains(q) || (p.TitleVi ?? "").Contains(q))
                .Select(p => new { Text = p.TitleEn ?? p.TitleVi ?? "", PropertyId = p.PropertyId })
                .Distinct()
                .Take(limit)
                .ToListAsync(ct);

            return results.Select(r => (r.Text, (string?)null, (long?)r.PropertyId)).ToList();
        }

        public async Task<List<Properties>> GetPropertiesByIdsAsync(IEnumerable<long> ids, CancellationToken ct = default)
        {
            if (ids == null) return new List<Properties>();
            var idList = ids as IList<long> ?? ids.ToList();
            if (!idList.Any()) return new List<Properties>();

            var items = await _db.Properties
                .Where(p => idList.Contains(p.PropertyId))
                .Include(p => p.Propertyphotos)
                .Include(p => p.Propertyamenities)
                .ToListAsync(ct);

            var ordered = idList
                .Select(id => items.FirstOrDefault(x => x.PropertyId == id))
                .Where(x => x != null)
                .ToList()!;

            return ordered;
        }

        public async Task<List<(string ItemType, string ItemId, float Score)>> SemanticSearchAsync(float[] embedding, IDictionary<string, string>? filters, int topK, CancellationToken ct = default)
        {
            if (embedding == null || embedding.Length == 0) return new List<(string, string, float)>();
            if (topK <= 0) topK = 10;

            var qVec = Normalize(embedding);

            var query = _db.Set<Embeddings>().AsQueryable();

            if (filters != null && filters.TryGetValue("item_type", out var itemTypeVal) && !string.IsNullOrWhiteSpace(itemTypeVal))
            {
                query = query.Where(e => e.ItemType == itemTypeVal);
            }

            if (filters != null && filters.TryGetValue("updated_after", out var updatedAfterStr) && DateTime.TryParse(updatedAfterStr, out var updatedAfter))
            {
                query = query.Where(e => e.UpdatedAt.HasValue && e.UpdatedAt.Value >= updatedAfter);
            }

            var projected = query.Select(e => new
            {
                e.ItemType,
                e.ItemId,
                e.VectorBlob
            });

            var rows = await projected.ToListAsync(ct);

            var hits = new List<(string ItemType, string ItemId, float Score)>(capacity: Math.Min(topK, rows.Count));
            foreach (var row in rows)
            {
                ct.ThrowIfCancellationRequested();

                if (row.VectorBlob == null || row.VectorBlob.Length == 0) continue;

                float[] vec;
                try
                {
                    vec = DeserializeFloatArray(row.VectorBlob);
                }
                catch
                {
                    continue;
                }

                if (vec.Length != qVec.Length) continue;

                var normVec = Normalize(vec);
                var score = Dot(qVec, normVec);

                hits.Add((row.ItemType ?? "", row.ItemId ?? "", score));
            }

            var top = hits.OrderByDescending(h => h.Score).ThenBy(h => h.ItemId).Take(topK).ToList();
            return top;
        }

        // ---------- helpers ----------
        private static float[] DeserializeFloatArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return Array.Empty<float>();
            int floatCount = bytes.Length / 4;
            var result = new float[floatCount];
            for (int i = 0; i < floatCount; i++)
            {
                int offset = i * 4;
                result[i] = BitConverter.ToSingle(bytes, offset); // assumes little-endian float32
            }
            return result;
        }

        private static float[] Normalize(float[] v)
        {
            double sum = 0;
            for (int i = 0; i < v.Length; i++) sum += (double)v[i] * v[i];
            double norm = Math.Sqrt(sum);
            if (norm == 0) return v.Select(x => 0f).ToArray();
            var outv = new float[v.Length];
            for (int i = 0; i < v.Length; i++) outv[i] = (float)(v[i] / norm);
            return outv;
        }

        private static float Dot(float[] a, float[] b)
        {
            double sum = 0;
            for (int i = 0; i < a.Length; i++) sum += (double)a[i] * b[i];
            return (float)sum;
        }
    }
}
