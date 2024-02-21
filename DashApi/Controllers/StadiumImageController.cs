using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using ServiceLayer.Dtos.Stadium.Dash;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StadiumImageController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public StadiumImageController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("StadiumImages")]
        public async Task<IActionResult> StadiumImages()
        {
            return Ok(await _context.StadiumImages.ToListAsync());
        }

        [HttpGet("StadiumImage/{id}")]
        public async Task<IActionResult> StadiumImages(int id)
        {
            return Ok(await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == id));
        }

        [HttpPost("addStadiumImage")]
        public async Task<IActionResult> addStadiumImage(CreateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            StadiumImage StadiumImage = _mapper.Map<StadiumImage>(dto);
            StadiumImage.CreateDate = DateTime.Now;
            StadiumImage.IsActive = true;

            await _context.StadiumImages.AddAsync(StadiumImage);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("upadteStadiumImage")]
        public async Task<IActionResult> upadteStadiumImage(UpdateStadiumImageDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            StadiumImage? DBstadiumimage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == dto.Id);

            if (DBstadiumimage is null) return NotFound();

            StadiumImage stadiumimage = _mapper.Map<StadiumImage>(dto);

            _context.Entry(DBstadiumimage).CurrentValues.SetValues(stadiumimage);

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeStadiumImage(int id)
        {
            StadiumImage? StadiumImage = await _context.StadiumImages.SingleOrDefaultAsync(x => x.Id == id);

            if (StadiumImage is null) return NotFound();

            _context.StadiumImages.Remove(StadiumImage);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
