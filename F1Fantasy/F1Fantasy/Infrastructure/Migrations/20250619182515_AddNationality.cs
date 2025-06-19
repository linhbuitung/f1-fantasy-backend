using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace F1Fantasy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNationality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "nationality",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "constructor");

            migrationBuilder.DropColumn(
                name: "nationality",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "nationality_id",
                table: "driver",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nationality_id",
                table: "constructor",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nationality_id",
                table: "AspNetUsers",
                type: "character varying(100)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "refresh_token",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "nationalities",
                columns: table => new
                {
                    nationality_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    names = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_nationalities", x => x.nationality_id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_driver_nationality_id",
                table: "driver",
                column: "nationality_id");

            migrationBuilder.CreateIndex(
                name: "ix_constructor_nationality_id",
                table: "constructor",
                column: "nationality_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_users_nationality_id",
                table: "AspNetUsers",
                column: "nationality_id");

            migrationBuilder.AddForeignKey(
                name: "fk_asp_net_users_nationalities_nationality_id",
                table: "AspNetUsers",
                column: "nationality_id",
                principalTable: "nationalities",
                principalColumn: "nationality_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_constructor_nationalities_nationality_id",
                table: "constructor",
                column: "nationality_id",
                principalTable: "nationalities",
                principalColumn: "nationality_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_driver_nationalities_nationality_id",
                table: "driver",
                column: "nationality_id",
                principalTable: "nationalities",
                principalColumn: "nationality_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_asp_net_users_nationalities_nationality_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "fk_constructor_nationalities_nationality_id",
                table: "constructor");

            migrationBuilder.DropForeignKey(
                name: "fk_driver_nationalities_nationality_id",
                table: "driver");

            migrationBuilder.DropTable(
                name: "nationalities");

            migrationBuilder.DropIndex(
                name: "ix_driver_nationality_id",
                table: "driver");

            migrationBuilder.DropIndex(
                name: "ix_constructor_nationality_id",
                table: "constructor");

            migrationBuilder.DropIndex(
                name: "ix_asp_net_users_nationality_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "nationality_id",
                table: "driver");

            migrationBuilder.DropColumn(
                name: "nationality_id",
                table: "constructor");

            migrationBuilder.DropColumn(
                name: "nationality_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "refresh_token_expiry_time",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "driver",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "constructor",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nationality",
                table: "AspNetUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
