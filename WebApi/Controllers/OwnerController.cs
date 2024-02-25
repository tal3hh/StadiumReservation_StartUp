using DomainLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Account;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Reservation.Home;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.ViewModels;

namespace WebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        public OwnerController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        //ADD
        [HttpPost("Owner/Register")]
        public async Task<IActionResult> Register(UserCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var user = new AppUser
            {
                Fullname = dto.Fullname,
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.Number,
                CreateDate = DateTime.Now
            };
            IdentityResult identity = await _userManager.CreateAsync(user, dto.Password);

            if (identity.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Owner");
                return Ok();
            }

            foreach (var error in identity.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return BadRequest(dto);
        }

        
        //LIST
        [HttpGet("Owners")]
        public async Task<IActionResult> GetUsers()
        {
            var ownerUsers = await _userManager.GetUsersInRoleAsync("Owner");

            List<UserDto> userDtos = ownerUsers.Select(user => new UserDto
            {
                Fullname = user.Fullname,
                Username = user.UserName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                CreateDate = user.CreateDate
            }).ToList();

            return Ok(userDtos);
        }


        // REZERVLER 
        [HttpGet("{stadiumId}/Reservations/AfterNow")]
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

        [HttpPost("{stadiumId}/Reservations/dateFilter")]
        public async Task<IActionResult> StadiumDetailDate(int stadiumId, OwnerReservFilter vm)
        {
            Stadium? stadium = await _context.Stadiums
                        .AsNoTracking()
                        .Include(s => s.Areas)
                        .ThenInclude(a => a.Reservations)
                        .FirstOrDefaultAsync(s => s.Id == stadiumId);

            if (stadium == null)
                return NotFound($"Stadium with Id {stadiumId} not found.");

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
                    date = r.Date.ToString("dd/MMMM/yyyy")
                })
                .ToList();

            return Ok(reservationDtos);
        }
    }
}
