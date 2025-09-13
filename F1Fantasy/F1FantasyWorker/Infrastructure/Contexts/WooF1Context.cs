using System;
using System.Collections.Generic;
using F1FantasyWorker.Core.Common;
using Microsoft.EntityFrameworkCore;

namespace F1FantasyWorker.Infrastructure.Contexts;

public partial class WooF1Context : DbContext
{
    public WooF1Context()
    {
    }

    public WooF1Context(DbContextOptions<WooF1Context> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Circuit> Circuits { get; set; }

    public virtual DbSet<Constructor> Constructors { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

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

    public virtual DbSet<Season> Seasons { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=woof1;TrustServerCertificate=True;Username=woof1;Password=AVerySecretPassword;Include Error Detail=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_asp_net_roles");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.Name)
                .HasMaxLength(256)
                .HasColumnName("name");
            entity.Property(e => e.NormalizedName)
                .HasMaxLength(256)
                .HasColumnName("normalized_name");
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_asp_net_role_claims");

            entity.HasIndex(e => e.RoleId, "ix_asp_net_role_claims_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaimType).HasColumnName("claim_type");
            entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("fk_asp_net_role_claims_asp_net_roles_role_id");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_asp_net_users");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.HasIndex(e => e.ConstructorId, "ix_asp_net_users_constructor_id");

            entity.HasIndex(e => e.CountryId, "ix_asp_net_users_country_id");

            entity.HasIndex(e => e.DriverId, "ix_asp_net_users_driver_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcceptNotification).HasColumnName("accept_notification");
            entity.Property(e => e.AccessFailedCount).HasColumnName("access_failed_count");
            entity.Property(e => e.ConcurrencyStamp).HasColumnName("concurrency_stamp");
            entity.Property(e => e.ConstructorId).HasColumnName("constructor_id");
            entity.Property(e => e.CountryId)
                .HasMaxLength(100)
                .HasColumnName("country_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DisplayName)
                .HasMaxLength(100)
                .HasColumnName("display_name");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .HasColumnName("email");
            entity.Property(e => e.EmailConfirmed).HasColumnName("email_confirmed");
            entity.Property(e => e.LastLogin).HasColumnName("last_login");
            entity.Property(e => e.LockoutEnabled).HasColumnName("lockout_enabled");
            entity.Property(e => e.LockoutEnd).HasColumnName("lockout_end");
            entity.Property(e => e.LoginStreak).HasColumnName("login_streak");
            entity.Property(e => e.NormalizedEmail)
                .HasMaxLength(128)
                .HasColumnName("normalized_email");
            entity.Property(e => e.NormalizedUserName)
                .HasMaxLength(128)
                .HasColumnName("normalized_user_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.PhoneNumber).HasColumnName("phone_number");
            entity.Property(e => e.PhoneNumberConfirmed).HasColumnName("phone_number_confirmed");
            entity.Property(e => e.SecurityStamp).HasColumnName("security_stamp");
            entity.Property(e => e.TwoFactorEnabled).HasColumnName("two_factor_enabled");
            entity.Property(e => e.UserName)
                .HasMaxLength(128)
                .HasColumnName("user_name");

            entity.HasOne(d => d.Constructor).WithMany(p => p.AspNetUsers)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asp_net_users_constructor_constructor_id");

            entity.HasOne(d => d.Country).WithMany(p => p.AspNetUsers)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_asp_net_users_country_country_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.AspNetUsers)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_asp_net_users_driver_driver_id");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_roles_role_id"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_asp_net_user_roles_asp_net_users_user_id"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("pk_asp_net_user_roles");
                        j.ToTable("AspNetUserRoles");
                        j.HasIndex(new[] { "RoleId" }, "ix_asp_net_user_roles_role_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_asp_net_user_claims");

            entity.HasIndex(e => e.UserId, "ix_asp_net_user_claims_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ClaimType).HasColumnName("claim_type");
            entity.Property(e => e.ClaimValue).HasColumnName("claim_value");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_asp_net_user_claims_asp_net_users_user_id");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey }).HasName("pk_asp_net_user_logins");

            entity.HasIndex(e => e.UserId, "ix_asp_net_user_logins_user_id");

            entity.Property(e => e.LoginProvider).HasColumnName("login_provider");
            entity.Property(e => e.ProviderKey).HasColumnName("provider_key");
            entity.Property(e => e.ProviderDisplayName).HasColumnName("provider_display_name");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_asp_net_user_logins_asp_net_users_user_id");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name }).HasName("pk_asp_net_user_tokens");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.LoginProvider)
                .HasMaxLength(128)
                .HasColumnName("login_provider");
            entity.Property(e => e.Name)
                .HasMaxLength(128)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_asp_net_user_tokens_asp_net_users_user_id");
        });

        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_circuit");

            entity.ToTable("circuit");

            entity.HasIndex(e => e.Code, "ak_circuit_code").IsUnique();

            entity.HasIndex(e => e.CountryId, "ix_circuit_country_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CircuitName)
                .HasMaxLength(300)
                .HasColumnName("circuit_name");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CountryId)
                .HasMaxLength(100)
                .HasColumnName("country_id");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .HasColumnName("img_url");
            entity.Property(e => e.Latitude)
                .HasPrecision(9, 7)
                .HasColumnName("latitude");
            entity.Property(e => e.Locality)
                .HasMaxLength(200)
                .HasColumnName("locality");
            entity.Property(e => e.Longtitude)
                .HasPrecision(10, 7)
                .HasColumnName("longtitude");

            entity.HasOne(d => d.Country).WithMany(p => p.Circuits)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_circuit_country_country_id");
        });

        modelBuilder.Entity<Constructor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_constructor");

            entity.ToTable("constructor");

            entity.HasIndex(e => e.Code, "ak_constructor_code").IsUnique();

            entity.HasIndex(e => e.CountryId, "ix_constructor_country_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CountryId)
                .HasMaxLength(100)
                .HasColumnName("country_id");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .HasColumnName("img_url");
            entity.Property(e => e.Name)
                .HasMaxLength(300)
                .HasColumnName("name");

            entity.HasOne(d => d.Country).WithMany(p => p.Constructors)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_constructor_country_country_id");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_country");

            entity.ToTable("country");

            entity.Property(e => e.Id)
                .HasMaxLength(100)
                .HasColumnName("id");
            entity.Property(e => e.Nationalities).HasColumnName("nationalities");
            entity.Property(e => e.ShortName)
                .HasMaxLength(200)
                .HasColumnName("short_name");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_driver");

            entity.ToTable("driver");

            entity.HasIndex(e => e.Code, "ak_driver_code").IsUnique();

            entity.HasIndex(e => e.CountryId, "ix_driver_country_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasColumnName("code");
            entity.Property(e => e.CountryId)
                .HasMaxLength(100)
                .HasColumnName("country_id");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FamilyName)
                .HasMaxLength(300)
                .HasColumnName("family_name");
            entity.Property(e => e.GivenName)
                .HasMaxLength(300)
                .HasColumnName("given_name");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .HasColumnName("img_url");

            entity.HasOne(d => d.Country).WithMany(p => p.Drivers)
                .HasForeignKey(d => d.CountryId)
                .HasConstraintName("fk_driver_country_country_id");
        });

        modelBuilder.Entity<DriverPrediction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_driver_prediction");

            entity.ToTable("driver_prediction");

            entity.HasIndex(e => e.ConstructorId, "ix_driver_prediction_constructor_id");

            entity.HasIndex(e => e.DriverId, "ix_driver_prediction_driver_id");

            entity.HasIndex(e => e.PredictionId, "ix_driver_prediction_prediction_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructorId).HasColumnName("constructor_id");
            entity.Property(e => e.Crashed).HasColumnName("crashed");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.FinalPosition).HasColumnName("final_position");
            entity.Property(e => e.GridPosition).HasColumnName("grid_position");
            entity.Property(e => e.PredictionId).HasColumnName("prediction_id");

            entity.HasOne(d => d.Constructor).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_driver_prediction_constructor_constructor_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_driver_prediction_driver_driver_id");

            entity.HasOne(d => d.Prediction).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.PredictionId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_driver_prediction_prediction_prediction_id");
        });

        modelBuilder.Entity<FantasyLineup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_fantasy_lineup");

            entity.ToTable("fantasy_lineup");

            entity.HasIndex(e => e.RaceId, "ix_fantasy_lineup_race_id");

            entity.HasIndex(e => e.UserId, "ix_fantasy_lineup_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PointsGained).HasColumnName("points_gained");
            entity.Property(e => e.RaceId).HasColumnName("race_id");
            entity.Property(e => e.TotalAmount).HasColumnName("total_amount");
            entity.Property(e => e.TransferPointsDeducted).HasColumnName("transfer_points_deducted");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Race).WithMany(p => p.FantasyLineups)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_fantasy_lineup_race_race_id");

            entity.HasOne(d => d.User).WithMany(p => p.FantasyLineups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_fantasy_lineup_application_user_user_id");
        });

        modelBuilder.Entity<FantasyLineupDriver>(entity =>
        {
            entity.HasKey(e => new { e.FantasyLineupId, e.DriverId }).HasName("pk_fantasy_lineup_driver");

            entity.ToTable("fantasy_lineup_driver");

            entity.HasIndex(e => e.DriverId, "ix_fantasy_lineup_driver_driver_id");

            entity.HasIndex(e => e.DriverId1, "ix_fantasy_lineup_driver_driver_id1");

            entity.HasIndex(e => e.FantasyLineupId1, "ix_fantasy_lineup_driver_fantasy_lineup_id1");

            entity.Property(e => e.FantasyLineupId).HasColumnName("fantasy_lineup_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.DriverId1)
                .HasDefaultValue(0)
                .HasColumnName("driver_id1");
            entity.Property(e => e.FantasyLineupId1)
                .HasDefaultValue(0)
                .HasColumnName("fantasy_lineup_id1");

            entity.HasOne(d => d.Driver).WithMany(p => p.FantasyLineupDriverDrivers)
                .HasForeignKey(d => d.DriverId)
                .HasConstraintName("fk_fantasy_lineup_driver_driver_driver_id");

            entity.HasOne(d => d.DriverId1Navigation).WithMany(p => p.FantasyLineupDriverDriverId1Navigations)
                .HasForeignKey(d => d.DriverId1)
                .HasConstraintName("fk_fantasy_lineup_driver_driver_driver_id1");

            entity.HasOne(d => d.FantasyLineup).WithMany(p => p.FantasyLineupDriverFantasyLineups)
                .HasForeignKey(d => d.FantasyLineupId)
                .HasConstraintName("fk_fantasy_lineup_driver_fantasy_lineup_fantasy_lineup_id");

            entity.HasOne(d => d.FantasyLineupId1Navigation).WithMany(p => p.FantasyLineupDriverFantasyLineupId1Navigations)
                .HasForeignKey(d => d.FantasyLineupId1)
                .HasConstraintName("fk_fantasy_lineup_driver_fantasy_lineup_fantasy_lineup_id1");
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_league");

            entity.ToTable("league");

            entity.HasIndex(e => e.UserId, "ix_league_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.MaxPlayersNum).HasColumnName("max_players_num");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Leagues)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_league_application_user_user_id");

            entity.HasMany(d => d.Users).WithMany(p => p.LeaguesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UserLeague",
                    r => r.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_user_league_application_user_user_id"),
                    l => l.HasOne<League>().WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("fk_user_league_league_league_id"),
                    j =>
                    {
                        j.HasKey("LeagueId", "UserId").HasName("pk_user_league");
                        j.ToTable("user_league");
                        j.HasIndex(new[] { "UserId" }, "ix_user_league_user_id");
                        j.IndexerProperty<int>("LeagueId").HasColumnName("league_id");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                    });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_notification");

            entity.ToTable("notification");

            entity.HasIndex(e => e.UserId, "ix_notification_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Content)
                .HasMaxLength(1000)
                .HasColumnName("content");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Header)
                .HasMaxLength(200)
                .HasColumnName("header");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_notification_application_user_user_id");
        });

        modelBuilder.Entity<Powerup>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_powerup");

            entity.ToTable("powerup");

            entity.HasIndex(e => e.Type, "ak_powerup_type").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .HasColumnName("img_url");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .HasColumnName("type");
        });

        modelBuilder.Entity<PowerupFantasyLineup>(entity =>
        {
            entity.HasKey(e => new { e.FantasyLineupId, e.PowerupId }).HasName("pk_powerup_fantasy_lineup");

            entity.ToTable("powerup_fantasy_lineup");

            entity.HasIndex(e => e.DriverId, "ix_powerup_fantasy_lineup_driver_id");

            entity.HasIndex(e => e.FantasyLineupId1, "ix_powerup_fantasy_lineup_fantasy_lineup_id1");

            entity.HasIndex(e => e.PowerupId, "ix_powerup_fantasy_lineup_powerup_id");

            entity.HasIndex(e => e.PowerupId1, "ix_powerup_fantasy_lineup_powerup_id1");

            entity.Property(e => e.FantasyLineupId).HasColumnName("fantasy_lineup_id");
            entity.Property(e => e.PowerupId).HasColumnName("powerup_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.FantasyLineupId1)
                .HasDefaultValue(0)
                .HasColumnName("fantasy_lineup_id1");
            entity.Property(e => e.PowerupId1)
                .HasDefaultValue(0)
                .HasColumnName("powerup_id1");

            entity.HasOne(d => d.Driver).WithMany(p => p.PowerupFantasyLineups)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_powerup_fantasy_lineup_driver_driver_id");

            entity.HasOne(d => d.FantasyLineup).WithMany(p => p.PowerupFantasyLineupFantasyLineups)
                .HasForeignKey(d => d.FantasyLineupId)
                .HasConstraintName("fk_powerup_fantasy_lineup_fantasy_lineup_fantasy_lineup_id");

            entity.HasOne(d => d.FantasyLineupId1Navigation).WithMany(p => p.PowerupFantasyLineupFantasyLineupId1Navigations)
                .HasForeignKey(d => d.FantasyLineupId1)
                .HasConstraintName("fk_powerup_fantasy_lineup_fantasy_lineup_fantasy_lineup_id1");

            entity.HasOne(d => d.Powerup).WithMany(p => p.PowerupFantasyLineupPowerups)
                .HasForeignKey(d => d.PowerupId)
                .HasConstraintName("fk_powerup_fantasy_lineup_powerup_powerup_id");

            entity.HasOne(d => d.PowerupId1Navigation).WithMany(p => p.PowerupFantasyLineupPowerupId1Navigations)
                .HasForeignKey(d => d.PowerupId1)
                .HasConstraintName("fk_powerup_fantasy_lineup_powerup_powerup_id1");
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_prediction");

            entity.ToTable("prediction");

            entity.HasIndex(e => e.CircuitId, "ix_prediction_circuit_id");

            entity.HasIndex(e => e.UserId, "ix_prediction_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CircuitId).HasColumnName("circuit_id");
            entity.Property(e => e.DatePredicted).HasColumnName("date_predicted");
            entity.Property(e => e.PredictYear).HasColumnName("predict_year");
            entity.Property(e => e.Rain).HasColumnName("rain");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Circuit).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_prediction_circuit_circuit_id");

            entity.HasOne(d => d.User).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_prediction_application_user_user_id");
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_race");

            entity.ToTable("race");

            entity.HasIndex(e => e.CircuitId, "ix_race_circuit_id");

            entity.HasIndex(e => e.SeasonId, "ix_race_season_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Calculated).HasColumnName("calculated");
            entity.Property(e => e.CircuitId).HasColumnName("circuit_id");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");
            entity.Property(e => e.RaceDate).HasColumnName("race_date");
            entity.Property(e => e.Round)
                .HasDefaultValue(0)
                .HasColumnName("round");
            entity.Property(e => e.SeasonId)
                .HasDefaultValue(0)
                .HasColumnName("season_id");

            entity.HasOne(d => d.Circuit).WithMany(p => p.Races)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_race_circuit_circuit_id");

            entity.HasOne(d => d.Season).WithMany(p => p.Races)
                .HasForeignKey(d => d.SeasonId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_race_season_season_id");
        });

        modelBuilder.Entity<RaceEntry>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_race_entry");

            entity.ToTable("race_entry");

            entity.HasIndex(e => e.ConstructorId, "ix_race_entry_constructor_id");

            entity.HasIndex(e => e.DriverId, "ix_race_entry_driver_id");

            entity.HasIndex(e => e.RaceId, "ix_race_entry_race_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConstructorId)
                .HasDefaultValue(0)
                .HasColumnName("constructor_id");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.FastestLap).HasColumnName("fastest_lap");
            entity.Property(e => e.Finished)
                .HasDefaultValue(false)
                .HasColumnName("finished");
            entity.Property(e => e.Grid).HasColumnName("grid");
            entity.Property(e => e.PointsGained).HasColumnName("points_gained");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.RaceId).HasColumnName("race_id");

            entity.HasOne(d => d.Constructor).WithMany(p => p.RaceEntries)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_race_entry_constructor_constructor_id");

            entity.HasOne(d => d.Driver).WithMany(p => p.RaceEntries)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_race_entry_driver_driver_id");

            entity.HasOne(d => d.Race).WithMany(p => p.RaceEntries)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_race_entry_race_race_id");
        });

        modelBuilder.Entity<Season>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_season");

            entity.ToTable("season");

            entity.HasIndex(e => e.Year, "ak_season_year").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.Year).HasColumnName("year");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
