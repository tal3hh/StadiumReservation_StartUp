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
using ServiceLayer.Utlities;

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

        [HttpGet("{id}")]
        public async Task<IActionResult> FindBy(int id)
        {
            return Ok(await _stadiumService.FindById(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> addStadium(CreateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumService.CreateAsync(dto);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest) return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> upadteStadium(UpdateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumService.UpdateAsync(dto);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest) return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadium(int id)
        {
            var result = await _stadiumService.RemoveAsync(id);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }
    }
}
