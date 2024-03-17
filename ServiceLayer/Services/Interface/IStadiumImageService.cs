using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
