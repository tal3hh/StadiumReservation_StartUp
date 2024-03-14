using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.StadiumDetail;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<DashStadiumDetailDto> FindById(int id)
        {
            var entity = await _context.StadiumDetails.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashStadiumDetailDto>(entity);
        }

        public async Task CreateAsync(CreateStadiumDetailDto dto)
        {
            StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);
            StadiumDetail.CreateDate = DateTimeAz.Now;
            StadiumDetail.IsActive = true;

            await _context.StadiumDetails.AddAsync(StadiumDetail);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(UpdateStadiumDetailDto dto)
        {
            StadiumDetail? DBStadiumDetail = await _context.StadiumDetails.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumDetail != null)
            {
                StadiumDetail StadiumDetail = _mapper.Map<StadiumDetail>(dto);

                _context.Entry(DBStadiumDetail).CurrentValues.SetValues(StadiumDetail);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            StadiumDetail? StadiumDetail = await _context.StadiumDetails.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumDetail is null)
            {
                _context.StadiumDetails.Remove(StadiumDetail);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
