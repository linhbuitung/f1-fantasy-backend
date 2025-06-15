using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class RaceConfiguration : IEntityTypeConfiguration<Race>
    {
        public void Configure(EntityTypeBuilder<Race> entity)
        {
            entity.HasMany(r => r.RaceEntries)
                .WithOne(re => re.Race)
                .HasForeignKey(re => re.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}