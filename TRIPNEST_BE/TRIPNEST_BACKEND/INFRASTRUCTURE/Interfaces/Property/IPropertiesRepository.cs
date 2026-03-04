using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Common;

namespace INFRASTRUCTURE.Interfaces.Property
{
    public interface IPropertiesRepository
    {
        Task<Properties?> GetByIdAsync(long id);
        Task<PagedResult<Properties>> GetListAsync(string? q, string? city, double? lat, double? lng, double? radiusMeters,
            decimal? minPrice, decimal? maxPrice, List<int>? amenities, string? sort, int page = 1, int pageSize = 20);
        Task<Properties> AddAsync(Properties property);
        Task<Properties> UpdateAsync(Properties property);
        Task SoftDeleteAsync(Properties property);
        Task AddPhotoAsync(Propertyphotos photo);
        Task<Propertyphotos?> GetPhotoAsync(long propertyId, long photoId);
        Task<IEnumerable<Propertyphotos>> GetPhotosAsync(long propertyId);
        Task<IEnumerable<DOMAIN.Models.Rooms>> GetRoomsAsync(long propertyId);
        Task SaveChangesAsync();
    }
}
