﻿using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Utlities.Pagine;
using ServiceLayer.ViewModels;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumService
    {
        //Home
        Task<List<HomeListStadiumDto>> HomeStadiumOrderByListAsync();
        Task<List<HomeListStadiumDto>> HomeStadiumCompanyListAsync();


        //Stadium List
        Task<Paginate<HomeListStadiumDto>> StadiumListPagineAsync(StadiumPagineVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumSearchListPagineAsync(SearchStadiumVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumFilterListPagineAsync(FilterStadiumVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumTimeFilterListPagineAsync(TimeFilterStadiumVM vm);


        //Stadium Detail
        Task<HomeDetailStadiumDto> StadiumDetailAsync(int stadiumId);


        //Dash
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateStadiumDto dto);
        Task<IResponse> CreateAsync(CreateStadiumDto dto);
        Task<UpdateStadiumDto> FindById(int id);
        Task<List<DashStadiumDto>> AllAsync();
    }
}
