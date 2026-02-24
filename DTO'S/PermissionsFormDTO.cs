public class PermissionsFormDTO
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public List<CheckBoxDTO> RoleClaims { get; set; }
}