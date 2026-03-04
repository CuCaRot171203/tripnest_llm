using APPLICATION.DTOs.Common;
using APPLICATION.DTOs.FileService;
using APPLICATION.DTOs.Property;
using APPLICATION.Interfaces.Property;
using AutoMapper;
using DOMAIN.Models;
using INFRASTRUCTURE.Interfaces.Property;
using Microsoft.AspNetCore.Http;

namespace APPLICATION.Services.Property
{
    public class PropertiesService : IPropertiesService
    {
        private readonly IPropertiesRepository _repo;
        private readonly IFileStorageService _fileStorage;
        private readonly IMapper _mapper;

        public PropertiesService(IPropertiesRepository repo, IFileStorageService fileStorage, IMapper mapper)
        {
            _repo = repo;
            _fileStorage = fileStorage;
            _mapper = mapper;
        }

        public async Task<PropertyDto?> CreateAsync(CreatePropertyRequest request, Guid? ownerUserId = null)
        {
            var entity = new Properties
            {
                OwnerUserId = ownerUserId,
                CompanyId = request.CompanyId,
                TitleEn = request.Title,
                TitleVi = request.Title,
                DescriptionEn = request.Description,
                DescriptionVi = request.Description,
                Street = request.Street,
                City = request.City,
                Province = request.Province,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                AddressFormatted = request.AddressFormatted,
                PropertyType = request.PropertyType,
                PriceBase = request.PriceBase,
                Currency = request.Currency,
                Status = true
            };

            var created = await _repo.AddAsync(entity);

            if (request.Amenities != null && request.Amenities.Any())
            {
                foreach (var a in request.Amenities)
                {
                    created.Propertyamenities.Add(new Propertyamenities
                    {
                        AmenityId = a,
                        PropertyId = created.PropertyId
                    });
                }
                await _repo.SaveChangesAsync();
            }

            return _mapper.Map<PropertyDto>(created);
        }

        public async Task<PropertyDto?> UpdateAsync(UpdatePropertyRequest request, Guid? userId = null)
        {
            var exist = await _repo.GetByIdAsync(request.PropertyId);
            if(exist == null)
            {
                return null;
            }

            exist.TitleEn = request.Title;
            exist.TitleVi = request.Title;
            exist.DescriptionEn = request.Description;
            exist.DescriptionVi = request.Description;
            exist.Street = request.Street;
            exist.City = request.City;
            exist.Province = request.Province;
            exist.Country = request.Country;
            exist.Latitude = request.Latitude;
            exist.Longitude = request.Longitude;
            exist.AddressFormatted = request.AddressFormatted;
            exist.PropertyType = request.PropertyType;
            exist.PriceBase = request.PriceBase;
            exist.Currency = request.Currency;

            if (request.Status.HasValue)
            {
                exist.Status = request.Status;
            }

            if(request.Amenities != null)
            {
                var current = exist.Propertyamenities.ToList();
                exist.Propertyamenities.Clear();
                foreach (var c in current) { }
                foreach(var a in request.Amenities)
                {
                    exist.Propertyamenities.Add(new Propertyamenities { AmenityId = a, PropertyId = exist.PropertyId });
                }
            }

            var updated = await _repo.UpdateAsync(exist);
            return _mapper.Map<PropertyDto>(updated);
        }

        public async Task<bool> SoftDeleteAsync(long propertyId, Guid? userId = null)
        {
            var exist = await _repo.GetByIdAsync(propertyId);
            if (exist == null)
            {
                return false;
            }
            await _repo.SoftDeleteAsync(exist);
            return true;
        }

        public async Task<PagedResult<PropertyDto>> GetListAsync(string? q, string? city, double? lat, double? lng, double? radiusMeters,
            decimal? minPrice = null, decimal? maxPrice = null, List<int>? amenities = null, string? sort = null, int page = 1, int pageSize = 20)
        {
            var paged = await _repo.GetListAsync(q, city, lat, lng, radiusMeters, minPrice, maxPrice, amenities, sort, page, pageSize);
            var dtoItems = paged.Items.Select(p => _mapper.Map<PropertyDto>(p));
            return new PagedResult<PropertyDto> { Page = paged.Page, PageSize = paged.PageSize, Total = paged.Total, Items = dtoItems };
        }

        public async Task<PropertyDto?> GetByIdAsync(long id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null)
            {
                return null;
            }
            var dto = _mapper.Map<PropertyDto>(p);
            dto.PhotoUrls = p.Propertyphotos?.OrderBy(x => x.Order).Select(x => x.Url).ToList() ?? new List<string>();
            dto.Amenities = p.Propertyamenities?.Select(x => x.AmenityId).ToList() ?? new List<int>();
            return dto;
        }

        public async Task<UploadPhotoResponse> UploadPhotoAsync(long propertyId, IFormFile file, long? order = null, string? meta = null)
        {
            var prop = await _repo.GetByIdAsync(propertyId);
            if (prop == null)
            {
                throw new Exception("Property not found");
            }

            // upload
            var url = await _fileStorage.UploadFileAsync(file, $"properties/{propertyId}");

            var photo = new Propertyphotos
            {
                PropertyId = propertyId,
                Url = url,
                Order = (int?)(order ?? (prop.Propertyphotos?.Count + 1)),
                Meta = meta
            };

            await _repo.AddPhotoAsync(photo);

            return new UploadPhotoResponse { PhotoId = photo.PhotoId, Url = photo.Url };
        }

        public async Task<IEnumerable<UploadPhotoResponse>> GetPhotosAsync(long propertyId)
        {
            var photos = await _repo.GetPhotosAsync(propertyId);
            return photos.Select(p => new UploadPhotoResponse { PhotoId = p.PhotoId, Url = p.Url });
        }

        public async Task<IEnumerable<ShortRoomDto>> GetRoomsAsync(long propertyId)
        {
            var rooms = await _repo.GetRoomsAsync(propertyId);
            return rooms.Select(r => new ShortRoomDto
            {
                RoomId = r.RoomId,
                Name = r.NameEn,
                Price = r.PricePerNight
            });
        }
    }
}
