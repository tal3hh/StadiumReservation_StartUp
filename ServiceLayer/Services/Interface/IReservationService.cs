using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Dtos.Reservation.Dash;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.Interface
{
    public interface IReservationService
    {
        Task<bool> RemoveAsync(int id);
        Task<bool> UpdateAsync(UpdateReservationDto dto);
        Task<int> CreateAsync(CreateReservationDto dto);
        Task<DashReservationDto> FindById(int id);
        Task<List<DashReservationDto>> AllAsync();
    }
}
