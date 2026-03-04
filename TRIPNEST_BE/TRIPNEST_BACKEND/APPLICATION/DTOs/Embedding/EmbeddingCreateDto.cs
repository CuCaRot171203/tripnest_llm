using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.DTOs.Embedding
{
    public class EmbeddingCreateDto
    {
        [Required]
        public string ItemType { get; set; } = null!;
        [Required]
        public string ItemId { get; set; } = null!;
        [Required]
        public string Content { get; set; } = null!;
        public string? VectorRef { get; set; }
        public byte[]? VectorBlob { get; set; }
    }
}
