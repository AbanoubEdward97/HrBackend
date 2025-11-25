using System.ComponentModel.DataAnnotations;
public class AddEmpDTO
{
    //[Required]
    //[StringLength(100, MinimumLength = 2)]
    //[RegularExpression(@"^[A-Za-z]+(?: [A-Za-z]+)*$", ErrorMessage = "Name can only contain letters and a single space")]
    public string Name { get; set; } = "";
    [Required]
    public DateOnly BirthDate { get; set; }
    [Required]
    [Phone]
    [StringLength(20)]
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
    public DateTime AttendDate { get; set; }
    [Required]
    public DateTime LeaveDate { get; set; }
}