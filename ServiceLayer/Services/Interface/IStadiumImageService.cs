using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.StadiumImage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumImageService
    {
        Task<bool> RemoveAsync(int id);
        Task<bool> UpdateAsync(UpdateStadiumImageDto dto);
        Task CreateAsync(CreateStadiumImageDto dto);
        Task<DashStadiumImageDto> FindById(int id);
        Task<List<DashStadiumImageDto>> AllAsync();
    }
}
