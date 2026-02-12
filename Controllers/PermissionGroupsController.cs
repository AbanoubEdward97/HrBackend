using HrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Tasks;
using Microsoft.EntityFrameworkCore;
[ApiController]
[Route("api/[controller]")]
public class PermissionGroupsController : ControllerBase
{
    private readonly HrContext _context;

    public PermissionGroupsController(HrContext context)
    {
        _context = context;
    }

    // Add your action methods here to manage permissions and permission groups
    [HttpPost]
    public async Task<IActionResult> Create(CreatePermissionGroupDto dto)
    {
        dto.PermissionKeys = dto?.PermissionKeys?
        .Where(k => !string.IsNullOrWhiteSpace(k))
        .Distinct()
        .ToList();

        // Trim name to avoid duplicates with spaces
        var groupName = dto?.Name?.Trim();

        // Check if a group with the same name exists
        var exists = await _context.PermissionGroups
            .AnyAsync(g => g.Name.ToLower() == groupName!.ToLower());

        if (exists)
            return BadRequest("A permission group already exists.");
        if (dto?.PermissionKeys == null || !dto.PermissionKeys.Any())
            return BadRequest("At least one permission key must be provided.");


        var permissions = await _context.Permissions
            .Where(p => dto.PermissionKeys.Contains(p.Key!))
            .ToListAsync();

        // CRITICAL VALIDATION
        if (permissions.Count != dto.PermissionKeys.Count)
            return BadRequest("One or more permission keys are invalid.");

        foreach (var item in dto.PermissionKeys.Any().ToString())
        {
            Console.WriteLine(item);
            //System.Console.WriteLine(item.Length);
        }


        var group = new PermissionGroup
        {
            Name = dto.Name!,
        };

        _context.PermissionGroups.Add(group);
        await _context.SaveChangesAsync();

        foreach (var permission in permissions)
        {
            var groupPermission = new PermissionGroupPermission
            {
                GroupId = group.Id,
                PermissionId = permission.Id
            };
            _context.PermissionGroupPermissions.Add(groupPermission);

        }
        await _context.SaveChangesAsync();
        return Ok(new { Message = "Permission group created successfully.", GroupId = group.Id });
    }

}