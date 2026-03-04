using APPLICATION.DTOs.Message;
using AutoMapper;
using DOMAIN.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPLICATION.Mapping
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Messages, MessageDto>().ReverseMap();
        }
    }
}
