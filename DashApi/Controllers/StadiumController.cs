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
        private readonly IStadiumService _stadiumService;

        public StadiumController(IStadiumService stadiumService)
        {
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

            int result = await _stadiumService.CreateAsync(dto);

            if (result == 1) return NotFound("Istifadeci tapilmadi.");

            if (result == 2) return BadRequest("Sahibkar kimi qeydiyyatda deyilsiniz.");

            if (result == 3) return BadRequest("Bu sahibkarın adında artiq bir stadion var. " +
                                                        "(Yeni bir istifadəçi yaradın)");

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
