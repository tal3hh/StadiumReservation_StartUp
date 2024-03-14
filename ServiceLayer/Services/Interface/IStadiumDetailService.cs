using ServiceLayer.Dtos.StadiumDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumDetailService
    {
        Task<bool> RemoveAsync(int id);
        Task<bool> UpdateAsync(UpdateStadiumDetailDto dto);
        Task CreateAsync(CreateStadiumDetailDto dto);
        Task<DashStadiumDetailDto> FindById(int id);
        Task<List<DashStadiumDetailDto>> AllAsync();
    }
}
