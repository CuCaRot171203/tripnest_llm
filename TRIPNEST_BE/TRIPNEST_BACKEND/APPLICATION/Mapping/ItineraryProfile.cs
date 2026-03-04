using APPLICATION.DTOs.Itineraries;
using AutoMapper;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Mapping
{
    public class ItineraryProfile : Profile
    {
        public ItineraryProfile() 
        {
            CreateMap<Itineraries, ItineraryResponseDto>()
                .ForMember(dest => dest.Waypoints, 
                    opt => opt.MapFrom(src => src.Waypoints));

            CreateMap<Waypoints, WaypointDto>().ReverseMap();
            CreateMap<ItineraryCreateDto, Itineraries>()
                .ForMember(d => d.ItineraryId, 
                    opt => opt.Ignore())
                .ForMember(d => d.Waypoints, opt => opt.Ignore());
            CreateMap<ItineraryUpdateDto, Itineraries>()
                .ForMember(d => d.ItineraryId, 
                    opt => opt.Ignore())
                .ForMember(d => d.Waypoints, opt => opt.Ignore());
        }
    }
}
