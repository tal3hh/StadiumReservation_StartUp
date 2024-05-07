using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Services;
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

        [HttpGet("Stadium/{stadiumiId}")]
        public async Task<IActionResult> FindByStadium(int stadiumiId)
        {
            return Ok(await _stadiumDetailService.FindByIdStadiumDetails(stadiumiId));
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

            var result = await _stadiumDetailService.CreateAsync(dto);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("upadte")]
        public async Task<IActionResult> upadteStadiumDetail(UpdateStadiumDetailDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumDetailService.UpdateAsync(dto);

            if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            else if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumDetail(int id)
        {
            var result = await _stadiumDetailService.RemoveAsync(id);

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
