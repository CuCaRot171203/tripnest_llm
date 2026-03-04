using APPLICATION.DTOs.Notification;
using AutoMapper;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Mapping
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notifications, NotificationDto>()
                .ForMember(
                    dest => dest.IsRead, 
                    opt => opt
                    .MapFrom(src => src.IsRead ?? false));
        }
    }
}
