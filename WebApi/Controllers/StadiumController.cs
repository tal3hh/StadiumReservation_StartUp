using DomainLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Home;
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

        public StadiumController(AppDbContext context)
        {
            _context = context;
        }


        //STADIONLAR LIST
        [HttpGet("all-stadiums")]
        public async Task<IActionResult> GetAllStadiumsEmptyHours()
        {
            var stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .ToListAsync();

            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;  // Şu anki saatte bir rezervasyon olamaz

            var allStadiumsEmptyHours = stadiums.Select(stadium =>
            {
                var reservedHours = stadium.Areas
                    .SelectMany(a => a.Reservations)
                    .Where(r =>
                        r.Date.Date == today &&
                        r.Date.Hour >= nowHour &&
                        r.Date < today.AddDays(1) &&
                        stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                    )
                    .Select(r => r.Date.Hour)
                    .Distinct()
                    .ToList();

                var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                    .Except(reservedHours)
                    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                    .Take(3)
                    .ToList();

                return new HomeListStadiumDto
                {
                    name = stadium.Name,
                    path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    minPrice = stadium.minPrice,
                    maxPrice = stadium.maxPrice,
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return Ok(allStadiumsEmptyHours);
        }

        [HttpPost("all-stadiums/Pagine")]
        public async Task<IActionResult> GetPagineStadiumsAsync(StadiumVM vm)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .ToListAsync();

            var paginatedStadiums = allStadiumsEmptyHours
                .Select(stadium =>
                {
                    var reservedHours = stadium.Areas
                        .SelectMany(a => a.Reservations)
                        .Where(r =>
                            r.Date.Date == today &&
                            r.Date.Hour >= nowHour &&
                            r.Date < today.AddDays(1) &&
                            stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                        )
                        .Select(r => r.Date.Hour)
                        .Distinct()
                        .ToList();

                    var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                        .Except(reservedHours)
                        .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                        .Take(3)
                        .ToList();

                    return new HomeListStadiumDto
                    {
                        name = stadium.Name,
                        path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return Ok(paginateResult);
        }

        [HttpPost("all-stadiums/Search/Pagine")]
        public async Task<IActionResult> GetSearchPagineStadiumsAsync(SearchStadiumVM vm)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .Where(x => x.Name.Contains(vm.search.Trim()))
                .ToListAsync();

            var paginatedStadiums = allStadiumsEmptyHours
                .Select(stadium =>
                {
                    var reservedHours = stadium.Areas
                        .SelectMany(a => a.Reservations)
                        .Where(r =>
                            r.Date.Date == today &&
                            r.Date.Hour >= nowHour &&
                            r.Date < today.AddDays(1) &&
                            stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                        )
                        .Select(r => r.Date.Hour)
                        .Distinct()
                        .ToList();

                    var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                        .Except(reservedHours)
                        .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                        .Take(3)
                        .ToList();

                    return new HomeListStadiumDto
                    {
                        name = stadium.Name,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return Ok(paginateResult);
        }

        [HttpPost("all-stadiums/Filter/Pagine")]
        public async Task<IActionResult> GetFilterPagineStadiumsAsync(FilterStadiumVM vm)
        {
            if (vm.minPrice > vm.maxPrice)
                (vm.minPrice, vm.maxPrice) = (vm.maxPrice, vm.minPrice);

            var query = _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice)
                .AsQueryable();

            if (!string.IsNullOrEmpty(vm.City))
                query = query.Where(x => x.City.Contains(vm.City));

            var allStadiumsEmptyHours = await query.ToListAsync();

            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var paginatedStadiums = allStadiumsEmptyHours
                .Select(stadium =>
                {
                    var reservedHours = stadium.Areas
                        .SelectMany(a => a.Reservations)
                        .Where(r =>
                            r.Date.Date == today &&
                            r.Date.Hour >= nowHour &&
                            r.Date < today.AddDays(1) &&
                            stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                        )
                        .Select(r => r.Date.Hour)
                        .Distinct()
                        .ToList();

                    var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                        .Except(reservedHours)
                        .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                        .Take(3)
                        .ToList();

                    return new HomeListStadiumDto
                    {
                        name = stadium.Name,
                        path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return Ok(paginateResult);
        }


        //STADIUM DETALLARI SEHIFESI
        [HttpGet("{stadiumId}/stadiumDetail")]
        public async Task<IActionResult> StadiumDetail(int stadiumId)
        {
            Stadium? stadium = await _context.Stadiums
                     .AsNoTracking()
                     .Include(s => s.StadiumImages)
                     .Include(s => s.Areas)
                     .ThenInclude(a => a.Reservations)
                     .FirstOrDefaultAsync(s => s.Id == stadiumId);

            if (stadium == null) return NotFound($"Stadium with Id {stadiumId} not found.");

            return Ok(await nowStadiumDetail(stadium));
        }

        [HttpPost("stadiumDetail/dateFilter")]
        public async Task<IActionResult> StadiumDetailDate(StadiumDetailVM vm)  
        {
            Stadium? stadium = await _context.Stadiums
                     .AsNoTracking()
                     .Include(s => s.StadiumImages)
                     .Include(s => s.Areas)
                     .ThenInclude(a => a.Reservations)
                     .FirstOrDefaultAsync(s => s.Id == vm.stadiumId);

            if (stadium is null) return NotFound($"Stadium with Id {vm.stadiumId} not found.");

            if (vm.date.Date == DateTime.Today)
                return Ok(await nowStadiumDetail(stadium));


            return Ok(dateStadiumDetail(stadium, vm.date));
        }


//--------------------------------------------
        private Paginate<T> PaginateItems<T>(List<T> list, int page, int take)
        {
            take = take > 0 ? take : 10;

            int totalPages = (int)Math.Ceiling(list.Count / (double)take);

            int currentPage = page > 0 ? page : 1;

            var paginatedResult = list
                .Skip((page - 1) * take)
                .Take(take)
                .ToList();

            return new Paginate<T>(paginatedResult, currentPage, totalPages);
        }
        private async Task<HomeDetailStadiumDto> nowStadiumDetail(Stadium stadium)
        {
            var now = DateTime.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;  //indiki saatda bir rezerv ola bilmez...

            var reservedHours = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r =>
                    r.Date.Date == today &&
                    r.Date.Hour >= nowHour &&
                    r.Date < today.AddDays(1) &&
                    stadium.Areas.Any(a => a.Reservations.Any(ar => ar.Date.Hour == r.Date.Hour && ar.Id != r.Id))
                )
                .GroupBy(r => r.Date.Hour)
                .Where(grp => grp.Count() == stadium.Areas.Count)
                .Select(grp => grp.Key)
                .ToList();

            var availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                .Except(reservedHours)
                .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                .ToList();

            var homeDetailStadiumDto = new HomeDetailStadiumDto
            {
                name = stadium.Name,
                phoneNumber = stadium.PhoneNumber,
                addres = stadium.Address,
                price = stadium.minPrice,
                description = stadium.Description,
                view = stadium.View,
                emptyDates = availableHourRanges,
                stadiumImages = stadium.StadiumImages.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList()
            };

            return homeDetailStadiumDto;
        }
        private HomeDetailStadiumDto dateStadiumDetail(Stadium stadium, DateTime date)
        {
            var reservedHours = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r =>
                    r.Date.Date == date.Date &&
                    r.Date < date.Date.AddDays(1) &&
                    stadium.Areas.Any(a => a.Reservations.Any(ar => ar.Date.Hour == r.Date.Hour && ar.Id != r.Id))
                )
                .GroupBy(r => r.Date.Hour)
                .Where(grp => grp.Count() == stadium.Areas.Count)
                .Select(grp => grp.Key)
                .ToList();

            //var reservedHours = stadium.Areas
            //    .SelectMany(a => a.Reservations)
            //    .Where(r =>
            //        r.Date.Date == date.Date &&
            //        r.Date < date.Date.AddDays(1) &&
            //        stadium.Areas.Any(a => a.Reservations.Any(ar => ar.Date.Hour == r.Date.Hour && ar.Id != r.Id))
            //    )
            //    .Select(r => r.Date.Hour)
            //    .Distinct()
            //    .ToList();

            var availableHourRanges = Enumerable.Range(0, 24)
                .Except(reservedHours)
                .Where(h => (h >= 0 && h < 4) || (h >= 9 && h <= 24)) // Saat araligi
                .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                .ToList();

            var homeDetailStadiumDto = new HomeDetailStadiumDto
            {
                name = stadium.Name,
                phoneNumber = stadium.PhoneNumber,
                addres = stadium.Address,
                price = stadium.minPrice,
                description = stadium.Description,
                view = stadium.View,
                emptyDates = availableHourRanges,
                stadiumImages = stadium.StadiumImages.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList()
            };

            return homeDetailStadiumDto;
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
