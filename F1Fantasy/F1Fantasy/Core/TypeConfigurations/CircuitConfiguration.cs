using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class CircuitConfiguration : IEntityTypeConfiguration<Circuit>
    {
        public void Configure(EntityTypeBuilder<Circuit> entity)
        {
            entity.HasAlternateKey(c => c.Code);

            entity.HasMany(c => c.Predictions)
                .WithOne(p => p.Circuit)
                .HasForeignKey(p => p.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(c => c.Races)
                .WithOne(r => r.Circuit)
                .HasForeignKey(r => r.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}