using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity.Property(u => u.UserName).HasMaxLength(128);
            entity.Property(u => u.NormalizedUserName).HasMaxLength(128);
            entity.Property(u => u.Email).HasMaxLength(128);
            entity.Property(u => u.NormalizedEmail).HasMaxLength(128);

            entity.HasOne(e => e.Constructor)
                .WithMany(c => c.Users)
                .HasForeignKey(e => e.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Driver)
                .WithMany(d => d.Users)
                .HasForeignKey(e => e.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Each User can have many UserClaims
            entity.HasMany(e => e.Claims)
                .WithOne(e => e.User)
                .HasForeignKey(uc => uc.UserId)
                .IsRequired();

            // Each User can have many UserLogins
            entity.HasMany(e => e.Logins)
                .WithOne(e => e.User)
                .HasForeignKey(ul => ul.UserId)
                .IsRequired();

            // Each User can have many UserTokens
            entity.HasMany(e => e.Tokens)
                .WithOne(e => e.User)
                .HasForeignKey(ut => ut.UserId)
                .IsRequired();

            // Each User can have many entries in the UserRole join table
            entity.HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
        }
    }
}