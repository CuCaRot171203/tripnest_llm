using APPLICATION.DTOs.Itineraries;
using APPLICATION.Interfaces.Itinerary;
using AutoMapper;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.IItinerary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Itinerary
{
    public class ItineraryService : IItineraryService
    {
        private readonly IItineraryRepository _repo;
        private readonly IMapper _mapper;

        public ItineraryService(
            IItineraryRepository repo, 
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ItineraryResponseDto> CreateItineraryAsync(
            Guid userId, 
            ItineraryCreateDto dto, 
            CancellationToken ct = default)
        {
            var entity = new Itineraries
            {
                ItineraryId = Guid.NewGuid(),
                UserId = userId,
                NameVi = dto.NameVi,
                NameEn = dto.NameEn,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Metadata = dto.Metadata,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.Waypoints != null && dto.Waypoints.Any())
            {
                entity.Waypoints = dto.Waypoints.Select(w => new Waypoints
                {
                    ItineraryId = entity.ItineraryId,
                    Arrival = w.Arrival,
                    Lat = w.Lat,
                    Lng = w.Lng,
                    Order = w.Order,
                    PlaceId = w.PlaceId
                }).ToList();
            }

            await _repo.AddAsync(entity, ct);
            await _repo.SaveChangesAsync(ct);

            return _mapper.Map<ItineraryResponseDto>(entity);
        }

        public async Task<bool> DeleteItineraryAsync(
            Guid userId, 
            Guid id, 
            CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if(existing == null)
            {
                return false;
            }
            if(existing.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            await _repo.DeleteAsync(existing, ct);
            await _repo.SaveChangesAsync(ct);

            return true;
        }

        public async Task<List<ItineraryResponseDto>> GetAllAsync(
            int skip = 0, 
            int take = 100, 
            CancellationToken ct = default)
        {
            var items = await _repo.GetAllAsync(skip, take, ct);
            return _mapper.Map<List<ItineraryResponseDto>>(items);
        }

        public async Task<ItineraryResponseDto?> GetItineraryByIdAsync(
            Guid id, 
            CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);
            return entity == null ? null : _mapper.Map<ItineraryResponseDto>(entity);
        }

        public async Task<List<ItineraryResponseDto>> GetItinerariesByUserAsync(
            Guid userId, 
            CancellationToken ct = default)
        {
            var items = await _repo.GetByUserIdAsync(userId, ct);
            return _mapper.Map<List<ItineraryResponseDto>>(items);
        }

        public async Task<bool> UpdateItineraryAsync(Guid userId, Guid id, ItineraryUpdateDto dto, CancellationToken ct = default)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null)
            {
                return false;
            }
            if (existing.UserId != userId)
            {
                throw new UnauthorizedAccessException();
            }

            // Updated
            existing.NameEn = dto.NameEn ?? existing.NameEn;
            existing.NameVi = dto.NameVi ?? existing.NameVi;
            existing.StartDate = dto.StartDate ?? existing.StartDate;
            existing.EndDate = dto.EndDate ?? existing.EndDate;
            existing.Metadata = dto.Metadata ?? existing.Metadata;

            // Waypoint
            if (dto.Waypoints != null)
            {
                // remove old
                existing.Waypoints.Clear();
                foreach (var w in dto.Waypoints)
                {
                    existing.Waypoints.Add(new Waypoints
                    {
                        ItineraryId = existing.ItineraryId,
                        Arrival = w.Arrival,
                        Lat = w.Lat,
                        Lng = w.Lng,
                        Order = w.Order,
                        PlaceId = w.PlaceId
                    });
                }
            }

            await _repo.UpdateAsync(existing, ct);
            await _repo.SaveChangesAsync(ct);

            return true;
        }

        public async Task<SuggestItineraryResponseDto> SuggestItineraryAsync(
            Guid? userId, 
            SuggestItineraryRequestDto req, 
            CancellationToken ct = default)
        {
            // Plug LLM Models to chat

            var text = $"Suggested itinerary for {req.Location} from {req.StartDate} to {req.EndDate}. Interests: {(req.Interests == null ? "none" : string.Join(", ", req.Interests))}. Budget: {req.Budget?.ToString() ?? "N/A"}";

            var result = new SuggestItineraryResponseDto
            {
                ItineraryText = text,
                StructuredItineraries = new List<ItineraryResponseDto>() // optionally fill
            };

            return await Task.FromResult(result);
        }

        public async Task<bool> PushEmbeddingAsync(string sourceId, string payload, CancellationToken ct = default)
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
