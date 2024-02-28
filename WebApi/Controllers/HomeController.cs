using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IStadiumService _stadiumService;

        public HomeController(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }

        [HttpGet("Home/all-stadiums/orderPrice")]
        public async Task<IActionResult> HomeAllOrderStadiums()
        {
            var list = await _stadiumService.HomeStadiumOrderByListAsync();

            return Ok(list);
        }

    }
}
