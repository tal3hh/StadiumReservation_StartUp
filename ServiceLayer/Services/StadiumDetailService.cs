using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services
{
    public class StadiumDetailService : IStadiumDetailService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumDetailService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashStadiumDetailDto>> AllAsync()
        {
            var list = await _context.StadiumDetails.Include(x => x.Stadium).ToListAsync();

            return _mapper.Map<List<DashStadiumDetailDto>>(list);
        }

        public async Task<List<DashStadiumDetailDto>> FindByIdStadiumDetails(int stdiumId)
        {
            var entity = await _context.StadiumImages.Include(x => x.Stadium)
                                                     .Where(x => x.StadiumId == stdiumId).ToListAsync();

            return _mapper.Map<List<DashStadiumDetailDto>>(entity);
        }

        public async Task<UpdateStadiumDetailDto> FindById(int id)
        {
            var entity = await _context.StadiumDetails.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UpdateStadiumDetailDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumDetailDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);
            StadiumDetail.CreateDate = DateTimeAz.Now;
            StadiumDetail.IsActive = true;

            await _context.StadiumDetails.AddAsync(StadiumDetail);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Stadion detalı əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumDetailDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDetail? DBStadiumDetail = await _context.StadiumDetails.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumDetail != null)
            {
                StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);

                _context.Entry(DBStadiumDetail).CurrentValues.SetValues(StadiumDetail);

                await _context.SaveChangesAsync();
                return new Response(RespType.Success, "Stadion detalı dəyişildi.");
            }
            return new Response(RespType.BadReqest, "Stadion detalı tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumDetail? StadiumDetail = await _context.StadiumDetails.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumDetail is null)
            {
                _context.StadiumDetails.Remove(StadiumDetail);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Stadion detalı dəyişildi.");
            }
            return new Response(RespType.BadReqest, "Stadion detalı tapılmadı.");
        }
    }
}
