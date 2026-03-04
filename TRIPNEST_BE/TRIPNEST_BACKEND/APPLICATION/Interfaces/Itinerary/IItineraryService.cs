using APPLICATION.DTOs.Itineraries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Itinerary
{
    public interface IItineraryService
    {
        Task<ItineraryResponseDto> CreateItineraryAsync(Guid userId, ItineraryCreateDto dto, CancellationToken ct = default);
        Task<ItineraryResponseDto?> GetItineraryByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<ItineraryResponseDto>> GetItinerariesByUserAsync(Guid userId, CancellationToken ct = default);
        Task<List<ItineraryResponseDto>> GetAllAsync(int skip = 0, int take = 100, CancellationToken ct = default);
        Task<bool> UpdateItineraryAsync(Guid userId, Guid id, ItineraryUpdateDto dto, CancellationToken ct = default);
        Task<bool> DeleteItineraryAsync(Guid userId, Guid id, CancellationToken ct = default);
        Task<SuggestItineraryResponseDto> SuggestItineraryAsync(Guid? userId, SuggestItineraryRequestDto req, CancellationToken ct = default);
        Task<bool> PushEmbeddingAsync(string sourceId, string payload, CancellationToken ct = default);
    }
}
