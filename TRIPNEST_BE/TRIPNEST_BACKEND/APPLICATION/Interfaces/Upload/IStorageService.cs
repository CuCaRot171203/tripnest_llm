using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Upload
{
    public interface IStorageService
    {
        Task<(string uploadUrl, string key, DateTime expiresAt)> GetSignedUploadUrlAsync(string path, int expirySeconds = 300, string? contentType = null, CancellationToken ct = default);
        Task<string> UploadStreamAsync(System.IO.Stream stream, string key, string? contentType = null, CancellationToken ct = default);
        Task DeleteAsync(string key, CancellationToken ct = default);
    }
}
