using System;
using System.Collections.Generic;

namespace DOMAIN.Models;

public partial class Embeddings
{
    public long EmbeddingId { get; set; }

    public string ItemType { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public string? VectorRef { get; set; }

    public byte[]? VectorBlob { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
