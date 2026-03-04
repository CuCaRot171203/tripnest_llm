using APPLICATION.DTOs.Embedding;
using APPLICATION.Interfaces.Embedding;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Embedding
{
    public class EmbeddingsService : IEmbeddingsService
    {
        private readonly IEmbeddingsRepository _repo;

        public EmbeddingsService(IEmbeddingsRepository repo /*, IEmbeddingProvider provider */)
        {
            _repo = repo;
            // _provider = provider;
        }

        public async Task<EmbeddingDto> GenerateEmbeddingAsync(EmbeddingCreateDto dto, CancellationToken ct = default)
        {
            // 1. Optionally call external provider to compute vectorRef / blob
            // For now we'll simulate provider by creating a VectorRef GUID.
            // TODO: replace this with real call to _provider.GenerateAsync(dto.Content)

            var vectorRef = dto.VectorRef ?? Guid.NewGuid().ToString();

            // Try get existing
            var existing = await _repo.GetByItemAsync(dto.ItemType, dto.ItemId, ct);

            if (existing == null)
            {
                var entity = new Embeddings
                {
                    ItemType = dto.ItemType,
                    ItemId = dto.ItemId,
                    VectorRef = vectorRef,
                    // optionally store blob
                    VectorBlob = dto.VectorBlob,
                    UpdatedAt = DateTime.UtcNow
                };

                await _repo.AddAsync(entity, ct);
                await _repo.SaveChangesAsync(ct);

                return new EmbeddingDto
                {
                    EmbeddingId = entity.EmbeddingId,
                    ItemType = entity.ItemType,
                    ItemId = entity.ItemId,
                    VectorRef = entity.VectorRef,
                    HasVectorBlob = entity.VectorBlob != null,
                    UpdatedAt = entity.UpdatedAt
                };
            }
            else
            {
                existing.VectorRef = vectorRef;
                if (dto.VectorBlob != null) existing.VectorBlob = dto.VectorBlob;
                existing.UpdatedAt = DateTime.UtcNow;

                await _repo.UpdateAsync(existing, ct);
                await _repo.SaveChangesAsync(ct);

                return new EmbeddingDto
                {
                    EmbeddingId = existing.EmbeddingId,
                    ItemType = existing.ItemType,
                    ItemId = existing.ItemId,
                    VectorRef = existing.VectorRef,
                    HasVectorBlob = existing.VectorBlob != null,
                    UpdatedAt = existing.UpdatedAt
                };
            }
        }

        public async Task<EmbeddingDto?> GetByItemAsync(string itemType, string itemId, CancellationToken ct = default)
        {
            var e = await _repo.GetByItemAsync(itemType, itemId, ct);
            if (e == null) return null;
            return new EmbeddingDto
            {
                EmbeddingId = e.EmbeddingId,
                ItemType = e.ItemType,
                ItemId = e.ItemId,
                VectorRef = e.VectorRef,
                HasVectorBlob = e.VectorBlob != null,
                UpdatedAt = e.UpdatedAt
            };
        }
    }
}
