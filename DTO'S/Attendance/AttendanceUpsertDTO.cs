namespace HrBackend.DTO_S.Attendance;

public class AttendanceUpsertDTO
{
    public long EmployeeId { get; set; }
    public DateTime WorkDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public string? Notes { get; set; }
}
