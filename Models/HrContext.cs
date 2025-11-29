using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace HrApi.Models;
public class HrContext : IdentityDbContext<ApplicationUser>
{
    public HrContext(DbContextOptions<HrContext> options)  : base(options)
    {
        
    }

    public DbSet<Employee> Employees { get; set; }
}
