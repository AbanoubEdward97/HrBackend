namespace HrBackend.DTO_S.Attendance;

public class AttendanceQueryDTO
{
    public string? EmployeeName { get; set; }
    public int? DepartmentId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool IncludeOffDays { get; set; } = false;
}
