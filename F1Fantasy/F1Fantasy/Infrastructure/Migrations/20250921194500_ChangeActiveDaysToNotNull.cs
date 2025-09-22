using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeActiveDaysToNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "consecutive_active_days",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "consecutive_active_days",
                table: "AspNetUsers",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}
