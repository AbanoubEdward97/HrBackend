public class PermissionGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }

    public ICollection<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
    public ICollection<UserPermissionGroup> UserPermissionGroups { get; set; }

    public PermissionGroup()
    {
        PermissionGroupPermissions = new List<PermissionGroupPermission>();
        UserPermissionGroups = new List<UserPermissionGroup>();
    }
}
