using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services
{
    public class StadiumDiscountService : IStadiumDiscountService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumDiscountService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashStadiumDiscountDto>> AllAsync()
        {
            var list = await _context.StadiumDiscounts.Include(x => x.Stadium).ToListAsync();

            return _mapper.Map<List<DashStadiumDiscountDto>>(list);
        }

        public async Task<List<DashStadiumDiscountDto>> FindByIdStadiums(int stadiumId)
        {
            var entity = await _context.StadiumDiscounts.Include(x => x.Stadium).Where(x => x.StadiumId == stadiumId).ToListAsync();

            return _mapper.Map<List<DashStadiumDiscountDto>>(entity);
        }

        public async Task<UpdateStadiumDiscountDto> FindById(int id)
        {
            var entity = await _context.StadiumDiscounts.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UpdateStadiumDiscountDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumDiscountDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);
            StadiumDiscount.CreateDate = DateTimeAz.Now;
            StadiumDiscount.IsActive = true;

            await _context.StadiumDiscounts.AddAsync(StadiumDiscount);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Stadion discount əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumDiscountDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDiscount? DBStadiumDiscount = await _context.StadiumDiscounts.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumDiscount != null)
            {
                StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);

                _context.Entry(DBStadiumDiscount).CurrentValues.SetValues(StadiumDiscount);

                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Stadion discount dəyişildi.");
            }
            return new Response(RespType.NotFound, "Stadion discount tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumDiscount? StadiumDiscount = await _context.StadiumDiscounts.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumDiscount is null)
            {
                _context.StadiumDiscounts.Remove(StadiumDiscount);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Stadion discount silindi.");
            }
            return new Response(RespType.NotFound, "Stadion discount tapılmadı.");
        }
    }
}
