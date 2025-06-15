using F1Fantasy.Core.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace F1Fantasy.Core.TypeConfigurations
{
    public class UserLeagueConfiguration : IEntityTypeConfiguration<UserLeague>
    {
        public void Configure(EntityTypeBuilder<UserLeague> entity)
        {
            entity.HasKey(ul => new { ul.LeagueId, ul.UserId });

            entity.HasOne(ul => ul.League)
                .WithMany(l => l.UserLeagues)
                .HasForeignKey(ul => ul.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ul => ul.User)
                .WithMany(u => u.UserLeagues)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}