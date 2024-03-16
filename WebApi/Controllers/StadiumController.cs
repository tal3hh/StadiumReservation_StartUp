using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using ServiceLayer.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IStadiumService _stadiumService;

        public StadiumController(AppDbContext context, IStadiumService stadiumService)
        {
            _context = context;
            _stadiumService = stadiumService;
        }


        //STADIONLAR LIST
        [HttpPost("Pagine")]
        public async Task<IActionResult> GetPagineStadiumsAsync(StadiumPagineVM vm)
        {
            var pagineDto = await _stadiumService.StadiumListPagineAsync(vm);

            return Ok(pagineDto);
        }

        [HttpPost("Pagine-Search")]
        public async Task<IActionResult> GetSearchPagineStadiumsAsync(SearchStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumSearchListPagineAsync(vm);

            return Ok(paginateDto);
        }

        [HttpPost("Pagine-Filter")]
        public async Task<IActionResult> GetFilterPagineStadiumsAsync(FilterStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumFilterListPagineAsync(vm);

            return Ok(paginateDto);
        }

        [HttpPost("Pagine-Filter-Time")]
        public async Task<IActionResult> GetFilterTimePagineStadiumsAsync(TimeFilterStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumTimeFilterListPagineAsync(vm);

            return Ok(paginateDto);
        }

    }
}
