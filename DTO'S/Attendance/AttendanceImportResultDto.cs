namespace HrBackend.DTO_S.Attendance;

public class AttendanceImportResultDto
{
    public int TotalRows { get; set; }
    public int Inserted { get; set; }
    public int Updated { get; set; }
    public int Failed { get; set; }
    public List<AttendanceImportRowErrorDTO> Errors { get; set; } = new();
}
