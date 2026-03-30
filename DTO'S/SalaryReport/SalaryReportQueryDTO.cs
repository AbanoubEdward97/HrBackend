namespace HrBackend.DTO_S.SalaryReport;

public class SalaryReportQueryDTO
{
    public int Month { get; set; } // 1..12
    public int Year { get; set; } // >= 2008
    public long? EmployeeId { get; set; } //optional filter 
}
