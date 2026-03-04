using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.IItinerary;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Itinerary
{
    public class ItineraryRepository : IItineraryRepository
    {
        private readonly TripnestDbContext _db;

        public ItineraryRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(
            Itineraries entity, 
            CancellationToken ct = default)
        {
            await _db.Itineraries.AddAsync(entity, ct);
        }

        public async Task DeleteAsync(
            Itineraries entity, 
            CancellationToken ct = default)
        {
            _db.Itineraries.Remove(entity);
            await Task.CompletedTask;
        }

        public async Task<List<Itineraries>> GetAllAsync(int skip = 0, int take = 100, CancellationToken ct = default)
        {
            return await _db.Itineraries
                .Include(i => i.Waypoints)
                .OrderByDescending(i => i.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }

        public async Task<Itineraries?> GetByIdAsync(
            Guid id, 
            CancellationToken ct = default)
        {
            return await _db.Itineraries
                .Include(i => i.Waypoints)
                .FirstOrDefaultAsync(i => i.ItineraryId == id, ct);
        }

        public async Task<List<Itineraries>> GetByUserIdAsync(
            Guid userId, 
            CancellationToken ct = default)
        {
            return await _db.Itineraries
                .Where(i => i.UserId == userId)
                .Include(i => i.Waypoints)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Itineraries entity, CancellationToken ct = default)
        {
            _db.Itineraries.Update(entity);
            await Task.CompletedTask;
        }
    }
}
