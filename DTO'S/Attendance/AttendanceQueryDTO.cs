namespace HrBackend.DTO_S.Attendance;

public class AttendanceQueryDTO
{
    public string? EmployeeName { get; set; }
    public int? DepartmentId { get; set; }
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public bool IncludeOffDays { get; set; } = false;
}
