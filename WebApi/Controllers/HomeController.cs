using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Home;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext _context; 

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        //HOME 
        [HttpGet("Home/all-stadiums/orderPrice")]
        public async Task<IActionResult> HomeAllOrderStadiums()
        {
            var stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .OrderBy(x=> x.minPrice)
                .ToListAsync();

            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;  

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
                    Id = stadium.Id,
                    name = stadium.Name,
                    path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    minPrice = stadium.minPrice,
                    maxPrice = stadium.maxPrice,
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return Ok(allStadiumsEmptyHours);
        }
        
    }
}
