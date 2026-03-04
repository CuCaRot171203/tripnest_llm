using APPLICATION.DTOs.Property;
using AutoMapper;
using DOMAIN.Models;

namespace APPLICATION.Mapping
{
    public class PropertiesProfile : Profile
    {
        public PropertiesProfile()
        {
            CreateMap<Properties, PropertyDto>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TitleEn ?? src.TitleVi))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.DescriptionEn ?? src.DescriptionVi))
                .ForMember(dest => dest.PhotoUrls, opt => opt.Ignore())
                .ForMember(dest => dest.Amenities, opt => opt.MapFrom(src => src.Propertyamenities.Select(a => a.AmenityId)));
            CreateMap<Propertyphotos, UploadPhotoResponse>()
                .ForMember(d => d.PhotoId, o => o.MapFrom(s => s.PhotoId))
                .ForMember(d => d.Url, o => o.MapFrom(s => s.Url));
        }
    }
}
