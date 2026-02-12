using Microsoft.AspNetCore.Identity;

public static class DefaultRoles
{
    public static async Task seedAsync(RoleManager<IdentityRole> roleManager)
    {
        //if not any role exists in the database, create the default roles
        if (!roleManager.Roles.Any())
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
        }

    }
}

