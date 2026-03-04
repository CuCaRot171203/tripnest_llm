using APPLICATION.Interfaces.Embedding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Embedding
{
    public class LocalEmbeddingProvider : IEmbeddingProvider
    {
        private readonly int _dim;

        public LocalEmbeddingProvider(int dim = 1536)
        {
            _dim = dim;
        }

        public Task<float[]> GetEmbeddingAsync(string text, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(text))
                return Task.FromResult(Array.Empty<float>());

            // deterministic pseudo-random vector from hash
            var hash = Math.Abs(text.GetHashCode());
            var rng = new Random(hash);
            var vec = new float[_dim];
            for (int i = 0; i < _dim; i++)
            {
                // value in [-1,1]
                vec[i] = (float)(rng.NextDouble() * 2.0 - 1.0);
            }
            return Task.FromResult(vec);
        }
    }
}
