using HrBackend.Data.seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace HrApi.Models;

public class HrContext : IdentityDbContext<IdentityUser>
{
    public HrContext(DbContextOptions<HrContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PermissionGroupPermission (join table)
        modelBuilder.Entity<PermissionGroupPermission>()
            .HasKey(p => new { p.GroupId, p.PermissionId });

        modelBuilder.Entity<PermissionGroupPermission>()
            .HasOne(pgp => pgp.Group)
            .WithMany(g => g.PermissionGroupPermissions)
            .HasForeignKey(pgp => pgp.GroupId);

        modelBuilder.Entity<PermissionGroupPermission>()
            .HasOne(pgp => pgp.Permission)
            .WithMany(p => p.PermissionGroupPermissions)
            .HasForeignKey(pgp => pgp.PermissionId);

        // UserPermissionGroup (join table)
        modelBuilder.Entity<UserPermissionGroup>()
            .HasKey(upg => new { upg.UserId, upg.GroupId });

        modelBuilder.Entity<UserPermissionGroup>()
            .HasOne(upg => upg.User)
            .WithMany() // or .WithMany(u => u.UserPermissionGroups) if you add navigation
            .HasForeignKey(upg => upg.UserId);

        modelBuilder.Entity<UserPermissionGroup>()
            .HasOne(upg => upg.Group)
            .WithMany(g => g.UserPermissionGroups)
            .HasForeignKey(upg => upg.GroupId);
        // Seed initial data
        PermissionSeeder.Seed(modelBuilder);
    }


    public DbSet<Employee> Employees { get; set; }
    public DbSet<Permissions> Permissions { get; set; }
    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<PermissionGroupPermission> PermissionGroupPermissions { get; set; }
    public DbSet<UserPermissionGroup> UserPermissionGroups { get; set; }
    public DbSet<ChatMessage> ChatMessages { get; set; }
}
