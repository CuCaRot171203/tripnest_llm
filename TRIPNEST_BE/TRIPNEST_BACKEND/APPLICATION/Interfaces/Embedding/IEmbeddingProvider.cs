using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Embedding
{
    public interface IEmbeddingProvider
    {
        Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default);
    }
}
