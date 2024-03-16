using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
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

namespace OwnerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AccountController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }


        [HttpPost("Login")]
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

                if (!roles.Contains("Owner")) return NotFound("Sahibkar kimi qeydiyyatda deyilsiniz.");

                TokenResponseDto? token = _tokenService.GenerateJwtToken(user.UserName, (List<string>)roles);

                if (token == null) return BadRequest("Token is null");

                var userStadium = await _context.Stadiums.Include(x => x.AppUser).FirstOrDefaultAsync(x => x.AppUserId == user.Id);

                if (userStadium == null) return NotFound("Bu istifadəçinin stadionu yoxdur.");

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

        [HttpPost("Register")]
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

    }
}
