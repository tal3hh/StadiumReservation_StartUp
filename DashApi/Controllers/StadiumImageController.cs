using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Common.Result;
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

        [HttpGet("Stadium/{stadiumId}")]
        public async Task<IActionResult> FindStadiumImages(int stadiumId)
        {
            return Ok(await _stadiumImageService.FindByIdStadiumImages(stadiumId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> StadiumImages(int id)
        {
            return Ok(await _stadiumImageService.FindById(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> addStadiumImage(CreateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumImageService.CreateAsync(dto);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("upadte")]
        public async Task<IActionResult> upadteStadiumImage(UpdateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumImageService.UpdateAsync(dto);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumImage(int id)
        {
            var result = await _stadiumImageService.RemoveAsync(id);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }
    }
}
