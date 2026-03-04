using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.IItinerary
{
    public interface IItineraryRepository
    {
        Task<Itineraries?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Itineraries>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Itineraries>> GetAllAsync(int skip = 0, int take = 100, CancellationToken ct = default);
        Task AddAsync(Itineraries entity, CancellationToken ct = default);
        Task UpdateAsync(Itineraries entity, CancellationToken ct = default);
        Task DeleteAsync(Itineraries entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
