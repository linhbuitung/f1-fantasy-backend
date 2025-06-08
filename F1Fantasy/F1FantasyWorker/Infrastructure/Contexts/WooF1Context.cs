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

    public virtual DbSet<League> Leagues { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Powerup> Powerups { get; set; }

    public virtual DbSet<Prediction> Predictions { get; set; }

    public virtual DbSet<Race> Races { get; set; }

    public virtual DbSet<RaceEntry> RaceEntries { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.ToTable("circuit");

            entity.HasIndex(e => e.Code, "AK_circuit_Code").IsUnique();

            entity.Property(e => e.CircuitName).HasMaxLength(300);
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.Country)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.ImgUrl).HasMaxLength(300);
            entity.Property(e => e.Latitude).HasPrecision(9, 7);
            entity.Property(e => e.Locality)
                .HasMaxLength(200)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.Longtitude).HasPrecision(10, 7);
        });

        modelBuilder.Entity<Constructor>(entity =>
        {
            entity.ToTable("constructor");

            entity.HasIndex(e => e.Code, "AK_constructor_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.ImgUrl).HasMaxLength(300);
            entity.Property(e => e.Name).HasMaxLength(300);
            entity.Property(e => e.Nationality).HasMaxLength(200);
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.ToTable("driver");

            entity.HasIndex(e => e.Code, "AK_driver_Code").IsUnique();

            entity.Property(e => e.Code).HasMaxLength(50);
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.FamilyName)
                .HasMaxLength(300)
                .HasColumnName("family_name");
            entity.Property(e => e.GivenName)
                .HasMaxLength(300)
                .HasColumnName("given_name");
            entity.Property(e => e.ImgUrl).HasMaxLength(300);
            entity.Property(e => e.Nationality).HasMaxLength(200);
        });

        modelBuilder.Entity<DriverPrediction>(entity =>
        {
            entity.ToTable("driver_prediction");

            entity.HasIndex(e => e.ConstructorId, "IX_driver_prediction_ConstructorId");

            entity.HasIndex(e => e.DriverId, "IX_driver_prediction_DriverId");

            entity.HasIndex(e => e.PredictionId, "IX_driver_prediction_PredictionId");

            entity.HasOne(d => d.Constructor).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Driver).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Prediction).WithMany(p => p.DriverPredictions)
                .HasForeignKey(d => d.PredictionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FantasyLineup>(entity =>
        {
            entity.ToTable("fantasy_lineup");

            entity.HasIndex(e => e.RaceId, "IX_fantasy_lineup_RaceId");

            entity.HasIndex(e => e.UserId, "IX_fantasy_lineup_UserId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Race).WithMany(p => p.FantasyLineups)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User).WithMany(p => p.FantasyLineups)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Drivers).WithMany(p => p.FantasyLineups)
                .UsingEntity<Dictionary<string, object>>(
                    "FantasyLineupDriver",
                    r => r.HasOne<Driver>().WithMany()
                        .HasForeignKey("DriverId")
                        .OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne<FantasyLineup>().WithMany().HasForeignKey("FantasyLineupId"),
                    j =>
                    {
                        j.HasKey("FantasyLineupId", "DriverId");
                        j.ToTable("fantasy_lineup_driver");
                        j.HasIndex(new[] { "DriverId" }, "IX_fantasy_lineup_driver_DriverId");
                    });

            entity.HasMany(d => d.Powerups).WithMany(p => p.FantasyLineups)
                .UsingEntity<Dictionary<string, object>>(
                    "PowerupFantasyLineup",
                    r => r.HasOne<Powerup>().WithMany().HasForeignKey("PowerupId"),
                    l => l.HasOne<FantasyLineup>().WithMany().HasForeignKey("FantasyLineupId"),
                    j =>
                    {
                        j.HasKey("FantasyLineupId", "PowerupId");
                        j.ToTable("powerup_fantasy_lineup");
                        j.HasIndex(new[] { "PowerupId" }, "IX_powerup_fantasy_lineup_PowerupId");
                    });
        });

        modelBuilder.Entity<League>(entity =>
        {
            entity.ToTable("league");

            entity.HasIndex(e => e.UserId, "IX_league_UserId");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Type).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.Leagues)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(d => d.Users).WithMany(p => p.LeaguesNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "UserLeague",
                    r => r.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict),
                    l => l.HasOne<League>().WithMany()
                        .HasForeignKey("LeagueId")
                        .OnDelete(DeleteBehavior.Restrict),
                    j =>
                    {
                        j.HasKey("LeagueId", "UserId");
                        j.ToTable("user_league");
                        j.HasIndex(new[] { "UserId" }, "IX_user_league_UserId");
                    });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("notification");

            entity.HasIndex(e => e.UserId, "IX_notification_UserId");

            entity.Property(e => e.Content).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.Header).HasMaxLength(200);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Powerup>(entity =>
        {
            entity.ToTable("powerup");

            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImgUrl)
                .HasMaxLength(300)
                .HasDefaultValueSql("''::character varying");
            entity.Property(e => e.Type).HasMaxLength(100);
        });

        modelBuilder.Entity<Prediction>(entity =>
        {
            entity.ToTable("prediction");

            entity.HasIndex(e => e.CircuitId, "IX_prediction_CircuitId");

            entity.HasIndex(e => e.UserId, "IX_prediction_UserId");

            entity.Property(e => e.DatePredicted).HasColumnName("datePredicted");

            entity.HasOne(d => d.Circuit).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.User).WithMany(p => p.Predictions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Race>(entity =>
        {
            entity.ToTable("race");

            entity.HasIndex(e => e.CircuitId, "IX_race_CircuitId");

            entity.Property(e => e.Calculated).HasDefaultValue(false);
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.DeadlineDate).HasColumnName("deadline_date");

            entity.HasOne(d => d.Circuit).WithMany(p => p.Races)
                .HasForeignKey(d => d.CircuitId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RaceEntry>(entity =>
        {
            entity.ToTable("race_entry");

            entity.HasIndex(e => e.DriverId, "IX_race_entry_DriverId");

            entity.HasIndex(e => e.RaceId, "IX_race_entry_RaceId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Driver).WithMany(p => p.RaceEntries)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Race).WithMany(p => p.RaceEntries)
                .HasForeignKey(d => d.RaceId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasIndex(e => e.ConstructorId, "IX_user_ConstructorId");

            entity.HasIndex(e => e.DriverId, "IX_user_DriverId");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.LastLogin)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("last_login");
            entity.Property(e => e.Nationality).HasMaxLength(200);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.RefreshToken).HasDefaultValueSql("''::text");
            entity.Property(e => e.Salt).HasDefaultValueSql("''::text");

            entity.HasOne(d => d.Constructor).WithMany(p => p.Users)
                .HasForeignKey(d => d.ConstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Driver).WithMany(p => p.Users)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}