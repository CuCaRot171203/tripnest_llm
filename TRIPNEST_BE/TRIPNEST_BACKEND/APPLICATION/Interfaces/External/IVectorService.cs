using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.External
{
    public interface IVectorService
    {
        Task<(string? vectorRef, byte[]? vectorBlob)> CreateEmbeddingAsync(string text, CancellationToken ct = default);
    }
}
