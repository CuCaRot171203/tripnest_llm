using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Search
{
    public interface ISearchRepository
    {
        Task<(List<DOMAIN.Models.Properties> items, int total)> SearchPropertiesAsync(
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
            CancellationToken ct = default
        );

        Task<List<(string Text, string? Type, long? PropertyId)>> GetSuggestionsAsync(string q, int limit = 10, CancellationToken ct = default);


        Task<List<DOMAIN.Models.Properties>> GetPropertiesByIdsAsync(IEnumerable<long> ids, CancellationToken ct = default);

        Task<List<(string ItemType, string ItemId, float Score)>> SemanticSearchAsync(float[] embedding, IDictionary<string, string>? filters, int topK, CancellationToken ct = default);
    }
}
