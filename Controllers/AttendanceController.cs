using ClosedXML.Excel;
using HrBackend.DTO_S.Attendance;
using HrBackend.Models;
using HrBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{       
    private readonly IAttendanceService _attendanceService;

    public AttendanceController(HrContext context, IAttendanceService attendanceService, IGenerallSettingsService generallSettingsService)
    {
        _attendanceService = attendanceService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAttendance([FromQuery] AttendanceQueryDTO query)
    {
        
        var result = await _attendanceService.GetAttendance(query);
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Upsert(AttendanceUpsertDTO dto)
    {
        var record = await _attendanceService.Upsert(dto);
        return Ok(record);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> deleteRecord(int id)
    {
        var record = await _attendanceService.Delete(id);
        return Ok(record);
    }

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportExcel(ImportExcellDTO dto)
    {
        var result = await _attendanceService.ImportExcel(dto);
        return Ok(result);
    }
}
