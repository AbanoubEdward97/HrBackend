using AutoMapper;
using ClosedXML.Excel;
using HrBackend.DTO_S.Attendance;
using HrBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class AttendanceService : IAttendanceService
{
    private readonly HrContext _context;
    private readonly IGenerallSettingsService _generallSettingsService;
    private readonly IMapper _mapper;

    public AttendanceService(HrContext context, IGenerallSettingsService generallSettingsService, IMapper mapper)
    {
        _context = context;
        _generallSettingsService = generallSettingsService;
        _mapper = mapper;
    }
    public async Task<List<AttendanceRecord>> GetAttendance(AttendanceQueryDTO query)
    {
        var settings = await _generallSettingsService.GetSettings();
        var records = _context.AttendanceRecords.Include(e => e.Employee).AsQueryable();
        var offDays = new List<int> { (int)settings.WeeklyOfDay1 + 1 };
        if (settings.WeeklyOfDay2.HasValue)
        {
            offDays.Add((int)settings.WeeklyOfDay2.Value + 1);
        }
        if (!query.IncludeOffDays)
        {
            records = records.Where(r => !offDays.Contains(EF.Functions.DateDiffDay(DateTime.MinValue,r.WorkDate)%7 + 1));
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
        return await records.ToListAsync();
    }

    public async Task<AttendanceRecord?> Upsert(AttendanceUpsertDTO dto)
    {
        var generalSetting = await _generallSettingsService.GetSettings();
        if (isWeeklyOffDay(dto.WorkDate , generalSetting))
        {
            throw new InvalidOperationException("This is A weekly off Day");
        }
        var existing = await _context.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == dto.EmployeeId && a.WorkDate == dto.WorkDate);
        if (existing == null)
        {
            var record = _mapper.Map<AttendanceRecord>(dto);
            record.Source = AttendanceSource.Manual;
            _context.Add(record);
            await _context.SaveChangesAsync();
            return record;
        }
        _mapper.Map(dto, existing);
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return existing;
    }
    public async Task<AttendanceRecord> Delete(int id)
    {
        var record = await _context.AttendanceRecords.FindAsync(id);
        if (record == null)
            throw new KeyNotFoundException($"record with id : {id} not found");
        _context.Remove(record);
        await _context.SaveChangesAsync();
        return record;
    }

    private static bool isWeeklyOffDay(DateTime date, GeneralSettings settings)
    {
        var day = date.DayOfWeek;
        return day == settings.WeeklyOfDay1 || (settings.WeeklyOfDay2.HasValue && day == settings.WeeklyOfDay2);
    }

    public async Task<AttendanceImportResultDto> ImportExcel(ImportExcellDTO dto)
    {
        var file = dto.file;
        if (file == null || file.Length == 0)
        {
            throw new InvalidOperationException("Please Upload a valid excell file");
        }
        if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only .xlsx files are supported");
        }
        var result = new AttendanceImportResultDto();
        //Load settings 
        var settings = await _generallSettingsService.GetSettings();
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheets.First();

        int lastRow = ws.LastRowUsed().RowNumber();

        if (lastRow < 2)
        {
           throw new ArgumentNullException("Excell File contains no data rows");
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
            throw new InvalidOperationException("Missing required Column : WorkDate");

        }
        if (!hasEmployeeId && !hasEmployeeName)
        {
            throw new InvalidOperationException("Missing required Column : EmployeeId or EmployeeName");

        }
        int colEmployeeId = hasEmployeeId ? headerMap["EmployeeId"] : -1;
        int colEmployeeName = hasEmployeeName ? (headerMap.ContainsKey("EmployeeName") ? headerMap["EmployeeName"] : headerMap["Name"]) : -1;

        int colWorkDate = headerMap["WorkDate"];
        int colCheckIn = headerMap.ContainsKey("CheckIn") ? headerMap["CheckIn"] : -1;
        int colCheckOut = headerMap.ContainsKey("CheckOut") ? headerMap["CheckOut"] : -1;
        int colNotes = headerMap.ContainsKey("Notes") ? headerMap["Notes"] : -1;

        //Cache Employee by name if needed 
        Dictionary<string, long>? employeeNameToId = null;
        if (!hasEmployeeId && hasEmployeeName)
        {
            employeeNameToId = await _context.Employees.AsNoTracking().ToDictionaryAsync(e => e.Name.Trim(), e => e.Id, StringComparer.OrdinalIgnoreCase);
        }

        for (int r = 2; r <= lastRow; r++)
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
                else
                {
                    var empName = row.Cell(colEmployeeName).GetString().Trim();
                    if (string.IsNullOrWhiteSpace(empName))
                    {
                        throw new Exception("Employee Is Required!!");
                    }

                    if (employeeNameToId == null || !employeeNameToId.TryGetValue(empName, out employeeId))
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
                }
                else
                {
                    var rawDate = cellWorkDate.GetString().Trim();
                    if (!DateTime.TryParse(rawDate, out workDate))
                    {
                        throw new Exception("Invalid WorkDate");
                    }
                    workDate = workDate.Date;
                }

                //weekly offday block
                if (isWeeklyOffDay(workDate, settings))
                {
                    throw new Exception("workdate is weekly off day");
                }

                //checkIn / checkOut (optional)
                DateTime? CheckIn = null;
                DateTime? CheckOut = null;

                if (colCheckIn != -1)
                    CheckIn = ReadOptionalDateTime(row.Cell(colCheckIn), workDate);

                if (colCheckOut != -1)
                    CheckOut = ReadOptionalDateTime(row.Cell(colCheckOut), workDate);

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

       await  _context.SaveChangesAsync();
        return result;
    }

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
