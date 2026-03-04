using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Embedding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Embedding
{
    public class EmbeddingsRepository : IEmbeddingsRepository
    {
        private readonly TripnestDbContext _db;

        public EmbeddingsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Embeddings?> GetByItemAsync(string itemType, string itemId, CancellationToken ct = default)
        {
            return await _db.Embeddings
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.ItemType == itemType && e.ItemId == itemId, ct);
        }

        public async Task AddAsync(Embeddings entity, CancellationToken ct = default)
        {
            await _db.Embeddings.AddAsync(entity, ct);
        }

        public Task UpdateAsync(Embeddings entity, CancellationToken ct = default)
        {
            _db.Embeddings.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _db.SaveChangesAsync(ct);
        }
    }
}
