using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.Dtos.Account;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        #region Role
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole(string name)
        {
            if (string.IsNullOrEmpty(name)) return BadRequest();

            var role = new IdentityRole
            {
                Name = name
            };

            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
                return Ok();

            return BadRequest();
        }
        #endregion

        #region Register
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
                await _userManager.AddToRoleAsync(user, "Admin");

                return Ok();
            }

            foreach (var error in identity.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return BadRequest(dto);
        }
        #endregion

        #region Login
        [HttpPost("Login(Admin)")]
        public async Task<IActionResult> Login(LoginUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            AppUser? user = await _userManager.FindByEmailAsync(dto.UsernameorEmail);
            if (user is null)
                user = await _userManager.FindByNameAsync(dto.UsernameorEmail);

            if (user is null)
            {
                ModelState.AddModelError("", "İstifadəçi tapılmadı");
                return NotFound(dto);
            }

            var identity = await _signInManager.PasswordSignInAsync(user, dto.Password, dto.RememberMe, false);

            if (identity.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                TokenResponseDto? token = _tokenService.GenerateJwtToken(user.UserName, (List<string>)roles);

                if (token == null) return BadRequest("Token null");

                var ownerDto = new UserLoginListDto
                {
                    Username = user.UserName,
                    Fullname = user.Fullname,
                    Email = user.Email,
                    Token = token.Token,
                    ExpireDate = token.ExpireDate
                };
                return Ok(ownerDto);
            }
            else
            {
                if (!(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", $"Qeydiyyat zamanı daxil etdiyiniz e-poçtu təsdiqləyin." +
                                                    $"Əks halda hesaba daxil ola bilməzsiniz." +
                                                    $"E-poçt ünvanı: {user.Email}");
                    return BadRequest(dto);
                }
            }
            return BadRequest(dto);
        }
        #endregion

        #region UsersANDRoles
        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            var userList = await _userManager.Users.ToListAsync();

            List<UserDto> userDtos = new List<UserDto>();

            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDto
                {
                    Fullname = user.Fullname,
                    Username = user.UserName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    CreateDate = user.CreateDate,
                    userRole = roles.ToList()
                };

                userDtos.Add(userDto);
            }

            return Ok(userDtos);
        }

        [HttpGet("Roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.ToListAsync();

            List<RoleDto> roleDtos = roles.Select(role => new RoleDto
            {
                Name = role.Name

            }).ToList();

            return Ok(roleDtos);
        }
        #endregion

        #region Delete
        [HttpDelete("User/{UsernameorEmail}")]
        public async Task<IActionResult> UserRemove(string UsernameorEmail)
        {
            if (UsernameorEmail != null)
            {
                var user = new AppUser();

                user = await _userManager.FindByNameAsync(UsernameorEmail);
                if (user == null)
                    user = await _userManager.FindByEmailAsync(UsernameorEmail);

                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                    return Ok("Istifadeci silindi");
                }
                return NotFound("Istifadeci tapilmadi.");
            }

            return Unauthorized("UsernameorEmail bosdur");
        }

        [HttpDelete("Role/{name}")]
        public async Task<IActionResult> RoleRemove(string name)
        {
            if (name != null)
            {
                var role = await _roleManager.FindByNameAsync(name);

                if (role != null)
                {
                    await _roleManager.DeleteAsync(role);
                    return Ok("Role silindi");
                }
                return NotFound("Role tapilmadi.");
            }

            return Unauthorized("Name bosdur");
        }
        #endregion

        #region ResetPassword
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPassUserVM vm)
        {
            if (!ModelState.IsValid) return BadRequest(vm);

            AppUser? user = await _userManager.FindByNameAsync(vm.Username);
            if (user is null) return NotFound("Belə bir istifadəçi tapılmadı.");

            if (!await _userManager.CheckPasswordAsync(user, vm.OldPassword))
                return BadRequest("Əvvəlki şifrə yanlışdır.");

            var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);

            if (result.Succeeded)
                return Ok("Şifrə uğurla dəyişdirildi.");

            return BadRequest(result.Errors.Select(e => e.Description));
        }
        #endregion
    }
}
