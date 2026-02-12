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
    public string Gender { get; set; } = "";
    public string Nationality { get; set; } = "";
    public string NationalId { get; set; } = "";
    ////////////////////////////////////
    /// business data
    public DateOnly HireDate { get; set; }
    public decimal Salary { get; set; }
    public TimeOnly AttendDate { get; set; }
    public TimeOnly LeaveDate { get; set; }
}