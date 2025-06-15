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
        }
    }
}