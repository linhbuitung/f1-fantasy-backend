using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;


public class FantasyLineupConstructorConfiguration : IEntityTypeConfiguration<FantasyLineupConstructor>
{
    public void Configure(EntityTypeBuilder<FantasyLineupConstructor> entity)
    {
        entity.HasKey(flc => new { flc.FantasyLineupId, flc.ConstructorId });
        
        entity.HasOne(flc => flc.FantasyLineup)
            .WithMany(l => l.FantasyLineupConstructors)
            .HasForeignKey(flc => flc.FantasyLineupId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(flc => flc.Constructor)
            .WithMany(c => c.FantasyLineupConstructors)
            .HasForeignKey(flc => flc.ConstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
