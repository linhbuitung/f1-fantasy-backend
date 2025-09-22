using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAskAICreditField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "login_streak",
                table: "AspNetUsers",
                newName: "consecutive_active_days");

            migrationBuilder.RenameColumn(
                name: "last_interaction",
                table: "AspNetUsers",
                newName: "last_active_at");

            migrationBuilder.AddColumn<int>(
                name: "ask_ai_credits",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ask_ai_credits",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "last_active_at",
                table: "AspNetUsers",
                newName: "last_interaction");

            migrationBuilder.RenameColumn(
                name: "consecutive_active_days",
                table: "AspNetUsers",
                newName: "login_streak");
        }
    }
}
