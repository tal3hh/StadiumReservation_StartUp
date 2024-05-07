using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;

namespace ServiceLayer.Services
{
    public class StadiumDetailService : IStadiumDetailService
    {
        private readonly AppDbContext _context;
        private readonly IRepository<StadiumDetail> _repoDet;
        private readonly IRepository<Stadium> _repoStad;
        private readonly IMapper _mapper;
        public StadiumDetailService(AppDbContext context, IMapper mapper, IRepository<StadiumDetail> repoDet, IRepository<Stadium> repoStad)
        {
            _context = context;
            _mapper = mapper;
            _repoDet = repoDet;
            _repoStad = repoStad;
        }


        public async Task<List<DashStadiumDetailDto>> AllAsync()
        {
            var list = await _repoDet.GetListAsync(include: x=> x.Include(x=> x.Stadium));
            return _mapper.Map<List<DashStadiumDetailDto>>(list);
        }

        public async Task<List<DashStadiumDetailDto>> FindByIdStadiumDetails(int stdiumId)
        {
            var entity = await _repoDet.GetListAsync(include: x=> x.Include(x=> x.Stadium),
                                                     exp: x => x.StadiumId == stdiumId);
            return _mapper.Map<List<DashStadiumDetailDto>>(entity);
        }

        public async Task<UpdateStadiumDetailDto> FindById(int id)
        {
            var entity = await _repoDet.GetAsync(exp: x=> x.Id == id,
                                                 include: x=> x.Include(x=> x.Stadium));
            return _mapper.Map<UpdateStadiumDetailDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumDetailDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);
            StadiumDetail.CreateDate = DateTimeAz.Now;
            StadiumDetail.IsActive = true;

            await _repoDet.CreateAsync(StadiumDetail);
            await _repoDet.SaveChangesAsync();
            return new Response(RespType.Success, "Stadion detalı əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumDetailDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDetail? DBStadiumDetail = await _context.StadiumDetails.SingleOrDefaultAsync(x => x.Id == dto.Id);
            if (DBStadiumDetail != null)
            {
                StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);
                _repoDet.Update(StadiumDetail,DBStadiumDetail);
                await _repoDet.SaveChangesAsync();
                return new Response(RespType.Success, "Stadion detalı dəyişildi.");
            }
            return new Response(RespType.BadReqest, "Stadion detalı tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumDetail? StadiumDetail = await _repoDet.GetAsync(x => x.Id == id);
            if (StadiumDetail != null)
            {
                _repoDet.Remove(StadiumDetail);
                await _repoDet.SaveChangesAsync();
                return new Response(RespType.Success, "Stadion detalı dəyişildi.");
            }
            return new Response(RespType.BadReqest, "Stadion detalı tapılmadı.");
        }
    }
}
