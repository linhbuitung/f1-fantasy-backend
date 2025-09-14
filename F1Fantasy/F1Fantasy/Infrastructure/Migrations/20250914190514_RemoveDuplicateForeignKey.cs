using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDuplicateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "driver_fantasy_lineup",
                columns: table => new
                {
                    drivers_id = table.Column<int>(type: "integer", nullable: false),
                    fantasy_lineups_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_driver_fantasy_lineup", x => new { x.drivers_id, x.fantasy_lineups_id });
                    table.ForeignKey(
                        name: "fk_driver_fantasy_lineup_driver_drivers_id",
                        column: x => x.drivers_id,
                        principalTable: "driver",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_driver_fantasy_lineup_fantasy_lineup_fantasy_lineups_id",
                        column: x => x.fantasy_lineups_id,
                        principalTable: "fantasy_lineup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_driver_fantasy_lineup_fantasy_lineups_id",
                table: "driver_fantasy_lineup",
                column: "fantasy_lineups_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "driver_fantasy_lineup");
        }
    }
}
