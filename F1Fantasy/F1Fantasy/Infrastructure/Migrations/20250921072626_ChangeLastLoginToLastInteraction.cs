using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeLastLoginToLastInteraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_login",
                table: "AspNetUsers",
                newName: "last_interaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "last_interaction",
                table: "AspNetUsers",
                newName: "last_login");
        }
    }
}
