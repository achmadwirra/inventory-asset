using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory.Infrastructure.Persistence.Configurations;

public class AssetAssignmentConfiguration : IEntityTypeConfiguration<AssetAssignment>
{
    public void Configure(EntityTypeBuilder<AssetAssignment> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(x => x.AssetId)
            .OnDelete(DeleteBehavior.Cascade); // Adjust as needed
            
        builder.Property(x => x.AssignedAt)
            .IsRequired();
    }
}
