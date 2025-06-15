using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class FantasyLineupConfiguration : IEntityTypeConfiguration<FantasyLineup>
    {
        public void Configure(EntityTypeBuilder<FantasyLineup> entity)
        {
            entity.HasOne(f => f.User)
                .WithMany(u => u.FantasyLineups)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(f => f.Race)
                .WithMany(r => r.FantasyLineups)
                .HasForeignKey(f => f.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}