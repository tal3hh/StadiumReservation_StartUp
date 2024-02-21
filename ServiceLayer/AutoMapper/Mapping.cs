using AutoMapper;
using DomainLayer.Entities;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Stadium;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.AutoMapper
{
    public class MappingProofile : Profile
    {
        public MappingProofile()
        {
            //Home
            CreateMap<Stadium, HomeStadiumDto>().ReverseMap();

            //Dash
            CreateMap<Stadium, CreateStadiumDto>().ReverseMap();
            CreateMap<Stadium, UpdateStadiumDto>().ReverseMap();

            CreateMap<Reservation, CreateReservationDto>().ReverseMap();
            CreateMap<Reservation, UpdateReservationDto>().ReverseMap();

            CreateMap<Area, CreateAreaDto>().ReverseMap();
            CreateMap<Area, UpdateAreaDto>().ReverseMap();

            CreateMap<StadiumImage, CreateStadiumImageDto>().ReverseMap();
        }
    }
}
