namespace HrBackend.Models;
using HrApi.Models;
using System.Text.Json.Serialization;

public class Department
{
    public int DepartmentId { get; set; }
    [Required]
    [MaxLength(100)]
    public string Name { get; set; }
    [MaxLength(250)]
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //Nav property (one department ==> Many Employees)
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
