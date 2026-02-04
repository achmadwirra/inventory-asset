using System.Reflection;
using Inventory.Domain.Entities;
using Inventory.Infrastructure.AuditLogging;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();
    public DbSet<AssetCategory> AssetCategories => Set<AssetCategory>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        // Basic config for Category and AuditLog if not in separate files
        modelBuilder.Entity<AssetCategory>(b => 
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<AuditLog>(b => 
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.EntityName).IsRequired();
            b.Property(x => x.Action).IsRequired();
        });
    }
}
