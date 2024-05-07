using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Reservation.Dash;

namespace ServiceLayer.Services.Interface
{
    public interface IReservationService
    {
        Task<IResponse> RemoveAsync(int id);
        Task<IResponse> UpdateAsync(UpdateReservationDto dto);
        Task<IResponse> CreateAsync(CreateReservationDto dto);
        Task<UpdateReservationDto> FindById(int id);
        Task<List<DashReservationDto>> AllAsync();
    }
}
