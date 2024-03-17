using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Reservation.Home;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using ServiceLayer.ViewModels;

namespace OwnerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IAreaService _areaService;
        private readonly IReservationService _reservationService;
        public OwnerController(AppDbContext context, IAreaService areaService, IReservationService reservationService)
        {
            _context = context;
            _areaService = areaService;
            _reservationService = reservationService;
        }


        [HttpGet("Stadium/{stadiumId}")]
        public async Task<IActionResult> StadiumAreas(int stadiumId)
        {
            return Ok(await _areaService.FindByStadiumId(stadiumId));
        }

        [HttpPost("addReservation")]
        public async Task<IActionResult> addReservation(CreateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _reservationService.CreateAsync(dto);

            if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("updateReservation")]
        public async Task<IActionResult> upadteReservation(UpdateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _reservationService.UpdateAsync(dto);

            if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }


        // REZERVLER 
        [HttpGet("Reservations/AfterNow")]
        public async Task<IActionResult> GetFutureReservations(int stadiumId)
        {
            Stadium? stadium = await _context.Stadiums
                        .Include(s => s.Areas)
                        .ThenInclude(a => a.Reservations)
                        .FirstOrDefaultAsync(s => s.Id == stadiumId);

            if (stadium == null)
                return NotFound($"Stadium with Id {stadiumId} not found.");

            var nowHour = DateTime.Now.Hour + 1;

            var reservationDtos = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r => r.Date > DateTime.Now &&
                            r.Date.Hour >= nowHour)
                .OrderBy(x => x.Date)
                .Select(r => new HomeReservationDto
                {
                    byName = r.ByName,
                    phoneNumber = r.PhoneNumber,
                    Price = r.Price,
                    areaName = r.Area.Name,
                    date = $"{r.Date.ToString("HH:00")}-{r.Date.AddHours(1).ToString("HH:00")}"
                })
                .ToList();

            var stadiumDto = new HomeStadiumDto
            {
                name = stadium.Name,
                price = stadium.minPrice,
                addres = stadium.Address,
                phoneNumber = stadium.PhoneNumber,
                Dates = reservationDtos
            };

            return Ok(stadiumDto);
        }

        [HttpPost("Reservations/dateFilter-start-end")]
        public async Task<IActionResult> StadiumDetailDate(OwnerReservFilter vm)
        {
            Stadium? stadium = await _context.Stadiums
                        .AsNoTracking()
                        .Include(s => s.Areas)
                        .ThenInclude(a => a.Reservations)
                        .FirstOrDefaultAsync(s => s.Id == vm.stadiumId);

            if (stadium == null) return NotFound($"Stadium not found.");

            var reservationDtos = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r => r.Date.Date >= vm.startDate.Date &&
                            r.Date.Date <= vm.endDate.Date
                            )
                .OrderBy(x => x.Date)
                .Select(r => new OwnerReservDto
                {
                    byName = r.ByName,
                    phoneNumber = r.PhoneNumber,
                    Price = r.Price,
                    areaName = r.Area.Name,
                    hour = $"{r.Date.ToString("HH:00")}-{r.Date.AddHours(1).ToString("HH:00")}",
                    date = r.Date.ToString("dd/MM/yyyy")
                })
                .ToList();

            return Ok(reservationDtos);
        }

        [HttpPost("Reservations/dateFilter-Price")]
        public async Task<IActionResult> StadiumDetailDatePrice(OwnerReservFilter vm)
        {
            Stadium? stadium = await _context.Stadiums
                        .AsNoTracking()
                        .Include(s => s.Areas)
                        .ThenInclude(a => a.Reservations)
                        .FirstOrDefaultAsync(s => s.Id == vm.stadiumId);

            if (stadium == null) return NotFound($"Stadium not found.");

            var groupedReservations = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r => r.Date.Date >= vm.startDate.Date && r.Date.Date <= vm.endDate.Date)
                .GroupBy(r => r.Date.Date)
                .OrderBy(group => group.Key)
                .Select(group => new
                {
                    StadiumId = stadium.Id,
                    StadiumName = stadium.Name,
                    Date = group.Key.ToString("dd/MM/yyyy"),
                    TotalPrice = group.Sum(r => r.Price)
                })
                .ToList();

            return Ok(groupedReservations);
        }

        [HttpPost("Reservations/OneDateFilter")]
        public async Task<IActionResult> StadiumDetailDateReserv(OwnerDateReserv vm)
        {
            List<Reservation> reservations = await _context.Reservations
                        .AsNoTracking()
                        .Include(x => x.Area)
                        .ThenInclude(x => x.Stadium)
                        .Where(x => x.Date.Date == vm.Date.Date &&
                                   x.Area.StadiumId == vm.stadiumId)
                        .OrderBy(x => x.Date.Hour)
                        .ToListAsync();

            List<OwnerReservDto> ownerReservations = new List<OwnerReservDto>();

            foreach (var reservation in reservations)
            {
                var ownerReservDto = new OwnerReservDto
                {
                    byName = reservation.ByName,
                    Price = reservation.Price,
                    phoneNumber = reservation.PhoneNumber,
                    areaName = reservation.Area.Name,
                    date = reservation.Date.ToString("dd/MM/yyyy"),
                    hour = $"{reservation.Date.ToString("HH:00")}-{reservation.Date.AddHours(1).ToString("HH:00")}",
                };

                ownerReservations.Add(ownerReservDto);
            }

            return Ok(ownerReservations);
        }
    }
}
