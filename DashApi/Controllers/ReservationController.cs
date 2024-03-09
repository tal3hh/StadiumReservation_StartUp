using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;
        private readonly AppDbContext _context;

        public ReservationController(IReservationService reservationService, AppDbContext context)
        {
            _reservationService = reservationService;
            _context = context;
        }

        [HttpGet("Reservations")]
        public async Task<IActionResult> Reservations()
        {
            return Ok(await _reservationService.AllAsync());
        }

        [HttpGet("Reservations/{id}")]
        public async Task<IActionResult> FindReservations(int id)
        {
            return Ok(await _reservationService.FindById(id));
        }

        [HttpPost("addReservation")]
        public async Task<IActionResult> addReservation(CreateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (await _reservationService.CreateAsync(dto) == 0)
                return BadRequest($"{dto.Date.ToString("HH:00 | dd/MMMM/yyyy")} bu tarixde artiq rezerv olunub.");

            if (await _reservationService.CreateAsync(dto) == 1)
                return BadRequest("Keçmiş saat üçün rezerv oluna bilməz.");

            return Ok();
        }

        [HttpPut("upadteReservation")]
        public async Task<IActionResult> upadteReservation(UpdateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            if (await _reservationService.UpdateAsync(dto))
                return NotFound();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeReservation(int id)
        {
            if (await _reservationService.RemoveAsync(id))
                return NotFound();

            return Ok();
        }
    }
}
