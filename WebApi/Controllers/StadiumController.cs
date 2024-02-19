using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Home;
using ServiceLayer.Dtos.Stadium.Home;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StadiumController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("UTC/Aze")]
        public async Task<IActionResult> UTCAze()
        {

            // UTC olarak bir tarih oluşturun
            DateTime utcDateTime = DateTime.UtcNow;

            // Azerbaycan saat dilimini temsil eden TimeZoneInfo'yi alın
            TimeZoneInfo azerbaijanTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Azerbaijan Standard Time");

            // UTC tarihini Azerbaycan saat dilimine çevirin
            DateTime azerbaijanDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, azerbaijanTimeZone);
            return Ok(azerbaijanDateTime);
        }

        //Butun stadionlarin isyahisi (bos 3 saat)
        [HttpGet("all-stadiums/empty-hours")]
        public async Task<IActionResult> GetAllStadiumsEmptyHours()
        {
            var stadiums = await _context.Stadiums
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .ToListAsync();

            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;  // Şu anki saatte bir rezervasyon olamaz

            var allStadiumsEmptyHours = stadiums.Select(stadium =>
            {
                var reservedHours = stadium.Areas
                    .SelectMany(a => a.Reservations)
                    .Where(r =>
                        r.Date.Date == today &&
                        r.Date.Hour >= nowHour &&
                        r.Date < today.AddDays(1) &&
                        stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                    )
                    .Select(r => r.Date.Hour)
                    .Distinct()
                    .ToList();

                var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                    .Except(reservedHours)
                    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                    .Take(3)
                    .ToList();

                return new HomeListStadiumDto
                {
                    name = stadium.Name,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    price = stadium.Price,
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return Ok(allStadiumsEmptyHours);
        }

        //Stadionun detallari sehifesinde gorsenecek (Bos saatlar olacag)
        [HttpGet("{stadiumId}/stadiumDetail-empty-hours")]
        public async Task<IActionResult> StadiumDetailEmptyHour(int stadiumId)
        {
            Stadium? stadium = await _context.Stadiums
                     .Include(s => s.Areas)
                     .ThenInclude(a => a.Reservations)
                     .FirstOrDefaultAsync(s => s.Id == stadiumId);

            if (stadium == null) return NotFound($"Stadium with Id {stadiumId} not found.");

            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;  //indiki saatda bir rezerv ola bilmez

            var reservedHours = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r =>
                    r.Date.Date == today &&
                    r.Date.Hour >= nowHour &&
                    r.Date < today.AddDays(1) &&
                    stadium.Areas.Any(a => a.Reservations.Any(ar => ar.Date.Hour == r.Date.Hour && ar.Id != r.Id))
                )
                .Select(r => r.Date.Hour)
                .Distinct()
                .ToList();

            var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                .Except(reservedHours)
                .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                .ToList();

            var homeDetailStadiumDto = new HomeDetailStadiumDto
            {
                name = stadium.Name,
                phoneNumber = stadium.PhoneNumber,
                addres = stadium.Address,
                price = stadium.Price,
                description = stadium.Description,
                view = stadium.View,
                emptyDates = availableHourRanges
            };

            return Ok(homeDetailStadiumDto);
        }
    }
}
