using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;

namespace ServiceLayer.Services
{
    public class StadiumImageService : IStadiumImageService
    {
        private readonly IRepository<StadiumImage> _repoImg;
        private readonly IRepository<Stadium> _repoStad;
        private readonly IMapper _mapper;
        public StadiumImageService(IMapper mapper, IRepository<StadiumImage> repoImg, IRepository<Stadium> repoStad)
        {
            _mapper = mapper;
            _repoImg = repoImg;
            _repoStad = repoStad;
        }


        public async Task<List<DashStadiumImageDto>> AllAsync()
        {
            var list = await _repoImg.GetListAsync(include: x=> x.Include(x=> x.Stadium));
            return _mapper.Map<List<DashStadiumImageDto>>(list);
        }

        public async Task<List<DashStadiumImageDto>> FindByIdStadiumImages(int stdiumId)
        {
            var entity = await _repoImg.GetListAsync(exp: x => x.StadiumId == stdiumId,
                                                     include: x => x.Include(x => x.Stadium));
            return _mapper.Map<List<DashStadiumImageDto>>(entity);
        }

        public async Task<UpdateStadiumImageDto> FindById(int id)
        {
            var entity = await _repoImg.GetAsync(exp: x => x.Id == id,
                                                 include: x=> x.Include(x=> x.Stadium));
            return _mapper.Map<UpdateStadiumImageDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumImageDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            else if (dto.Main == true && await _repoImg.CountAsync(x => x.StadiumId == dto.StadiumId && x.Main) > 0)
                return new Response(RespType.BadReqest, "Stadionun bir əsas(main) şəkli ola bilər.");

            StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);
            StadiumImage.CreateDate = DateTimeAz.Now;
            StadiumImage.IsActive = true;

            await _repoImg.CreateAsync(StadiumImage);
            await _repoImg.SaveChangesAsync();
            return new Response(RespType.Success, "Şəkil əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumImageDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            else if (dto.Main == true&&await _repoImg.CountAsync(x => x.StadiumId == dto.StadiumId && x.Main && x.Id != dto.Id) > 0)
                return new Response(RespType.BadReqest, "Stadionun bir əsas(main) şəkli ola bilər.");

            else if (!await _repoImg.ExistsAsync(x => x.Id == dto.Id))
                return new Response(RespType.NotFound, "Stadion şəkli tapılmadı.");

            StadiumImage? DBStadiumImage = await _repoImg.FindAsync(dto.Id);
            if (DBStadiumImage != null)
            {
                StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);
                _repoImg.Update(StadiumImage,DBStadiumImage);
                await _repoImg.SaveChangesAsync();
                return new Response(RespType.Success, "Stadionun şəkli uğurla dəyişildi.");
            }
            return new Response(RespType.NotFound, "Stadionun şəkli tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumImage? StadiumImage = await _repoImg.FindAsync(id);
            if (StadiumImage != null)
            {
                _repoImg.Remove(StadiumImage);
                await _repoImg.SaveChangesAsync();
                return new Response(RespType.Success, "Stadionun şəkli silindi.");
            }
            return new Response(RespType.NotFound, "Stadionun şəkli tapılmadı.");
        }
    }
}
