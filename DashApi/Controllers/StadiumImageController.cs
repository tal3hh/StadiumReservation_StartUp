using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumImageController : ControllerBase
    {
        private readonly IStadiumImageService _stadiumImageService;

        public StadiumImageController(IStadiumImageService stadiumImageService)
        {
            _stadiumImageService = stadiumImageService;
        }

        [HttpGet("StadiumImages")]
        public async Task<IActionResult> StadiumImages()
        {
            return Ok(await _stadiumImageService.AllAsync());
        }

        [HttpGet("StadiumImage/{id}")]
        public async Task<IActionResult> StadiumImages(int id)
        {
            return Ok(await _stadiumImageService.FindById(id));
        }

        [HttpPost("addStadiumImage")]
        public async Task<IActionResult> addStadiumImage(CreateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _stadiumImageService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("upadteStadiumImage")]
        public async Task<IActionResult> upadteStadiumImage(UpdateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (!await _stadiumImageService.UpdateAsync(dto)) return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumImage(int id)
        {
            if (!await _stadiumImageService.RemoveAsync(id)) return NotFound();

            return Ok();
        }
    }
}
