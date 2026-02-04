using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Seeding;

public static class RoleSeeder
{
    public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid ITStaffRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid EmployeeRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static async Task SeedAsync(AppDbContext context)
    {
        var roles = new[]
        {
            new Role(AdminRoleId, "Admin"),
            new Role(ITStaffRoleId, "ITStaff"),
            new Role(EmployeeRoleId, "Employee")
        };

        foreach (var role in roles)
        {
            var exists = await context.Roles.AnyAsync(r => r.Id == role.Id);
            if (!exists)
            {
                context.Roles.Add(role);
            }
        }

        await context.SaveChangesAsync();
    }
}
