using APPLICATION.DTOs.Upload;
using APPLICATION.Interfaces.Upload;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Upload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Services.Upload
{
    public class UploadService : IUploadService
    {
        private readonly IStorageService _storage;
        private readonly IUploadRepository _repo;
        private readonly string _cdnBaseUrl;

        public UploadService(IStorageService storage, IUploadRepository repo, string cdnBaseUrl = "")
        {
            _storage = storage;
            _repo = repo;
            _cdnBaseUrl = cdnBaseUrl?.TrimEnd('/') ?? "";
        }

        public async Task<SignedUrlResponseDto> GetSignedUploadUrlAsync(GetSignedUrlRequestDto request, CancellationToken ct = default)
        {
            var (uploadUrl, key, expiresAt) = await _storage.GetSignedUploadUrlAsync(request.Path, request.ExpirySeconds, request.ContentType, ct);

            return new SignedUrlResponseDto
            {
                UploadUrl = uploadUrl,
                Key = key,
                ExpiresAt = expiresAt
            };
        }

        public async Task<UploadResponseDto> SaveUploadedFileRecordAsync(long propertyId, string key, int? order, string? meta, CancellationToken ct = default)
        {
            // Build public URL if needed
            string url = string.IsNullOrEmpty(_cdnBaseUrl) ? key : $"{_cdnBaseUrl}/{key}";

            var photo = new Propertyphotos
            {
                PropertyId = propertyId,
                Url = url,
                Order = order,
                Meta = meta,
                CreatedAt = DateTime.UtcNow
            };

            var saved = await _repo.AddPhotoAsync(photo, ct);
            await _repo.SaveChangesAsync(ct);

            return new UploadResponseDto
            {
                PhotoId = saved.PhotoId,
                PropertyId = saved.PropertyId,
                Url = saved.Url,
                Order = saved.Order,
                Meta = saved.Meta,
                CreatedAt = saved.CreatedAt
            };
        }

        public async Task<bool> DeletePhotoAsync(long photoId, CancellationToken ct = default)
        {
            var photo = await _repo.GetPhotoByIdAsync(photoId, ct);
            if (photo == null) return false;
            await _repo.DeletePhotoAsync(photo, ct);
            await _repo.SaveChangesAsync(ct);
            return true;
        }
    }
}
