using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUserToProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_lineup_user_UserId",
                table: "fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "FK_league_user_UserId",
                table: "league");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_user_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_prediction_user_UserId",
                table: "prediction");

            migrationBuilder.DropForeignKey(
                name: "FK_user_league_user_UserId",
                table: "user_league");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.RenameColumn(
                name: "deadline_date",
                table: "race",
                newName: "DeadlineDate");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "race",
                newName: "RaceDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "notification",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "given_name",
                table: "driver",
                newName: "GivenName");

            migrationBuilder.RenameColumn(
                name: "family_name",
                table: "driver",
                newName: "FamilyName");

            migrationBuilder.RenameColumn(
                name: "date_of_birth",
                table: "driver",
                newName: "DateOfBirth");

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "date", nullable: false),
                    Nationality = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "date", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "timestamp", nullable: false),
                    AcceptNotification = table.Column<bool>(type: "boolean", nullable: false),
                    LoginStreak = table.Column<int>(type: "integer", nullable: false),
                    ConstructorId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_constructor_ConstructorId",
                        column: x => x.ConstructorId,
                        principalTable: "constructor",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfiles_driver_DriverId",
                        column: x => x.DriverId,
                        principalTable: "driver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ConstructorId",
                table: "UserProfiles",
                column: "ConstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_DriverId",
                table: "UserProfiles",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_lineup_UserProfiles_UserId",
                table: "fantasy_lineup",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_league_UserProfiles_UserId",
                table: "league",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_UserProfiles_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prediction_UserProfiles_UserId",
                table: "prediction",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_league_UserProfiles_UserId",
                table: "user_league",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_fantasy_lineup_UserProfiles_UserId",
                table: "fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "FK_league_UserProfiles_UserId",
                table: "league");

            migrationBuilder.DropForeignKey(
                name: "FK_notification_UserProfiles_UserId",
                table: "notification");

            migrationBuilder.DropForeignKey(
                name: "FK_prediction_UserProfiles_UserId",
                table: "prediction");

            migrationBuilder.DropForeignKey(
                name: "FK_user_league_UserProfiles_UserId",
                table: "user_league");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "RaceDate",
                table: "race",
                newName: "date");

            migrationBuilder.RenameColumn(
                name: "DeadlineDate",
                table: "race",
                newName: "deadline_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "notification",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "GivenName",
                table: "driver",
                newName: "given_name");

            migrationBuilder.RenameColumn(
                name: "FamilyName",
                table: "driver",
                newName: "family_name");

            migrationBuilder.RenameColumn(
                name: "DateOfBirth",
                table: "driver",
                newName: "date_of_birth");

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ConstructorId = table.Column<int>(type: "integer", nullable: false),
                    DriverId = table.Column<int>(type: "integer", nullable: false),
                    AcceptNotification = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "date", nullable: false),
                    date_of_birth = table.Column<DateTime>(type: "date", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    last_login = table.Column<DateTime>(type: "timestamp", nullable: false),
                    LoginStreak = table.Column<int>(type: "integer", nullable: false),
                    Nationality = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: false),
                    RefreshTokenExp = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Salt = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_user_ConstructorId",
                table: "user",
                column: "ConstructorId");

            migrationBuilder.CreateIndex(
                name: "IX_user_DriverId",
                table: "user",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_fantasy_lineup_user_UserId",
                table: "fantasy_lineup",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_league_user_UserId",
                table: "league",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_notification_user_UserId",
                table: "notification",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_prediction_user_UserId",
                table: "prediction",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_league_user_UserId",
                table: "user_league",
                column: "UserId",
                principalTable: "user",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
