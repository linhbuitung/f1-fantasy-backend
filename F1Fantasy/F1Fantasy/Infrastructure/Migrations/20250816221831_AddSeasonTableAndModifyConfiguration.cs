using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonTableAndModifyConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "season_id",
                table: "race",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fantasy_lineup_id1",
                table: "powerup_fantasy_lineup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "powerup_id1",
                table: "powerup_fantasy_lineup",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "powerup",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "driver_id1",
                table: "fantasy_lineup_driver",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "fantasy_lineup_id1",
                table: "fantasy_lineup_driver",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_powerup_type",
                table: "powerup",
                column: "type");

            migrationBuilder.CreateTable(
                name: "season",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    year = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_season", x => x.id);
                    table.UniqueConstraint("ak_season_year", x => x.year);
                });

            migrationBuilder.CreateIndex(
                name: "ix_race_season_id",
                table: "race",
                column: "season_id");

            migrationBuilder.CreateIndex(
                name: "ix_powerup_fantasy_lineup_fantasy_lineup_id1",
                table: "powerup_fantasy_lineup",
                column: "fantasy_lineup_id1");

            migrationBuilder.CreateIndex(
                name: "ix_powerup_fantasy_lineup_powerup_id1",
                table: "powerup_fantasy_lineup",
                column: "powerup_id1");

            migrationBuilder.CreateIndex(
                name: "ix_fantasy_lineup_driver_driver_id1",
                table: "fantasy_lineup_driver",
                column: "driver_id1");

            migrationBuilder.CreateIndex(
                name: "ix_fantasy_lineup_driver_fantasy_lineup_id1",
                table: "fantasy_lineup_driver",
                column: "fantasy_lineup_id1");

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id",
                table: "fantasy_lineup_driver",
                column: "driver_id",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id1",
                table: "fantasy_lineup_driver",
                column: "driver_id1",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_driver_fantasy_lineup_fantasy_lineup_id1",
                table: "fantasy_lineup_driver",
                column: "fantasy_lineup_id1",
                principalTable: "fantasy_lineup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_powerup_fantasy_lineup_fantasy_lineup_fantasy_lineup_id1",
                table: "powerup_fantasy_lineup",
                column: "fantasy_lineup_id1",
                principalTable: "fantasy_lineup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_powerup_fantasy_lineup_powerup_powerup_id1",
                table: "powerup_fantasy_lineup",
                column: "powerup_id1",
                principalTable: "powerup",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_race_season_season_id",
                table: "race",
                column: "season_id",
                principalTable: "season",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropForeignKey(
                name: "fk_fantasy_lineup_driver_fantasy_lineup_fantasy_lineup_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropForeignKey(
                name: "fk_powerup_fantasy_lineup_fantasy_lineup_fantasy_lineup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "fk_powerup_fantasy_lineup_powerup_powerup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropForeignKey(
                name: "fk_race_season_season_id",
                table: "race");

            migrationBuilder.DropTable(
                name: "season");

            migrationBuilder.DropIndex(
                name: "ix_race_season_id",
                table: "race");

            migrationBuilder.DropIndex(
                name: "ix_powerup_fantasy_lineup_fantasy_lineup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropIndex(
                name: "ix_powerup_fantasy_lineup_powerup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_powerup_type",
                table: "powerup");

            migrationBuilder.DropIndex(
                name: "ix_fantasy_lineup_driver_driver_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropIndex(
                name: "ix_fantasy_lineup_driver_fantasy_lineup_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "race");

            migrationBuilder.DropColumn(
                name: "fantasy_lineup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropColumn(
                name: "powerup_id1",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropColumn(
                name: "driver_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.DropColumn(
                name: "fantasy_lineup_id1",
                table: "fantasy_lineup_driver");

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "powerup",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "fk_fantasy_lineup_driver_driver_driver_id",
                table: "fantasy_lineup_driver",
                column: "driver_id",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
