using APPLICATION.DTOs.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Upload
{
    public interface IUploadService
    {
        Task<SignedUrlResponseDto> GetSignedUploadUrlAsync(GetSignedUrlRequestDto request, CancellationToken ct = default);
        Task<UploadResponseDto> SaveUploadedFileRecordAsync(long propertyId, string key, int? order, string? meta, CancellationToken ct = default);
        Task<bool> DeletePhotoAsync(long photoId, CancellationToken ct = default);
    }
}
