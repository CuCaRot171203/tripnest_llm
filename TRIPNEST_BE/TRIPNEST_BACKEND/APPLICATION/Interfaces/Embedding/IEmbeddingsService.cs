using APPLICATION.DTOs.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Embedding
{
    public interface IEmbeddingsService
    {
        Task<EmbeddingDto> GenerateEmbeddingAsync(EmbeddingCreateDto dto, CancellationToken ct = default);
        Task<EmbeddingDto?> GetByItemAsync(string itemType, string itemId, CancellationToken ct = default);
    }
}
