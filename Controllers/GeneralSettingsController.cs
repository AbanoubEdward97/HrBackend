using HrBackend.DTO_S.Departments;
using HrBackend.DTO_S.GeneralSettings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GeneralSettingsController : ControllerBase
{
    private readonly HrContext _context;

    public GeneralSettingsController(HrContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> getGeneralSettings()
    {
        return Ok(await _context.GeneralSettings.ToListAsync());
    }

    [AllowAnonymous]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDept(int id, [FromBody] UpdateGeneralSettingsDTO dto)
    {
        var setting = await _context.GeneralSettings.SingleOrDefaultAsync(g => g.Id == id);
        if (setting == null)
        {
            return NotFound("Setting not found");
        }
        if (dto.WeeklyOfDay2.HasValue && dto.WeeklyOfDay1 == dto.WeeklyOfDay2)
        {
            return BadRequest("Weekly Ofdays must be different");
        }

        setting.OvertimeCalculationMethod = dto.OvertimeCalculationMethod;
        setting.OvertimeValue = dto.OvertimeValue;
        setting.DeductionCalculationMethod = dto.DeductionCalculationMethod;
        setting.DeductionValue = dto.DeductionValue;
        setting.LastUpdated = DateTime.UtcNow;
        setting.WeeklyOfDay1 = dto.WeeklyOfDay1;
        setting.WeeklyOfDay2 = dto.WeeklyOfDay2;

        _context.SaveChanges();
        //return Ok(new { 
        //    OvertimeCalculationMethod  = setting.OvertimeCalculationMethod.ToString() ,
        //    setting.OvertimeValue ,
        //    DeductionCalculationMethod = setting.DeductionCalculationMethod.ToString(),
        //    setting.DeductionValue ,
        //    setting.LastUpdated ,
        //    WeeklyOfDay1 = setting.WeeklyOfDay1.ToString(),
        //    WeeklyOfDay2 = setting.WeeklyOfDay2.ToString()}
        //);
        return Ok(setting);
    }
}
