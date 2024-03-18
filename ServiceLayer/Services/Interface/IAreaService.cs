using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services.Interface
{
    public interface IAreaService
    {
        Task<bool> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateAreaDto dto);
        Task<IResponse> CreateAsync(CreateAreaDto dto);
        Task<UpdateAreaDto> FindById(int id);
        Task<List<DashAreaDto>> FindByStadiumId(int stadiumId);
        Task<List<DashAreaDto>> AllAsync();
    }
}
