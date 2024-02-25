using ServiceLayer.Dtos.Stadium.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IStadiumService
    {


        Task RemoveAsync(int id);
        Task UpdateAsync(UpdateStadiumDto dto);
        Task CreateAsync(CreateStadiumDto dto);
        Task<DashStadiumDto> FindById(int id);
        Task<List<DashStadiumDto>> AllAsync();
    }
}
