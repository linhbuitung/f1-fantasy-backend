using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CircuitUpdatExtendDecimalPrecision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "circuit",
                type: "numeric(6,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(4,7)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "circuit",
                type: "numeric(6,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(4,6)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Longtitude",
                table: "circuit",
                type: "numeric(4,7)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitude",
                table: "circuit",
                type: "numeric(4,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(6,8)");
        }
    }
}
