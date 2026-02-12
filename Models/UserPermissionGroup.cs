using Microsoft.AspNetCore.Identity;

public class UserPermissionGroup
{
    public string? UserId { get; set; }
    public IdentityUser? User { get; set; }
    public int GroupId { get; set; }
    public PermissionGroup? Group { get; set; }
}