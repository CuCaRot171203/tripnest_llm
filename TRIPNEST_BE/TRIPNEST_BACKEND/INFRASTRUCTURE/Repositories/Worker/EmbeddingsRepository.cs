using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Worker;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Worker
{
    public class EmbeddingsRepository : IEmbeddingsRepository
    {
        private readonly TripnestDbContext _db;

        public EmbeddingsRepository( TripnestDbContext db)
        {
            _db = db;
        }
        public async Task<Embeddings?> GetByItemAsync(
            string itemType, 
            string itemId, 
            CancellationToken ct = default)
        {
            return await _db.Embeddings
                .FirstOrDefaultAsync(e => e.ItemType == itemType 
                    && e.ItemId == itemId, ct);
        }

        public Task AddAsync(
            Embeddings entity, 
            CancellationToken ct = default)
        {
            _db.Embeddings.Add(entity);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(
            Embeddings entity, 
            CancellationToken ct = default)
        {
            _db.Embeddings.Update(entity);
            return Task.CompletedTask;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }
    }
}
