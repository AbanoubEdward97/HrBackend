using HrBackend.DTO_S.Attendance;
using HrBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;

namespace HrBackend.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AttendanceController : ControllerBase
{       
    private readonly HrContext _context;

    public AttendanceController(HrContext context)
    {
        _context = context;
    }
    [HttpGet]
    public async Task<IActionResult> getAll([FromQuery] AttendanceQueryDTO query)
    {
        if (query.From.HasValue && query.To.HasValue && query.From > query.To)
        {
            return BadRequest("From Date cannot be greater than To date");
        }
        ;
        var records = _context.AttendanceRecords.Include(e => e.Employee).AsQueryable();
        if (!String.IsNullOrEmpty(query.EmployeeName))
        {
            records = records.Where(r => r.Employee.Name.Contains(query.EmployeeName));
        }
        if (query.DepartmentId.HasValue)
        {
            records = records.Where(r => r.Employee.DepartmentId == query.DepartmentId);
        }
        if (query.From.HasValue)
        {
            records = records.Where(r => r.WorkDate >= query.From);
        }
        if (query.To.HasValue)
        {
            records = records.Where(r => r.WorkDate <= query.To);
        }
        var result = await records.ToListAsync();
        return Ok(result);
    }
    [HttpPost]
    public async Task<IActionResult> Upsert(AttendanceUpsertDTO dto)
    {
        if (dto.CheckIn.HasValue && dto.CheckOut.HasValue && dto.CheckOut < dto.CheckIn)
        {
            return BadRequest("checkIn date must be less than checkOut date");
        }
        var generalSetting = await _context.GeneralSettings.AsNoTracking().FirstOrDefaultAsync(g => g.Id == 1);
        if (generalSetting == null)
        {
            return StatusCode(500, "general settings are not configured");
        }
        if (isWeeklyOffDay(dto.WorkDate, generalSetting))
        {
            return BadRequest("This is a weekly off day ");
        }   
        var existing = await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.WorkDate == dto.WorkDate);

        if (existing == null) {
            var record = new AttendanceRecord
            {
                EmployeeId = dto.EmployeeId,
                WorkDate = dto.WorkDate,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                Notes = dto.Notes,
                Source = AttendanceSource.Manual
            };
            _context.Add(record);
        }
        else
        {
            existing.CheckIn = dto.CheckIn;
            existing.CheckOut = dto.CheckOut;
            existing.Notes = dto.Notes;
            existing.UpdatedAt = DateTime.UtcNow;

        }

        _context.SaveChanges(); 
        return Ok(existing);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> deleteRecord(int id)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null)
        {
            return NotFound("no record is found with this id");
        }
        _context.Remove(record);
        _context.SaveChanges();
        return Ok(record);
    }

    private static bool isWeeklyOffDay(DateOnly date , GeneralSettings settings)
    {
        var day = date.DayOfWeek;
        return day == settings.WeeklyOfDay1 || (settings.WeeklyOfDay2.HasValue && day == settings.WeeklyOfDay2);
    }

}
