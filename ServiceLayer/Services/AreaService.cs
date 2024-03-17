using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services
{
    public class AreaService : IAreaService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public AreaService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashAreaDto>> AllAsync()
        {
            var list = await _context.Areas.Include(x => x.Stadium).ToListAsync();

            return _mapper.Map<List<DashAreaDto>>(list);
        }

        public async Task<UpdateAreaDto> FindById(int id)
        {
            var entity = await _context.Areas.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UpdateAreaDto>(entity);
        }

        public async Task<List<DashAreaDto>> FindByStadiumId(int stadiumId)
        {
            var list = await _context.Areas.Include(x => x.Stadium)
                                             .Where(x => x.StadiumId == stadiumId).ToListAsync();

            return _mapper.Map<List<DashAreaDto>>(list);
        }

        public async Task<IResponse> CreateAsync(CreateAreaDto dto)
        {
            if(!await _context.Stadiums.AnyAsync(x=> x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            if (await _context.Areas.AnyAsync(x => x.StadiumId == dto.StadiumId && x.Name.ToLower() == dto.Name.ToLower()))
                return new Response(RespType.BadReqest, $"Stadionda artıq '{dto.Name}' bu adlı area var.(ferqli bir ad seçin)");

            Area Area = _mapper.Map<Area>(dto);
            Area.CreateDate = DateTimeAz.Now;
            Area.IsActive = true;

            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Area uğurla əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateAreaDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x=> x.Id == dto.StadiumId))
                return new Response(RespType.Success, "Stadion tapılmadı.");

            Area? DBarea = await _context.Areas.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBarea != null)
            {
                Area area = _mapper.Map<Area>(dto);

                _context.Entry(DBarea).CurrentValues.SetValues(area);

                await _context.SaveChangesAsync();

                return new Response(RespType.Success,"Uğurla dəyişildi.");
            }
            return new Response(RespType.NotFound, "Area tapılmadı."); ;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            Area? Area = await _context.Areas.SingleOrDefaultAsync(x => x.Id == id);

            if (Area != null)
            {
                _context.Areas.Remove(Area);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
