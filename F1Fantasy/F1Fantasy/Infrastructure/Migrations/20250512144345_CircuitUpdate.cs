using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CircuitUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Longttitude",
                table: "circuit",
                newName: "Longtitude");

            migrationBuilder.RenameColumn(
                name: "Lattitude",
                table: "circuit",
                newName: "Latitude");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "circuit",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Locality",
                table: "circuit",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "circuit");

            migrationBuilder.DropColumn(
                name: "Locality",
                table: "circuit");

            migrationBuilder.RenameColumn(
                name: "Longtitude",
                table: "circuit",
                newName: "Longttitude");

            migrationBuilder.RenameColumn(
                name: "Latitude",
                table: "circuit",
                newName: "Lattitude");
        }
    }
}
