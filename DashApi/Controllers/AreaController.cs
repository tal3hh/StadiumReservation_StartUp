using AutoMapper;
using DomainLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Contexts;
using RepositoryLayer.Migrations;
using ServiceLayer.Dtos.Area.Dash;
using ServiceLayer.Services.Interface;

namespace DashApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly IAreaService _areaService;

        public AreaController(IAreaService areaService)
        {
            _areaService = areaService;
        }

        [HttpGet("Areas")]
        public async Task<IActionResult> Areas()
        {
            return Ok(await _areaService.AllAsync());
        }

        [HttpGet("Area/{id}")]
        public async Task<IActionResult> Areas(int id)
        {
            return Ok(await _areaService.FindById(id));
        }

        [HttpPost("addArea")]
        public async Task<IActionResult> addArea(CreateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _areaService.CreateAsync(dto);

            return Ok();
        }

        [HttpPut("upadteArea")]
        public async Task<IActionResult> addArea(UpdateAreaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(dto);

            await _areaService.UpdateAsync(dto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> removeArea(int id)
        {
            await _areaService.RemoveAsync(id);

            return Ok();
        }
    }
}
