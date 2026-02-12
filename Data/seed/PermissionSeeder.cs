using Microsoft.EntityFrameworkCore;

namespace HrBackend.Data.seed
{
    public static class PermissionSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>().HasData(
                new Permissions { Id = 1, Screen = "Employees", Action="View", Key = "Employees.View" },
                new Permissions { Id = 2, Screen = "Employees", Action="Add", Key = "Employees.Add" },
                new Permissions { Id = 3, Screen = "Employees", Action="Edit", Key = "Employees.Edit" },
                new Permissions { Id = 4, Screen = "Employees", Action="Delete", Key = "Employees.Delete" }
            );
        }
    }
}