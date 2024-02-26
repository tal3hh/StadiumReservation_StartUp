using ServiceLayer.Dtos.Area.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IAreaService
    {
        Task<bool> RemoveAsync(int id);
        Task<bool> UpdateAsync(UpdateAreaDto dto);
        Task CreateAsync(CreateAreaDto dto);
        Task<DashAreaDto> FindById(int id);
        Task<List<DashAreaDto>> AllAsync();
    }
}
