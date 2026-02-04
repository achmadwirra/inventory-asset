using System.Security.Cryptography;
using System.Text;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Seeding;

public static class UserSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var users = new[]
        {
            new
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Email = "admin@company.com",
                Password = "Admin123!",
                RoleId = RoleSeeder.AdminRoleId
            },
            new
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Email = "itstaff@company.com",
                Password = "IT123!",
                RoleId = RoleSeeder.ITStaffRoleId
            },
            new
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Email = "employee@company.com",
                Password = "User123!",
                RoleId = RoleSeeder.EmployeeRoleId
            }
        };

        foreach (var userData in users)
        {
            var exists = await context.Users.AnyAsync(u => u.Email == userData.Email);
            if (!exists)
            {
                var passwordHash = HashPassword(userData.Password);
                var user = new User(userData.Id, userData.Email, passwordHash, userData.RoleId);
                context.Users.Add(user);
            }
        }

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        // Using SHA256 for simplicity. In production, use BCrypt or PBKDF2.
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
