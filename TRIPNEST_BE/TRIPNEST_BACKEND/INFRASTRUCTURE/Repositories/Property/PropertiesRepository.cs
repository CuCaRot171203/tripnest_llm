using DOMAIN;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Property;
using Microsoft.EntityFrameworkCore;
using INFRASTRUCTURE.Interfaces.Common;

namespace INFRASTRUCTURE.Repositories.Property
{
    public class PropertiesRepository : IPropertiesRepository
    {
        private readonly TripnestDbContext _db;

        public PropertiesRepository(TripnestDbContext db)
        {
            _db = db;
        }

        public async Task<Properties?> GetByIdAsync(long id)
        {
            return await _db.Properties
                .Include(p => p.Propertyphotos)
                .Include(p => p.Propertyamenities)
                .Include(p => p.Rooms)
                .FirstOrDefaultAsync(p => p.PropertyId == id && (p.Status == true || p.Status == false));
        }

        public async Task<PagedResult<Properties>> GetListAsync(string? q, string? city, double? lat, double? lng, double? radiusMeters,
            decimal? minPrice, decimal? maxPrice, List<int>? amenities, string? sort, int page = 1, int pageSize = 20)
        {
            var query = _db.Properties.AsQueryable();

            if (!string.IsNullOrEmpty(q))
                query = query.Where(p => p.TitleEn!.Contains(q) || p.TitleVi!.Contains(q) || p.DescriptionEn!.Contains(q) || p.DescriptionVi!.Contains(q));

            if (!string.IsNullOrEmpty(city))
                query = query.Where(p => p.City == city);

            if (minPrice.HasValue)
                query = query.Where(p => p.PriceBase >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PriceBase <= maxPrice.Value);

            if (amenities != null && amenities.Any())
            {
                query = query.Where(p => p.Propertyamenities.Any(pa => amenities.Contains(pa.AmenityId)));
            }

            // NOTE: radius filtering (lat/lng) is approximate — replace with spatial queries if available
            if (lat.HasValue && lng.HasValue && radiusMeters.HasValue)
            {
                double radiusKm = radiusMeters.Value / 1000.0;
                // naive bounding box (approx)
                double deg = radiusKm / 111.0;
                query = query.Where(p => p.Latitude >= lat - deg && p.Latitude <= lat + deg && p.Longitude >= lng - deg && p.Longitude <= lng + deg);
            }

            // sorting
            if (!string.IsNullOrEmpty(sort))
            {
                if (sort.Equals("price_asc", StringComparison.OrdinalIgnoreCase)) query = query.OrderBy(p => p.PriceBase);
                else if (sort.Equals("price_desc", StringComparison.OrdinalIgnoreCase)) query = query.OrderByDescending(p => p.PriceBase);
                else query = query.OrderByDescending(p => p.CreatedAt);
            }
            else
            {
                query = query.OrderByDescending(p => p.CreatedAt);
            }

            var total = await query.LongCountAsync();
            var items = await query.Skip((page - 1) * pageSize).Take(pageSize)
                .Include(p => p.Propertyphotos)
                .ToListAsync();

            return new PagedResult<Properties>
            {
                Page = page,
                PageSize = pageSize,
                Total = total,
                Items = items
            };
        }

        public async Task<Properties> AddAsync(Properties property)
        {
            property.CreatedAt = DateTime.UtcNow;
            _db.Properties.Add(property);
            await _db.SaveChangesAsync();
            return property;
        }

        public async Task<Properties> UpdateAsync(Properties property)
        {
            property.UpdatedAt = DateTime.UtcNow;
            _db.Properties.Update(property);
            await _db.SaveChangesAsync();
            return property;
        }

        public async Task SoftDeleteAsync(Properties property)
        {
            property.Status = false;
            property.UpdatedAt = DateTime.UtcNow;
            _db.Properties.Update(property);
            await _db.SaveChangesAsync();
        }

        public async Task AddPhotoAsync(Propertyphotos photo)
        {
            photo.CreatedAt = DateTime.UtcNow;
            _db.Propertyphotos.Add(photo);
            await _db.SaveChangesAsync();
        }

        public async Task<Propertyphotos?> GetPhotoAsync(long propertyId, long photoId)
        {
            return await _db.Propertyphotos.FirstOrDefaultAsync(p => p.PropertyId == propertyId && p.PhotoId == photoId);
        }

        public async Task<IEnumerable<Propertyphotos>> GetPhotosAsync(long propertyId)
        {
            return await _db.Propertyphotos.Where(p => p.PropertyId == propertyId).OrderBy(p => p.Order).ToListAsync();
        }

        public async Task<IEnumerable<DOMAIN.Models.Rooms>> GetRoomsAsync(long propertyId)
        {
            return await _db.Rooms.Where(r => r.PropertyId == propertyId).ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
