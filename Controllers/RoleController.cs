
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
//[Authorize(Roles = "SuperAdmin")]
public class RoleController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }
    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        var name = dto.Name?.Trim();
        var roleExists = await _roleManager.RoleExistsAsync(name);
        if (roleExists)
        {
            return BadRequest("This role already exists.");
        }

        var role = new IdentityRole
        {
            Name = name
        };;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new { dto, Message = "Role created successfully." });
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _roleManager.Roles.Select(r => new { r.Id, r.Name }).ToListAsync();
        return Ok(roles);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteRole(string Name)
    {
        var role = await _roleManager.FindByNameAsync(Name);
        if (role == null)
            return NotFound();

        await _roleManager.DeleteAsync(role);
        return Ok("Role deleted successfully.");
    }

    [HttpGet("ManagePermissions/{roleId:guid}")]
    public async Task<IActionResult> ManagePermissions(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
            return NotFound("Role not found.");

        var roleClaims =  _roleManager.GetClaimsAsync(role).Result.Select(r => r.Value).ToList();
        var AllClaims = Enums.Permissions.GenerateAllPermissions();
        var AllPermissions = AllClaims.Select(p => new CheckBoxDTO {DisplayValue = p}).ToList();
        
        foreach (var permission in AllPermissions)
        {
            if (roleClaims.Any(c => c == permission.DisplayValue))
            {
                permission.IsSelected = true;
            }
        }
        
        var dto = new PermissionsFormDTO
        {
            RoleId = role.Id,
            RoleName = role.Name,
            RoleClaims = AllPermissions
        };
        
        return Ok(dto);
    }


    [HttpPut("ManagePermissions")]
    public async Task<IActionResult> ManagePermissions(PermissionsFormDTO model)
    {
        var role = await _roleManager.FindByIdAsync(model.RoleId.ToString());
        if (role == null)
            return NotFound("Role not found.");

        var roleClaims =  await _roleManager.GetClaimsAsync(role);
        foreach (var claim in roleClaims)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
        var selectedClaims = model.RoleClaims.Where(c => c.IsSelected).ToList();
        foreach (var claim in selectedClaims)
        {
            await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.DisplayValue));
        }
        return Ok(new { model, Message = "Permissions updated successfully." });
    }
}