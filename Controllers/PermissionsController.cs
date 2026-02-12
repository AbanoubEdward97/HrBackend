using HrApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly HrContext _context;

    public PermissionsController(HrContext context)
    {
        _context = context;
    }

    // Add your action methods here to manage permissions and permission groups
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _context.Permissions.Select(
            p=> new PermissionDto
            {
                Id = p.Id,
                Screen = p.Screen!,
                Action = p.Action!,
                Key = p.Key!
            }
        ).ToListAsync();
        return Ok(permissions);
    }
}