using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Embedding
{
    public class EmbeddingDto
    {
        public long EmbeddingId { get; set; }
        public string ItemType { get; set; } = null!;
        public string ItemId { get; set; } = null!;
        public string? VectorRef { get; set; }
        public bool HasVectorBlob { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
