public class PermissionGroupPermission
{
    public int GroupId { get; set; }
    public PermissionGroup? Group { get; set; }
    public int PermissionId { get; set; }
    public Permissions? Permission { get; set; }
}