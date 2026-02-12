public class Permissions
{
    public int Id { get; set; }
    public string? Screen { get; set; }
    public string? Action { get; set; }
    public string? Key { get; set; }
    public ICollection<PermissionGroupPermission>? PermissionGroupPermissions { get; set; }
}


