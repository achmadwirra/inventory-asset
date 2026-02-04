using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Seeding;

public static class AssetCategorySeeder
{
    public static readonly Guid LaptopCategoryId = Guid.Parse("c1111111-1111-1111-1111-111111111111");
    public static readonly Guid MonitorCategoryId = Guid.Parse("c2222222-2222-2222-2222-222222222222");
    public static readonly Guid ServerCategoryId = Guid.Parse("c3333333-3333-3333-3333-333333333333");
    public static readonly Guid NetworkDeviceCategoryId = Guid.Parse("c4444444-4444-4444-4444-444444444444");

    public static async Task SeedAsync(AppDbContext context)
    {
        var categories = new[]
        {
            new AssetCategory(LaptopCategoryId, "Laptop"),
            new AssetCategory(MonitorCategoryId, "Monitor"),
            new AssetCategory(ServerCategoryId, "Server"),
            new AssetCategory(NetworkDeviceCategoryId, "Network Device")
        };

        foreach (var category in categories)
        {
            var exists = await context.AssetCategories.AnyAsync(c => c.Id == category.Id);
            if (!exists)
            {
                context.AssetCategories.Add(category);
            }
        }

        await context.SaveChangesAsync();
    }
}
