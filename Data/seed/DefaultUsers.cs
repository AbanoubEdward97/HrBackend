using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

public static class DefaultUsers
{
    public static async Task seedBasicUserAsync(UserManager<IdentityUser> userManager)
    {
        var defaultUser = new IdentityUser
        {
            UserName = "pipo.edward@gmail.com",
            Email = "pipo.edward@gmail.com",
            EmailConfirmed = true,

        };

        // try to find the user by email from db to avoid duplicate seeding
        var user = await userManager.FindByEmailAsync(defaultUser.Email);
        if (user == null)
        {
            await userManager.CreateAsync(defaultUser, "User@123");
            //Add the user to the "User" role
            await userManager.AddToRoleAsync(defaultUser, Roles.User.ToString());
        }
    }

    public static async Task seedSuperAdminAsync(UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager)
    {
        var superAdminUser = new IdentityUser
        {
            UserName = "superadmin@gmail.com",
            Email = "superadmin@gmail.com",
            EmailConfirmed = true,

        };

        // try to find the user by email from db to avoid duplicate seeding
        var user = await userManager.FindByEmailAsync(superAdminUser.Email);
        if (user == null)
        {
            await userManager.CreateAsync(superAdminUser, "SuperAdmin@123");
            //Add the user to the "SuperAdmin" role
            //await userManager.AddToRoleAsync(superAdminUser, Roles.SuperAdmin.ToString());

            //since SuperAdmin has all permissions, we can add the user to all roles
            await userManager.AddToRolesAsync(superAdminUser,
            new List<string> {
                Roles.SuperAdmin.ToString(),
                Roles.Admin.ToString(),
                Roles.User.ToString() 
            });
        }

        // to do : seed claims 
        await roleManager.SeedClaimsForSuperAdmin();               

    }
    public static async Task SeedClaimsForSuperAdmin(this RoleManager<IdentityRole> roleManager)
    {
        var superadminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
        if (superadminRole != null)
        {
            await roleManager.AddPermissionClaims(superadminRole, "Employees");
        }
    } 

    public static async Task AddPermissionClaims(this RoleManager<IdentityRole> roleManager , IdentityRole role, string module)
    {
        var roleClaims = await roleManager.GetClaimsAsync(role);
        var allPermissions = Enums.Permissions.GeneratePermissionsList(module);
 
        foreach (var permission in allPermissions)
        {
            if (!roleClaims.Any(rc => rc.Type == "Permission" && rc.Value == permission  ))
            {
                await roleManager.AddClaimAsync(role,new Claim("Permission", permission));
            }
        }
    }
}
//new System.Security.Claims.Claim("Permission", permission)