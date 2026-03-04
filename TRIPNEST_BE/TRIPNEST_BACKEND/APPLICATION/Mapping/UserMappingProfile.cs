using APPLICATION.DTOs.Auth.Request;
using APPLICATION.DTOs.Auth.Response;
using AutoMapper;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile() 
        {
            CreateMap<CreateUserDto, DOMAIN.Models.Users>()
                .ForMember(d => d.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            CreateMap<Users, UserDto>()
                .ForMember(d => d.RoleId, opt => opt.MapFrom(s => s.RoleId));

            CreateMap<RefreshTokenDto, Refreshtokens>()
                .ForMember(dest => dest.TokenHash, opt => opt.MapFrom(src => src.Token))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ExpiresAt));
        }
    }
}
