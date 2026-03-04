using APPLICATION.DTOs.Search;
using INFRASTRUCTURE.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Search
{
    public interface ISearchService
    {
        Task<SearchResponseDto> SearchAsync(SearchQueryDto query, CancellationToken ct = default);
        Task<List<SuggestionDto>> GetSuggestionsAsync(string q, int limit = 10, CancellationToken ct = default);
        Task<SearchResponseDto> SemanticSearchAsync(SemanticSearchRequestDto request, CancellationToken ct = default);
    }
}
