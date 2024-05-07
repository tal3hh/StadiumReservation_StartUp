using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Common.Result;
using ServiceLayer.Dtos.Reservation.Dash;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }



        [HttpGet("Reservations")]
        public async Task<IActionResult> Reservations()
        {
            return Ok(await _reservationService.AllAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> FindReservations(int id)
        {
            return Ok(await _reservationService.FindById(id));
        }

        [HttpPost("create")]
        public async Task<IActionResult> addReservation(CreateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _reservationService.CreateAsync(dto);

            if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpPut("update")]
        public async Task<IActionResult> upadteReservation(UpdateReservationDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            var result = await _reservationService.UpdateAsync(dto);

            if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.BadReqest)
                return BadRequest(result.Message);

            else if (result.RespType == RespType.NotFound)
                return NotFound(result.Message);

            return BadRequest("Xəta baş verdi.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeReservation(int id)
        {
            var result = await _reservationService.RemoveAsync(id);

            if (result.RespType == RespType.Success)
                return Ok(result.Message);

            else if (result.RespType == RespType.NotFound)
                return BadRequest(result.Message);

            return BadRequest("Xəta baş verdi.");
        }
    }
}
