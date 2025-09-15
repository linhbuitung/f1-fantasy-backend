using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;

public class RaceEntryConfiguration : IEntityTypeConfiguration<RaceEntry>
{
    public void Configure(EntityTypeBuilder<RaceEntry> entity)
    {
    }
}