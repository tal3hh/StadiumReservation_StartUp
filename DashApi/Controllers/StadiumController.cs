using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Stadiums")]
        public async Task<IActionResult> Stadiums()
        {
            return Ok(await _context.Stadiums.ToListAsync());
        }

        [HttpGet("Stadium/{id}")]
        public async Task<IActionResult> Stadiums(int id)
        {
            return Ok(await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == id));
        }

        [HttpPost("addStadium")]
        public async Task<IActionResult> addStadium(CreateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            Stadium stadium = _mapper.Map<Stadium>(dto);
            stadium.CreateDate = DateTime.Now;
            stadium.IsActive = true;

            await _context.Stadiums.AddAsync(stadium);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("upadteStadium")]
        public async Task<IActionResult> upadteStadium(UpdateStadiumDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            Stadium? DBstadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBstadium is null) return NotFound();

            Stadium stadium = _mapper.Map<Stadium>(dto);

            _context.Entry(DBstadium).CurrentValues.SetValues(stadium);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadium(int id)
        {
            Stadium? stadium = await _context.Stadiums.SingleOrDefaultAsync(x => x.Id == id);

            if (stadium is null) return NotFound();

            _context.Stadiums.Remove(stadium);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
