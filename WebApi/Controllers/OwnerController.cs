using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Home;
using ServiceLayer.Dtos.Stadium.Home;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OwnerController(AppDbContext context)
        {
            _context = context;
        }

        // Rezerv olunanlar (indiki vaxtdan sonralar)
        [HttpGet("{stadiumId}/reservations-hours-byNames")]
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
    }
}
