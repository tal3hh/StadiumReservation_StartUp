using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.StadiumImage;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<DashStadiumImageDto> FindById(int id)
        {
            var entity = await _context.StadiumImages.Include(x => x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashStadiumImageDto>(entity);
        }

        public async Task CreateAsync(CreateStadiumImageDto dto)
        {
            StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);
            StadiumImage.CreateDate = DateTimeAz.Now;
            StadiumImage.IsActive = true;

            await _context.StadiumImages.AddAsync(StadiumImage);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(UpdateStadiumImageDto dto)
        {
            StadiumImage? DBStadiumImage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBStadiumImage != null)
            {
                StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);

                _context.Entry(DBStadiumImage).CurrentValues.SetValues(StadiumImage);

                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            StadiumImage? StadiumImage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumImage is null)
            {
                _context.StadiumImages.Remove(StadiumImage);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
