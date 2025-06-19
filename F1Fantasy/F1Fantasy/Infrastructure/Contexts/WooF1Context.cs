using F1Fantasy.Core.Auth;
using F1Fantasy.Core.Common;
using F1Fantasy.Core.TypeConfigurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace F1Fantasy.Infrastructure.Contexts
{
    public class WooF1Context : IdentityDbContext<ApplicationUser,
        ApplicationRole,
        /*key type*/Guid,
        ApplicationUserClaim,
        ApplicationUserRole,
        ApplicationUserLogin,
        ApplicationRoleClaim,
        ApplicationUserToken>
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
            // Ensure the required package is installed: Npgsql.EntityFrameworkCore.PostgreSQL.NamingConvention

            optionsBuilder.UseSnakeCaseNamingConvention();
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

        public virtual DbSet<ApplicationUser> UserProfiles { get; set; }

        public virtual DbSet<UserLeague> UserLeagues { get; set; }

        public virtual DbSet<Nationality> Nationalities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new CircuitConfiguration());
            modelBuilder.ApplyConfiguration(new ConstructorConfiguration());
            modelBuilder.ApplyConfiguration(new DriverConfiguration());
            modelBuilder.ApplyConfiguration(new DriverPredictionConfiguration());
            modelBuilder.ApplyConfiguration(new FantasyLineupConfiguration());
            modelBuilder.ApplyConfiguration(new FantasyLineupDriverConfiguration());
            modelBuilder.ApplyConfiguration(new PowerupFantasyLineupConfiguration());
            modelBuilder.ApplyConfiguration(new UserLeagueConfiguration());
            modelBuilder.ApplyConfiguration(new LeagueConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
            modelBuilder.ApplyConfiguration(new PredictionConfiguration());
            modelBuilder.ApplyConfiguration(new RaceConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationAuthConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ApplicationUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new NationalityConfiguration());
        }
    }
}