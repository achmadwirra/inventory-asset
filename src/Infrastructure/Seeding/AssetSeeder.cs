using Inventory.Domain.Entities;
using Inventory.Domain.Enums;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Seeding;

public static class AssetSeeder
{
    // Employee User ID from UserSeeder
    private static readonly Guid EmployeeUserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");

    public static async Task SeedAsync(AppDbContext context)
    {
        var assets = new (Guid Id, string Code, string Name, Guid CategoryId, AssetStatus Status, string Location, Guid? AssignedTo)[]
        {
            // Laptops
            (Guid.Parse("a1111111-1111-1111-1111-111111111111"), "LAP-001", "Dell Latitude 7420", AssetCategorySeeder.LaptopCategoryId, AssetStatus.InStock, "Jakarta", null),
            (Guid.Parse("a2222222-2222-2222-2222-222222222222"), "LAP-002", "MacBook Pro M1", AssetCategorySeeder.LaptopCategoryId, AssetStatus.Assigned, "Jakarta", EmployeeUserId),
            (Guid.Parse("a3333333-3333-3333-3333-333333333333"), "LAP-003", "Lenovo ThinkPad X1", AssetCategorySeeder.LaptopCategoryId, AssetStatus.InStock, "Bandung", null),
            
            // Monitors
            (Guid.Parse("a4444444-4444-4444-4444-444444444444"), "MON-001", "Dell 24\" Monitor", AssetCategorySeeder.MonitorCategoryId, AssetStatus.InStock, "Bandung", null),
            (Guid.Parse("a5555555-5555-5555-5555-555555555555"), "MON-002", "LG UltraWide 34\"", AssetCategorySeeder.MonitorCategoryId, AssetStatus.Assigned, "Surabaya", EmployeeUserId),
            
            // Servers
            (Guid.Parse("a6666666-6666-6666-6666-666666666666"), "SRV-001", "Dell PowerEdge R740", AssetCategorySeeder.ServerCategoryId, AssetStatus.Maintenance, "Data Center", null),
            (Guid.Parse("a7777777-7777-7777-7777-777777777777"), "SRV-002", "HP ProLiant DL380", AssetCategorySeeder.ServerCategoryId, AssetStatus.InStock, "Data Center", null),
            
            // Network Devices
            (Guid.Parse("a8888888-8888-8888-8888-888888888888"), "NET-001", "Cisco Router ISR 4000", AssetCategorySeeder.NetworkDeviceCategoryId, AssetStatus.InStock, "Jakarta", null),
        };

        foreach (var assetData in assets)
        {
            var exists = await context.Assets.AnyAsync(a => a.Id == assetData.Id);
            if (!exists)
            {
                var asset = new Asset(
                    assetData.Id,
                    assetData.Code,
                    assetData.Name,
                    assetData.CategoryId,
                    DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-6)),
                    assetData.Location
                );

                // Use reflection to set private Status and AssignedToUserId since constructor defaults to InStock
                if (assetData.Status != AssetStatus.InStock)
                {
                    var statusProperty = typeof(Asset).GetProperty("Status");
                    statusProperty?.SetValue(asset, assetData.Status);
                }
                
                if (assetData.AssignedTo.HasValue)
                {
                    var assignedProperty = typeof(Asset).GetProperty("AssignedToUserId");
                    assignedProperty?.SetValue(asset, assetData.AssignedTo);
                }

                context.Assets.Add(asset);
            }
        }

        await context.SaveChangesAsync();
    }
}
