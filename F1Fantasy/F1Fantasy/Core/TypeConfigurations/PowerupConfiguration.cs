using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations;

public class PowerupConfiguration : IEntityTypeConfiguration<Powerup>
{
    public void Configure(EntityTypeBuilder<Powerup> entity)
    {
       entity.HasAlternateKey(powerup => powerup.Type);
    }
}