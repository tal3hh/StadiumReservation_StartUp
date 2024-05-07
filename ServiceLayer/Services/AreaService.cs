using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;

namespace ServiceLayer.Services
{
    public class AreaService : IAreaService
    {
        private readonly IRepository<Area> _repoArea;
        private readonly IRepository<Stadium> _repoStad;
        private readonly IMapper _mapper;
        public AreaService(IMapper mapper, IRepository<Area> repo, IRepository<Stadium> repoStad)
        {
            _mapper = mapper;
            _repoArea = repo;
            _repoStad = repoStad;
        }


        public async Task<List<DashAreaDto>> AllAsync()
        {
            var list = await _repoArea.GetListAsync(include: x=> x.Include(x=> x.Stadium));
            return _mapper.Map<List<DashAreaDto>>(list);
        }

        public async Task<UpdateAreaDto> FindById(int id)
        {
            var entity = await _repoArea.FindAsync(id);
            return _mapper.Map<UpdateAreaDto>(entity);
        }

        public async Task<List<DashAreaDto>> FindByStadiumId(int stadiumId)
        {
            var list = await _repoArea.GetListAsync(exp: x => x.StadiumId == stadiumId, 
                                                include: x => x.Include(x => x.Stadium));
            return _mapper.Map<List<DashAreaDto>>(list);
        }

        public async Task<IResponse> CreateAsync(CreateAreaDto dto)
        {
            if(!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            if (await _repoArea.ExistsAsync(x => x.StadiumId == dto.StadiumId && x.Name.ToLower() == dto.Name.ToLower()))
                return new Response(RespType.BadReqest, $"Stadionda artıq '{dto.Name}' bu adlı area var.(ferqli bir ad seçin)");

            Area Area = _mapper.Map<Area>(dto);
            Area.CreateDate = DateTimeAz.Now;
            Area.IsActive = true;

            await _repoArea.CreateAsync(Area);
            await _repoArea.SaveChangesAsync();

            return new Response(RespType.Success, "Area uğurla əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateAreaDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.Success, "Stadion tapılmadı.");

            Area? DBarea = await _repoArea.GetAsync(x => x.Id == dto.Id);
            if (DBarea != null)
            {
                Area area = _mapper.Map<Area>(dto);

                _repoArea.Update(area,DBarea);
                await _repoArea.SaveChangesAsync();

                return new Response(RespType.Success,"Uğurla dəyişildi.");
            }
            return new Response(RespType.NotFound, "Area tapılmadı."); ;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            Area? Area = await _repoArea.GetAsync(x => x.Id == id);
            if (Area != null)
            {
                _repoArea.Remove(Area);
                await _repoArea.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
