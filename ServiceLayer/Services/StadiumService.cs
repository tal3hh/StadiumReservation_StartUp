using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using ServiceLayer.ViewModels;

namespace ServiceLayer.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public StadiumService(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Home
        public async Task<List<HomeListStadiumDto>> HomeStadiumOrderByListAsync()
        {
            List<Stadium>? stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .OrderBy(x => x.minPrice)
                .ToListAsync();

            var now = DateTimeAz.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = stadiums.Select(stadium =>
            {
                List<int> reservedHours = stadium.Areas
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

                List<string>? availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                    .Except(reservedHours)
                    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                    .Take(3)
                    .ToList();

                return new HomeListStadiumDto
                {
                    id = stadium.Id,
                    name = stadium.Name,
                    path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path ?? null,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    minPrice = stadium.minPrice,
                    maxPrice = stadium.maxPrice,
                    discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return allStadiumsEmptyHours;
        }

        public async Task<List<HomeListStadiumDto>> HomeStadiumCompanyListAsync()
        {
            List<Stadium>? stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .Where(s => s.StadiumDiscounts.Any())  //Endirimi olan stadionlar
                .ToListAsync();

            var now = DateTimeAz.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = stadiums.Select(stadium =>
            {
                List<int> reservedHours = stadium.Areas
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

                List<string>? availableHourRanges = Enumerable.Range(nowHour, 24 - nowHour)
                    .Except(reservedHours)
                    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                    .Take(3)
                    .ToList();

                return new HomeListStadiumDto
                {
                    id = stadium.Id,
                    name = stadium.Name,
                    path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path ?? null,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    minPrice = stadium.minPrice,
                    maxPrice = stadium.maxPrice,
                    discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return allStadiumsEmptyHours;
        }
        #endregion

        #region Stadiums
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

        public async Task<Paginate<HomeListStadiumDto>> StadiumListPagineAsync(StadiumPagineVM vm)
        {
            var now = DateTimeAz.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
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
                        id = stadium.Id,
                        name = stadium.Name,
                        path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return paginateResult;
        }

        public async Task<Paginate<HomeListStadiumDto>> StadiumSearchListPagineAsync(SearchStadiumVM vm)
        {
            var now = DateTimeAz.Now;
            var today = now.Date;
            var nowHour = now.Hour + 1;

            var allStadiumsEmptyHours = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
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
                        id = stadium.Id,
                        name = stadium.Name,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return paginateResult;
        }

        public async Task<Paginate<HomeListStadiumDto>> StadiumFilterListPagineAsync(FilterStadiumVM vm)
        {
            if (vm.minPrice > vm.maxPrice)
                (vm.minPrice, vm.maxPrice) = (vm.maxPrice, vm.minPrice);

            var query = _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice)
                .AsQueryable();

            if (!string.IsNullOrEmpty(vm.City))
                query = query.Where(x => x.City.Contains(vm.City));

            var allStadiumsEmptyHours = await query.ToListAsync();

            var now = DateTimeAz.Now;
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
                        id = stadium.Id,
                        name = stadium.Name,
                        path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            var paginateResult = PaginateItems(paginatedStadiums, vm.page, vm.take);

            return paginateResult;
        }

        public async Task<Paginate<HomeListStadiumDto>> StadiumTimeFilterListPagineAsync(TimeFilterStadiumVM vm)
        {
            var query = _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .AsQueryable();

            //Price 
            if (vm.minPrice > 0 || vm.maxPrice > 0)
            {
                if (vm.minPrice > vm.maxPrice)
                    (vm.minPrice, vm.maxPrice) = (vm.maxPrice, vm.minPrice);

                query = query.Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice);
            }

            List<Stadium> stadiums = await query.ToListAsync();

            //Date
            DateTime date = vm.Date.Date >= DateTimeAz.Now.Date ? vm.Date.Date : DateTimeAz.Now.Date;

            List<HomeListStadiumDto> stadiumList = stadiums
                .Select(stadium =>
                {
                    //var reservedHours = stadium.Areas
                    //    .SelectMany(a => a.Reservations)
                    //    .Where(r =>
                    //        r.Date.Date == date &&
                    //        stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                    //    )
                    //    .Select(r => r.Date.Hour)
                    //    .Distinct()
                    //    .ToList();

                    var reservedHours = stadium.Areas
                          .SelectMany(a => a.Reservations)
                          .Where(r =>
                              r.Date.Date == date &&
                              stadium.Areas.Any(a => a.Reservations.Any(x => x.Date.Hour == r.Date.Hour && x.Id != r.Id))
                          )
                          .GroupBy(r => r.Date.Hour)
                          .Where(grp => grp.Count() == stadium.Areas.Count)
                          .Select(grp => grp.Key)
                          .ToList();


                    //TIME
                    if (date == DateTimeAz.Now.Date)
                    {
                        vm.startTime = vm.startTime <= DateTimeAz.Now.Hour ? DateTimeAz.Now.Hour + 1 : vm.startTime;
                        vm.endTime = vm.endTime > 24 || vm.endTime <= vm.startTime ? 24 : vm.endTime;
                    }
                    else
                    {
                        vm.startTime = vm.startTime < 0 || vm.startTime >= 24 ? 0 : vm.startTime;
                        vm.endTime = vm.endTime > 24 || vm.endTime <= vm.startTime ? 24 : vm.endTime;
                    }

                    List<string> availableHourRanges = Enumerable.Range(vm.startTime, vm.endTime - vm.startTime)
                        .Except(reservedHours)
                        .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                        .Take(3)
                        .ToList();

                    return new HomeListStadiumDto
                    {
                        id = stadium.Id,
                        name = stadium.Name,
                        path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path,
                        phoneNumber = stadium.PhoneNumber,
                        addres = stadium.Address,
                        minPrice = stadium.minPrice,
                        maxPrice = stadium.maxPrice,
                        discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                        emptyDates = availableHourRanges
                    };
                })
                .ToList();

            // Paginate
            Paginate<HomeListStadiumDto> paginateResult = PaginateItems(stadiumList, vm.page, vm.take);

            return paginateResult;
        }

        #endregion

        #region Details
        public async Task<HomeDetailStadiumDto> StadiumDetailAsync(int stadiumId)
        {
            Stadium? stadium = await _context.Stadiums
                     .AsNoTracking()
                     .Include(s => s.StadiumImages)
                     .Include(s => s.StadiumDetails)
                     .Include(s => s.StadiumDiscounts)
                     .Include(s => s.Areas)
                     .ThenInclude(a => a.Reservations)
                     .FirstOrDefaultAsync(s => s.Id == stadiumId);

            if (stadium == null) return new HomeDetailStadiumDto();

            var now = DateTimeAz.Now;
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
                emptyDates = availableHourRanges,
                descriptions = stadium.StadiumDetails?.Select(x => x.Description).ToList() ?? new List<string>(),
                discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                stadiumImages = stadium.StadiumImages?.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList() ?? new List<StadiumImageDto>()
            };

            return homeDetailStadiumDto;
        }

        public async Task<HomeDetailStadiumDto> DateStadiumDetailAsync(StadiumDetailVM vm)
        {
            Stadium? stadium = await _context.Stadiums
                     .AsNoTracking()
                     .Include(s => s.StadiumImages)
                     .Include(s => s.TimeStadium)
                     .Include(s => s.StadiumDetails)
                     .Include(s => s.StadiumDiscounts)
                     .Include(s => s.Areas)
                     .ThenInclude(a => a.Reservations)
                     .FirstOrDefaultAsync(s => s.Id == vm.stadiumId);

            if (stadium == null) return new HomeDetailStadiumDto();

            if (vm.date.Date == DateTimeAz.Today)
                return await StadiumDetailAsync(vm.stadiumId);

            var reservedHours = stadium.Areas
            .SelectMany(a => a.Reservations)
                .Where(r =>
                    r.Date.Date == vm.date.Date &&
                    r.Date < vm.date.Date.AddDays(1) &&
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

            //SAAT araligi
            var availableHourRanges = new List<string>();

            var time = await _context.TimeStadiums.FirstOrDefaultAsync(x => x.StadiumId == stadium.Id);

            if (time == null)
            {
                availableHourRanges = Enumerable.Range(0, 24)
                .Except(reservedHours)
                .Where(h => (h >= 0 && h < 4) || (h >= 9 && h <= 24))
                .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                .ToList();
            }
            else
            {
                availableHourRanges = Enumerable.Range(0, 24)
                .Except(reservedHours)
                .Where(h => (h >= 0 && h < time.nightTime) || (h >= time.openTime && h <= time.closeTime)) 
                .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                .ToList();
            }


            var homeDetailStadiumDto = new HomeDetailStadiumDto
            {
                name = stadium.Name,
                phoneNumber = stadium.PhoneNumber,
                addres = stadium.Address,
                price = stadium.minPrice,
                emptyDates = availableHourRanges,
                descriptions = stadium.StadiumDetails?.Select(x => x.Description).ToList() ?? new List<string>(),
                discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string>(),
                stadiumImages = stadium.StadiumImages?.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList() ?? new List<StadiumImageDto>()
            };
            return homeDetailStadiumDto;
        }
        #endregion

        #region Dash
        public async Task<List<DashStadiumDto>> AllAsync()
        {
            List<Stadium>? list = await _context.Stadiums.Include(x => x.AppUser).Include(x=> x.StadiumDetails)
                .ToListAsync();

            return _mapper.Map<List<DashStadiumDto>>(list);
        }

        public async Task<DashStadiumDto> FindById(int id)
        {
            Stadium? entity = await _context.Stadiums.Include(x => x.AppUser).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashStadiumDto>(entity);
        }

        public async Task<int> CreateAsync(CreateStadiumDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user is null) return 1;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Owner")) return 2;

            if (user.Stadium != null) return 3;

            Stadium stadium = _mapper.Map<Stadium>(dto);
            stadium.CreateDate = DateTimeAz.Now;
            stadium.IsActive = true;

            await _context.Stadiums.AddAsync(stadium);
            await _context.SaveChangesAsync();

            return 0;
        }

        public async Task UpdateAsync(UpdateStadiumDto dto)
        {
            Stadium? DBstadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBstadium != null)
            {
                Stadium stadium = _mapper.Map<Stadium>(dto);

                _context.Entry(DBstadium).CurrentValues.SetValues(stadium);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(int id)
        {
            Stadium? stadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == id);

            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
