using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumDiscountController : ControllerBase
    {
        private readonly IStadiumDiscountService _stadiumDiscountService;
        public StadiumDiscountController(IStadiumDiscountService stadiumDiscountService)
        {
            _stadiumDiscountService = stadiumDiscountService;
        }

        [HttpGet]
        public async Task<IActionResult> StadiumDiscounts()
        {
            return Ok(await _stadiumDiscountService.AllAsync());
        }

        [HttpGet("Stadium/{stadiumId}")]
        public async Task<IActionResult> FindStadiums(int stadiumId)
        {
            return Ok(await _stadiumDiscountService.FindByIdStadiums(stadiumId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Find(int id)
        {
            return Ok(await _stadiumDiscountService.FindById(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> addStadiumDiscount(CreateStadiumDiscountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumDiscountService.CreateAsync(dto);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest) return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> upadteStadiumDiscount(UpdateStadiumDiscountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _stadiumDiscountService.UpdateAsync(dto);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest) return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumDiscount(int id)
        {
            var result = await _stadiumDiscountService.RemoveAsync(id);

            if (result.RespType == RespType.Success) return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest) return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound) return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }
    }
}
