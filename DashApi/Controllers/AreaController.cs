using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Common.Result;
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
        public async Task<IActionResult> FindByArea(int id)
        {
            return Ok(await _areaService.FindById(id));
        }

        [HttpGet("Stadium/{stadiumId}")]
        public async Task<IActionResult> StadiumAreas(int stadiumId)
        {
            return Ok(await _areaService.FindByStadiumId(stadiumId));
        }

        [HttpPost("create")]
        public async Task<IActionResult> addArea(CreateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _areaService.CreateAsync(dto);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> addArea(UpdateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _areaService.UpdateAsync(dto);
            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);
           
            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeArea(int id)
        {
            if (!await _areaService.RemoveAsync(id)) 
                return NotFound("Area tapılmadı.");

            return Ok();
        }
    }
}
