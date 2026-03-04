using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Embedding
{
    public interface IEmbeddingsRepository
    {
        Task<Embeddings?> GetByItemAsync(string itemType, string itemId, CancellationToken ct = default);
        Task AddAsync(Embeddings entity, CancellationToken ct = default);
        Task UpdateAsync(Embeddings entity, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
