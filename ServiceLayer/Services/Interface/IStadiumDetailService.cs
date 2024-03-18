using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumDetailService
    {
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateStadiumDetailDto dto);
        Task<IResponse> CreateAsync(CreateStadiumDetailDto dto);
        Task<UpdateStadiumDetailDto> FindById(int id);
        Task<List<DashStadiumDetailDto>> FindByIdStadiumDetails(int stdiumId);
        Task<List<DashStadiumDetailDto>> AllAsync();
    }
}
