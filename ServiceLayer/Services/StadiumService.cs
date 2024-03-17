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
using System.Diagnostics;

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

                List<string> availableHourRanges = new List<string>();

                
                //Time Filter

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
                        availableHourRanges = new List<string>();  //gorulmesin (frountcu bu stadionlari "Bu gun bos saat yoxdur." yazsin)
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
                        availableHourRanges = new List<string>();  //gorulmesin (frountcu bu stadionlari "Bu gun bos saat yoxdur." yazsin)
                    }
                }
                else
                {
                    availableHourRanges = new List<string>();  //sona alinsin (frountcu bu stadionlari "Bu gun bos saat yoxdur." yazsin)
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
            List<Stadium>? stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .OrderBy(x => x.minPrice)
                .ToListAsync();

            return await TodayEmptyHourStadiums(stadiums);
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

            return await TodayEmptyHourStadiums(stadiums);
        }

        #endregion


        #region Stadiums
        public async Task<Paginate<HomeListStadiumDto>> StadiumListPagineAsync(StadiumPagineVM vm)
        {
            List<Stadium> stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .ToListAsync();

            List<HomeListStadiumDto> emptyHourStadiums = await TodayEmptyHourStadiums(stadiums);

            // Paginate
            var paginateResult = PaginateItems(emptyHourStadiums, vm.page, vm.take);

            return paginateResult;
        }

        public async Task<Paginate<HomeListStadiumDto>> StadiumSearchListPagineAsync(SearchStadiumVM vm)
        {
            List<Stadium> stadiums = await _context.Stadiums
                .AsNoTracking()
                .Include(s => s.StadiumImages)
                .Include(s => s.StadiumDiscounts)
                .Include(s => s.Areas)
                .ThenInclude(a => a.Reservations)
                .Where(x => x.Name.Contains(vm.search.Trim()))
                .ToListAsync();

            List<HomeListStadiumDto> emptyHourStadiums = await TodayEmptyHourStadiums(stadiums);

            // Paginate
            var paginateResult = PaginateItems(emptyHourStadiums, vm.page, vm.take);

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

            if (!string.IsNullOrEmpty(vm.Address))
                query = query.Where(x => x.Address.Contains(vm.Address));

            //Price 
            if (vm.minPrice > 0 || vm.maxPrice > 0)
            {
                if (vm.minPrice > vm.maxPrice)
                    (vm.minPrice, vm.maxPrice) = (vm.maxPrice, vm.minPrice);

                query = query.Where(x => x.minPrice >= vm.minPrice && x.minPrice <= vm.maxPrice);
            }

            List<Stadium> stadiums = await query.ToListAsync();

            var now = DateTimeAz.Now;
            //Date
            DateTime date = vm.Date.Date >= now.Date ? vm.Date.Date : now.Date;

            List<HomeListStadiumDto> stadiumList = stadiums
                .Select(stadium =>
                {
                    var reservedHours = stadium.Areas
                          .SelectMany(a => a.Reservations)
                          .Where(r =>
                              r.Date.Date == date &&
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


                    //TIME
                    if (date == now.Date)
                    {
                        vm.startTime = vm.startTime <= now.Hour ? now.Hour + 1 : vm.startTime;
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

        public async Task<HomeDetailStadiumDto> DateStadiumDetailAsync(StadiumDetailVM vm)
        {
            //Stadium? stadium = await _context.Stadiums
            //         .AsNoTracking()
            //         .Include(s => s.StadiumImages)
            //         .Include(s => s.StadiumDetails)
            //         .Include(s => s.StadiumDiscounts)
            //         .Include(s => s.Areas)
            //         .ThenInclude(a => a.Reservations)
            //         .FirstOrDefaultAsync(s => s.Id == vm.stadiumId);

            //if (stadium == null) return new HomeDetailStadiumDto();

            //if (vm.date.Date == DateTimeAz.Today)
            //    return await StadiumDetailAsync(vm.stadiumId);

            //var reservedHours = stadium.Areas
            //.SelectMany(a => a.Reservations)
            //    .Where(r =>
            //        r.Date.Date == vm.date.Date &&
            //        r.Date < vm.date.Date.AddDays(1) &&
            //        stadium.Areas.Any(a => a.Reservations.Any(ar => ar.Date.Hour == r.Date.Hour && ar.Id != r.Id))
            //    )
            //    .GroupBy(r => r.Date.Hour)
            //    .Where(grp => grp.Count() == stadium.Areas.Count)
            //    .Select(grp => grp.Key)
            //    .ToList();

            ////SAAT araligi
            //var availableHourRanges = new List<string>();

            //var time = await _context.TimeStadiums.FirstOrDefaultAsync(x => x.StadiumId == stadium.Id);

            //if (time == null)
            //{
            //    availableHourRanges = Enumerable.Range(0, 24)
            //    .Except(reservedHours)
            //    .Where(h => (h >= 0 && h < 4) || (h >= 9 && h <= 24))
            //    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
            //    .ToList();
            //}
            //else
            //{
            //    availableHourRanges = Enumerable.Range(0, 24)
            //    .Except(reservedHours)
            //    .Where(h => (h >= 0 && h < time.nightTime) || (h >= time.openTime && h <= time.closeTime))
            //    .Select(h => $"{h:00}:00-{(h + 1):00}:00")
            //    .ToList();
            //}


            //var homeDetailStadiumDto = new HomeDetailStadiumDto
            //{
            //    name = stadium.Name,
            //    phoneNumber = stadium.PhoneNumber,
            //    addres = stadium.Address,
            //    openDay = stadium.openDay,
            //    closeDay = stadium.closeDay,
            //    openTime = stadium.openTime,
            //    closeTime = stadium.closeTime,
            //    price = stadium.minPrice,
            //    emptyDates = availableHourRanges,
            //    descriptions = stadium.StadiumDetails?.Select(x => x.Description).ToList() ?? new List<string?>(),
            //    discounts = stadium.StadiumDiscounts?.Select(d => d.Path).ToList() ?? new List<string?>(),
            //    stadiumImages = stadium.StadiumImages?.Select(i => new StadiumImageDto { path = i.Path, main = i.Main }).ToList() ?? new List<StadiumImageDto>()
            //};
            //return homeDetailStadiumDto;

            return new HomeDetailStadiumDto();
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
