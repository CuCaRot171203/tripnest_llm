using APPLICATION.DTOs.Common;
using APPLICATION.DTOs.Property;
using DOMAIN.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Interfaces.Property
{
    public interface IPropertiesService
    {
        Task<PropertyDto?> CreateAsync(CreatePropertyRequest request, Guid? ownerUserId = null);
        Task<PropertyDto?> UpdateAsync(UpdatePropertyRequest request, Guid? userId = null);
        Task<bool> SoftDeleteAsync(long propertyId, Guid? userId = null);
        Task<PagedResult<PropertyDto>> GetListAsync(string? q, string? city, double? lat, double? lng, double? radiusMeters,
            decimal? minPrice = null, decimal? maxPrice = null, List<int>? amenities = null, string? sort = null, int page = 1, int pageSize = 20);
        Task<PropertyDto?> GetByIdAsync(long id);
        Task<UploadPhotoResponse> UploadPhotoAsync(long propertyId, IFormFile file, long? order = null, string? meta = null);
        Task<IEnumerable<UploadPhotoResponse>> GetPhotosAsync(long propertyId);
        Task<IEnumerable<ShortRoomDto>> GetRoomsAsync(long propertyId);
    }
}
