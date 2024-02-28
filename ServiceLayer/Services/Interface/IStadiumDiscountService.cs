using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Dtos.StadiumImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumDiscountService
    {
        Task<bool> RemoveAsync(int id);
        Task<bool> UpdateAsync(UpdateStadiumDiscountDto dto);
        Task CreateAsync(CreateStadiumDiscountDto dto);
        Task<DashStadiumDiscountDto> FindById(int id);
        Task<List<DashStadiumDiscountDto>> AllAsync();
    }
}
