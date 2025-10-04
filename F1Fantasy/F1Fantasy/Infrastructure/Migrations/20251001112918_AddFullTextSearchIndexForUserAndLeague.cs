using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFullTextSearchIndexForUserAndLeague : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_league_name",
                table: "league",
                column: "name")
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_display_name_email",
                table: "AspNetUsers",
                columns: new[] { "display_name", "email" })
                .Annotation("Npgsql:IndexMethod", "GIN")
                .Annotation("Npgsql:TsVectorConfig", "english");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_league_name",
                table: "league");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_display_name_email",
                table: "AspNetUsers");
        }
    }
}
