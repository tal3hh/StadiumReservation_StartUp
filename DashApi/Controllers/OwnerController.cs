using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Account;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Dtos.Reservation.Home;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;
using ServiceLayer.ViewModels;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public OwnerController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        [HttpPost("Owner/Login(Owner)")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            AppUser? user = await _userManager.FindByEmailAsync(dto.UsernameorEmail);
            if (user is null)
                user = await _userManager.FindByNameAsync(dto.UsernameorEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "Sahibkar tapılmadı");
                return NotFound(dto);
            }

            var identity = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);

            if (identity.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (!roles.Contains("Owner")) return NotFound("Sahibkar deyilsiniz."); 

                TokenResponseDto? token = _tokenService.GenerateJwtToken(user.UserName, (List<string>)roles);

                if (token == null) return BadRequest("Token is null");

                var userStadium = await _context.Stadiums.Include(x=> x.AppUser).FirstOrDefaultAsync(x => x.AppUserId == user.Id);
                if (userStadium == null) return NotFound("Bu istifadecinin stadiumu yoxdur.");

                var ownerDto = new OwnerDto
                {
                    stadiumId = userStadium.Id,
                    Username = user.UserName,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Token = token.Token,
                    ExpireDate = token.ExpireDate
                };
                return Ok(ownerDto);
            }
            return BadRequest(dto);
        }
        //ADD   
        [HttpPost("Owner/Register")]
        public async Task<IActionResult> Register(RegisterUserDto dto)
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

        [HttpPost("Reservations/dateFilter-start-end/{stadiumId}")]
        public async Task<IActionResult> StadiumDetailDate(int stadiumId, OwnerReservFilter vm)
        {
            Stadium? stadium = await _context.Stadiums
                        .AsNoTracking()
                        .Include(s => s.Areas)
                        .ThenInclude(a => a.Reservations)
                        .FirstOrDefaultAsync(s => s.Id == stadiumId);

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
                    date = r.Date.ToString("dd/MMMM/yyyy")
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
                    Date = group.Key.ToString("dd/MMMM/yyyy"),
                    TotalPrice = group.Sum(r => r.Price)
                })
                .ToList();

            return Ok(groupedReservations);
        }

        [HttpPost("Reservations/OneDateFilter-Reservations")]
        public async Task<IActionResult> StadiumDetailDateReserv(int stadiumId, DateTime date)
        {
            List<Reservation> reservations = await _context.Reservations
                        .AsNoTracking()
                        .Include(x => x.Area)
                        .ThenInclude(x => x.Stadium)
                        .Where(x => x.Date.Date == date.Date &&
                                   x.Area.StadiumId == stadiumId)
                        .OrderBy(x=> x.Date.Hour)
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
                    date = reservation.Date.ToString("DD-MMMM-yyyy"),
                    hour = $"{reservation.Date.ToString("HH:00")}-{reservation.Date.AddHours(1).ToString("HH:00")}",
                };

                ownerReservations.Add(ownerReservDto);
            }

            return Ok(ownerReservations);
        }
    }
}
