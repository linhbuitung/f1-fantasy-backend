using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddConnectionLineupConstructorAndPickableItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "constructor_fantasy_lineup",
                columns: table => new
                {
                    constructors_id = table.Column<int>(type: "integer", nullable: false),
                    fantasy_lineups_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_constructor_fantasy_lineup", x => new { x.constructors_id, x.fantasy_lineups_id });
                    table.ForeignKey(
                        name: "fk_constructor_fantasy_lineup_constructor_constructors_id",
                        column: x => x.constructors_id,
                        principalTable: "constructor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_constructor_fantasy_lineup_fantasy_lineup_fantasy_lineups_id",
                        column: x => x.fantasy_lineups_id,
                        principalTable: "fantasy_lineup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "fantasy_lineup_constructor",
                columns: table => new
                {
                    fantasy_lineup_id = table.Column<int>(type: "integer", nullable: false),
                    constructor_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fantasy_lineup_constructor", x => new { x.fantasy_lineup_id, x.constructor_id });
                    table.ForeignKey(
                        name: "fk_fantasy_lineup_constructor_constructor_constructor_id",
                        column: x => x.constructor_id,
                        principalTable: "constructor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_fantasy_lineup_constructor_fantasy_lineup_fantasy_lineup_id",
                        column: x => x.fantasy_lineup_id,
                        principalTable: "fantasy_lineup",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_constructor_fantasy_lineup_fantasy_lineups_id",
                table: "constructor_fantasy_lineup",
                column: "fantasy_lineups_id");

            migrationBuilder.CreateIndex(
                name: "ix_fantasy_lineup_constructor_constructor_id",
                table: "fantasy_lineup_constructor",
                column: "constructor_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "constructor_fantasy_lineup");

            migrationBuilder.DropTable(
                name: "fantasy_lineup_constructor");
        }
    }
}
