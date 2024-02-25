using AutoMapper;
using AutoMapper.Configuration.Annotations;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IStadiumService _stadiumService;

        public StadiumController(UserManager<AppUser> userManager, IStadiumService stadiumService)
        {
            _userManager = userManager;
            _stadiumService = stadiumService;
        }

        [HttpGet("Stadiums")]
        public async Task<IActionResult> Stadiums()
        {
            return Ok(await _stadiumService.AllAsync());
        }

        [HttpGet("Stadium/{id}")]
        public async Task<IActionResult> FindBy(int id)
        {
            return Ok(await _stadiumService.FindById(id));
        }

        [HttpPost("addStadium")]
        public async Task<IActionResult> addStadium(CreateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var user = await _userManager.FindByIdAsync(dto.AppUserId);
            if (user is null) return NotFound("Istifadeci tapilmadi.");

            await _stadiumService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("upadteStadium")]
        public async Task<IActionResult> upadteStadium(UpdateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _stadiumService.UpdateAsync(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadium(int id)
        {
            await _stadiumService.RemoveAsync(id);

            return Ok();
        }
    }
}
