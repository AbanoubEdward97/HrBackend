using HrBackend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrApi.Models;

public class Employee
{
    ////////////////////////////////////
    /// Personal data
    public long Id { get; set; }

    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public DateOnly BirthDate { get; set; }

    public string PhoneNumber { get; set; } = "";
    public Gender Gender { get; set; }
    public string Nationality { get; set; } = "";
    public string NationalId { get; set; } = "";
    [ForeignKey(nameof(Department.DepartmentId))]
    public int DepartmentId { get; set; } // Foreign Key
    public Department Department { get; set; }

    ////////////////////////////////////
    /// business data
    public DateOnly HireDate { get; set; }
    public decimal Salary { get; set; }
    public DateTime AttendDate { get; set; }
    public DateTime LeaveDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}