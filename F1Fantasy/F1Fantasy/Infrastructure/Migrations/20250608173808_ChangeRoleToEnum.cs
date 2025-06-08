using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeRoleToEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "user");
            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "user",
                type: "integer",
                nullable: false
                );

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "user",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExp",
                table: "user",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "user",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "user");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExp",
                table: "user");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "user");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "user",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }
    }
}