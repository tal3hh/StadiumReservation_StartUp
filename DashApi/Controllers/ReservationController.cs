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
            return Ok(await _context.Reservations.ToListAsync());
        }

        [HttpGet("Reservations/{id}")]
        public async Task<IActionResult> FindReservations(int id)
        {
            return Ok(await _context.Reservations.SingleOrDefaultAsync(x => x.Id == id));
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

            Reservation? reservation = await _context.Reservations.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (reservation is null) return NotFound();

            _context.Reservations.Update(reservation);
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
