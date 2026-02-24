namespace HrBackend.DTO_S.OfficialHolidays;

public class UpdateOfficialHolidayDTO
{
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [Required]
    public DateOnly Date { get; set; }
}
