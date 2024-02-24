using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Migrations;
using ServiceLayer.Dtos.Area.Dash;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public AreaController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("Areas")]
        public async Task<IActionResult> Areas()
        {
            var list = await _context.Areas.Include(x => x.Stadium).ToListAsync();

            return Ok(_mapper.Map<List<DashAreaDto>>(list));
        }

        [HttpGet("Area/{id}")]
        public async Task<IActionResult> Areas(int id)
        {
            var entity = await _context.Areas.Include(x=> x.Stadium).SingleOrDefaultAsync(x => x.Id == id);

            return Ok(_mapper.Map<DashAreaDto>(entity));
        }

        [HttpPost("addArea")]
        public async Task<IActionResult> addArea(CreateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            Area Area = _mapper.Map<Area>(dto);
            Area.CreateDate = DateTime.Now;
            Area.IsActive = true;

            await _context.Areas.AddAsync(Area);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("upadteArea")]
        public async Task<IActionResult> addArea(UpdateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            Area? DBarea = await _context.Areas.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBarea is null) return NotFound();

            Area area = _mapper.Map<Area>(dto);

            _context.Entry(DBarea).CurrentValues.SetValues(area);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeArea(int id)
        {
            Area? Area = await _context.Areas.SingleOrDefaultAsync(x => x.Id == id);

            if (Area is null) return NotFound();

            _context.Areas.Remove(Area);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
