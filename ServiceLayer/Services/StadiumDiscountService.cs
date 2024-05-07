using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Repositories;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.StadiumDiscount;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities.TimeZone;

namespace ServiceLayer.Services
{
    public class StadiumDiscountService : IStadiumDiscountService
    {
        private readonly IRepository<StadiumDiscount> _repoDis;
        private readonly IRepository<Stadium> _repoStad;
        private readonly IMapper _mapper;
        public StadiumDiscountService(IMapper mapper, IRepository<StadiumDiscount> repoDis, IRepository<Stadium> repoStad)
        {
            _mapper = mapper;
            _repoDis = repoDis;
            _repoStad = repoStad;
        }


        public async Task<List<DashStadiumDiscountDto>> AllAsync()
        {
            var list = await _repoDis.GetListAsync(include: x => x.Include(x => x.Stadium));
            return _mapper.Map<List<DashStadiumDiscountDto>>(list);
        }

        public async Task<List<DashStadiumDiscountDto>> FindByIdStadiums(int stadiumId)
        {
            var list = await _repoDis.GetListAsync(exp: x => x.StadiumId == stadiumId,
                                                     include: x => x.Include(x => x.Stadium));
            return _mapper.Map<List<DashStadiumDiscountDto>>(list);
        }

        public async Task<UpdateStadiumDiscountDto> FindById(int id)
        {
            var entity = await _repoDis.GetAsync(exp: x => x.Id == id, include: x => x.Include(x => x.Stadium));
            return _mapper.Map<UpdateStadiumDiscountDto>(entity);
        }

        public async Task<IResponse> CreateAsync(CreateStadiumDiscountDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);
            StadiumDiscount.CreateDate = DateTimeAz.Now;
            StadiumDiscount.IsActive = true;

            await _repoDis.CreateAsync(StadiumDiscount);
            await _repoDis.SaveChangesAsync();
            return new Response(RespType.Success, "Stadion discount əlavə edildi.");
        }

        public async Task<IResponse> UpdateAsync(UpdateStadiumDiscountDto dto)
        {
            if (!await _repoStad.ExistsAsync(x => x.Id == dto.StadiumId))
                return new Response(RespType.NotFound, "Stadion tapılmadı.");

            StadiumDiscount? DBStadiumDiscount = await _repoDis.GetAsync(x => x.Id == dto.Id);
            if (DBStadiumDiscount != null)
            {
                StadiumDiscount StadiumDiscount = _mapper.Map<StadiumDiscount>(dto);
                _repoDis.Update(StadiumDiscount, DBStadiumDiscount);
                await _repoDis.SaveChangesAsync();
                return new Response(RespType.Success, "Stadion discount dəyişildi.");
            }
            return new Response(RespType.NotFound, "Stadion discount tapılmadı.");
        }

        public async Task<IResponse> RemoveAsync(int id)
        {
            StadiumDiscount? StadiumDiscount = await _repoDis.GetAsync(x => x.Id == id);
            if (StadiumDiscount != null)
            {
                _repoDis.Remove(StadiumDiscount);
                await _repoDis.SaveChangesAsync();
                return new Response(RespType.Success, "Stadion discount silindi.");
            }
            return new Response(RespType.NotFound, "Stadion discount tapılmadı.");
        }
    }
}
