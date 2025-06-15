using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.HasAlternateKey(d => d.Code);

            entity.HasMany(d => d.DriverPredictions)
                .WithOne(dp => dp.Driver)
                .HasForeignKey(dp => dp.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.FantasyLineupDrivers)
                .WithOne(fld => fld.Driver)
                .HasForeignKey(fld => fld.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.RaceEntries)
                .WithOne(re => re.Driver)
                .HasForeignKey(re => re.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}