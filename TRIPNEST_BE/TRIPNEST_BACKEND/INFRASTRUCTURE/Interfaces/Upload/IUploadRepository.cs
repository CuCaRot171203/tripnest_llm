using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INFRASTRUCTURE.Interfaces.Upload
{
    public interface IUploadRepository
    {
        Task<Propertyphotos> AddPhotoAsync(Propertyphotos photo, CancellationToken ct = default);
        Task<Propertyphotos?> GetPhotoByIdAsync(long photoId, CancellationToken ct = default);
        Task DeletePhotoAsync(Propertyphotos photo, CancellationToken ct = default);
        Task<Messages> AddMessageAsync(Messages message, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
