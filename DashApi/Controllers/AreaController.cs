using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet("Areas")]
        public async Task<IActionResult> Areas()
        {
            return Ok(await _areaService.AllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Findareas(int id)
        {
            return Ok(await _areaService.FindById(id));
        }

        [HttpGet("Stadium/{stadiumId}")]
        public async Task<IActionResult> StadiumAreas(int stadiumId)
        {
            return Ok(await _areaService.FindByStadiumId(stadiumId));
        }

        [HttpPost("addArea")]
        public async Task<IActionResult> addArea(CreateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _areaService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("upadteArea")]
        public async Task<IActionResult> addArea(UpdateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (!await _areaService.UpdateAsync(dto)) return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeArea(int id)
        {
            if (!await _areaService.RemoveAsync(id)) return NotFound();

            return Ok();
        }
    }
}
