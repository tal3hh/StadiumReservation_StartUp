using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;
using ServiceLayer.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class StadiumService : IStadiumService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<DashStadiumDto>> AllAsync()
        {
            List<Stadium>? list = await _context.Stadiums.Include(x => x.AppUser).ToListAsync();

            return _mapper.Map<List<DashStadiumDto>>(list);
        }

        public async Task<DashStadiumDto> FindById(int id)
        {
            Stadium? entity = await _context.Stadiums.Include(x => x.AppUser).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashStadiumDto>(entity);
        }

        public async Task CreateAsync(CreateStadiumDto dto)
        {
            Stadium stadium = _mapper.Map<Stadium>(dto);
            stadium.CreateDate = DateTime.Now;
            stadium.IsActive = true;

            await _context.Stadiums.AddAsync(stadium);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UpdateStadiumDto dto)
        {
            Stadium? DBstadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBstadium != null)
            {
                Stadium stadium = _mapper.Map<Stadium>(dto);

                _context.Entry(DBstadium).CurrentValues.SetValues(stadium);

                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveAsync(int id)
        {
            Stadium? stadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == id);

            if (stadium != null)
            {
                _context.Stadiums.Remove(stadium);
                await _context.SaveChangesAsync();
            }
        }
    }
}
