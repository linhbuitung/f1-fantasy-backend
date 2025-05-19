using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CircuitUpdatFixDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "circuit",
                type: "numeric(3,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "circuit",
                type: "numeric(2,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,8)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "circuit",
                type: "numeric(6,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "circuit",
                type: "numeric(6,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,8)");
        }
    }
}
