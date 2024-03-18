using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumDiscountService
    {
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateStadiumDiscountDto dto);
        Task<IResponse> CreateAsync(CreateStadiumDiscountDto dto);
        Task<UpdateStadiumDiscountDto> FindById(int id);
        Task<List<DashStadiumDiscountDto>> FindByIdStadiums(int stadiumId);
        Task<List<DashStadiumDiscountDto>> AllAsync();
    }
}
