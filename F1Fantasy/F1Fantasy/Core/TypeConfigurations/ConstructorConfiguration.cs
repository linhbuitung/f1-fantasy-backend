using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class ConstructorConfiguration : IEntityTypeConfiguration<Constructor>
    {
        public void Configure(EntityTypeBuilder<Constructor> entity)
        {
            entity.HasAlternateKey(con => con.Code);

            entity.HasMany(c => c.DriverPredictions)
                .WithOne(dp => dp.Constructor)
                .HasForeignKey(dp => dp.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Users)
                .WithOne(u => u.Constructor)
                .HasForeignKey(u => u.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(c => c.RaceEntries)
                .WithOne(re => re.Constructor)
                .HasForeignKey(re => re.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}