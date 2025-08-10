using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
            entity.HasMany(e => e.Users)
               .WithOne(e => e.Country)
               .HasForeignKey(e => e.CountryId)
               .IsRequired(false);

            entity.HasMany(e => e.Drivers)
                .WithOne(e => e.Country)
                .HasForeignKey(e => e.CountryId)
                .IsRequired();

            entity.HasMany(e => e.Constructors)
                .WithOne(e => e.Country)
                .HasForeignKey(e => e.CountryId)
                .IsRequired();

            entity.HasMany(e => e.Circuits)
                .WithOne(e => e.Country)
                .HasForeignKey(e => e.CountryId)
                .IsRequired();
        }
    }
}