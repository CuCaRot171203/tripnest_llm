using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Admin
{
    public interface IEmbeddingsRepository
    {
        Task<List<Embeddings>> GetByItemTypeAsync(string itemType, CancellationToken ct = default);
        Task UpdateEmbeddingAsync(Embeddings embedding, CancellationToken ct = default);
    }
}
