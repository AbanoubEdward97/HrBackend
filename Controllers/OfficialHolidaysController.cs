using HrBackend.DTO_S.OfficialHolidays;
using HrBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OfficialHolidaysController : ControllerBase
{
    private readonly HrContext _context;

    public OfficialHolidaysController(HrContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? year)
    {
        var query = await _context.OfficialHolidays.ToListAsync();
        if (year.HasValue)
        {
            query = query.Where(h => h.Date.Year == year.Value).ToList();
        }
        query = query.OrderBy(x => x.Date).ToList();
        return Ok(query);
    }

    [HttpPost]
    public async Task<IActionResult> AddHoliday([FromBody] CreateOfficialHolidayDTO dto)
    {
        var date = dto.Date;
        var exists = await _context.OfficialHolidays.AnyAsync(x => x.Date == date);
        if (exists)
        {
            return BadRequest("Holiday With That date already exists");
        }
        var holiday = new OfficialHoliday
        {
            Date = date,
            Name = dto.Name.Trim()
        };

        _context.Add(holiday);
        _context.SaveChanges();
        return Ok(holiday);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateHoliday(int id, [FromBody] UpdateOfficialHolidayDTO dto)
    {
        var holiday = await _context.OfficialHolidays.FindAsync(id);
        if (holiday == null)
        {
            return BadRequest("Holiday Not Found");
        }

        var date = dto.Date;
        var exists = await _context.OfficialHolidays.AnyAsync(h => h.Id != id && h.Date == date);
        if (exists)
        {
            return BadRequest("A holiday already exists on that date !!");
        }

        holiday.Name = dto.Name.Trim();
        holiday.Date = date;

        _context.SaveChanges();
        return Ok(holiday);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHoliday(int id)
    {
        var holiday = await _context.OfficialHolidays.FindAsync(id);
        if (holiday == null)
        {
            return NotFound("holiday not found");
        }
        _context.Remove(holiday);
        _context.SaveChanges();

        return Ok(holiday);
    }
}
