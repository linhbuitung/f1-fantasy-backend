using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class PowerupFantasyLineupConfiguration : IEntityTypeConfiguration<PowerupFantasyLineup>
    {
        public void Configure(EntityTypeBuilder<PowerupFantasyLineup> entity)
        {
            entity.HasKey(pf => new { pf.FantasyLineupId, pf.PowerupId });
            
            entity.HasOne(pf => pf.FantasyLineup)
                .WithMany(l => l.PowerupFantasyLineups)
                .HasForeignKey(pf => pf.FantasyLineupId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pf => pf.Powerup)
                .WithMany(u => u.PowerupFantasyLineups)
                .HasForeignKey(pf => pf.PowerupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}