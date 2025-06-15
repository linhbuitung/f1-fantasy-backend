using F1Fantasy.Core.Common;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Infrastructure.Contexts
{
    public class WooF1Context : DbContext
    {
        public WooF1Context(DbContextOptions<WooF1Context> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("YourConnectionStringHere");
            }
        }

        public virtual DbSet<Circuit> Circuits { get; set; }
        public virtual DbSet<Constructor> Constructors { get; set; }
        public virtual DbSet<Driver> Drivers { get; set; }

        public virtual DbSet<DriverPrediction> DriverPredictions { get; set; }

        public virtual DbSet<FantasyLineup> FantasyLineups { get; set; }

        public virtual DbSet<FantasyLineupDriver> FantasyLineupDrivers { get; set; }

        public virtual DbSet<League> Leagues { get; set; }

        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Powerup> Powerups { get; set; }
        public virtual DbSet<PowerupFantasyLineup> PowerupFantasyLineups { get; set; }
        public virtual DbSet<Prediction> Predictions { get; set; }

        public virtual DbSet<Race> Races { get; set; }

        public virtual DbSet<RaceEntry> RaceEntries { get; set; }

        public virtual DbSet<UserProfile> UserProfiles { get; set; }

        public virtual DbSet<UserLeague> UserLeagues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Circuit ---
            modelBuilder.Entity<Circuit>(entity =>
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
            });

            // --- Constructor ---
            modelBuilder.Entity<Constructor>(entity =>
            {
                entity.HasAlternateKey(con => con.Code);

                entity.HasMany(c => c.DriverPredictions)
                    .WithOne(dp => dp.Constructor)
                    .HasForeignKey(dp => dp.ConstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Users)
                    .WithOne(u => u.Constructor)
                    .HasForeignKey(u => u.ConstructorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Driver ---
            modelBuilder.Entity<Driver>(entity =>
            {
                entity.HasAlternateKey(d => d.Code);

                entity.HasMany(d => d.DriverPredictions)
                    .WithOne(dp => dp.Driver)
                    .HasForeignKey(dp => dp.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.FantasyLineupDrivers)
                    .WithOne(fld => fld.Driver)
                    .HasForeignKey(fld => fld.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(d => d.RaceEntries)
                    .WithOne(re => re.Driver)
                    .HasForeignKey(re => re.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- DriverPrediction ---
            modelBuilder.Entity<DriverPrediction>(entity =>
            {
                entity.HasOne(dp => dp.Prediction)
                    .WithMany(p => p.DriverPredictions)
                    .HasForeignKey(dp => dp.PredictionId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- FantasyLineup ---
            modelBuilder.Entity<FantasyLineup>(entity =>
            {
                entity.HasOne(f => f.User)
                    .WithMany(u => u.FantasyLineups)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.Race)
                    .WithMany(r => r.FantasyLineups)
                    .HasForeignKey(f => f.RaceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- FantasyLineupDriver ---
            modelBuilder.Entity<FantasyLineupDriver>(entity =>
            {
                entity.HasKey(fld => new { fld.FantasyLineupId, fld.DriverId });
            });

            // --- PowerupFantasyLineup ---
            modelBuilder.Entity<PowerupFantasyLineup>(entity =>
            {
                entity.HasKey(pf => new { pf.FantasyLineupId, pf.PowerupId });
            });

            // --- UserLeague ---
            modelBuilder.Entity<UserLeague>(entity =>
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
            });

            // --- League ---
            modelBuilder.Entity<League>(entity =>
            {
                entity.HasOne(l => l.User)
                    .WithMany(u => u.Leagues)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Notification ---
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Prediction ---
            modelBuilder.Entity<Prediction>(entity =>
            {
                entity.HasOne(p => p.User)
                    .WithMany(u => u.Predictions)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- Race ---
            modelBuilder.Entity<Race>(entity =>
            {
                entity.HasMany(r => r.RaceEntries)
                    .WithOne(re => re.Race)
                    .HasForeignKey(re => re.RaceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // --- ApplicationUser ---
            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(e => e.Constructor)
                    .WithMany(c => c.Users)
                    .HasForeignKey(e => e.ConstructorId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Driver)
                    .WithMany(d => d.Users)
                    .HasForeignKey(e => e.DriverId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}