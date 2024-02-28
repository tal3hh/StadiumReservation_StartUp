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
        [HttpPost("all-stadiums/Pagine")]
        public async Task<IActionResult> GetPagineStadiumsAsync(StadiumPagineVM vm)
        {
            var pagineDto = await _stadiumService.StadiumListPagineAsync(vm);

            return Ok(pagineDto);
        }

        [HttpPost("all-stadiums/Search/Pagine")]
        public async Task<IActionResult> GetSearchPagineStadiumsAsync(SearchStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumSearchListPagineAsync(vm);

            return Ok(paginateDto);
        }

        [HttpPost("all-stadiums/Filter/Pagine")]
        public async Task<IActionResult> GetFilterPagineStadiumsAsync(FilterStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumFilterListPagineAsync(vm);

            return Ok(paginateDto);
        }

        [HttpPost("all-stadiums/Filter-Time/Pagine")]
        public async Task<IActionResult> GetFilterTimePagineStadiumsAsync(TimeFilterStadiumVM vm)
        {
            var paginateDto = await _stadiumService.StadiumTimeFilterListPagineAsync(vm);

            return Ok(paginateDto);
        }

        //STADIUM DETALLARI SEHIFESI
        [HttpGet("stadiumDetail/{stadiumId}")]
        public async Task<IActionResult> StadiumDetail(int stadiumId)
        {
            var stadiumDetailDto = await _stadiumService.StadiumDetailAsync(stadiumId);

            return Ok(stadiumDetailDto);
        }

        [HttpPost("stadiumDetail/dateFilter")]
        public async Task<IActionResult> StadiumDetailDate(StadiumDetailVM vm)
        {
            var stadiumDetailDto = await _stadiumService.DateStadiumDetailAsync(vm);

            return Ok(stadiumDetailDto);
        }


        //[HttpPost("all-stadiums/Filter2/Pagine")]
        //public async Task<IActionResult> GetFilterPagineStadiumsAsync2(FilterStadiumVM vm)
        //{
        //    //VALIDATION
        //    // 1. startDate indiki >= vaxtdan sonra olmalidir.
        //    // 2. startDate endDate 3-5 gunden cox kecmesin.
        //    // (Cunki normalda stadion sahibleride cox sonraya almazlar)
        //    // 3. startDate endDate'den boyuk olsa yerleri deyissin.





        //    if (vm.startDate > vm.endDate)
        //        (vm.startDate, vm.endDate) = (vm.endDate, vm.startDate);

        //    var query = _context.Stadiums
        //        .AsNoTracking()
        //        .Include(s => s.StadiumImages)
        //        .Include(s => s.Areas)
        //        .ThenInclude(a => a.Reservations)
        //        .AsQueryable();

        //    if (!string.IsNullOrEmpty(vm.City))
        //        query.Where(x => x.City.Contains(vm.City));

        //    if (vm.minPrice != 0 || vm.maxPrice != 0)
        //    {
        //        if (vm.minPrice > vm.maxPrice)
        //            (vm.minPrice, vm.maxPrice) = (vm.maxPrice, vm.minPrice);

        //        query.Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice);
        //    }

        //    var stadium = await query.ToListAsync();

        //    if (vm.startDate.Date >= DateTime.Now.Date &&
        //        vm.startDate.Date <= DateTime.Now.AddDays(5) &&
        //        vm.endDate.Date <= DateTime.Now.AddDays(5))
        //    {
        //        var paginatedStadiums = stadium
        //       .Select(x =>
        //       {
        //           var reservedHours = x.Areas
        //               .SelectMany(a => a.Reservations)
        //               .Where(r =>
        //                   r.Date.Date >= vm.startDate.Date &&
        //                   r.Date.Hour >= vm.startDate.Date.Hour &&
        //                   r.Date <= vm.endDate.Date &&
        //                   x.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
        //               )
        //               .Select(r => r.Date.Hour)
        //               .Distinct()
        //               .ToList();

        //           var availableHourRanges = Enumerable.Range(0, 24 - 0)
        //               .Except(reservedHours)
        //               .Select(h => $"{h:00}:00-{(h + 1):00}:00")
        //               .Take(3)
        //               .ToList();

        //           return new HomeListStadiumDto
        //           {
        //               name = x.Name,
        //               path = x.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
        //               phoneNumber = x.PhoneNumber,
        //               addres = x.Address,
        //               minPrice = x.minPrice,
        //               maxPrice = x.maxPrice,
        //               emptyDates = availableHourRanges
        //           };
        //       })
        //       .ToList();

        //        return Ok(paginatedStadiums);
        //    }

        //    return BadRequest();
        //}
    }
}
