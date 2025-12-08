using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class DriverConfiguration : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> entity)
        {
            entity.Property(e => e.CountryId)
                .HasColumnType("varchar(100)")
                .IsRequired(false)          // allow NULL
                .HasDefaultValue(null);  
            
            entity.HasAlternateKey(d => d.Code);

            entity.HasMany(d => d.DriverPredictions)
                .WithOne(dp => dp.Driver)
                .HasForeignKey(dp => dp.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(d => d.RaceEntries)
                .WithOne(re => re.Driver)
                .HasForeignKey(re => re.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}