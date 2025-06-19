using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;
using System.Reflection.Metadata;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class NationalityConfiguration : IEntityTypeConfiguration<Nationality>
    {
        public void Configure(EntityTypeBuilder<Nationality> entity)
        {
            entity.HasMany(e => e.Users)
               .WithOne(e => e.Nationality)
               .HasForeignKey(e => e.NationalityId)
               .IsRequired();

            entity.HasMany(e => e.Drivers)
                .WithOne(e => e.Nationality)
                .HasForeignKey(e => e.NationalityId)
                .IsRequired();

            entity.HasMany(e => e.Constructors)
                .WithOne(e => e.Nationality)
                .HasForeignKey(e => e.NationalityId)
                .IsRequired();
        }
    }
}