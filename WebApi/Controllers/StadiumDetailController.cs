using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Contexts;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;
using ServiceLayer.ViewModels;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumDetailController : ControllerBase
    {
        private readonly IStadiumService _stadiumService;

        public StadiumDetailController(IStadiumService stadiumService)
        {
            _stadiumService = stadiumService;
        }


        //STADIUM DETALLARI SEHIFESI
        [HttpGet("{stadiumId}")]
        public async Task<IActionResult> FindStadium(int stadiumId)
        {
            var stadiumDetailDto = await _stadiumService.StadiumDetailAsync(stadiumId);

            return Ok(stadiumDetailDto);
        }

        [HttpPost("Filter-Date")]
        public async Task<IActionResult> StadiumDateFilter(StadiumDetailVM vm)
        {
            if (vm.date.Date <= DateTimeAz.Today)
                return Ok(await _stadiumService.StadiumDetailAsync(vm.stadiumId));

            return Ok(await _stadiumService.DateStadiumDetailAsync(vm));
        }
    }
}
