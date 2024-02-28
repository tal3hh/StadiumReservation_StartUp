using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Utlities;
using ServiceLayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumService
    {
        //Home
        Task<List<HomeListStadiumDto>> HomeStadiumOrderByListAsync();


        //Stadium List
        Task<Paginate<HomeListStadiumDto>> StadiumListPagineAsync(StadiumPagineVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumSearchListPagineAsync(SearchStadiumVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumFilterListPagineAsync(FilterStadiumVM vm);
        Task<Paginate<HomeListStadiumDto>> StadiumTimeFilterListPagineAsync(TimeFilterStadiumVM vm);


        //Stadium Detail
        Task<HomeDetailStadiumDto> StadiumDetailAsync(int stadiumId);
        Task<HomeDetailStadiumDto> DateStadiumDetailAsync(StadiumDetailVM vm);


        //Dash
        Task RemoveAsync(int id);
        Task UpdateAsync(UpdateStadiumDto dto);
        Task CreateAsync(CreateStadiumDto dto);
        Task<DashStadiumDto> FindById(int id);
        Task<List<DashStadiumDto>> AllAsync();
    }
}
