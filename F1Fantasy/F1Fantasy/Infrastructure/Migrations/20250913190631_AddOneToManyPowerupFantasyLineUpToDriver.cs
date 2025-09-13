using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOneToManyPowerupFantasyLineUpToDriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "driver_id",
                table: "powerup_fantasy_lineup",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_powerup_fantasy_lineup_driver_id",
                table: "powerup_fantasy_lineup",
                column: "driver_id");

            migrationBuilder.AddForeignKey(
                name: "fk_powerup_fantasy_lineup_driver_driver_id",
                table: "powerup_fantasy_lineup",
                column: "driver_id",
                principalTable: "driver",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_powerup_fantasy_lineup_driver_driver_id",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropIndex(
                name: "ix_powerup_fantasy_lineup_driver_id",
                table: "powerup_fantasy_lineup");

            migrationBuilder.DropColumn(
                name: "driver_id",
                table: "powerup_fantasy_lineup");
        }
    }
}
