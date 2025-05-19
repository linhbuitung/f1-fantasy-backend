using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddImgUrlFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PredictionType",
                table: "prediction");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "powerup",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "driver",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "constructor",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImgUrl",
                table: "circuit",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "powerup");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "constructor");

            migrationBuilder.DropColumn(
                name: "ImgUrl",
                table: "circuit");

            migrationBuilder.AddColumn<string>(
                name: "PredictionType",
                table: "prediction",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
