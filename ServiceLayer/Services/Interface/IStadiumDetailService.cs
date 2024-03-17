using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumDetailService
    {
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateStadiumDetailDto dto);
        Task<IResponse> CreateAsync(CreateStadiumDetailDto dto);
        Task<DashStadiumDetailDto> FindById(int id);
        Task<List<DashStadiumDetailDto>> AllAsync();
    }
}
