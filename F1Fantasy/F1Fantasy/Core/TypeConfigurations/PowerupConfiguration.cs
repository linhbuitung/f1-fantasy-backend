using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;

public class PowerupConfiguration : IEntityTypeConfiguration<Powerup>
{
    public void Configure(EntityTypeBuilder<Powerup> entity)
    {
       entity.HasAlternateKey(powerup => powerup.Type);
       
       entity.HasMany(e => e.FantasyLineups)
           .WithMany(e => e.Powerups)
           .UsingEntity<PowerupFantasyLineup>(
               r => r.HasOne<FantasyLineup>().WithMany().HasForeignKey(e => e.FantasyLineupId),
               l => l.HasOne<Powerup>().WithMany().HasForeignKey(e => e.PowerupId));
    }
}