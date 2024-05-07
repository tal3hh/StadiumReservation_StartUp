using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumImage;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumImageService
    {
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateStadiumImageDto dto);
        Task<IResponse> CreateAsync(CreateStadiumImageDto dto);
        Task<List<DashStadiumImageDto>> FindByIdStadiumImages(int stdiumId);
        Task<UpdateStadiumImageDto> FindById(int id);
        Task<List<DashStadiumImageDto>> AllAsync();
    }
}
