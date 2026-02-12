using HrApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly HrContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(HrContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }


    // Add your action methods here to manage users
    [HttpGet]
    public async Task<IActionResult> GetAllUsers()
    {
        ////first solution//////////////////////////////////
        // var users = await _userManager.Users
        // .AsNoTracking().Select(u => new UserDto { Id = u.Id, UserName = u.UserName, Email = u.Email }).
        // ToListAsync();
        // var result = new List<UserDto>();
        // foreach (var user in users)
        // {
        // var roles = await _userManager.GetRolesAsync(new IdentityUser { Id = user.Id }); 

        //     result.Add(new UserDto
        //     {
        //         Id = user.Id,
        //         UserName = user.UserName,
        //         Email = user.Email,
        //         Roles = roles
        //     });
        // }
        // return Ok(result);
        ///////////////////End First Solution////////////////////////////////
        ///second solution////////////////////////////////// 
        var users = await _context.Users
        .AsNoTracking()
        .Select(u => new
     {
         u.Id,
         u.UserName,
         u.Email,
         Roles = (
             from ur in _context.UserRoles
             join r in _context.Roles on ur.RoleId equals r.Id
             where ur.UserId == u.Id
             select r.Name
         ).ToList()
     })
     .ToListAsync();

        return Ok(users);

        /// End second solution/////////////////////////////
        ////////////////////////////////////////////////////
        // [course code]var roles = await _userManager.Users.Select(u => new UserDto { Id = u.Id, UserName = u.UserName, Email = u.Email, Roles = _userManager.GetRolesAsync(u).Result }).AsNoTracking().ToListAsync();
    }
    [HttpGet]
    public async Task<IActionResult> ManageRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var allRoles = await _roleManager.Roles.ToListAsync();
        var dto = new UserRolesDTO
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = allRoles.Select(role => new RoleDTO
            {
                RoleName = role.Name,
                IsSelected = _userManager.IsInRoleAsync(user, role.Name).Result
            }).ToList()
        };

        return Ok(dto);
    }
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
        {
            return BadRequest("Email and Password are required.");
        }

        var userExists = await _userManager.FindByEmailAsync(dto.Email);

        if (userExists != null)
        {
            return BadRequest("This email is already used.");
        }

        //create new user
        var user = new IdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        //Assign Role
        if (!string.IsNullOrWhiteSpace(dto.Role))
        {
            var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
            if (!roleExists)
            {
                return BadRequest("The specified role does not exist.");
            }
            await _userManager.AddToRoleAsync(user, dto.Role);
        }

        //Assign Permission Groups
        if (dto.PermissionGroupIds != null && dto.PermissionGroupIds.Any())
        {
            foreach (var groupId in dto.PermissionGroupIds)
            {
                var group = await _context.PermissionGroups.FindAsync(groupId);
                if (group != null)
                {
                    // Logic to assign permissions from the group to the user
                    // This part depends on how you manage user permissions in your system
                    _context.UserPermissionGroups.Add(new UserPermissionGroup
                    {
                        UserId = user.Id,
                        GroupId = groupId
                    });
                }
            }
        }
        await _context.SaveChangesAsync();
        return Ok(new { user.Id, user.Email });
    }


}

