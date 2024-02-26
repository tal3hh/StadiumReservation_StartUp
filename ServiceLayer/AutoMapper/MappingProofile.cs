using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using ServiceLayer.Dtos.Account;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Stadium;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Dtos.StadiumImage;
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
            CreateMap<Stadium, DashStadiumDto>()
                .ForMember(x => x.appuserName, y => y.MapFrom(z => z.AppUser.Fullname));
            CreateMap<DashStadiumDto, Stadium>();


            CreateMap<Reservation, CreateReservationDto>().ReverseMap();
            CreateMap<Reservation, UpdateReservationDto>().ReverseMap();
            CreateMap<Reservation, DashReservationDto>()
                .ForMember(x => x.arenaName, y => y.MapFrom(z => z.Area.Name));
            CreateMap<DashReservationDto, Reservation>();


            CreateMap<Area, CreateAreaDto>().ReverseMap();
            CreateMap<Area, UpdateAreaDto>().ReverseMap();
            CreateMap<Area, DashAreaDto>()
                .ForMember(x => x.StadiumName, y => y.MapFrom(z => z.Stadium.Name));
            CreateMap<DashAreaDto, Area>();


            CreateMap<StadiumImage, DashStadiumImageDto>()
                .ForMember(x => x.stadiumName, y => y.MapFrom(z => z.Stadium.Name));
            CreateMap<DashStadiumImageDto, StadiumImage>();
            CreateMap<StadiumImage, CreateStadiumImageDto>().ReverseMap();
            CreateMap<StadiumImage, UpdateStadiumImageDto>().ReverseMap();
        }
    }
}
