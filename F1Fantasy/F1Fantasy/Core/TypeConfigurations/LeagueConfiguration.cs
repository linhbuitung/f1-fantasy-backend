using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class LeagueConfiguration : IEntityTypeConfiguration<League>
    {
        public void Configure(EntityTypeBuilder<League> entity)
        {
            entity.Property(l => l.Type)
                .HasConversion<string>();
            
            entity.HasAlternateKey(ld => ld.Name);
            
            entity.HasOne(l => l.User)
                .WithMany(u => u.Leagues)
                .HasForeignKey(l => l.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(l => new { l.Name, l.Description })
                .HasMethod("GIN")
                .HasAnnotation("Npgsql:TsVectorConfig", "english");
        }
    }
}