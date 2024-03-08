using AutoMapper;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Services.Interface;
using ServiceLayer.Utlities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        public async Task<List<DashReservationDto>> AllAsync()
        {
            var list = await _context.Reservations.Include(x => x.Area).ToListAsync();

            return _mapper.Map<List<DashReservationDto>>(list);
        }

        public async Task<DashReservationDto> FindById(int id)
        {
            var entity = await _context.Reservations.Include(x => x.Area).SingleOrDefaultAsync(x => x.Id == id);

            return _mapper.Map<DashReservationDto>(entity);
        }

        public async Task<bool> CreateAsync(CreateReservationDto dto)
        {
            if (await _context.Reservations.AnyAsync(x => x.Date.Hour == dto.Date.Hour &&
                                                       x.Date.Day == dto.Date.Day &&
                                                       x.Date.Year == dto.Date.Year &&
                                                       x.AreaId == dto.areaId))
                return false;   

            Reservation Reservation = _mapper.Map<Reservation>(dto);
            Reservation.CreateDate = DateTimeAz.Now;
            Reservation.IsActive = true;

            await _context.Reservations.AddAsync(Reservation);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateAsync(UpdateReservationDto dto)
        {
            Reservation? DBReservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBReservation != null)
            {
                Reservation Reservation = _mapper.Map<Reservation>(dto);

                _context.Entry(DBReservation).CurrentValues.SetValues(Reservation);

                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }

        public async Task<bool> RemoveAsync(int id)
        {
            Reservation? Reservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == id);

            if (Reservation != null)
            {
                _context.Reservations.Remove(Reservation);
                await _context.SaveChangesAsync();

                return true;
            }
            return false;
        }
    }
}
