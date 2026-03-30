using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HrBackend.Services;

public class UserService : IUserService
{
    private readonly HrContext _context;

    public UserService(HrContext context)
    {
        _context = context;
    }

    public async Task<List<UserDto>> GetAllUsers()
    {
        var users = await _context.Users
       .AsNoTracking()
       .Select(u => new UserDto
       {
            Email = u.Email,
            UserName = u.UserName,
             Id = u.Id,
             Roles =  (
            from ur in _context.UserRoles
            join r in _context.Roles on ur.RoleId equals r.Id
            where ur.UserId == u.Id
            select r.Name
        ).ToList()
       }).ToListAsync();
       return users;
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }
}
