namespace HrBackend.DTO_S.Departments;

public class UpdateDeptDTO
{
    //public int DepartmentId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(250)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
