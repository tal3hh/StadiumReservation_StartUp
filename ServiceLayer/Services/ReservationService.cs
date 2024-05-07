using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;

namespace ServiceLayer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IRepository<Reservation> _repoRes;
        private readonly IRepository<Stadium> _repoStad;
        private readonly IMapper _mapper;
        public ReservationService(IMapper mapper, IRepository<Reservation> repoRes, IRepository<Stadium> repoStad)
        {
            _context = context;
            _mapper = mapper;
            _repoRes = repoRes;
            _repoStad = repoStad;
        }


        public async Task<List<DashReservationDto>> AllAsync()
        {
            var list = await _repoRes.GetListAsync(include: x => x.Include(x => x.Area));
            return _mapper.Map<List<DashReservationDto>>(list);
        }

        public async Task<UpdateReservationDto> FindById(int id)
        {
            Reservation? entity = await _repoRes.GetAsync(exp: x => x.Id == id,
                                                          include: x => x.Include(x => x.Area));
            return _mapper.Map<UpdateReservationDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateReservationDto dto)
        {
            var stadium = await _repoStad.GetAsync(include: x => x.Include(x => x.Areas),
                                                    exp: x => x.Areas.Any(x => x.Id == dto.areaId));
            if (stadium == null)
                return new Response(RespType.NotFound,
                    "Stadion bazada tapılmadı.");

            if (await _repoRes.ExistsAsync(x => x.Date.Hour == dto.Date.Hour && x.Date.Day == dto.Date.Day &&
                                                x.Date.Month == dto.Date.Month && x.AreaId == dto.areaId))
                return new Response(RespType.BadReqest,
                    $"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");

            if (dto.Date.Hour < DateTimeAz.Now.Hour && dto.Date.Date == DateTimeAz.Today)
                return new Response(RespType.BadReqest,
                    "Keçmiş saat üçün rezerv oluna bilməz.");

            if (dto.Date.Hour > stadium.closeHour || (dto.Date.Hour < stadium.openHour && dto.Date.Hour > stadium.nightHour))
                return new Response(RespType.BadReqest,
                    $"Stadion saatlari '{stadium.openHour}:00-{stadium.closeHour}:00' arasında açıqdır. Gecə saatlarında isə '{stadium.nightHour}:00' sonra bağlıdır.");

            Reservation Reservation = _mapper.Map<Reservation>(dto);
            Reservation.CreateDate = DateTimeAz.Now;
            Reservation.IsActive = true;

            await _repoRes.CreateAsync(Reservation);
            await _repoRes.SaveChangesAsync();
            return new Response(RespType.Success, "Rezerv əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateReservationDto dto)
        {
            var stadium = await _repoStad.GetAsync(include: x => x.Include(x => x.Areas),
                                                    exp: x => x.Areas.Any(x => x.Id == dto.areaId));
            if (stadium == null)
                return new Response(RespType.NotFound, "Stadion bazada tapılmadı.");

            var reserv = await _repoRes.GetAsync(x => x.Date.Hour == dto.Date.Hour && x.Date.Day == dto.Date.Day &&
                                                      x.Date.Month == dto.Date.Month && x.AreaId == dto.areaId);

            if (reserv != null && reserv.Id != dto.Id)
                return new Response(RespType.BadReqest,
                    $"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");

            if (dto.Date.Hour < DateTimeAz.Now.Hour && dto.Date.Date == DateTimeAz.Today)
                return new Response(RespType.BadReqest, "Keçmiş saat üçün rezerv oluna bilməz.");

            if (dto.Date.Hour > stadium.closeHour || (dto.Date.Hour < stadium.openHour && dto.Date.Hour > stadium.nightHour))
                return new Response(RespType.BadReqest,
                    $"Stadion saatlari '{stadium.openHour}:00-{stadium.closeHour}:00' arasında açıqdır. Gecə saatlarında isə '{stadium.nightHour}:00' sonra bağlıdır.");

            Reservation? DBReservation = await _repoRes.GetAsync(x => x.Id == dto.Id);
            if (DBReservation == null)
                return new Response(RespType.NotFound, "Rezerv tapılmadlı.");

            Reservation Reservation = _mapper.Map<Reservation>(dto);
            _repoRes.Update(Reservation, DBReservation);
            await _repoRes.SaveChangesAsync();
            return new Response(RespType.Success, "Rezerv uğurla dəyişildi.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            Reservation? Reservation = await _repoRes.GetAsync(x => x.Id == id);
            if (Reservation == null)
                return new Response(RespType.NotFound, "Rezerv tapılmadı.");

            _repoRes.Remove(Reservation);
            await _repoRes.SaveChangesAsync();
            return new Response(RespType.Success, "Rezerv uğurla silindi.");
        }
    }
}
