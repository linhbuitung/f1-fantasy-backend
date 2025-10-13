using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifyDateFieldsForPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "predict_year",
                table: "prediction");

            migrationBuilder.AddColumn<DateTime>(
                name: "qualifying_date",
                table: "prediction",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "race_date",
                table: "prediction",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "qualifying_date",
                table: "prediction");

            migrationBuilder.DropColumn(
                name: "race_date",
                table: "prediction");

            migrationBuilder.AddColumn<int>(
                name: "predict_year",
                table: "prediction",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
