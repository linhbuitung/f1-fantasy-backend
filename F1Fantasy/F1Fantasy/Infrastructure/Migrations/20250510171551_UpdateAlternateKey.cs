using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAlternateKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_circuit_CircuitName",
                table: "circuit");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_constructor_Code",
                table: "constructor",
                column: "Code");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_circuit_Code",
                table: "circuit",
                column: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_constructor_Code",
                table: "constructor");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_circuit_Code",
                table: "circuit");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_circuit_CircuitName",
                table: "circuit",
                column: "CircuitName");
        }
    }
}
