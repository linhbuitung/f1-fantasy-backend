using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "circuit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CircuitName = table.Column<int>(type: "integer", nullable: false),
                    Lattitude = table.Column<decimal>(type: "numeric(2,4)", nullable: false),
                    Longttitude = table.Column<decimal>(type: "numeric(3,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_circuit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "constructor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Nationality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_constructor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "driver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    given_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    family_name = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: false),
                    Nationality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "powerup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_powerup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "race",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    date = table.Column<DateTime>(type: "date", nullable: false),
                    deadline_date = table.Column<DateTime>(type: "date", nullable: false),
                    CircuitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_race", x => x.Id);
                    table.ForeignKey(
                        name: "FK_race_circuit_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "circuit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: false),
                    Nationality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "date", nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp", nullable: false),
                    AcceptNotification = table.Column<bool>(type: "boolean", nullable: false),
                    LoginStreak = table.Column<int>(type: "integer", nullable: false),
                    ConstructorId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    Role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user", x => x.Id);
                    table.ForeignKey(
                        name: "FK_user_constructor_ConstructorId",
                        column: x => x.ConstructorId,
                        principalTable: "constructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "race_entry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: true),
                    Grid = table.Column<int>(type: "integer", nullable: true),
                    FastestLap = table.Column<int>(type: "integer", nullable: true),
                    PointsGained = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    RaceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_race_entry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_race_entry_driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_race_entry_race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fantasy_lineup",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    TotalAmount = table.Column<int>(type: "integer", nullable: false),
                    TransferPointsDeducted = table.Column<int>(type: "integer", nullable: false),
                    PointsGained = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    RaceId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fantasy_lineup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_fantasy_lineup_race_RaceId",
                        column: x => x.RaceId,
                        principalTable: "race",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fantasy_lineup_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "league",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MaxPlayersNum = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_league", x => x.Id);
                    table.ForeignKey(
                        name: "FK_league_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "notification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Header = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    created_at = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_notification_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "prediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    datePredicted = table.Column<DateTime>(type: "date", nullable: false),
                    PredictYear = table.Column<int>(type: "integer", nullable: false),
                    Rain = table.Column<bool>(type: "boolean", nullable: false),
                    PredictionType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CircuitId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_prediction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_prediction_circuit_CircuitId",
                        column: x => x.CircuitId,
                        principalTable: "circuit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_prediction_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fantasy_lineup_driver",
                columns: table => new
                {
                    FantasyLineupId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_fantasy_lineup_driver", x => new { x.FantasyLineupId, x.DriverId });
                    table.ForeignKey(
                        name: "FK_fantasy_lineup_driver_driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_fantasy_lineup_driver_fantasy_lineup_FantasyLineupId",
                        column: x => x.FantasyLineupId,
                        principalTable: "fantasy_lineup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "powerup_fantasy_lineup",
                columns: table => new
                {
                    FantasyLineupId = table.Column<int>(type: "integer", nullable: false),
                    PowerupId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_powerup_fantasy_lineup", x => new { x.FantasyLineupId, x.PowerupId });
                    table.ForeignKey(
                        name: "FK_powerup_fantasy_lineup_fantasy_lineup_FantasyLineupId",
                        column: x => x.FantasyLineupId,
                        principalTable: "fantasy_lineup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_powerup_fantasy_lineup_powerup_PowerupId",
                        column: x => x.PowerupId,
                        principalTable: "powerup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_league",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_league", x => new { x.LeagueId, x.UserId });
                    table.ForeignKey(
                        name: "FK_user_league_league_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "league",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_user_league_user_UserId",
                        column: x => x.UserId,
                        principalTable: "user",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "driver_prediction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GridPosition = table.Column<int>(type: "integer", nullable: true),
                    FinalPosition = table.Column<int>(type: "integer", nullable: true),
                    Crashed = table.Column<bool>(type: "boolean", nullable: false),
                    PredictionId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    ConstructorId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_driver_prediction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_driver_prediction_constructor_ConstructorId",
                        column: x => x.ConstructorId,
                        principalTable: "constructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_driver_prediction_driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_driver_prediction_prediction_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "prediction",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_driver_prediction_ConstructorId",
                table: "driver_prediction",
                column: "ConstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_driver_prediction_DriverId",
                table: "driver_prediction",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_driver_prediction_PredictionId",
                table: "driver_prediction",
                column: "PredictionId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_lineup_RaceId",
                table: "fantasy_lineup",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_lineup_UserId",
                table: "fantasy_lineup",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_fantasy_lineup_driver_DriverId",
                table: "fantasy_lineup_driver",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_league_UserId",
                table: "league",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_notification_UserId",
                table: "notification",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_powerup_fantasy_lineup_PowerupId",
                table: "powerup_fantasy_lineup",
                column: "PowerupId");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_CircuitId",
                table: "prediction",
                column: "CircuitId");

            migrationBuilder.CreateIndex(
                name: "IX_prediction_UserId",
                table: "prediction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_race_CircuitId",
                table: "race",
                column: "CircuitId");

            migrationBuilder.CreateIndex(
                name: "IX_race_entry_DriverId",
                table: "race_entry",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_race_entry_RaceId",
                table: "race_entry",
                column: "RaceId");

            migrationBuilder.CreateIndex(
                name: "IX_user_ConstructorId",
                table: "user",
                column: "ConstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_user_DriverId",
                table: "user",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_user_league_UserId",
                table: "user_league",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "driver_prediction");

            migrationBuilder.DropTable(
                name: "fantasy_lineup_driver");

            migrationBuilder.DropTable(
                name: "notification");

            migrationBuilder.DropTable(
                name: "powerup_fantasy_lineup");

            migrationBuilder.DropTable(
                name: "race_entry");

            migrationBuilder.DropTable(
                name: "user_league");

            migrationBuilder.DropTable(
                name: "prediction");

            migrationBuilder.DropTable(
                name: "fantasy_lineup");

            migrationBuilder.DropTable(
                name: "powerup");

            migrationBuilder.DropTable(
                name: "league");

            migrationBuilder.DropTable(
                name: "race");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "circuit");

            migrationBuilder.DropTable(
                name: "constructor");

            migrationBuilder.DropTable(
                name: "driver");
        }
    }
}
