using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Dtos.Stadium.Home;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.Pagine;
using ServiceLayer.Utlities.TimeZone;
using ServiceLayer.ViewModels;

namespace ServiceLayer.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<Stadium> _repoStad;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        public StadiumService(AppDbContext context, IMapper mapper, UserManager<AppUser> userManager, IRepository<Stadium> repoStad)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _repoStad = repoStad;
        }

        private Paginate<T> PaginateItems<T>(List<T> list, int page = 1, int take = 10) where T : class
        {
            int totalPages = (int)Math.Ceiling(list.Count / (double)take);
            var paginatedResult = list
                .Skip((page - 1) * take)
                .Take(take)
                .ToList();
            return new Paginate<T>(paginatedResult, page, totalPages);
        }

        private Task<List<HomeListStadiumDto>> TodayEmptyHourStadiums(List<Stadium> stadiums)
        {
            DateTime today = DateTimeAz.Now.Date;
            int nowHour = DateTimeAz.Now.Hour + 1;

            var allStadiumsEmptyHours = stadiums.Select(stadium =>
            {
                List<int> reservedHours = stadium.Areas
                    .SelectMany(a => a.Reservations)
                    .Where(r =>
                        r.Date.Date == today &&
                        r.Date.Hour >= nowHour &&

                        stadium.Areas.All(a =>
                            a.Reservations != null && a.Reservations.Any(x =>
                                                                         x.Date.Hour == r.Date.Hour
                            )
                        )
                    )
                    .Select(r => r.Date.Hour)
                    .Distinct()
                    .ToList();


                // Time Filter
                List<string> availableHourRanges = new List<string>();
                
                if (stadium.closeHour > nowHour && nowHour > stadium.openHour)
                {
                    availableHourRanges = Enumerable.Range(nowHour, stadium.closeHour - nowHour)
                    .Except(reservedHours)
                    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                    .Take(3)
                    .ToList();
                }
                else if (stadium.closeHour < nowHour)  // mes: saat 10dan sonra isleyen stadionlar.
                {
                    availableHourRanges = new List<string>();  //gorulmesin (frountcu bu stadionlari sona alsin)
                }
                else if (nowHour == 24)
                {
                    reservedHours = stadium.Areas
                            .SelectMany(a => a.Reservations)
                            .Where(r =>
                                r.Date.Date == today.AddDays(1) &&
                                r.Date.Hour >= 0 &&

                                stadium.Areas.All(a =>
                                    a.Reservations != null && a.Reservations.Any(x =>
                                                                                 x.Date.Hour == r.Date.Hour
                                    )
                                )
                            )
                            .Select(r => r.Date.Hour)
                            .Distinct()
                            .ToList();

                    if (stadium.nightHour == 0)
                    {
                        availableHourRanges = new List<string>();  
                    }
                    else
                    {
                        

                        availableHourRanges = Enumerable.Range(0, stadium.nightHour - 0)
                        .Except(reservedHours)
                        .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                        .Take(3)
                        .ToList();
                    }
                }
                else if (nowHour < stadium.openHour)
                {
                    reservedHours = stadium.Areas
                            .SelectMany(a => a.Reservations)
                            .Where(r =>
                                r.Date.Date == today.AddDays(1) &&
                                r.Date.Hour >= nowHour &&

                                stadium.Areas.All(a =>
                                    a.Reservations != null && a.Reservations.Any(x =>
                                                                                 x.Date.Hour == r.Date.Hour
                                    )
                                )
                            )
                            .Select(r => r.Date.Hour)
                            .Distinct()
                            .ToList();

                    if (stadium.nightHour > nowHour)
                    {
                        availableHourRanges = Enumerable.Range(nowHour, stadium.nightHour - nowHour)
                            .Except(reservedHours)
                            .Select(h => $"{h:00}:00-{(h + 1):00}:00")
                            .Take(3)
                            .ToList();
                    }
                    else
                    {
                        availableHourRanges = new List<string>();  
                    }
                }
                else
                {
                    availableHourRanges = new List<string>();  
                }


                return new HomeListStadiumDto
                {
                    id = stadium.Id,
                    name = stadium.Name,
                    path = stadium.StadiumImages?.FirstOrDefault(x => x.Main)?.Path ?? null,
                    phoneNumber = stadium.PhoneNumber,
                    addres = stadium.Address,
                    minPrice = stadium.minPrice,
                    maxPrice = stadium.maxPrice,
                    discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string?>(),
                    emptyDates = availableHourRanges
                };
            }).ToList();

            return Task.FromResult(allStadiumsEmptyHours);
        }

        #region Home
        public async Task<List<HomeListStadiumDto>> HomeStadiumOrderByListAsync()
        {
            List<Stadium>? stadiums = await _repoStad.GetListAsync(orderBy: x => x.OrderBy(x => x.minPrice),
                                                                   enableTracking: false,
                                                                   include: x => x.Include(s => s.StadiumImages)
                                                                                   .Include(s => s.StadiumDiscounts)
                                                                                   .Include(s => s.Areas)
                                                                                   .ThenInclude(a => a.Reservations));
            return await TodayEmptyHourStadiums(stadiums);
        }

        public async Task<List<HomeListStadiumDto>> HomeStadiumCompanyListAsync()
        {
            List<Stadium>? stadiums = await _repoStad.GetListAsync(exp: x => x.StadiumDiscounts.Any(),
                                                                    enableTracking: false,
                                                                    include: x => x.Include(s => s.StadiumImages)
                                                                                   .Include(s => s.StadiumDiscounts)
                                                                                   .Include(s => s.Areas)
                                                                                   .ThenInclude(a => a.Reservations));
            return await TodayEmptyHourStadiums(stadiums);
        }

        #endregion


        #region Stadiums
        public async Task<Paginate<HomeListStadiumDto>> StadiumListPagineAsync(StadiumPagineVM vm)
        {
            List<Stadium> stadiums = await _repoStad.GetListAsync(include: x => x.Include(s => s.StadiumImages)
                                                                                 .Include(s => s.StadiumDiscounts)
                                                                                 .Include(s => s.Areas)
                                                                                 .ThenInclude(a => a.Reservations),
                                                                    enableTracking: false);

            List<HomeListStadiumDto> emptyHourStadiums = await TodayEmptyHourStadiums(stadiums);
            return PaginateItems(emptyHourStadiums, vm.page, vm.take);
        }

        public async Task<Paginate<HomeListStadiumDto>> StadiumSearchListPagineAsync(SearchStadiumVM vm)
        {
            List<Stadium> stadiums = await _repoStad.GetListAsync(include: x => x.Include(s => s.StadiumImages)
                                                                                 .Include(s => s.StadiumDiscounts)
                                                                                 .Include(s => s.Areas)
                                                                                 .ThenInclude(a => a.Reservations),
                                                                    exp: x => x.Name.Contains(vm.search.Trim()),
                                                                    enableTracking: false);

            List<HomeListStadiumDto> emptyHourStadiums = await TodayEmptyHourStadiums(stadiums);
            return PaginateItems(emptyHourStadiums, vm.page, vm.take);
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

            List<HomeListStadiumDto> emptyHourStadiums = await TodayEmptyHourStadiums(await query.ToListAsync());

            // Paginate
            var paginateResult = PaginateItems(emptyHourStadiums, vm.page, vm.take);

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
            

            // ADDRESS
            if (!string.IsNullOrEmpty(vm.Address))
                query = query.Where(x => x.Address.Contains(vm.Address));


            // PRICE 
            if (vm.minPrice > 0 || vm.maxPrice > 0)
            {
                if (vm.minPrice >= vm.maxPrice)
                    query = query.Where(x => x.minPrice >= vm.minPrice);
                else
                    query = query.Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice);
            }

            // DATE  
            var now = DateTimeAz.Now;
            DateTime date = vm.Date.Date >= now.Date ? vm.Date.Date : now.Date;  //(kecmis tarix olmasin)


            List<Stadium> stadiums = await query.ToListAsync();

            List<HomeListStadiumDto> stadiumList = stadiums
                .Select(stadium =>
                {
                    //var reservedHours = stadium.Areas
                    //      .SelectMany(a => a.Reservations)
                    //      .Where(r =>
                    //          r.Date.Date == date &&
                    //          stadium.Areas.All(a =>
                    //        a.Reservations != null &&
                    //        a.Reservations.Any(x =>
                    //            x.Date.Hour == r.Date.Hour
                    //        )
                    //    )
                    //      )
                    //      .GroupBy(r => r.Date.Hour)
                    //      .Where(grp => grp.Count() == stadium.Areas.Count)
                    //      .Select(grp => grp.Key)
                    //      .ToList();

                    List<int> reservedHours = stadium.Areas
                    .SelectMany(a => a.Reservations)
                    .Where(r =>
                        r.Date.Date == date &&

                        stadium.Areas.All(a =>
                            a.Reservations != null && a.Reservations.Any(x =>
                                                                         x.Date.Hour == r.Date.Hour
                            )
                        )
                    )
                    .Select(r => r.Date.Hour)
                    .Distinct()
                    .ToList();


                    // Start-End HOUR
                    if (date == now.Date)
                    {
                        vm.startTime = vm.startTime <= now.Hour || vm.startTime >= 24 || vm.startTime > vm.endTime ? now.Hour + 1 : vm.startTime;
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
                        discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string?>(),
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

            var today = DateTimeAz.Now.Date;
            var nowHour = DateTimeAz.Now.Hour + 1;  //indiki saatda bir rezerv ola bilmez...

            var reservedHours = stadium.Areas
                .SelectMany(a => a.Reservations)
                .Where(r =>
                    r.Date.Date == today &&
                    r.Date.Hour >= nowHour &&
                    r.Date < today.AddDays(1) &&
                    stadium.Areas.All(a =>
                            a.Reservations != null &&
                            a.Reservations.Any(x =>
                                x.Date.Hour == r.Date.Hour
                            )
                        )
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
                OpenCloseDay = stadium.OpenCloseDay,
                OpenCloseHour = stadium.OpenCloseHour,
                minPrice = stadium.minPrice,
                maxPrice = stadium.maxPrice,
                emptyDates = availableHourRanges,
                descriptions = stadium.StadiumDetails?.Select(x => x.Description).ToList() ?? new List<string?>(),
                discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string?>(),
                stadiumImages = stadium.StadiumImages?.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList() ?? new List<StadiumImageDto>()
            };

            return homeDetailStadiumDto;
        }
        #endregion


        #region Dash
        public async Task<List<DashStadiumDto>> AllAsync()
        {
            List<Stadium>? list = await _context.Stadiums.Include(x => x.AppUser).Include(x => x.StadiumDetails)
                .ToListAsync();

            return _mapper.Map<List<DashStadiumDto>>(list);
        }

        public async Task<UpdateStadiumDto> FindById(int id)
        {
            Stadium? entity = await _context.Stadiums.Include(x => x.AppUser)
                                                .SingleOrDefaultAsync(x => x.Id == id);

            if (entity != null)
                return _mapper.Map<UpdateStadiumDto>(entity);

            return new UpdateStadiumDto();
        }

        public async Task<IResponse> CreateAsync(CreateStadiumDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user is null) 
                return new Response(RespType.NotFound, "Istifadeci tapilmadi.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Owner"))
                return new Response(RespType.BadReqest, "Sahibkar kimi qeydiyyatda deyilsiniz.");

            if (await _context.Stadiums.AnyAsync(x => x.AppUserId == user.Id))
                return new Response(RespType.BadReqest, "Bu sahibkarın adında artiq bir stadion var. " +
                                                        "(Yeni bir istifadəçi yaradın)");

            Stadium stadium = _mapper.Map<Stadium>(dto);
            stadium.AppUserId = user.Id;
            stadium.CreateDate = DateTimeAz.Now;
            stadium.IsActive = true;

            await _context.Stadiums.AddAsync(stadium);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Stadion uğurla əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user is null)
                return new Response(RespType.NotFound, "Istifadeci tapilmadi.");

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Owner"))
                return new Response(RespType.BadReqest, "Sahibkar kimi qeydiyyatda deyilsiniz.");

            if (await _context.Stadiums.AnyAsync(x => x.AppUserId == user.Id))
                return new Response(RespType.BadReqest, "Bu sahibkarın adında artiq bir stadion var. " +
                                                        "(Yeni bir istifadəçi yaradın)");


            Stadium? DBstadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBstadium != null)
            {
                Stadium stadium = _mapper.Map<Stadium>(dto);
                stadium.AppUserId = user.Id;

                _context.Entry(DBstadium).CurrentValues.SetValues(stadium);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Uğurla dəyişildi.");
            }

            return new Response(RespType.NotFound, "Stadion tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            Stadium? stadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == id);

            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Stadion uğurla silindi.");
            }

            return new Response(RespType.NotFound, "Stadion tapilmadi.");
        }
        #endregion
    }
}
