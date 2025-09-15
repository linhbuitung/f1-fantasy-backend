using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;

public class SeasonConfiguration : IEntityTypeConfiguration<Season>
{
    public void Configure(EntityTypeBuilder<Season> entity)
    {
        entity.HasAlternateKey(season => season.Year);
        
        entity.HasMany(r => r.Races)
            .WithOne(re => re.Season)
            .HasForeignKey(re => re.SeasonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}