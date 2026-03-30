using HrBackend.DTO_S.OfficialHolidays;
using HrBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class OfficialHolidaysController : ControllerBase
{
    private readonly IOfficialHolidaysService _officialHolidaysService;

    public OfficialHolidaysController(IOfficialHolidaysService officialHolidaysService)
    {
        _officialHolidaysService = officialHolidaysService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? year)
    {
        var query = await _officialHolidaysService.GetAll();
        if (year.HasValue)
        {
            query =  query.Where(h => h.Date.Year == year.Value).ToList();
        }
        query = query.OrderBy(x => x.Date).ToList();
        return Ok(query);
    }

    [HttpPost]
    public async Task<IActionResult> AddHoliday([FromBody] CreateOfficialHolidayDTO dto)
    {
        var holiday = await _officialHolidaysService.Add(dto);

        return Ok(holiday);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateHoliday(int id, [FromBody] UpdateOfficialHolidayDTO dto)
    {
        var holiday = await _officialHolidaysService.UpdateHoliday(id, dto);
        return Ok(holiday);
    }
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteHoliday(int id)
    {
        var holiday = await _officialHolidaysService.DeleteHoliday(id);

        return Ok(holiday);
    }
}
