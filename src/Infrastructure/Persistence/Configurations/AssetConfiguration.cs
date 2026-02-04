using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : IEntityTypeConfiguration<Asset>
{
    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.AssetCode)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.HasIndex(x => x.AssetCode)
            .IsUnique();

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Location)
            .IsRequired();
            
        // Enums typically stored as int or string. Int is default.
        builder.Property(x => x.Status)
            .HasConversion<string>(); // Readable in DB
            
        // Ignore DomainEvents
        builder.Ignore(x => x.DomainEvents);
    }
}
