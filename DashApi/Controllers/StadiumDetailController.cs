using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumDetailController : ControllerBase
    {
        private readonly IStadiumDetailService _stadiumDetailService;

        public StadiumDetailController(IStadiumDetailService stadiumDetailService)
        {
            _stadiumDetailService = stadiumDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> StadiumDetails()
        {
            return Ok(await _stadiumDetailService.AllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> StadiumDetails(int id)
        {
            return Ok(await _stadiumDetailService.FindById(id));
        }

        [HttpPost("add")]
        public async Task<IActionResult> addStadiumDetail(CreateStadiumDetailDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _stadiumDetailService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("upadte")]
        public async Task<IActionResult> upadteStadiumDetail(UpdateStadiumDetailDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (!await _stadiumDetailService.UpdateAsync(dto)) return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumDetail(int id)
        {
            if (!await _stadiumDetailService.RemoveAsync(id)) return NotFound();

            return Ok();
        }
    }
}
