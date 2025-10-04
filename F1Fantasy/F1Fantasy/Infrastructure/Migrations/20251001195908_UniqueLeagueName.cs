using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UniqueLeagueName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_league_name",
                table: "league",
                column: "name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "ak_league_name",
                table: "league");
        }
    }
}
