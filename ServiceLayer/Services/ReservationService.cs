using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashReservationDto>> AllAsync()
        {
            var list = await _context.Reservations.Include(x => x.Area).ToListAsync();

            return _mapper.Map<List<DashReservationDto>>(list);
        }

        public async Task<UpdateReservationDto> FindById(int id)
        {
            Reservation? entity = await _context.Reservations.Include(x => x.Area).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UpdateReservationDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateReservationDto dto)
        {
            var stadium = await _context.Stadiums.Include(x => x.Areas).FirstOrDefaultAsync(x => x.Areas.Any(x => x.Id == dto.areaId));
            if (stadium == null)
                return new Response(RespType.NotFound,
                    "Stadion bazada tapılmadı.");

            if (await _context.Reservations.AnyAsync(x => x.Date.Hour == dto.Date.Hour &&
                                                       x.Date.Day == dto.Date.Day &&
                                                       x.Date.Year == dto.Date.Year &&
                                                       x.Date.Month == dto.Date.Month &&
                                                       x.AreaId == dto.areaId))
                return new Response(RespType.BadReqest, 
                    $"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");

            if (dto.Date.Hour < DateTimeAz.Now.Hour &&
                dto.Date.Date == DateTimeAz.Today)
                return new Response(RespType.BadReqest, 
                    "Keçmiş saat üçün rezerv oluna bilməz.");

            if (dto.Date.Hour > stadium.closeHour || 
                (dto.Date.Hour < stadium.openHour && dto.Date.Hour > stadium.nightHour))
                return new Response(RespType.BadReqest, 
                    $"Stadion saatlari '{stadium.openHour}:00-{stadium.closeHour}:00' arasında açıqdır. Gecə saatlarında isə '{stadium.nightHour}:00' sonra bağlıdır.");


            
            Reservation Reservation = _mapper.Map<Reservation>(dto);
            Reservation.CreateDate = DateTimeAz.Now;
            Reservation.IsActive = true;

            await _context.Reservations.AddAsync(Reservation);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Rezerv əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateReservationDto dto)
        {
            var stadium = await _context.Stadiums.Include(x => x.Areas).FirstOrDefaultAsync(x => x.Areas.Any(x => x.Id == dto.areaId));
            if (stadium == null)
                return new Response(RespType.NotFound,
                    "Stadion bazada tapılmadı.");

            var reserv = await _context.Reservations.FirstOrDefaultAsync(x => x.Date.Hour == dto.Date.Hour &&
                                                                              x.Date.Day == dto.Date.Day &&
                                                                              x.Date.Year == dto.Date.Year &&
                                                                              x.Date.Month == dto.Date.Month &&
                                                                              x.AreaId == dto.areaId);
            if (reserv != null && reserv.Id != dto.Id)
                return new Response(RespType.BadReqest,
                    $"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");

            if (dto.Date.Hour < DateTimeAz.Now.Hour &&
                dto.Date.Date == DateTimeAz.Today)
                return new Response(RespType.BadReqest,
                    "Keçmiş saat üçün rezerv oluna bilməz.");

            if (dto.Date.Hour > stadium.closeHour ||
                (dto.Date.Hour < stadium.openHour && dto.Date.Hour > stadium.nightHour))
                return new Response(RespType.BadReqest,
                    $"Stadion saatlari '{stadium.openHour}:00-{stadium.closeHour}:00' arasında açıqdır. Gecə saatlarında isə '{stadium.nightHour}:00' sonra bağlıdır.");


            Reservation? DBReservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBReservation != null)
            {
                Reservation Reservation = _mapper.Map<Reservation>(dto);

                _context.Entry(DBReservation).CurrentValues.SetValues(Reservation);

                await _context.SaveChangesAsync();

                return new Response(RespType.Success,
                    "Rezerv uğurla dəyişildi.");
            }
            return new Response(RespType.NotFound,
                    "Rezerv tapılmadlı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            Reservation? Reservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == id);

            if (Reservation != null)
            {
                _context.Reservations.Remove(Reservation);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success,
                    "Rezerv uğurla silindi.");
            }
            return new Response(RespType.NotFound,
                    "Rezerv tapılmadı.");
        }
    }
}
