
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
    public async Task<IActionResult> CreateRole(CreateRoleDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest("Role name is required.");
        }

        var roleExists = await _roleManager.RoleExistsAsync(dto.Name);
        if (roleExists)
        {
            return BadRequest("This role already exists.");
        }

        var role = new IdentityRole
        {
            Name = dto.Name
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("Role created successfully.");
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

}