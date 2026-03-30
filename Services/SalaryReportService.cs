using AutoMapper;
using HrBackend.DTO_S.Employees;
using HrBackend.DTO_S.SalaryReport;
using HrBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class SalaryReportService : ISalaryReportService
{
    private readonly IGenerallSettingsService _generallSettingsService;
    private readonly HrContext _context;
    private readonly IMapper _mapper;

    public SalaryReportService(IGenerallSettingsService generallSettingsService, HrContext context, IMapper mapper)
    {
        _generallSettingsService = generallSettingsService;
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<SalaryReportRowDTO>> Get(SalaryReportQueryDTO query)
    {
        var start = new DateTime(query.Year, query.Month, 1);
        var end = start.AddMonths(1).AddDays(-1);

        //load settings 
        var settings = await _generallSettingsService.GetSettings();


        if (settings == null)
        {
            throw new InvalidOperationException("General settings are not configured");
        }
        //Load Employees (optional filter)
        var employeeQ = _context.Employees.Include(e => e.Department).AsNoTracking().AsQueryable();

        if (query.EmployeeId.HasValue)
        {
            employeeQ = employeeQ.Where(e => e.Id == query.EmployeeId.Value);
        }
        var employees = await employeeQ.ToListAsync();

        if (query.EmployeeId.HasValue && employees.Count == 0)
        {
            throw new InvalidOperationException("Employee Not Found.");
        }

        var employeeIds = employees.Select(e => e.Id).ToList();

        var attendance = await _context.AttendanceRecords.AsNoTracking().Where(a => employeeIds.Contains(a.EmployeeId) && a.WorkDate >= start && a.WorkDate <= end).ToListAsync();

        //group attendance by employee
        var attendanceByEmp = attendance.GroupBy(a => a.EmployeeId).ToDictionary(g => g.Key, g => g.ToList());
        var rows = new List<SalaryReportRowDTO>();


        foreach (var employee in employees)
        {
            //var empdto = new EmpDetailsDTO()
            //{
            //    Id = employee.Id,
            //    Name = employee.Name,
            //    HireDate = employee.HireDate,
            //    AttendDate = employee.AttendDate,
            //    BirthDate = employee.BirthDate,
            //    DepartmentName = employee.Department.Name,
            //    GenderName = Enum.GetName(employee.Gender),
            //    Nationality = employee.Nationality,
            //    PhoneNumber = employee.PhoneNumber,
            //    LeaveDate = employee.LeaveDate,
            //    Salary = employee.Salary,
            //    NationalId = employee.NationalId,
            //    DepartmentId = employee.DepartmentId
            //};
            var empdto = _mapper.Map<EmpDetailsDTO>(employee);

            var empAttendance = attendanceByEmp.TryGetValue(employee.Id, out var list) ? list : new List<AttendanceRecord>();

            //calculating working days (excluding weekly off days)
            var workingDays = CountWorkingDays(start, end, settings);
            if (workingDays <= 0)
            {
                throw new InvalidOperationException("Working days counted as zero , check settings weekly off days");
            }

            var presentDays = empAttendance.Select(a => a.WorkDate).Distinct().Count();
            var absentDays = workingDays - presentDays;
            if (absentDays < 0)
            {
                absentDays = 0;
            }
            //SCheduled work minutes per day
            var scheduledMinutes = GetScheduledMinutes(employee.AttendDate, employee.LeaveDate);
            if (scheduledMinutes <= 0)
            {
                scheduledMinutes = 8 * 60; //fallback 
            }

            //Hourly rate derived from monthly salary / (workingDays * hoursPerday)
            var hoursPerDay = scheduledMinutes / 60m;
            var hourlyRate = employee.Salary / (workingDays * hoursPerDay);
            var dailyRate = employee.Salary / workingDays;

            //compute overtime 
            var overtimeHours = 0m;
            var deductionHours = 0m;

            foreach (var rec in empAttendance)
            {
                //if time is missing , skip
                if (!rec.CheckIn.HasValue || !rec.CheckOut.HasValue)
                {
                    continue;
                }
                var scheduledIn = rec.WorkDate.Date + employee.AttendDate.TimeOfDay;
                var scheduledOut = rec.WorkDate.Date + employee.LeaveDate.TimeOfDay;

                if (rec.CheckIn.Value > scheduledIn)
                {
                    var late = rec.CheckIn.Value - scheduledIn;
                    deductionHours += (decimal)late.TotalMinutes / 60m;
                }

                if (rec.CheckOut.Value > scheduledOut)
                {
                    var ot = rec.CheckOut.Value - scheduledOut;
                    overtimeHours += (decimal)ot.TotalMinutes / 60m;
                }

            }
            //Money calculations based on settings 
            var totalOvertime = CalcAmount(settings.OvertimeCalculationMethod, settings.OvertimeValue, overtimeHours, hourlyRate);
            var totalDeduction = CalcAmount(settings.DeductionCalculationMethod, settings.DeductionValue, deductionHours, hourlyRate);

            //absence deductions 
            var absenceDeduction = absentDays * dailyRate;
            var net = employee.Salary + totalOvertime - totalDeduction - absenceDeduction;


            rows.Add(new SalaryReportRowDTO
            {
                EmployeeId = employee.Id,
                EmployeeName = employee.Name,
                DepartmentName = empdto.DepartmentName,
                BasicSalary = employee.Salary,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                OvertimeHours = Decimal.Round(totalOvertime, 2),
                DeductionHours = Decimal.Round(totalDeduction, 2),
                AbsenceDeduction = Decimal.Round(absenceDeduction, 2),
                NetSalary = Decimal.Round(net, 2),
                DepartmentId = empdto.DepartmentId
            });

        }
        return  rows;
    }

    private static int CountWorkingDays(DateTime start, DateTime end, GeneralSettings settings)
    {
        int count = 0;
        for (var d = start; d <= end; d = d.AddDays(1))
        {
            var dow = d.DayOfWeek;
            var isoff = dow == settings.WeeklyOfDay1 || (settings.WeeklyOfDay2.HasValue && dow == settings.WeeklyOfDay2.Value);
            if (!isoff)
            {
                count++;
            }
        }
        return count;

    }

    private static int GetScheduledMinutes(DateTime attend, DateTime leave)
    {
        var minutes = (leave - attend).TotalMinutes;
        return (int)minutes;
    }

    private static decimal CalcAmount(CalculationMethod method, decimal value, decimal hours, decimal hourlyRate)
    {
        //Interpretation :
        // FixedAmount ==> value is money per hour 
        // SalaryBased ==> value is multiplier ( 2 means 2x hourly rate)
        if (hours <= 0)
        {
            return 0;
        }
        return method switch
        {
            CalculationMethod.FixedAmount => hours * value,
            CalculationMethod.Multiplier => hours * (value * hourlyRate),
            _ => 0
        };
    }
}
