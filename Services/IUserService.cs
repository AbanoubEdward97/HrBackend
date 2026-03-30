using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HrBackend.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsers();
    Task SaveChanges();
}
