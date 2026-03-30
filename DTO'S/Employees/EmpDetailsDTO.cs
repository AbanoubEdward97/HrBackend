using System.ComponentModel.DataAnnotations.Schema;

namespace HrBackend.DTO_S.Employees;

//public record EmpDTO(long Id, string Name, DateOnly HireDate,
//        DateTime AttendDate,
//        DateOnly BirthDate,
//        string DeptName,
//        string Gender,
//        string Nationality,
//        string PhoneNumber,
//        DateTime LeaveDate,
//        decimal Salary,
//        string NationalId);
public class EmpDetailsDTO
{
    ////////////////////////////////////
    /// Personal data
    public long Id { get; set; }

    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public DateOnly BirthDate { get; set; }

    public string PhoneNumber { get; set; } = "";
    public string GenderName { get; set; }
    public string Nationality { get; set; } = "";
    public string NationalId { get; set; } = "";
    public int DepartmentId { get; set; } // Foreign Key
    public string DepartmentName { get; set; }
    ////////////////////////////////////
    /// business data
    public DateOnly HireDate { get; set; }
    public decimal Salary { get; set; }
    public DateTime AttendDate { get; set; }
    public DateTime LeaveDate { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
