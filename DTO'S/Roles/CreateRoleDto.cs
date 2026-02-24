using System.ComponentModel.DataAnnotations;

public class CreateRoleDto
{
    [Required]
    [StringLength(256, MinimumLength = 2, ErrorMessage = "Role name must be between 2 and 256 characters.")]
    public string? Name { get; set; }
}