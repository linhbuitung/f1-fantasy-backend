using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class FantasyLineupDriverConfiguration : IEntityTypeConfiguration<FantasyLineupDriver>
    {
        public void Configure(EntityTypeBuilder<FantasyLineupDriver> entity)
        {
            entity.HasKey(fld => new { fld.FantasyLineupId, fld.DriverId });
            
            entity.HasOne(fld => fld.FantasyLineup)
                .WithMany(l => l.FantasyLineupDrivers)
                .HasForeignKey(fld => fld.FantasyLineupId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(fld => fld.Driver)
                .WithMany(u => u.FantasyLineupDrivers)
                .HasForeignKey(fld => fld.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}