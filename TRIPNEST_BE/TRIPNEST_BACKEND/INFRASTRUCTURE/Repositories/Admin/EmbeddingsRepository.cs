using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Repositories.Admin
{
    public class EmbeddingsRepository : IEmbeddingsRepository
    {
        private readonly TripnestDbContext _db;

        public EmbeddingsRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<List<Embeddings>> GetByItemTypeAsync(
            string itemType, 
            CancellationToken ct = default)
        {
            return await _db.Embeddings
                .Where(e => e.ItemType == itemType)
                .ToListAsync(ct);
        }

        public async Task UpdateEmbeddingAsync(
            Embeddings embedding, 
            CancellationToken ct = default)
        {
            _db.Embeddings.Update(embedding);
            await _db.SaveChangesAsync(ct);
        }
    }
}
