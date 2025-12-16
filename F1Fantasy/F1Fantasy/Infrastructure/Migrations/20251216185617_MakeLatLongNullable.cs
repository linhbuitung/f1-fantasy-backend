using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeLatLongNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "circuit",
                type: "numeric(10,7)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "circuit",
                type: "numeric(9,7)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(9,7)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "longitude",
                table: "circuit",
                type: "numeric(10,7)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,7)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "latitude",
                table: "circuit",
                type: "numeric(9,7)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(9,7)",
                oldNullable: true);
        }
    }
}
