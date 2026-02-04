using Inventory.Infrastructure.Persistence;

namespace Inventory.Infrastructure.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await RoleSeeder.SeedAsync(context);
        await UserSeeder.SeedAsync(context);
        await AssetCategorySeeder.SeedAsync(context);
        await AssetSeeder.SeedAsync(context);
    }
}
