using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IStadiumService _stadiumService;
        public StadiumDiscountController(IStadiumDiscountService stadiumDiscountService, IStadiumService stadiumService)
        {
            _stadiumDiscountService = stadiumDiscountService;
            _stadiumService = stadiumService;
        }

        [HttpGet("StadiumDiscounts")]
        public async Task<IActionResult> StadiumDiscounts()
        {
            return Ok(await _stadiumDiscountService.AllAsync());
        }

        [HttpGet("StadiumDiscount/{id}")]
        public async Task<IActionResult> StadiumDiscounts(int id)
        {
            return Ok(await _stadiumDiscountService.FindById(id));
        }

        [HttpPost("StadiumDiscount")]
        public async Task<IActionResult> addStadiumDiscount(CreateStadiumDiscountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (await _stadiumService.FindById(dto.StadiumId) == null)
                return NotFound("Stadium not found");

            await _stadiumDiscountService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("StadiumDiscount")]
        public async Task<IActionResult> upadteStadiumDiscount(UpdateStadiumDiscountDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (await _stadiumService.FindById(dto.StadiumId) == null)
                return NotFound("Stadium not found");

            if (!await _stadiumDiscountService.UpdateAsync(dto)) return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumDiscount(int id)
        {
            if (!await _stadiumDiscountService.RemoveAsync(id)) return NotFound();

            return Ok();
        }
    }
}
