using HrBackend.DTO_S.Departments;
using HrBackend.DTO_S.GeneralSettings;
using HrBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GeneralSettingsController : ControllerBase
{
    //private readonly HrContext _context;
    private readonly IGenerallSettingsService _generallSettingsService;

    public GeneralSettingsController(/**HrContext context,**/ IGenerallSettingsService generallSettingsService)
    {
       // _context = context;
        _generallSettingsService = generallSettingsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetSettings()
    {
        var settings = await _generallSettingsService.GetSettings();
        return Ok(settings);
    }

    [AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateGeneralSettingsDTO dto)
    {
        //var setting = await _generallSettingsService.GetSettings();
        //if (setting == null)
        //{
        //    return NotFound("Setting not found");
        //}
        if (dto.WeeklyOfDay2.HasValue && dto.WeeklyOfDay1 == dto.WeeklyOfDay2)
        {
            return BadRequest("Weekly Ofdays must be different");
        }
        var setting = await _generallSettingsService.UpdateSettings(dto);

        //setting.OvertimeCalculationMethod = dto.OvertimeCalculationMethod;
        //setting.OvertimeValue = dto.OvertimeValue;
        //setting.DeductionCalculationMethod = dto.DeductionCalculationMethod;
        //setting.DeductionValue = dto.DeductionValue;
        //setting.LastUpdated = DateTime.UtcNow;
        //setting.WeeklyOfDay1 = dto.WeeklyOfDay1;
        //setting.WeeklyOfDay2 = dto.WeeklyOfDay2;

        //_context.SaveChanges();
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
