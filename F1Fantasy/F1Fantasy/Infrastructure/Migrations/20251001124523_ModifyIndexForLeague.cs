using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyIndexForLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_league_name",
                table: "league");

            migrationBuilder.CreateIndex(
                name: "ix_league_name_description",
                table: "league",
                columns: new[] { "name", "description" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_league_name_description",
                table: "league");

            migrationBuilder.CreateIndex(
                name: "ix_league_name",
                table: "league",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }
    }
}
