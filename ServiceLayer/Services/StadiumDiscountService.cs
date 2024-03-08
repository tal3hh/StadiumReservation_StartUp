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

        public async Task<DashStadiumDiscountDto> FindById(int id)
        {
            var entity = await _context.StadiumDiscounts.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashStadiumDiscountDto>(entity);
        }

        public async Task CreateAsync(CreateStadiumDiscountDto dto)
        {
            StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);
            StadiumDiscount.CreateDate = DateTimeAz.Now;
            StadiumDiscount.IsActive = true;

            await _context.StadiumDiscounts.AddAsync(StadiumDiscount);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(UpdateStadiumDiscountDto dto)
        {
            StadiumDiscount? DBStadiumDiscount = await _context.StadiumDiscounts.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumDiscount != null)
            {
                StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);

                _context.Entry(DBStadiumDiscount).CurrentValues.SetValues(StadiumDiscount);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            StadiumDiscount? StadiumDiscount = await _context.StadiumDiscounts.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumDiscount is null)
            {
                _context.StadiumDiscounts.Remove(StadiumDiscount);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
