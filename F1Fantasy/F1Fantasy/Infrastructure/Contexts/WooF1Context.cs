using F1Fantasy.Core.Common;
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

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserLeague> UserLeagues { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //alternate keys
            modelBuilder.Entity<Driver>()
                .HasAlternateKey(d => d.Code);
            modelBuilder.Entity<Circuit>()
                .HasAlternateKey(c => c.Code);
            modelBuilder.Entity<Constructor>()
                .HasAlternateKey(con => con.Code);
            // Configure composite keys for join tables

            modelBuilder.Entity<FantasyLineupDriver>()
                .HasKey(fld => new { fld.FantasyLineupId, fld.DriverId });
            modelBuilder.Entity<PowerupFantasyLineup>()
                .HasKey(pf => new { pf.FantasyLineupId, pf.PowerupId });
            modelBuilder.Entity<UserLeague>()
                .HasKey(ul => new { ul.LeagueId, ul.UserId });

            // Circuit relationships
            modelBuilder.Entity<Circuit>()
                .HasMany(c => c.Predictions)
                .WithOne(p => p.Circuit)
                .HasForeignKey(p => p.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Circuit>()
                .HasMany(c => c.Races)
                .WithOne(r => r.Circuit)
                .HasForeignKey(r => r.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Constructor>()
                .HasMany(c => c.DriverPredictions)
                .WithOne(dp => dp.Constructor)
                .HasForeignKey(dp => dp.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Constructor>()
                .HasMany(c => c.Users)
                .WithOne(u => u.Constructor)
                .HasForeignKey(u => u.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Driver relationships

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.DriverPredictions)
                .WithOne(dp => dp.Driver)
                .HasForeignKey(dp => dp.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.FantasyLineupDrivers)
                .WithOne(fld => fld.Driver)
                .HasForeignKey(fld => fld.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.RaceEntries)
                .WithOne(re => re.Driver)
                .HasForeignKey(re => re.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // DriverPrediction relationships
            modelBuilder.Entity<DriverPrediction>()
                .HasOne(dp => dp.Prediction)
                .WithMany(p => p.DriverPredictions)
                .HasForeignKey(dp => dp.PredictionId)
                .OnDelete(DeleteBehavior.Restrict);

            // FantasyLineup relationship
            modelBuilder.Entity<FantasyLineup>()
                .HasOne(f => f.User)
                .WithMany(u => u.FantasyLineups)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FantasyLineup>()
                .HasOne(f => f.Race)
                .WithMany(r => r.FantasyLineups)
                .HasForeignKey(f => f.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // League relationship
            modelBuilder.Entity<League>()
                .HasOne(l => l.User)
                .WithMany(u => u.Leagues)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notification relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prediction relationship
            modelBuilder.Entity<Prediction>()
                .HasOne(p => p.User)
                .WithMany(u => u.Predictions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Race relationships
            modelBuilder.Entity<Race>()
                .HasMany(r => r.RaceEntries)
                .WithOne(re => re.Race)
                .HasForeignKey(re => re.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            // User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Driver)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // UserJoinLeague is configured via composite key (above)
            modelBuilder.Entity<UserLeague>()
                .HasOne(ul => ul.League)
                .WithMany(l => l.UserLeagues)
                .HasForeignKey(ul => ul.LeagueId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserLeague>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.UserLeagues)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}