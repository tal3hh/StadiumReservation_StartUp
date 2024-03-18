using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;

namespace ServiceLayer.Services
{
    public class StadiumImageService : IStadiumImageService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumImageService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashStadiumImageDto>> AllAsync()
        {
            var list = await _context.StadiumImages.Include(x => x.Stadium).ToListAsync();

            return _mapper.Map<List<DashStadiumImageDto>>(list);
        }

        public async Task<List<DashStadiumImageDto>> FindByIdStadiumImages(int stdiumId)
        {
            var entity = await _context.StadiumImages.Include(x => x.Stadium)
                                                     .Where(x => x.StadiumId == stdiumId).ToListAsync();

            return _mapper.Map<List<DashStadiumImageDto>>(entity);
        }

        public async Task<UpdateStadiumImageDto> FindById(int id)
        {
            var entity = await _context.StadiumImages.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<UpdateStadiumImageDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumImageDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            else if (dto.Main == true && await _context.StadiumImages.Where(x => x.StadiumId == dto.StadiumId && x.Main).CountAsync() > 0)
                return new Response(RespType.BadReqest, "Stadionun bir əsas(main) şəkli ola bilər.");


            StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);
            StadiumImage.CreateDate = DateTimeAz.Now;
            StadiumImage.IsActive = true;

            await _context.StadiumImages.AddAsync(StadiumImage);
            await _context.SaveChangesAsync();

            return new Response(RespType.Success, "Şəkil əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumImageDto dto)
        {
            if (!await _context.Stadiums.AnyAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            else if (dto.Main == true && 
                await _context.StadiumImages.Where(x => x.StadiumId == dto.StadiumId && x.Main && x.Id != dto.Id).CountAsync() > 0)
                return new Response(RespType.BadReqest, "Stadionun bir əsas(main) şəkli ola bilər.");

            else if (!await _context.StadiumImages.AnyAsync(x => x.Id == dto.Id))
                return new Response(RespType.NotFound, "Stadion şəkli tapılmadı.");

            StadiumImage? DBStadiumImage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumImage != null)
            {
                StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);

                _context.Entry(DBStadiumImage).CurrentValues.SetValues(StadiumImage);

                await _context.SaveChangesAsync();
                return new Response(RespType.Success, "Stadionun şəkli uğurla dəyişildi.");
            }
            return new Response(RespType.NotFound, "Stadionun şəkli tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumImage? StadiumImage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumImage != null)
            {
                _context.StadiumImages.Remove(StadiumImage);
                await _context.SaveChangesAsync();

                return new Response(RespType.Success, "Stadionun şəkli silindi.");
            }
            return new Response(RespType.NotFound, "Stadionun şəkli tapılmadı.");
        }
    }
}
