using ClosedXML;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.ExtendedProperties;
using HrBackend.DTO_S.Attendance;
using HrBackend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using NuGet.Configuration;
using System.Diagnostics;

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
        var settings = await _context.GeneralSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.Id ==1);
        if (settings == null)
        {
            return StatusCode(500 , "General settings are not configured");
        }
        var records = _context.AttendanceRecords.Include(e => e.Employee).AsQueryable();
        if (!query.IncludeOffDays)
        {
            records = records.Where(r => r.WorkDate.DayOfWeek != settings.WeeklyOfDay1 && 
            (!settings.WeeklyOfDay2.HasValue || r.WorkDate.DayOfWeek != settings.WeeklyOfDay2.Value)
            );
        }
        if (query.From.HasValue && query.To.HasValue && query.From > query.To)
        {
            return BadRequest("From Date cannot be greater than To date");
        }
        ;
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

    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ImportExcell(ImportExcellDTO dto)
    {
        var file = dto.file;
        if (file == null || file.Length == 0) {
            return BadRequest("Please Upload a valid excell file");
        }
        if (!Path.GetExtension(file.FileName).Equals(".xlsx",StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Only .xlsx files are supported");
        }
        var result = new AttendanceImportResultDto();

        //Load settings 
        var settings = await _context.GeneralSettings.AsNoTracking().FirstOrDefaultAsync(x=>x.Id == 1);
        if (settings == null)
        {
            return StatusCode(500, "General settings are not configured");

        }

        using var stream  = new MemoryStream(); 
        await file.CopyToAsync(stream);
        stream.Position = 0;
        
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.First();

        int lastRow = ws.LastRowUsed().RowNumber();
        if (lastRow < 2)
        {
            return BadRequest("Excell File contains no data rows");
        }

        var headerRow = ws.Row(1);
        var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var cell in headerRow.CellsUsed())
        {
            var name = cell.GetString().Trim();
            if (!string.IsNullOrWhiteSpace(name))
            {
                headerMap[name] = cell.Address.ColumnNumber;
            }
        }
        bool hasEmployeeId = headerMap.ContainsKey("EmployeeId");
        bool hasEmployeeName = headerMap.ContainsKey("EmployeeName") || headerMap.ContainsKey("Name");

        bool hasWorkDate = headerMap.ContainsKey("WorkDate");
        if (!hasWorkDate)
        {
            return BadRequest("Missing required Column : WorkDate");

        }
        if (!hasWorkDate)
        {
            return BadRequest("Missing required Column : WorkDate");

        }
        if (!hasEmployeeId && !hasEmployeeName)
        {
            return BadRequest("Missing required Column : EmployeeId or EmployeeName");

        }
        int colEmployeeId = hasEmployeeId ? headerMap["EmployeeId"] : -1;
        int colEmployeeName = hasEmployeeName ? (headerMap.ContainsKey("EmployeeName") ? headerMap["EmployeeName"] : headerMap["Name"]) : -1;

        int colWorkDate = headerMap["Workdate"];
        int colCheckIn = headerMap.ContainsKey("CheckIn") ? headerMap["CheckIn"] : -1;
        int colCheckOut = headerMap.ContainsKey("CheckOut") ? headerMap["CheckOut"] : -1;
        int colNotes = headerMap.ContainsKey("Notes") ? headerMap["Notes"] : -1;

        //Cache Employee by name if needed 
        Dictionary<string,long>? employeeNameToId = null;
        if (!hasEmployeeId && hasEmployeeName) 
        { 
            employeeNameToId = await _context.Employees.AsNoTracking().ToDictionaryAsync(e => e.Name.Trim(),e=>e.Id,StringComparer.OrdinalIgnoreCase);
        }

        for (int r = 2; r  <= lastRow; r++)
        {
            try
            {
                var row = ws.Row(r);

            long employeeId;
            if (hasEmployeeId)
            {
                var raw = row.Cell(colEmployeeId).GetString().Trim();
                if (!long.TryParse(raw, out employeeId) || employeeId <= 0)
                {
                    throw new Exception("Invalid EmployeeId");
                }
            }
            else {
                var empName = row.Cell(colEmployeeName).GetString().Trim();
                if (string.IsNullOrWhiteSpace(empName))
                {
                    throw new Exception("Employee Is Required!!");
                }

                if (employeeNameToId == null || !employeeNameToId.TryGetValue(empName , out employeeId))
                {
                    throw new Exception($"Could not find {empName}");
                }
            }
            //Workdate 
            DateTime workDate;
            var cellWorkDate = row.Cell(colWorkDate);
            if (cellWorkDate.DataType == XLDataType.DateTime)
            {
                workDate = cellWorkDate.GetDateTime().Date;
            }else
            {
                var rawDate = cellWorkDate.GetString().Trim();
                if (!DateTime.TryParse(rawDate , out workDate))
                {
                    throw new Exception("Invalid WorkDate");
                }
                workDate = workDate.Date;
            }

            //weekly offday block
            if (isWeeklyOffDay(workDate , settings))
            {
                throw new Exception("workdate is weekly off day");
            }

            //checkIn / checkOut (optional)
            DateTime? CheckIn = null;
            DateTime? CheckOut = null;

            if (colCheckIn != -1)
                CheckIn = ReadOptionalDateTime(row.Cell(colCheckIn), workDate);

            if (colCheckOut != -1)
                CheckIn = ReadOptionalDateTime(row.Cell(colCheckOut), workDate);

            if (CheckIn.HasValue && CheckOut.HasValue && CheckOut.Value < CheckIn.Value)
                throw new Exception("CheckOut must be after CheckIn.");
                var notes = colNotes != -1 ? row.Cell(colNotes).GetString().Trim() : null;
                // UPSERT by (EmployeeId, WorkDate)
                var existing = await _context.AttendanceRecords
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.WorkDate.Date == workDate);

            if (existing == null)
            {
                _context.AttendanceRecords.Add(new AttendanceRecord
                {
                    EmployeeId = employeeId,
                    WorkDate = workDate,
                    CheckIn = CheckIn,
                    CheckOut = CheckOut,
                    Notes = notes,
                    Source = AttendanceSource.Excell
                });

                result.Inserted++;
            }
            else
            {
                existing.CheckIn = CheckIn;
                existing.CheckOut = CheckOut;
                existing.Notes = notes;
                existing.Source = AttendanceSource.Excell;
                existing.UpdatedAt = DateTime.UtcNow;

                result.Updated++;
            }
        }
        catch (Exception ex)
        {
            result.Failed++;
            result.Errors.Add(new AttendanceImportRowErrorDTO
            {
                RowNumber = r,
                Error = ex.Message
            });
        }
    }

    _context.SaveChanges();
    return Ok(result);
}


    private static bool isWeeklyOffDay(DateTime date , GeneralSettings settings)
    {
        var day = date.DayOfWeek;
        return day == settings.WeeklyOfDay1 || (settings.WeeklyOfDay2.HasValue && day == settings.WeeklyOfDay2);
    }

    // Helper: read optional time/date cell and combine with workDate if needed
    private static DateTime? ReadOptionalDateTime(IXLCell cell, DateTime workDate)
    {
        if (cell.IsEmpty())
            return null;

        if (cell.DataType == XLDataType.DateTime)
            return cell.GetDateTime();

        // If Excel contains time only like "08:30", combine with workDate
        var raw = cell.GetString().Trim();
        if (TimeSpan.TryParse(raw, out var t))
            return workDate.Date.Add(t);

        if (DateTime.TryParse(raw, out var dt))
            return dt;

        // If can't parse, treat as invalid
        throw new Exception($"Invalid datetime value: {raw}");
    }
}
