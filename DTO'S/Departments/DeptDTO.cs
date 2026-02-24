namespace HrBackend.DTO_S.Departments;

public class DeptDTO
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(250)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;
}
