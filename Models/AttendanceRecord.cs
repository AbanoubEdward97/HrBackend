namespace HrBackend.Models;

public class AttendanceRecord
{
    public int Id { get; set; }
    public long EmployeeId { get; set; }
    public Employee Employee { get; set; }
    public DateTime WorkDate { get; set; }
    public DateTime? CheckIn { get; set; }
    public DateTime? CheckOut { get; set; }
    public AttendanceSource Source { get; set; } = AttendanceSource.Manual;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
