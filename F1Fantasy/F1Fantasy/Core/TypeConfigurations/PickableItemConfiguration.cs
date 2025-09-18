using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;

public class PickableItemConfiguration : IEntityTypeConfiguration<PickableItem>
{
    public void Configure(EntityTypeBuilder<PickableItem> entity)
    {
        entity.HasMany(e => e.Drivers)
            .WithOne(d => d.PickableItem)
            .HasForeignKey(d => d.PickableItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
        
        entity.HasMany(e => e.Constructors)
            .WithOne(c => c.PickableItem)
            .HasForeignKey(c => c.PickableItemId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}
