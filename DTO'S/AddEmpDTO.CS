using System.ComponentModel.DataAnnotations;
public class AddEmpDTO
{
    //[Required]
    //[StringLength(100, MinimumLength = 2)]
    //[RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Name can only contain letters and a single space")]
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    [Required]
    public DateOnly BirthDate { get; set; }
    [Required]
    [Phone]
    [StringLength(11)]
    public string PhoneNumber { get; set; } = "";
    [Required]
    [RegularExpression("^(Male|Female)$", ErrorMessage = "Invalid Gender")]
    public string Gender { get; set; } = "";
    [Required]
    [StringLength(50)]
    public string Nationality { get; set; } = "";
    [Required]
    [StringLength(20)]
    public string NationalId { get; set; } = "";
    ////////////////////////////////////
    /// business data
    [Required]

    public DateOnly HireDate { get; set; }
    [Required]
    [Range(0, 1000000)]
    public decimal Salary { get; set; }
    [Required]
    public TimeOnly AttendDate { get; set; }
    [Required]
    public TimeOnly LeaveDate { get; set; }
    [Required]
    public int DepartmentId { get; set; } // Foreign Key

}