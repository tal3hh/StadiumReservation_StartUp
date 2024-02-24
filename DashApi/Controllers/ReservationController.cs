using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Dash;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Reservations")]
        public async Task<IActionResult> Reservations()
        {
            var list = await _context.Reservations.Include(x => x.Area).ToListAsync();

            return Ok(_mapper.Map<List<DashReservationDto>>(list));
        }

        [HttpGet("Reservations/{id}")]
        public async Task<IActionResult> FindReservations(int id)
        {
            var entity = await _context.Reservations.Include(x=> x.Area).SingleOrDefaultAsync(x => x.Id == id);

            if (entity is null) return NotFound();

            return Ok(_mapper.Map<DashReservationDto>(entity));
        }

        [HttpPost("addReservation")]
        public async Task<IActionResult> addReservation(CreateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (await _context.Reservations.AnyAsync(x => x.Date.Hour == dto.Date.Hour && 
                                                       x.Date.Day == dto.Date.Day &&
                                                       x.Date.Year == dto.Date.Year &&
                                                       x.AreaId == dto.areaId))
                return BadRequest($"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");


            Reservation reservation = _mapper.Map<Reservation>(dto);
            reservation.CreateDate = DateTime.Now;
            reservation.IsActive = true;

            await _context.Reservations.AddAsync(reservation);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("upadteReservation")]
        public async Task<IActionResult> addReservation(UpdateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            Reservation? DBrezerv = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBrezerv is null) return NotFound();

            Reservation rezerv = _mapper.Map<Reservation>(dto);

            _context.Entry(DBrezerv).CurrentValues.SetValues(rezerv);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeReservation(int id)
        {
            Reservation? Reservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == id);

            if (Reservation is null) return NotFound();

            _context.Reservations.Remove(Reservation);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
